using UnityEngine;

namespace Game.Scripts.AI
{
    public static class AiUtils
    {
        public static bool IsEnemyNearBase(AiContext ctx, float radius)
        {
            float r2 = radius * radius;

            Vector3 basePos = ctx.AiBasePosition;

            foreach (var enemy in ctx.EnemyArmy)
            {
                float dist = (enemy.transform.position - basePos).sqrMagnitude;

                if (dist < r2)
                    return true;
            }

            return false;
        }
    }
}