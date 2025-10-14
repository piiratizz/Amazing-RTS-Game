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
    [SerializeField] private BuildingCostView buildingCostView;
    [SerializeField] private Transform buildingCostsContainer;
    
    [SerializeField] private Image previewImage;
    [SerializeField] private Button button;
    
    private IReadOnlyCollection<ResourceCost> _buildingCosts;

    private ResourcesConfig[] _resourcesConfigs;

    private List<BuildingCostView> _buildingCostInstances;
    
    private bool _initialized;
    
    public void Initialize(Sprite preview, IReadOnlyCollection<ResourceCost> buildingCosts, UnityAction onClickCallback)
    {
        previewImage.sprite = preview;
        _buildingCosts = buildingCosts;
        button.onClick.AddListener(onClickCallback);
        
        if (!_initialized)
        {
            _buildingCostInstances = new List<BuildingCostView>();
            _resourcesConfigs = Resources.LoadAll<ResourcesConfig>("ResourceConfigs");
            _initialized = true;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach (var cost in _buildingCosts)
        {
            var instance = NightPool.Spawn(buildingCostView, buildingCostsContainer);
            var config = _resourcesConfigs.Where(c => c.ResourceType == cost.Resource);
            instance.Initialize(config.First().Icon, cost.Amount);
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