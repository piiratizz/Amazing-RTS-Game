using System;
using System.Collections.Generic;
using R3;
using TMPro;
using UnityEngine.UI;

namespace Game.Scripts.UI.Modules.Presenters
{
    public class BuildingsInfoPresenter : IEntityInfoPresenter
    {
        private readonly Slider _buildingHpSlider;
        private readonly TextMeshProUGUI _buildingHpText;
        private readonly TextMeshProUGUI _buildingNameText;
        private readonly Image _buildingIconImage;
        
        private CompositeDisposable _resourcesSubscription;
        
        private BuildingEntity _target;
        private HealthComponent _buildingHealthComponent;
        
        public BuildingsInfoPresenter(
            Slider buildingHpSlider,
            TextMeshProUGUI buildingHpText,
            TextMeshProUGUI buildingNameText,
            Image buildingIconImage)
        {
            _buildingHpSlider =  buildingHpSlider;
            _buildingHpText = buildingHpText;
            _buildingNameText = buildingNameText;
            _buildingIconImage =  buildingIconImage;
        }
        
        public void Show(List<Entity> entities)
        {
            if(entities.Count > 1) return;
            
            _target = entities[0] as BuildingEntity;

            if (_target == null)
            {
                throw new InvalidCastException("Entity must be BuildingEntity");
            }
            
            _buildingHpSlider.gameObject.SetActive(true);
            _buildingHpText.gameObject.SetActive(true);
            _buildingNameText.gameObject.SetActive(true);
            _buildingIconImage.gameObject.SetActive(true);

            var isDamageable = _target.TryGetComponent(out _buildingHealthComponent);
            
            if(!isDamageable) return;

            UpdateHealth(_buildingHealthComponent.CurrentHealth);
            _buildingIconImage.sprite = _target.Icon;
            _buildingNameText.text = _target.DisplayName;
            _buildingHealthComponent.OnHealthChanged += UpdateHealth;
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
            _buildingHealthComponent.OnHealthChanged -= UpdateHealth;
        }
    }
}