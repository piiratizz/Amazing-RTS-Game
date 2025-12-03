using Game.Scripts.GlobalSystems.GridSystem;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts.AI.RuleBasedSystem.Rules
{
    public class AttackPlayerArmyRule : IRule
    {
        public RuleCategory Category => RuleCategory.Military;

        private HeatMap _heatMap;
        
        public AttackPlayerArmyRule(HeatMap heatMap)
        {
            _heatMap = heatMap;
        }
        
        public bool IsValid(AiContext ctx)
        {
            if (ctx.AiArmyCost <= 0)
            {
                return false;
            }

            if (ctx.EnemyArmy.Count <= 0)
            {
                return false;
            }

            if (ctx.EnemyAttackingEntity)
            {
                return true;
            }
            
            if (ctx.EnemyNearBase)
            {
                return true;
            }
            
            return ctx.EnemyArmyCost * 1.5f < ctx.AiArmyCost;
        }

        public float GetUtility(AiContext ctx)
        {
            if (ctx.EnemyNearBase)
            {
                return 1;
            }
            
            if (ctx.EnemyAttackingEntity)
            {
                return 1;
            }
            
            return (float)ctx.EnemyArmyCost / ctx.AiArmyCost;
        }

        public void Perform(AiContext ctx)
        {
            foreach (var aiUnit in ctx.AiUnits)
            {
                if(aiUnit.UnitType == UnitType.Worker)
                    continue;
                
                if(aiUnit.IsDead) 
                    continue;

                if (aiUnit.GetEntityComponent<UnitStateComponent>().CurrentState == UnitState.Attack)
                {
                    continue;
                }

                int searchRadius = 1000;

                if (ctx.EnemyNearBase)
                {
                    searchRadius = 30;
                }
                
                var heatMapCellPosition = _heatMap.FindClosestEnemyCellWorld(
                    aiUnit.transform.position,
                    ctx.OwnerId,
                    searchRadius,
                    4
                    );
                
                NavMesh.SamplePosition(heatMapCellPosition, out NavMeshHit hit, 10f,  NavMesh.AllAreas);
                
                aiUnit.GetEntityComponent<UnitCommandDispatcher>().ExecuteCommand(UnitCommandsType.Move, new MoveArgs()
                {
                    AllowAutoAttack = true,
                    Position = hit.position
                });
            }
        }

        public float Cooldown => 0.5f;
        public float LastExecutionTime { get; set; }
    }
}