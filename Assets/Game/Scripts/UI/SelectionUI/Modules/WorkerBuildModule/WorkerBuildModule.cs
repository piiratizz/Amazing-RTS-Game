using System;
using System.Collections.Generic;
using Game.Scripts.UI;
using NTC.Pool;
using UnityEngine;

public class WorkerBuildModule : SelectionPanelModule
{
    [SerializeField] private BuildMenuItemView buildMenuItemView;
    [SerializeField] private GameObject background;
    [SerializeField] private Transform itemsContainer;
    
    private UnitBuildingComponent _unitBuildingComponent;
    
    private List<BuildMenuItemView> _instances;
    
    public override void Show(List<Entity> targets)
    {
        _unitBuildingComponent = null;
        _instances =  new List<BuildMenuItemView>();
        
        foreach (var entity in targets)
        {
            var component = entity.GetEntityComponent<UnitBuildingComponent>();
            if (component != null)
            {
                _unitBuildingComponent = component;
            }
        }

        if (_unitBuildingComponent == null)
        {
            return;
        }

        if (_unitBuildingComponent.AvailableBuildings == null)
        {
            return;
        }
        
        background.SetActive(true);
        
        foreach (var item in _unitBuildingComponent.AvailableBuildings)
        {
            var instance = NightPool.Spawn(buildMenuItemView, itemsContainer);
            instance.Initialize(item.Preview, item.BuildResourceCost, OnClickCallback);
            _instances.Add(instance);
        }
    }

    private void OnClickCallback()
    {
        
    }
    
    public override void Hide()
    {
        background.SetActive(false);

        if (_instances != null)
        {
            foreach (var item in _instances)
            {
                item.Dispose();
                NightPool.Despawn(item);
            }
        }

        _instances = null;
    }
}
