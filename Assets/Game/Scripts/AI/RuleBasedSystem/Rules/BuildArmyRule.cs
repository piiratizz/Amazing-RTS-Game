using System.Linq;
using UnityEngine;

namespace Game.Scripts.AI.RuleBasedSystem.Rules
{
    public class BuildArmyRule : IRule
    {
        private AiContext _context;
        
        public UnitType UnitToBuild;       
        public BuildingType ProductionBuilding; 
        public int DesiredCount = 10;      
        public float unitImportance = 1f;

        public RuleCategory Category => RuleCategory.Production;
        public bool IsValid(AiContext ctx)
        {
            var building = ctx.AiBuildings.Find(b => b.BuildingType == ProductionBuilding);

            if (building == null)
                return false;
            
            var prod = building.GetEntityComponent<BuildingUnitsProductionComponent>();
            var build = building.GetEntityComponent<BuildingBuildComponent>();
            if (!build.IsBuilded.CurrentValue)
            {
                return false;
            }
            
            var config = prod.UnitsAvailableToBuild.FirstOrDefault(u => u.Unit.Config.UnitType == UnitToBuild);
            if (config == null)
                return false;

            if (prod.ProductionQueue.Count >= 3)
                return false;
            
            int countNow = ctx.AiUnits.Count(u => u.UnitType == UnitToBuild);
            if (countNow >= DesiredCount)
                return false;

            return true;
        }

        public float GetUtility(AiContext ctx)
        {
            float armyPressure = 1f - Mathf.Clamp01((float)ctx.AiArmyCost / (ctx.EnemyArmyCost + 1));
            
            return armyPressure;
        }

        public void Perform(AiContext ctx)
        {
            _context = ctx;
            
            var buildings = ctx.AiBuildings
                .Where(b => b.BuildingType == ProductionBuilding)
                .ToList();

            if (buildings.Count == 0)
                return;

            foreach (var building in buildings)
            {
                var prod = building.GetEntityComponent<BuildingUnitsProductionComponent>();
                if (prod == null) 
                    continue;

                var build = building.GetEntityComponent<BuildingBuildComponent>();
                if (build != null && !build.IsBuilded.CurrentValue)
                    continue;
                
                var config = prod.UnitsAvailableToBuild
                    .FirstOrDefault(u => u.Unit.Config.UnitType == UnitToBuild);

                if (config == null)
                    continue;
                
                if (prod.ProductionQueue.Count >= 1)
                    continue;
                
                prod.TryAddUnitToProductionQueue(config.Unit.Config, null);
            }
        }

        
        public float Cooldown => 4;
        public float LastExecutionTime { get; set; }
    }
}