using System;
using System.Collections.Generic;
using R3;
using TMPro;
using UnityEngine.UI;

namespace Game.Scripts.UI.Modules.Presenters
{
    public class ResourceInfoPresenter : IEntityInfoPresenter
    {
        private readonly Slider _resourceSlider;
        private readonly TextMeshProUGUI _resourceCountText;
        private readonly TextMeshProUGUI _resourceNameText;
        private readonly Image _resourceIconImage;

        private int _baseResourcesAmount;
        private CompositeDisposable _resourcesSubscription;
        
        private ResourceEntity _target;

        public ResourceInfoPresenter(
            Slider resourceCountSlider,
            TextMeshProUGUI resourceCountText,
            TextMeshProUGUI resourceNameText,
            Image resourceIconImage)
        {
            _resourceSlider =  resourceCountSlider;
            _resourceCountText = resourceCountText;
            _resourceNameText = resourceNameText;
            _resourceIconImage =  resourceIconImage;
        }
        
        public void Show(List<Entity> entities)
        {
            if(entities.Count > 1) return;
            
            _target = entities[0] as  ResourceEntity;

            if (_target == null)
            {
                throw new InvalidCastException("Entity must be ResourceEntity");
            }
            
            _resourceSlider.gameObject.SetActive(true);
            _resourceCountText.gameObject.SetActive(true);
            _resourceNameText.gameObject.SetActive(true);
            _resourceIconImage.gameObject.SetActive(true);

            var resourceStorage = _target.GetEntityComponent<ResourceStorageComponent>();
            _baseResourcesAmount = resourceStorage.BaseAmount;
            _resourceSlider.maxValue = _baseResourcesAmount;
            _resourceIconImage.sprite = _target.Icon;
            _resourceNameText.text = _target.DisplayName;
            
            CreateSubscriptions(resourceStorage);
        }

        private void CreateSubscriptions(ResourceStorageComponent storage)
        {
            _resourcesSubscription = new CompositeDisposable();
            storage.Amount.Subscribe(UpdateResourcesCount).AddTo(_resourcesSubscription);
        }

        private void RemoveSubscriptions()
        {
            _resourcesSubscription?.Clear();
            _resourcesSubscription?.Dispose();
        }
        
        private void UpdateResourcesCount(int amount)
        {
            _resourceCountText.text = $"{amount} / {_baseResourcesAmount}";
            _resourceSlider.value = amount;
        }
        
        public void Hide()
        {
            _resourceSlider.gameObject.SetActive(false);
            _resourceCountText.gameObject.SetActive(false);
            _resourceNameText.gameObject.SetActive(false);
            _resourceIconImage.gameObject.SetActive(false);
            RemoveSubscriptions();
        }
    }
}