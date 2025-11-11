using System;
using System.Collections.Generic;
using System.Linq;
using NTC.Pool;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildMenuItemView : MonoBehaviour, IDisposable, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ResourceCostView resourceCostView;
    [SerializeField] private RectTransform buildingCostsContainer;
    
    [SerializeField] private Image previewImage;
    [SerializeField] private Button button;
    
    public BuildingType Type { get; private set; }
    
    private IReadOnlyCollection<ResourceCost> _buildingCosts;

    private ResourceData[] _resourcesData;

    private List<ResourceCostView> _buildingCostInstances;
    
    private bool _initialized;
    
    public void Initialize(Sprite preview, IReadOnlyCollection<ResourceCost> buildingCosts, BuildingType type, UnityAction<BuildingType> onClickCallback)
    {
        previewImage.sprite = preview;
        _buildingCosts = buildingCosts;
        button.onClick.AddListener( () => onClickCallback(Type));
        Type = type;
        
        if (!_initialized)
        {
            _buildingCostInstances = new List<ResourceCostView>();
            _resourcesData = Resources.LoadAll<ResourceData>("ResourcesData");
            _initialized = true;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach (var cost in _buildingCosts)
        {
            var instance = NightPool.Spawn(resourceCostView, Vector3.zero, Quaternion.identity, buildingCostsContainer);
            var data = _resourcesData.Where(c => c.ResourceType == cost.Resource);
            instance.Initialize(data.First().UiDisplayIcon, cost.Amount);
            _buildingCostInstances.Add(instance);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideCostInfo();
    }

    private void HideCostInfo()
    {
        foreach (var instance in _buildingCostInstances)
        {
            if(instance.isActiveAndEnabled)
                NightPool.Despawn(instance);
        }
    }
    
    public void Dispose()
    {
        HideCostInfo();
        _buildingCostInstances.Clear();
        button.onClick.RemoveAllListeners();
    }
    
}