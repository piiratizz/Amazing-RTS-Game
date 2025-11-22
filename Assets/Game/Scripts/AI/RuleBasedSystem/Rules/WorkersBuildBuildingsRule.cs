
using Game.Scripts.GlobalSystems.GridSystem;
using UnityEngine;

namespace Game.Scripts.AI.RuleBasedSystem.Rules
{
    public class WorkersBuildBuildingsRule : IRule
    {
        private BuildingEntity _buildingNeedToBuild;

        private HeatMap _heatMap;
        
        public RuleCategory Category => RuleCategory.Economy;

        public WorkersBuildBuildingsRule(HeatMap heatMap)
        {
            _heatMap = heatMap;
        }
        
        public bool IsValid(AiContext ctx)
        {
            foreach (var building in ctx.AiBuildings)
            {
                if(building == null)
                    continue;
                
                var healthComponent = building.GetEntityComponent<HealthComponent>();
                
                if(healthComponent == null || healthComponent.IsDead)
                    continue;
                
                if (healthComponent.CurrentHealth.CurrentValue < healthComponent.MaxHealth)
                {
                    _heatMap.GetCell(building.transform.position, out var x, out var z);
                    
                    if(_heatMap.GetEnemyHeat(ctx.OwnerId, x, z) > 0)
                        continue;
                    
                    _buildingNeedToBuild = building;
                    return true;
                }
            }
            return false;
        }

        public float GetUtility(AiContext ctx)
        {
            return 2;
        }

        public void Perform(AiContext ctx)
        {
            Debug.Log("Sending Workers to Build");
            
            var workers = ctx.AiUnits.FindAll(u => u.UnitType == UnitType.Worker);

            var rand = Random.Range(0,  workers.Count / 3);

            int workersOnBuilding = 0;
            workers.ForEach(w =>
            {
                if (w.GetEntityComponent<UnitStateComponent>().CurrentState == UnitState.Building)
                {
                    workersOnBuilding++;
                }
            });
            
            for (int i = 0; i < rand; i++)
            {
                if ((float)workersOnBuilding / workers.Count >= 0.3f)
                {
                    return;
                }
                
                if (workers[i].GetEntityComponent<UnitStateComponent>().CurrentState == UnitState.Building)
                {
                    workersOnBuilding++;
                    continue;
                }
                
                workers[i].GetEntityComponent<UnitCommandDispatcher>().ExecuteCommand(UnitCommandsType.Build, new BuildArgs()
                {
                    Building = _buildingNeedToBuild
                });
            }
        }

        public float Cooldown => 6;
        public float LastExecutionTime { get; set; }
    }
}