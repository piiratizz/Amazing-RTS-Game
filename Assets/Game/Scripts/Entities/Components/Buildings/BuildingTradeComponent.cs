using GlobalResourceStorageSystem;
using Zenject;

namespace Game.Scripts.Entities.Components.Buildings
{
    public class BuildingTradeComponent : EntityComponent
    {
        [Inject] private ResourcesStoragesManager _resourcesStoragesManager;
        private GlobalResourceStorage _globalResourceStorage;
        private BuildingConfig _config;

        public override void Init(Entity entity)
        {
            _globalResourceStorage = _resourcesStoragesManager.Get(entity.OwnerId);
        }

        public override void InitializeFields(EntityConfig config)
        {
            _config = config as BuildingConfig;

            if (_config == null)
            {
                throw new System.Exception("Invalid building config");
            }
        }

        public void SellResource(ResourceType resource, int amount)
        {
            if (_globalResourceStorage.TrySpend(resource, amount))
            {
                var gold = amount / _config.ResourceAmountForOneGold;
                _globalResourceStorage.Add(ResourceType.Gold, gold);
            }
        }
        
        public void BuyResource(ResourceType resource, int amount)
        {
            var gold = amount / _config.ResourceAmountForOneGold;
            
            if (_globalResourceStorage.TrySpend(ResourceType.Gold, gold))
            {
                _globalResourceStorage.Add(resource, amount);
            }
        }
    }
}