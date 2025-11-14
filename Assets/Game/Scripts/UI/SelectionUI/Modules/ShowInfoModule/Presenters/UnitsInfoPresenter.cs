using System;
using System.Collections.Generic;
using System.Globalization;
using ComponentsActionTypes;
using NTC.Pool;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Modules.Presenters
{
    public class UnitsInfoPresenter : IEntityInfoPresenter
    {
        private Player _player;
        
        private UnitInfoPanelView _unitInfoPanelViewPrefab;
        private Transform _unitInfoPanelUIContainer;

        private Slider _hpSlider;
        private TextMeshProUGUI _hpText;

        private TextMeshProUGUI _unitNameText;
        private Image _unitIconImage;

        private GameObject _statsContainer;
        private TextMeshProUGUI _attackStatsText;
        private TextMeshProUGUI _armorStatsText;
        private TextMeshProUGUI _speedStatsText;
        private TextMeshProUGUI _rangeStatsText;

        private Transform _upgradesContainer;
        private UpgradeView _upgradeView;
        private List<UpgradeView> _upgradesInstances = new List<UpgradeView>();

        private GlobalUpgradesManager _globalUpgradesManager;
        private HealthComponent _singleSelectedUnitHealthComponent;

        private readonly Dictionary<string, UnitInfoPanelView> _panelsInstances = new Dictionary<string, UnitInfoPanelView>();

        private readonly Dictionary<string, Entity> _groupedEntities = new Dictionary<string, Entity>(10);
        private readonly Dictionary<string, int> _entitiesCount = new Dictionary<string, int>(10);
        
        private CompositeDisposable _subscriptions = new CompositeDisposable();
        
        public UnitsInfoPresenter(
            UnitInfoPanelView unitInfoPanelViewPrefab,
            Transform unitInfoPanelUIContainer,
            Slider hpSlider,
            TextMeshProUGUI hpText,
            TextMeshProUGUI unitNameText,
            Image unitIconImage,
            GameObject statsContainer,
            TextMeshProUGUI attackStatsText,
            TextMeshProUGUI armorStatsText,
            TextMeshProUGUI speedStatsText,
            TextMeshProUGUI rangeStatsText,
            Player player,
            Transform upgradesContainer,
            UpgradeView upgradeView,
            GlobalUpgradesManager globalUpgradesManager)
        {
            _unitInfoPanelViewPrefab = unitInfoPanelViewPrefab;
            _unitInfoPanelUIContainer = unitInfoPanelUIContainer;
            _hpSlider = hpSlider;
            _hpText = hpText;
            _unitNameText = unitNameText;
            _unitIconImage = unitIconImage;
            _statsContainer = statsContainer;
            _attackStatsText = attackStatsText;
            _armorStatsText = armorStatsText;
            _speedStatsText = speedStatsText;
            _rangeStatsText = rangeStatsText;
            _upgradesContainer = upgradesContainer;
            _upgradeView = upgradeView;
            _player = player;
            _globalUpgradesManager = globalUpgradesManager;
        }


        public void Show(List<Entity> entities)
        {
            if (entities.Count == 1)
            {
                ShowSingleUnit(entities[0]);
            }
            else
            {
                ShowMultiUnits(entities);
            }
        }

        private void ShowMultiUnits(List<Entity> targets)
        {
            _groupedEntities.Clear();
            _entitiesCount.Clear();

            _hpSlider.gameObject.SetActive(false); 
            _hpText.gameObject.SetActive(false);
            _unitNameText.gameObject.SetActive(false);
            _unitIconImage.gameObject.SetActive(false);
            _statsContainer.SetActive(false);
            
            foreach (var target in targets)
            {
                if (target as UnitEntity == null)
                    continue;

                if (!_groupedEntities.ContainsKey(target.DisplayName))
                {
                    _groupedEntities.Add(target.DisplayName, target);
                    _entitiesCount.Add(target.DisplayName, 1);
                }
                else
                {
                    _entitiesCount[target.DisplayName]++;
                }
            }

            foreach (var entityGroup in _groupedEntities)
            {
                if (!_panelsInstances.ContainsKey(entityGroup.Key))
                {
                    var instance = CreateInfoPanelView(entityGroup.Key, entityGroup.Value, OnUnitGroupClicked);
                    _panelsInstances.Add(entityGroup.Key, instance);
                }
                else
                {
                    var instance = _panelsInstances[entityGroup.Key];
                    instance.gameObject.SetActive(true);
                    instance.UpdateCount(_entitiesCount[entityGroup.Key]);
                }
            }
        }

        private void OnUnitGroupClicked(string clickedEntityName)
        {
            _player.PlayerSelectionManager.RemoveAllFromSelection(e => e.DisplayName != clickedEntityName);
        }

        private void ShowSingleUnit(Entity target)
        {
            var unit = target as UnitEntity;

            if (unit == null)
            {
                Debug.LogError("Target is not a UnitEntity. Cannot display single unit info.");
                return;
            }
            
            _hpSlider.gameObject.SetActive(true);
            _hpText.gameObject.SetActive(true);
            _unitNameText.gameObject.SetActive(true);
            _unitIconImage.gameObject.SetActive(true);
            _statsContainer.SetActive(true);

            var upgrades = _globalUpgradesManager.GetByEntityType(unit.OwnerId, unit.EntityType);
            
            foreach (var upgrade in upgrades)
            {
                if (upgrade.IsCanBeAppliedOnEntity(unit))
                {
                    var upgradeInstance = NightPool.Spawn(_upgradeView, _upgradesContainer);
                    upgradeInstance.Initialize(upgrade);
                    _upgradesInstances.Add(upgradeInstance);
                }
            }
            
            _singleSelectedUnitHealthComponent = target.GetEntityComponent<HealthComponent>();
        
            if(_singleSelectedUnitHealthComponent == null) return;
            
            UpdateHealth(_singleSelectedUnitHealthComponent.CurrentHealth.CurrentValue);
            _unitIconImage.sprite = unit.Icon;
            _unitNameText.text = unit.DisplayName;

            string attackText = $"{unit.Damage}";
            
            var attackable = target.GetFirstComponentByInterface<IAttackable>();
            
            if (attackable != null)
            {
                if (attackable.BonusDamage != 0)
                {
                    attackText = $"{unit.Damage}(+{attackable.BonusDamage})";
                }
            }
            
            _attackStatsText.text = attackText;
            _armorStatsText.text = unit.Armor.ToString();
            _speedStatsText.text = unit.Speed.ToString(CultureInfo.CurrentCulture);
            _rangeStatsText.text = unit.Range.ToString(CultureInfo.CurrentCulture);
            
            _singleSelectedUnitHealthComponent.CurrentHealth.Subscribe(UpdateHealth).AddTo(_subscriptions);
        }

        private void UpdateHealth(int health)
        {
            if (_singleSelectedUnitHealthComponent != null)
            {
                _hpSlider.maxValue = _singleSelectedUnitHealthComponent.MaxHealth;
                _hpSlider.value = health;
                _hpText.text = $"{health} / {_singleSelectedUnitHealthComponent.MaxHealth}";
            }
        }

        private UnitInfoPanelView CreateInfoPanelView(string entityName, Entity selectable, UnityAction<string> onClick)
        {
            var instance = UnityEngine.Object.Instantiate(_unitInfoPanelViewPrefab, _unitInfoPanelUIContainer);
            instance.Initialize(entityName, _entitiesCount[entityName], selectable.Icon, onClick);
            return instance;
        }
        
        public void Hide()
        {
            foreach (var instance in _panelsInstances.Values)
            {
                instance.gameObject.SetActive(false);
            }
            
            _upgradesInstances.ForEach(i => NightPool.Despawn(i));
            _upgradesInstances.Clear();
            
            _hpSlider.gameObject.SetActive(false);
            _hpText.gameObject.SetActive(false);
            _unitNameText.gameObject.SetActive(false);
            _unitIconImage.gameObject.SetActive(false);
            _statsContainer.gameObject.SetActive(false);
            
            _subscriptions.Clear();
        }
    }
}