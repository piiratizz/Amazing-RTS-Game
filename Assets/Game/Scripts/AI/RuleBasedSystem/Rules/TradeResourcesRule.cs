using System.Linq;
using Game.Scripts.Entities.Components.Buildings;
using UnityEngine;

namespace Game.Scripts.AI.RuleBasedSystem.Rules
{
    public class TradeResourcesRule : IRule
    {
        public RuleCategory Category => RuleCategory.Economy;

        public float Cooldown => 6f;
        public float LastExecutionTime { get; set; }

        public bool IsValid(AiContext ctx)
        {
            // 1. Есть ли рынок
            var market = ctx.AiBuildings
                .Find(b => b.BuildingType == BuildingType.Market);
            Debug.Log(market);
            if (market == null)
                return false;

            var trade = market.GetEntityComponent<BuildingTradeComponent>();
            Debug.Log(trade);
            if (trade == null)
                return false;

            // 2. СЧИТАЕМ СРЕДНЕЕ — AAA-подход
            float avg = (ctx.Food + ctx.Wood + ctx.Gold) / 3f;

            // 3. Проверяем СУЩЕСТВЕННЫЙ дисбаланс
            bool deficit =
                ctx.Food < avg * 0.5f ||
                ctx.Wood < avg * 0.5f ||
                ctx.Gold < avg * 0.5f;
            Debug.Log(deficit);
            // bool excess =
            //     ctx.Food > avg * 1.5f ||
            //     ctx.Wood > avg * 1.5f ||
            //     ctx.Gold > avg * 1.5f;
            // Debug.Log(excess);
            return deficit;
        }

        public float GetUtility(AiContext ctx)
        {
            float avg = (ctx.Food + ctx.Wood + ctx.Gold) / 3f;

            float foodDef = Mathf.Clamp01((avg - ctx.Food) / avg);
            float woodDef = Mathf.Clamp01((avg - ctx.Wood) / avg);
            float goldDef = Mathf.Clamp01((avg - ctx.Gold) / avg);

            float maxDeficit = Mathf.Max(foodDef, woodDef, goldDef);
            
            return maxDeficit * 1.5f;
        }

        public void Perform(AiContext ctx)
        {
            Debug.Log("RESOURCE TRADED !!");
            var market = ctx.AiBuildings
                .Find(b => b.BuildingType == BuildingType.Market);

            if (market == null)
                return;

            var trade = market.GetEntityComponent<BuildingTradeComponent>();
            if (trade == null)
                return;

            float avg = (ctx.Food + ctx.Wood + ctx.Gold) / 3f;
            
            ResourceType deficit = ResourceType.Food;
            float minValue = ctx.Food;

            if (ctx.Wood < minValue)
            {
                minValue = ctx.Wood;
                deficit = ResourceType.Wood;
            }
            if (ctx.Gold < minValue)
            {
                minValue = ctx.Gold;
                deficit = ResourceType.Gold;
            }
            
            ResourceType donor = ResourceType.Food;
            float maxValue = ctx.Food;

            if (ctx.Wood > maxValue)
            {
                maxValue = ctx.Wood;
                donor = ResourceType.Wood;
            }
            if (ctx.Gold > maxValue)
            {
                maxValue = ctx.Gold;
                donor = ResourceType.Gold;
            }
            
            if (minValue > avg * 0.5f)
                return;

            if (maxValue < avg * 1.5f)
                return;

            int tradeAmount = Mathf.Max(
                20,
                Mathf.FloorToInt((maxValue - minValue) * 0.25f)
            );
            
            if (deficit != ResourceType.Gold && donor == ResourceType.Gold)
            {
                trade.BuyResource(deficit, tradeAmount);
            }
            else if (donor != ResourceType.Gold)
            {
                trade.SellResource(donor, tradeAmount);
                
                if (deficit != ResourceType.Gold)
                {
                    trade.BuyResource(deficit, tradeAmount);
                }
            }
        }
    }
}
