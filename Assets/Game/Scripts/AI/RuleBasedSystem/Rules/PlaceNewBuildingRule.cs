using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts.AI.RuleBasedSystem.Rules
{
    public class PlaceNewBuildingRule : IRule
    {
        public RuleCategory Category => RuleCategory.Building;

        private BuildingTypePrefabLink _selectedBuilding;
        private BuildingConfig _selectedBuildingConfig;
        private GlobalBuildingsStagesController _stagesController;
        private BuildingFactory _factory;
        
        public PlaceNewBuildingRule(GlobalBuildingsStagesController stagesController, BuildingFactory factory)
        {
            _stagesController = stagesController;
            _factory = factory;
        }
        
        public bool IsValid(AiContext ctx)
        {
            if (ctx.AiBuildings.Count(b =>
                    !b.GetEntityComponent<BuildingBuildComponent>().IsBuilded.CurrentValue) > 2)
            {
                return false;
            }
            
            var worker = ctx.AiUnits.FirstOrDefault(u => u.UnitType == UnitType.Worker);
            if (worker == null)
                return false;
            
            var buildComponent = worker.GetEntityComponent<UnitBuildingComponent>();
            if (buildComponent == null || buildComponent.AvailableBuildings.Count == 0)
                return false;
            
            _selectedBuildingConfig = ChooseBestBuilding(ctx, buildComponent);
            
            if(_selectedBuildingConfig == null)
                return false;

            _selectedBuilding = worker.GetEntityComponent<UnitBuildingComponent>()
                .AvailableBuildings
                .FirstOrDefault(b => 
                    b.Type == _selectedBuildingConfig.Type
                );
            
            return true;
        }

        private BuildingConfig ChooseBestBuilding(AiContext ctx, UnitBuildingComponent builder)
        {
            BuildingConfig best = null;
            float bestScore = float.MinValue;

            foreach (var building in builder.AvailableBuildings)
            {
                var config = _stagesController.GetActualConfig(ctx.OwnerId, building.Type);
                
                if (!CanAffordWithReserve(ctx, config))
                    continue;
                
                if (AlreadyExists(ctx, config))
                    continue;

                float utility = CalculateUtility(ctx, config);

                if (utility > bestScore)
                {
                    bestScore = utility;
                    best = config;
                }
            }

            return best;
        }
        
        private bool CanAffordWithReserve(AiContext ctx, BuildingConfig config)
        {
            float reserve = 1.2f * ctx.AiBuildings.Count(b => b.BuildingType == config.Type);

            foreach (var cost in config.BuildResourceCost)
            {
                switch (cost.Resource)
                {
                    case ResourceType.Food when ctx.Food < cost.Amount * reserve:
                    case ResourceType.Wood when ctx.Wood < cost.Amount * reserve:
                    case ResourceType.Gold when ctx.Gold < cost.Amount * reserve:
                        return false;
                }
            }
            

            return true;
        }
        
        private bool AlreadyExists(AiContext ctx, BuildingConfig config)
        {
            if (config.Category == BuildingCategory.Military) return false;
            return ctx.AiBuildings.Any(b => b.BuildingType == config.Type);
        }
        
        public float GetUtility(AiContext ctx)
        {
            if (_selectedBuildingConfig == null)
                return 0;

            return CalculateUtility(ctx, _selectedBuildingConfig);
        }

        private float CalculateUtility(AiContext ctx, BuildingConfig config)
        {
            bool isMilitary = config.Category == BuildingCategory.Military;
            bool isEco = config.Category == BuildingCategory.Economy;

            float score = 1f;


            int count = ctx.AiBuildings.Count(b => b.BuildingType == config.Type);
            score *= (1f / (1 + count));


            float totalRes = ctx.Food + ctx.Wood + ctx.Gold;

            float ecoNeedFactor = Mathf.Clamp01(1f - (totalRes / 400f));


            if (isEco)
                score += ecoNeedFactor * 5f;   


            if (isMilitary)
                score -= ecoNeedFactor * 3f;


            if (ctx.EnemyNearBase)
            {
                if (isMilitary) score += 5f;
                if (isEco) score -= 2f;
            }
            
            if (ctx.AiArmyCost < ctx.EnemyArmyCost)
            {
                if (isMilitary) score += 3f;
                if (isEco) score -= 1f;
            }

            return score;
        }

        
        public async void Perform(AiContext ctx)
        {
            if (_selectedBuildingConfig == null)
                return;
            

            var worker = ctx.AiUnits.FirstOrDefault(u => u.UnitType == UnitType.Worker);
            if (worker == null)
                return;

            var buildComponent = worker.GetEntityComponent<UnitBuildingComponent>();

            Vector3 buildPos;
            bool found = TryGetRandomBuildPosition(
                ctx.AiBasePosition,
                25f,
                new Vector3(_selectedBuildingConfig.SizeX, 3, _selectedBuildingConfig.SizeZ),
                out buildPos
            );

            if (!found)
            {
                return;
            }

            _factory.Create(ctx.OwnerId, buildPos, _selectedBuilding);
        }
        
        public static bool TryGetRandomBuildPosition(
            Vector3 center,
            float searchRadius,
            Vector3 buildingSize,  
            out Vector3 result)
        {
            result = Vector3.zero;

            for (int attempts = 0; attempts < 20; attempts++)
            {
                Vector3 random = center + new Vector3(
                    Random.Range(-searchRadius, searchRadius),
                    0,
                    Random.Range(-searchRadius, searchRadius)
                );

                if (!NavMesh.SamplePosition(random, out NavMeshHit hit, 4f, NavMesh.AllAreas))
                    continue;

                Vector3 pos = hit.position;

                if (CanPlaceBuilding(pos, buildingSize))
                {
                    result = pos;
                    return true;
                }
            }

            return false;
        }

        private static bool CanPlaceBuilding(Vector3 pos, Vector3 halfExtents)
        {
            Collider[] hits = Physics.OverlapBox(
                pos,
                halfExtents,
                Quaternion.identity,
                LayerMask.GetMask("Buildings", "Resources")
            );

            return hits.Length == 0;
        }

        public float Cooldown => 10;
        public float LastExecutionTime { get; set; }
    }
}