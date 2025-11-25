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
            if(ctx.AiBuildings.Count == 0) return false;
            
            _townhall = ctx.AiBuildings.Find(b => b.BuildingType == BuildingType.Townhall);
            var prod = _townhall.GetEntityComponent<BuildingUnitsProductionComponent>();
            var config = prod.UnitsAvailableToBuild.FirstOrDefault(u => u.Unit.Config.UnitType == UnitType.Worker);
            if (config == null)
                return false;

            if (prod.ProductionQueue.Count >= 1)
                return false;
            
            return ctx.Food > 50;
        }

        public float GetUtility(AiContext ctx)
        {
            float ratio = (float)ctx.Workers / ctx.MaxWorkers;
            return 1f - Mathf.Clamp01(ratio);
        }

        public void Perform(AiContext ctx)
        {
            _context = ctx;
            var productionComponent = _townhall.GetEntityComponent<BuildingUnitsProductionComponent>();
            productionComponent.TryAddUnitToProductionQueue(
                productionComponent.UnitsAvailableToBuild.First(c => c.Unit.Config.UnitType == UnitType.Worker).Unit
                    .Config, null);
        }

        public float Cooldown => 5;
        public float LastExecutionTime { get; set; }
    }
}