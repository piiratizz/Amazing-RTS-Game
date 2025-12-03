using System;
using System.Collections.Generic;
using System.Linq;
using NTC.Pool;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Modules.Presenters
{
    public class BuildingsInfoPresenter : IEntityInfoPresenter
    {
        private readonly Slider _buildingHpSlider;
        private readonly TextMeshProUGUI _buildingHpText;
        private readonly TextMeshProUGUI _buildingNameText;
        private readonly Image _buildingIconImage;
        private readonly GameObject _buildingIconFrame;

        private GameObject _upgradeResourceCostPanel;
        private ResourceCostView _resourceCostViewPrefab;
        
        private CompositeDisposable _subscriptions;
        
        private BuildingEntity _target;
        
        private HealthComponent _buildingHealthComponent;
        private BuildingUpgradeComponent _buildingUpgradeComponent;
        private BuildingBuildComponent _buildingBuildComponent;
        private PointerHandler _pointerHandler;
        
        private Button _upgradeButton;

        private List<ResourceCostView> _resourceCostViewsInstances;

        private ResourceData[] _resources;
        
        public BuildingsInfoPresenter(
            Slider buildingHpSlider,
            TextMeshProUGUI buildingHpText,
            TextMeshProUGUI buildingNameText,
            Image buildingIconImage,
            GameObject buildingIconFrame,
            Button upgradeButton,
            PointerHandler pointerHandler,
            GameObject upgradeResourceCostPanel,
            ResourceCostView resourceCostViewPrefab)
        {
            _buildingHpSlider =  buildingHpSlider;
            _buildingHpText = buildingHpText;
            _buildingNameText = buildingNameText;
            _buildingIconImage =  buildingIconImage;
            _upgradeButton = upgradeButton;
            _pointerHandler = pointerHandler;
            _upgradeResourceCostPanel = upgradeResourceCostPanel;
            _resourceCostViewPrefab = resourceCostViewPrefab;
            _buildingIconFrame = buildingIconFrame;
            
            _resourceCostViewsInstances = new List<ResourceCostView>();
            _subscriptions = new CompositeDisposable();

            _resources = Resources.LoadAll<ResourceData>("ResourcesData");
        }
        
        public void Show(List<Entity> entities)
        {
            if(entities.Count > 1) return;
            
            _target = entities[0] as BuildingEntity;

            if (_target == null)
            {
                throw new InvalidCastException("Entity must be BuildingEntity");
            }
            
            _buildingUpgradeComponent = _target.GetEntityComponent<BuildingUpgradeComponent>();
            _buildingBuildComponent = _target.GetEntityComponent<BuildingBuildComponent>();
            _buildingHealthComponent = _target.GetEntityComponent<HealthComponent>();

            if (_buildingBuildComponent != null)
            {
                if (_buildingBuildComponent.IsBuilded.CurrentValue != true)
                {
                    _buildingBuildComponent.IsBuilded.Where(b => b == true).Subscribe(_ =>
                    {
                        UpdateFields();
                    }).AddTo(_subscriptions);
                }
            }
            
            _buildingHpSlider.gameObject.SetActive(true);
            _buildingHpText.gameObject.SetActive(true);
            _buildingNameText.gameObject.SetActive(true);
            _buildingIconImage.gameObject.SetActive(true);
            _buildingIconFrame.gameObject.SetActive(true);

            _pointerHandler.PointerEnterEvent += OnUpgradeButtonPointerEnter;
            _pointerHandler.PointerExitEvent += OnUpgradeButtonPointerExit;
            
            UpdateFields();
            
            if(_buildingHealthComponent == null) return;
            _buildingHealthComponent.CurrentHealth.Subscribe(UpdateHealth).AddTo(_subscriptions);
        }

        private void OnUpgradeButtonPointerEnter()
        {
            var upgradeCost = _buildingUpgradeComponent.GetCostOfUpgrade();

            if (upgradeCost == null)
            {
                return;
            }
            
            _upgradeResourceCostPanel.SetActive(true);
            
            foreach (var cost in upgradeCost)
            {
                var instance = NightPool.Spawn(_resourceCostViewPrefab, _upgradeResourceCostPanel.transform);
                var data = _resources.First(d => d.ResourceType == cost.Resource);
                instance.Initialize(data.UiDisplayIcon, cost.Amount);
                _resourceCostViewsInstances.Add(instance);
            }
        }
        
        private void OnUpgradeButtonPointerExit()
        {
            _upgradeResourceCostPanel.SetActive(false);

            foreach (var instance in _resourceCostViewsInstances)
            {
                NightPool.Despawn(instance);
            }
            _resourceCostViewsInstances.Clear();
        }
        
        private void UpdateFields()
        {
            _upgradeButton.onClick.RemoveAllListeners();
            
            if (_buildingUpgradeComponent != null)
            {
                var canShowUpgradeButton = false;
                if (_buildingBuildComponent != null)
                {
                    canShowUpgradeButton = _buildingBuildComponent.IsBuilded.CurrentValue;
                }
                else
                {
                    canShowUpgradeButton = true;
                }

                if (_buildingUpgradeComponent.IsFullUpgraded())
                {
                    canShowUpgradeButton = false;
                }
                
                if (canShowUpgradeButton)
                {
                    _upgradeButton.gameObject.SetActive(true);
                    _upgradeButton.onClick.AddListener(() =>
                    {
                        _buildingUpgradeComponent.Upgrade(_target.BuildingType);
                        UpdateFields();
                    });
                }
                else
                {
                    _upgradeButton.gameObject.SetActive(false);
                }
            }
            
            _buildingIconImage.sprite = _target.Icon;
            _buildingNameText.text = _target.DisplayName;
            
            if(_buildingHealthComponent == null) return;
            
            UpdateHealth(_buildingHealthComponent.CurrentHealth.CurrentValue);
        }

        private void UpdateHealth(int health)
        {
            _buildingHpSlider.maxValue = _buildingHealthComponent.MaxHealth;
            _buildingHpSlider.value = health;
            _buildingHpText.text = $"{health} / {_buildingHealthComponent.MaxHealth}";
        }
        
        public void Hide()
        {
            _buildingHpSlider.gameObject.SetActive(false);
            _buildingHpText.gameObject.SetActive(false);
            _buildingNameText.gameObject.SetActive(false);
            _buildingIconImage.gameObject.SetActive(false);
            _buildingIconFrame.gameObject.SetActive(false);
            _upgradeButton.gameObject.SetActive(false);
            _upgradeResourceCostPanel.SetActive(false);
            
            _pointerHandler.PointerEnterEvent -= OnUpgradeButtonPointerEnter;
            _pointerHandler.PointerExitEvent -= OnUpgradeButtonPointerExit;
            _upgradeButton.onClick.RemoveAllListeners();

            foreach (var resourceCostView in _resourceCostViewsInstances)
            {
                NightPool.Despawn(resourceCostView);
            }
            
            _resourceCostViewsInstances.Clear();
            _subscriptions?.Clear();
        }
    }
}