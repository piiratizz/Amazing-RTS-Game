using System;
using GlobalResourceStorageSystem;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public class BuildingUpgradeComponent : EntityComponent
{   
    [Inject] private GlobalBuildingsStagesController _globalBuildingsStagesController;
    private Entity _entity;
    private BuildingConfig _buildingConfig;
    private bool _initialized;
    
    public Action BuildingUpgradedEvent;
    
    public override void Init(Entity entity)
    {
        _entity = entity;
        
        _initialized = true;
        OnEnable();
    }

    public override void InitializeFields(EntityConfig config)
    {
        _buildingConfig = config as BuildingConfig;
        if (_buildingConfig == null)
        {
            throw new InvalidCastException("BuildingConfig needs to be a BuildingConfig");
        }
    }
    
    public void Upgrade(BuildingType buildingType)
    {
        _globalBuildingsStagesController.UpgradeStage(_entity.OwnerId, buildingType);
    }

    public ResourceCost[] GetCostOfUpgrade()
    {
        return _globalBuildingsStagesController.GetNextUpgradeCost(_entity.OwnerId, _buildingConfig.Type);
    }

    public bool IsFullUpgraded()
    {
        return _globalBuildingsStagesController.IsFullUpgraded(_entity.OwnerId, _buildingConfig.Type);
    }
    
    private void OnEnable()
    {
        if(!_initialized) return;
        
        _globalBuildingsStagesController.AnyBuildingUpgradedEvent += OnBuildingUpgraded;
    }

    private void OnBuildingUpgraded(int playerId, BuildingType type, int level)
    {
        if (_entity.OwnerId == playerId && _buildingConfig.Type == type)
        {
            var config = _globalBuildingsStagesController.GetActualConfig(_entity.OwnerId, _buildingConfig.Type);
            _entity.UpdateConfig(config);
            BuildingUpgradedEvent?.Invoke();
        }
    }

    private void OnDisable()
    {
        _globalBuildingsStagesController.AnyBuildingUpgradedEvent -= OnBuildingUpgraded;
    }
}