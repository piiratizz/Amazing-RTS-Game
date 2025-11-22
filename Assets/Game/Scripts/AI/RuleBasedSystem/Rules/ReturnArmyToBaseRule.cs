using System.Linq;
using Game.Scripts.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts.AI.RuleBasedSystem.Rules
{
    public class ReturnArmyToBaseRule : IRule
    {
        public RuleCategory Category => RuleCategory.Military;

        private Vector3 _safePoint;

        public bool IsValid(AiContext ctx)
        {
            if (ctx.EnemyNearBase)
                return true;
            
            if (ctx.EnemyArmyCost <= ctx.AiArmyCost * 1.2f)
                return false;
            
            if (ctx.AiUnits.All(u => u.UnitType == UnitType.Worker))
                return false;
            
            return TryFindSafePoint(ctx, out _safePoint);
        }

        public float GetUtility(AiContext ctx)
        {
            float ratio = (float)ctx.EnemyArmyCost / Mathf.Max(ctx.AiArmyCost, 1);
            return 5f * ratio; 
        }

        public void Perform(AiContext ctx)
        {
            var positions = FormationUtils.GetSquareFormationPositions(_safePoint, ctx.AiUnits, 1);

            for (int i = 0, j = 0; i < ctx.AiUnits.Count; i++)
            {
                var unit = ctx.AiUnits[i];
                
                if(unit == null || unit.IsDead)
                    continue;
                
                if (unit.UnitType == UnitType.Worker)
                    continue;
                
                var dispatcher = unit.GetEntityComponent<UnitCommandDispatcher>();
                dispatcher.ExecuteCommand(UnitCommandsType.Move, new MoveArgs()
                {
                    Position = positions[j]
                });
                j++;
            }
            
            LastExecutionTime = Time.time;
        }

        public float Cooldown => 8f;
        public float LastExecutionTime { get; set; }


        private bool TryFindSafePoint(AiContext ctx, out Vector3 result)
        {
            const float retreatRadius = 25f; 
            const int attempts = 15;

            for (int i = 0; i < attempts; i++)
            {
                Vector2 random = Random.insideUnitCircle * retreatRadius;
                Vector3 candidate = ctx.AiBasePosition + new Vector3(random.x, 0, random.y);
                
                if (NavMesh.SamplePosition(candidate, out var hit, 4f, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }

            result = ctx.AiBasePosition; 
            return false;
        }
    }
}