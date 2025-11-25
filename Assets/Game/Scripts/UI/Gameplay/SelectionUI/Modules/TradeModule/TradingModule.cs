using System.Collections.Generic;
using Game.Scripts.Entities.Components.Buildings;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.TradeModule
{
    public class TradingModule : SelectionPanelModule
    {
        [SerializeField] private Button buyFoodButton;
        [SerializeField] private Button sellFoodButton;
        
        [SerializeField] private Button buyWoodButton;
        [SerializeField] private Button sellWoodButton;

        [SerializeField] private GameObject viewPanel;
        
        private BuildingTradeComponent _tradeComponent;
        private BuildingBuildComponent _buildComponent;

        public override void Initialize(int ownerId)
        {
            buyFoodButton.onClick.AddListener(() => BuyResource(ResourceType.Food, 100));
            sellFoodButton.onClick.AddListener(() => SellResource(ResourceType.Food, 100));
            buyWoodButton.onClick.AddListener(() => BuyResource(ResourceType.Wood, 100));
            sellWoodButton.onClick.AddListener(() => SellResource(ResourceType.Wood, 100));
        }

        public override void Show(List<Entity> targets)
        {
            if(targets.Count > 1 || targets.Count == 0) return;
        
            var target = targets[0];
        
            if (target is not BuildingEntity) return;

            _tradeComponent = target.GetEntityComponent<BuildingTradeComponent>();
            _buildComponent = target.GetEntityComponent<BuildingBuildComponent>();

            if(_tradeComponent == null) return;
            
            if(_buildComponent != null && !_buildComponent.IsBuilded.CurrentValue) return;
            
            viewPanel.SetActive(true);
        }

        private void BuyResource(ResourceType resource, int amount)
        {
            _tradeComponent.BuyResource(resource, amount);
        }

        private void SellResource(ResourceType resource, int amount)
        {
            _tradeComponent.SellResource(resource, amount);
        }
        
        public override void Hide()
        {
            viewPanel.SetActive(false);
        }
    }
}