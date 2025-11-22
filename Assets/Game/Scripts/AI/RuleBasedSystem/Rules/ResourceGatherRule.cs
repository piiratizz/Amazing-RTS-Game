using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.AI.RuleBasedSystem.Rules
{
    public class ResourceGatherRule : IRule
    {
        public RuleCategory Category => RuleCategory.Economy;

        public bool IsValid(AiContext ctx)
        {
            return ctx.Workers > 0;
        }

        public float GetUtility(AiContext ctx)
        {
            if (ctx.LazyWorkers > 0)
                return 3f;

            if (ctx.FoodWorkers == 0 || ctx.WoodWorkers == 0 || ctx.GoldWorkers == 0)
                return 2f;

            int max = Mathf.Max(ctx.FoodWorkers, ctx.WoodWorkers, ctx.GoldWorkers);
            int min = Mathf.Min(ctx.FoodWorkers, ctx.WoodWorkers, ctx.GoldWorkers);

            if (max - min >= 2)
                return 1.5f;

            return 0.3f;
        }

        public void Perform(AiContext ctx)
        {
            ResourceType targetType = GetBalancedResourceTarget(ctx);

            int food = ctx.FoodWorkers;
            int wood = ctx.WoodWorkers;
            int gold = ctx.GoldWorkers;

            int min = Mathf.Min(food, Mathf.Min(wood, gold));

            bool excessFood = (food - min) >= 2 && targetType != ResourceType.Food;
            bool excessWood = (wood - min) >= 2 && targetType != ResourceType.Wood;
            bool excessGold = (gold - min) >= 2 && targetType != ResourceType.Gold;

            var workers = ctx.AiUnits.FindAll(u => u.UnitType == UnitType.Worker);

            List<UnitEntity> idleWorkers = new();
            List<UnitEntity> donorWorkers = new();

            foreach (var worker in workers)
            {
                var state = worker.GetEntityComponent<UnitStateComponent>().CurrentState;
                if (state == UnitState.Building)
                    continue;
                
                var gatherComp = worker.GetEntityComponent<UnitResourceGatherComponent>();
                ResourceType? currentResource = null;

                if (gatherComp != null && gatherComp.IsGathering.Item1)
                    currentResource = gatherComp.IsGathering.Item2;
                
                if (currentResource == targetType)
                    continue;
                
                if (state == UnitState.Idle)
                {
                    idleWorkers.Add(worker);
                    continue;
                }
                
                bool isDonor =
                    (excessFood && currentResource == ResourceType.Food) ||
                    (excessWood && currentResource == ResourceType.Wood) ||
                    (excessGold && currentResource == ResourceType.Gold);

                if (isDonor)
                    donorWorkers.Add(worker);
            }
            
            int movedCount = 0;
            const int maxWorkersToMove = 2;

            foreach (var worker in idleWorkers)
            {
                if (SendToResource(worker, ctx, targetType))
                    movedCount++;
            }

            foreach (var worker in donorWorkers)
            {
                if (movedCount >= maxWorkersToMove)
                    return;

                if (SendToResource(worker, ctx, targetType))
                    movedCount++;
            }
        }

        private bool SendToResource(UnitEntity worker, AiContext ctx, ResourceType targetType)
        {
            var target = GetNearestResourceOfType(targetType, worker, ctx);
            if (target == null)
                return false;

            worker.GetEntityComponent<UnitCommandDispatcher>().ExecuteCommand(
                UnitCommandsType.ResourceGather,
                new ResourceGatherArgs { Resource = target });

            return true;
        }


        public float Cooldown => 5;
        public float LastExecutionTime { get; set; }

        private ResourceEntity GetNearestResourceOfType(ResourceType type, Entity worker, AiContext ctx)
        {
            ResourceEntity nearest = null;
            float bestDist = float.MaxValue;

            foreach (var r in ctx.ResourcesOnMap)
            {
                if (r == null) continue;

                var source = r.GetEntityComponent<ResourceSourceComponent>();
                if (source == null) continue;
                if (source.IsEmpty.CurrentValue) continue;
                if (source.ResourceType != type) continue;

                float dist = (r.transform.position - worker.transform.position).sqrMagnitude;

                if (dist < bestDist)
                {
                    bestDist = dist;
                    nearest = r;
                }
            }

            return nearest;
        }

        private ResourceType GetBalancedResourceTarget(AiContext ctx)
        {
            int food = ctx.FoodWorkers;
            int wood = ctx.WoodWorkers;
            int gold = ctx.GoldWorkers;

            if (food == 0) return ResourceType.Food;
            if (wood == 0) return ResourceType.Wood;
            if (gold == 0) return ResourceType.Gold;

            int min = Mathf.Min(food, Mathf.Min(wood, gold));

            if (food == min) return ResourceType.Food;
            if (wood == min) return ResourceType.Wood;
            return ResourceType.Gold;
        }
    }
}