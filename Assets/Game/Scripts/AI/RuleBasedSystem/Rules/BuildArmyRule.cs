using System.Linq;
using UnityEngine;

namespace Game.Scripts.AI.RuleBasedSystem.Rules
{
    public class BuildArmyRule : IRule
    {
        private AiContext _context;
        
        public UnitType UnitToBuild;       
        public BuildingType ProductionBuilding; 
        public int DesiredCount = 10;      
        public float unitImportance = 1f;

        public RuleCategory Category => RuleCategory.Production;
        public bool IsValid(AiContext ctx)
        {
            var building = ctx.AiBuildings.Find(b => b.BuildingType == ProductionBuilding);
            if (building == null)
                return false;
            
            var prod = building.GetEntityComponent<BuildingUnitsProductionComponent>();
            var config = prod.UnitsAvailableToBuild.FirstOrDefault(u => u.Unit.Config.UnitType == UnitToBuild);
            if (config == null)
                return false;

            if (prod.ProductionQueue.Count >= 3)
                return false;
            
            int countNow = ctx.AiUnits.Count(u => u.UnitType == UnitToBuild);
            if (countNow >= DesiredCount)
                return false;

            return true;
        }

        public float GetUtility(AiContext ctx)
        {
            float armyPressure = 1f - Mathf.Clamp01((float)ctx.AiArmyCost / (ctx.EnemyArmyCost + 1));
            
            return armyPressure;
        }

        public void Perform(AiContext ctx)
        {
            _context = ctx;
            
            var building = ctx.AiBuildings.Find(b => b.BuildingType == ProductionBuilding);
            var prod = building.GetEntityComponent<BuildingUnitsProductionComponent>();
            
            var config = prod.UnitsAvailableToBuild
                .First(u => u.Unit.Config.UnitType == UnitToBuild)
                .Unit.Config;

            prod.TryAddUnitToProductionQueue(config, OnUnitProduced);
        }

        private void OnUnitProduced(ProductionCompleteCallbackArgs args)
        {
            _context.AiPlayer.AiEntitiesController.RegisterMyEntity(args.Entity);
        }
        
        public float Cooldown => 4;
        public float LastExecutionTime { get; set; }
    }
}