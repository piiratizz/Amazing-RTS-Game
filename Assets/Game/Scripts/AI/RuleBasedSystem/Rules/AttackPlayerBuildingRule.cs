using System.Linq;
using Game.Scripts.GlobalSystems.GridSystem;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts.AI.RuleBasedSystem.Rules
{
    public class AttackPlayerBuildingRule : IRule
    {
        public RuleCategory Category => RuleCategory.Military;

        private readonly HeatMap _heatMap;

        public AttackPlayerBuildingRule(HeatMap heatMap)
        {
            _heatMap = heatMap;
        }
        
        public bool IsValid(AiContext ctx)
        {
            if (ctx.AiArmyCost <= 0)
                return false;
            
            bool enemyHasBuildings = ctx.EnemyBuildings.Count > 0;
            if (!enemyHasBuildings)
                return false;
            
            if (ctx.EnemyNearBase)
                return false;
            
            return ctx.EnemyArmyCost <= 0;
        }

        public float GetUtility(AiContext ctx)
        {
            if(ctx.EnemyArmyCost <= 0)
                return 2;

            return 0;
        }

        public void Perform(AiContext ctx)
        {
            var enemyBuildings = ctx.EnemyBuildings
                .Where(u => !u.IsDead)
                .Select(b => b)
                .ToList();

            if (enemyBuildings.Count == 0)
                return;
            
            var targetBuilding = enemyBuildings
                .OrderBy(b => (b.transform.position - ctx.AiBasePosition).sqrMagnitude)
                .FirstOrDefault();

            if (targetBuilding == null)
                return;

            Vector3 targetPos = targetBuilding.transform.position;
            
            Vector3 heatTarget = _heatMap.FindClosestEnemyCellWorld(
                targetPos,
                ctx.OwnerId,
                80f,
                2f,
                0.2f,
                20f,
                6f
            );
            
            Vector3 attackPoint = heatTarget == Vector3.negativeInfinity ? targetPos : heatTarget;
            
            NavMesh.SamplePosition(attackPoint, out NavMeshHit hit, 8f, NavMesh.AllAreas);
            
            foreach (var aiUnit in ctx.AiUnits)
            {
                if (aiUnit.UnitType == UnitType.Worker || aiUnit.UnitType == UnitType.Archer)
                    continue;

                if (aiUnit.IsDead)
                    continue;

                var state = aiUnit.GetEntityComponent<UnitStateComponent>().CurrentState;

                if (state == UnitState.Attack)
                    continue;
                
                aiUnit.GetEntityComponent<UnitCommandDispatcher>().ExecuteCommand(
                    UnitCommandsType.Attack,
                    new AttackArgs
                    {
                        Entity = targetBuilding,
                        TotalUnits = 1,
                        UnitOffsetIndex = 0
                    }
                );
            }
        }

        public float Cooldown => 3f;
        public float LastExecutionTime { get; set; }
    }
}
