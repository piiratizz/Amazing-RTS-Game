using System.Linq;
using UnityEngine;

namespace Game.Scripts.AI.RuleBasedSystem.Rules
{
    public class BuildWorkersRule : IRule
    {
        private BuildingEntity _townhall;
        private AiContext _context;

        public RuleCategory Category => RuleCategory.Production;

        public bool IsValid(AiContext ctx)
        {
            return ctx.Food > 10;
        }

        public float GetUtility(AiContext ctx)
        {
            float ratio = (float)ctx.Workers / ctx.MaxWorkers;
            return 1f - Mathf.Clamp01(ratio);
        }

        public void Perform(AiContext ctx)
        {
            _context = ctx;

            _townhall = ctx.AiBuildings.Find(b => b.BuildingType == BuildingType.Townhall);
            var productionComponent = _townhall.GetEntityComponent<BuildingUnitsProductionComponent>();
            productionComponent.TryAddUnitToProductionQueue(
                productionComponent.UnitsAvailableToBuild.First(c => c.Unit.Config.UnitType == UnitType.Worker).Unit
                    .Config, RegisterWorker);
        }

        public float Cooldown => 5;
        public float LastExecutionTime { get; set; }

        private void RegisterWorker(ProductionCompleteCallbackArgs args)
        {
            _context.AiPlayer.AiEntitiesController.RegisterMyEntity(args.Entity);
        }
    }
}