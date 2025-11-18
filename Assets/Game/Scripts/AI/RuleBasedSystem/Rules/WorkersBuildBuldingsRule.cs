
using UnityEngine;

namespace Game.Scripts.AI.RuleBasedSystem.Rules
{
    public class WorkersBuildBuldingsRule : IRule
    {
        private BuildingEntity _buildingNeedToBuild;

        public RuleCategory Category => RuleCategory.Economy;

        public bool IsValid(AiContext ctx)
        {
            foreach (var building in ctx.AiBuildings)
            {
                var healthComponent = building.GetEntityComponent<HealthComponent>();
                if (healthComponent.CurrentHealth.CurrentValue < healthComponent.MaxHealth)
                {
                    _buildingNeedToBuild = building;
                    return true;
                }
            }
            return false;
        }

        public float GetUtility(AiContext ctx)
        {
            return 1;
        }

        public void Perform(AiContext ctx)
        {
            Debug.Log("Sending Workers to Build");
            
            var workers = ctx.AiUnits.FindAll(u => u.UnitType == UnitType.Worker);

            var rand = Random.Range(0,  workers.Count / 2);
            for (int i = 0; i < rand; i++)
            {
                if(workers[i].GetEntityComponent<UnitStateComponent>().CurrentState == UnitState.Building)
                    continue;
                
                workers[i].GetEntityComponent<UnitCommandDispatcher>().ExecuteCommand(UnitCommandsType.Build, new BuildArgs()
                {
                    Building = _buildingNeedToBuild
                });
            }
        }

        public float Cooldown => 2;
        public float LastExecutionTime { get; set; }
    }
}