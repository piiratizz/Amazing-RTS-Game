using UnityEngine;

namespace Game.Scripts.AI.RuleBasedSystem.Rules
{
    public class ResourceGatherRule : IRule
    {
        public RuleCategory Category => RuleCategory.Economy;

        public bool IsValid(AiContext ctx)
        {
            return true;
        }

        public float GetUtility(AiContext ctx)
        {
            if (ctx.LazyWorkers > 0)
            {
                return 1;
            }
            return 0;
        }

        public void Perform(AiContext ctx)
        {
            Debug.Log("Sending Workers to Gather");
            
            ResourceType targetType = GetLeastWorkersTarget(ctx);
            
            var workers = ctx.AiUnits.FindAll(u => u.UnitType == UnitType.Worker);

            foreach (var worker in workers)
            {
                var state = worker.GetEntityComponent<UnitStateComponent>().CurrentState;
                if (state != UnitState.Idle)
                    continue;
                
                var target = GetNearestResourceOfType(targetType, worker, ctx);
                if (target == null)
                    continue;
                
                worker.GetEntityComponent<UnitCommandDispatcher>().ExecuteCommand(
                    UnitCommandsType.ResourceGather,
                    new ResourceGatherArgs
                    {
                        Resource = target
                    });
            }
        }

        public float Cooldown  => 3;
        public float LastExecutionTime { get; set; }

        private ResourceEntity GetNearestResourceOfType(ResourceType type, Entity worker, AiContext ctx)
        {
            ResourceEntity nearest = null;
            float bestDist = float.MaxValue;

            foreach (var r in ctx.ResourcesOnMap)
            {
                if(r == null)
                    continue;
                
                var source = r.GetEntityComponent<ResourceSourceComponent>();
                
                if(source == null)
                    continue;
                
                if(source.IsEmpty.CurrentValue)
                    continue;
                
                if (source.ResourceType != type)
                    continue;
                
                float dist = (r.transform.position - worker.transform.position).sqrMagnitude;

                if (dist < bestDist)
                {
                    bestDist = dist;
                    nearest = r;
                }
            }

            return nearest;
        }
        
        private ResourceType GetLeastWorkersTarget(AiContext ctx)
        {
            int food = ctx.FoodWorkers;
            int wood = ctx.WoodWorkers;
            int gold = ctx.GoldWorkers;

            if (food <= wood && food <= gold) 
                return ResourceType.Food;

            if (wood <= food && wood <= gold)
                return ResourceType.Wood;

            return ResourceType.Gold;
        }
    }
}