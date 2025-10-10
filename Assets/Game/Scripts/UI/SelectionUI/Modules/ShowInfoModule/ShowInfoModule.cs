using System;
using System.Collections.Generic;
using System.Globalization;
using R3;
using Zenject;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Modules
{
    public class ShowInfoModule : SelectionPanelModule
    {
        [SerializeField] private UnitInfoPanelView unitInfoPanelViewPrefab;
        [SerializeField] private Transform unitInfoPanelUIContainer;
        [SerializeField] private GameObject background;
        
        [SerializeField] private Slider hpSlider;
        [SerializeField] private TextMeshProUGUI hpText;
        
        [SerializeField] private TextMeshProUGUI unitNameText;
        [SerializeField] private Image unitIconImage;

        [SerializeField] private GameObject statsContainer;
        [SerializeField] private TextMeshProUGUI attackStatsText;
        [SerializeField] private TextMeshProUGUI armorStatsText;
        [SerializeField] private TextMeshProUGUI speedStatsText;
        [SerializeField] private TextMeshProUGUI rangeStatsText;
        
        [Inject] private Player _player;
        
        private readonly Dictionary<string, UnitInfoPanelView> _panelsInstances = new Dictionary<string, UnitInfoPanelView>();
        
        private readonly Dictionary<string, Entity> _groupedEntities = new Dictionary<string, Entity>(10);
        private readonly Dictionary<string, int> _entitiesCount = new Dictionary<string, int>(10);
        
        private HealthComponent _singleSelectedUnitHealthComponent;
        
        private int _baseResourcesAmount;
        private readonly CompositeDisposable _resourcesSubscription = new();
        
        public override void Show(List<Entity> targets)
        {
            _player.PlayerSelectionManager.OnSelectionChanged += UpdateView;
            UpdateView(targets);
        }
        
        private void UpdateView(List<Entity> targets)
        {
            if(targets.Count == 0) return;
            
            background.SetActive(true);

            bool isUnit = false;
            
            foreach (var target in targets)
            {
                if (target.EntityType == EntityType.Unit)
                {
                    isUnit = true;
                }
            }

            if (isUnit)
            {
                ShowUnits(targets);
                return;
            }
            
            if(targets.Count == 1)
            {
                if (targets[0] is ResourceEntity resourceEntity)
                {
                    ShowResource(resourceEntity);
                }
                if (targets[0] is BuildingEntity buildingEntity)
                {
                    ShowBuilding(buildingEntity);
                }
            }
        }
        
        private void ShowResource(ResourceEntity target)
        {
            hpSlider.gameObject.SetActive(true);
            hpText.gameObject.SetActive(true);
            unitNameText.gameObject.SetActive(true);
            unitIconImage.gameObject.SetActive(true);

            var resourceStorage = target.GetEntityComponent<ResourceStorageComponent>();
            _baseResourcesAmount = resourceStorage.BaseAmount;
            hpSlider.maxValue = _baseResourcesAmount;
            resourceStorage.Amount.Subscribe(UpdateResourcesCount).AddTo(_resourcesSubscription);
            unitIconImage.sprite = target.Icon;
            unitNameText.text = target.DisplayName;
        }

        private void UpdateResourcesCount(int amount)
        {
            hpText.text = $"{amount} / {_baseResourcesAmount}";
            hpSlider.value = amount;
        }
        
        private void ShowUnits(List<Entity> targets)
        {
            if (targets.Count > 1)
            {
                ShowMultiUnits(targets);
            }
            else
            {
                ShowSingleUnit(targets[0]);
            }
        }
        
        private void ShowMultiUnits(List<Entity> targets)
        {
            _groupedEntities.Clear();
            _entitiesCount.Clear();
            
            foreach (var target in targets)
            {
                if(target.EntityType != EntityType.Unit) 
                    continue;
                
                var count = GetSelectablesCount(target);
                if (count == 0)
                {
                    _entitiesCount.Add(target.DisplayName, 1);
                    _groupedEntities.Add(target.DisplayName, target);
                }
                else
                {
                    _entitiesCount[target.DisplayName]++;
                }
            }
            
            foreach (var entity in _groupedEntities)
            {
                if (!_panelsInstances.ContainsKey(entity.Key))
                {
                    var instance = CreateInfoPanelView(entity.Key, entity.Value);
                    _panelsInstances.Add(entity.Key, instance);
                }
                else
                {
                    var instance = _panelsInstances[entity.Key];
                    instance.gameObject.SetActive(true);
                    instance.UpdateCount(_entitiesCount[entity.Key]);
                }
            }
        }

        private void ShowSingleUnit(Entity target)
        {
            var unit = target as UnitEntity;

            if (unit == null)
            {
                throw new InvalidCastException("target is not a UnitEntity");
            }
            
            hpSlider.gameObject.SetActive(true);
            hpText.gameObject.SetActive(true);
            unitNameText.gameObject.SetActive(true);
            unitIconImage.gameObject.SetActive(true);
            statsContainer.SetActive(true);
            
            var isDamageable = target.TryGetComponent(out _singleSelectedUnitHealthComponent);
            
            if(!isDamageable) return;

            UpdateHealth(_singleSelectedUnitHealthComponent.CurrentHealth);
            unitIconImage.sprite = unit.Icon;
            unitNameText.text = unit.DisplayName;
            attackStatsText.text = unit.Damage.ToString();
            armorStatsText.text = unit.Armor.ToString();
            speedStatsText.text = unit.Speed.ToString(CultureInfo.CurrentCulture);
            rangeStatsText.text = unit.Range.ToString(CultureInfo.CurrentCulture);
            
            _singleSelectedUnitHealthComponent.OnHealthChanged += UpdateHealth;
        }

        private void ShowBuilding(BuildingEntity building)
        {
            hpSlider.gameObject.SetActive(true);
            hpText.gameObject.SetActive(true);
            unitNameText.gameObject.SetActive(true);
            unitIconImage.gameObject.SetActive(true);

            var isDamageable = building.TryGetComponent(out _singleSelectedUnitHealthComponent);
            
            if(!isDamageable) return;

            UpdateHealth(_singleSelectedUnitHealthComponent.CurrentHealth);
            unitIconImage.sprite = building.Icon;
            unitNameText.text = building.DisplayName;
            _singleSelectedUnitHealthComponent.OnHealthChanged += UpdateHealth;
        }
        
        private void UpdateHealth(int health)
        {
            hpSlider.maxValue = _singleSelectedUnitHealthComponent.MaxHealth;
            hpSlider.value = health;
            hpText.text = $"{health} / {_singleSelectedUnitHealthComponent.MaxHealth}";
        }
        
        public override void Hide()
        {
            _player.PlayerSelectionManager.OnSelectionChanged -= UpdateView;

            if (_singleSelectedUnitHealthComponent != null)
            {
                _singleSelectedUnitHealthComponent.OnHealthChanged -= UpdateHealth;
            }
            
            
            foreach (var instance in _panelsInstances.Values)
            {
                instance.gameObject.SetActive(false);
            }
            
            background.SetActive(false);
            hpSlider.gameObject.SetActive(false);
            hpText.gameObject.SetActive(false);
            unitNameText.gameObject.SetActive(false);
            unitIconImage.gameObject.SetActive(false);
            statsContainer.SetActive(false);
            
            _resourcesSubscription?.Clear();
        }
        
        // UTILS
        
        private UnitInfoPanelView CreateInfoPanelView(string entityName, Entity selectable)
        {
            var instance = Instantiate(unitInfoPanelViewPrefab, unitInfoPanelUIContainer);
            instance.Initialize(_entitiesCount[entityName], selectable.Icon);
            return instance;
        }

        private int GetSelectablesCount(Entity target)
        {
            if (_entitiesCount.ContainsKey(target.DisplayName))
            {
                return _entitiesCount[target.DisplayName];
            }
            return 0;
        }
    }
}