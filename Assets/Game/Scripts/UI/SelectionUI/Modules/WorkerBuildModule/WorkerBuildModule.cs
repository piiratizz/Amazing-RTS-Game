using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.UI;
using GlobalResourceStorageSystem;
using NTC.Pool;
using UnityEngine;
using Zenject;

public class WorkerBuildModule : SelectionPanelModule
{
    [SerializeField] private BuildMenuItemView buildMenuItemView;
    [SerializeField] private GameObject background;
    [SerializeField] private Transform itemsContainer;
    
    [Inject] private SignalBus _signalBus;
    [Inject] private ResourcesStoragesManager _storagesManager;
    [Inject] private GlobalBuildingsStagesController _stagesController;
    private GlobalResourceStorage _globalResourceStorage;
    
    private UnitBuildingComponent _unitBuildingComponent;
    
    private List<BuildMenuItemView> _instances;

    private int _ownerId;
    
    public override void Initialize(int ownerId)
    {
        _ownerId = ownerId;
    }

    public override void Show(List<Entity> targets)
    {
        _unitBuildingComponent = null;
        _instances =  new List<BuildMenuItemView>();
        _globalResourceStorage = _storagesManager.Get(_ownerId);
        
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
        
        if(_unitBuildingComponent.AvailableBuildings == null)
            return;
        
        foreach (var item in _unitBuildingComponent.AvailableBuildings)
        {
            if(item == null) 
                continue;
            
            var actualConfig = _stagesController.GetActualConfig(_ownerId,  item.Type);
            
            var instance = NightPool.Spawn(buildMenuItemView, itemsContainer);
            instance.Initialize(actualConfig.Preview, actualConfig.BuildResourceCost, item.Type, OnClickCallback);
            _instances.Add(instance);
        }
    }

    private void OnClickCallback(BuildingType type)
    {
        var config = _stagesController.GetActualConfig(_ownerId, type);
        
        foreach (var cost in config.BuildResourceCost)
        {
            if (!_globalResourceStorage.IsEnough(cost.Resource, cost.Amount))
            {
                return;
            }
        }
        
        var selectedBuilding = 
            _unitBuildingComponent.AvailableBuildings
                .First(c => c.Type == type);
        
        _signalBus.Fire(new ChanglePlayerModeSignal()
        {
            Mode = PlayerModes.Building,
            Link = selectedBuilding
        });
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
