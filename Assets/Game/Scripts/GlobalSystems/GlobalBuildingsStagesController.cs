using System;
using System.Collections.Generic;
using System.Linq;
using GlobalResourceStorageSystem;
using UnityEngine;
using Zenject;

public class GlobalBuildingsStagesController
{
    private BuildingUpgragesConfig[] _configs;
    private List<PlayerUpgradesInformation> _playerUpgradesInformation;
    
    private ResourcesStoragesManager _storagesManager;
    
    public Action<int,BuildingType, int> AnyBuildingUpgradedEvent;
    
    public GlobalBuildingsStagesController(ResourcesStoragesManager storagesManager)
    {
        _playerUpgradesInformation = new List<PlayerUpgradesInformation>();
        _storagesManager = storagesManager;
        _configs = Resources.LoadAll<BuildingUpgragesConfig>("BuildingUpgrades");
    }

    public void RegisterNewPlayer(int playerId, int startStage)
    {
        _playerUpgradesInformation.Add(new PlayerUpgradesInformation(playerId, _configs, startStage));
    }
    
    public BuildingConfig GetActualConfig(int playerId, BuildingType buildingType)
    {
        var info = _playerUpgradesInformation.First(x => x.PlayerId == playerId);
        var level = info.GetLevelOfBuilding(buildingType);
        var upgradeConfig = _configs.First( c => c.BuildingType == buildingType);

        if(upgradeConfig == null)
            return null;
        
        return upgradeConfig.Stages[level].Stage;
    }

    public ResourceCost[] GetNextUpgradeCost(int playerId, BuildingType buildingType)
    {
        var info = _playerUpgradesInformation.First(x => x.PlayerId == playerId);
        var level = info.GetLevelOfBuilding(buildingType);
        var upgradeConfig = _configs.First( c => c.BuildingType == buildingType);

        if (upgradeConfig.Stages.Length <= level+1)
        {
            return null;
        }
        
        var cost = upgradeConfig.Stages[level+1];
        return cost.ResourceCosts;
    }

    public bool IsFullUpgraded(int playerId, BuildingType buildingType)
    {
        var info = _playerUpgradesInformation.First(x => x.PlayerId == playerId);
        var level = info.GetLevelOfBuilding(buildingType);
        var upgradeConfig = _configs.First( c => c.BuildingType == buildingType);
        
        if (upgradeConfig.Stages.Length <= level+1)
        {
            return true;
        }
        
        return false;
    }
    
    public bool UpgradeStage(int playerId, BuildingType buildingType)
    {
        var storage = _storagesManager.Get(playerId);
        var upgradeConfig = _configs.First( c => c.BuildingType == buildingType);
        var info = _playerUpgradesInformation.First(i => i.PlayerId == playerId);

        var stage = upgradeConfig.Stages[info.GetLevelOfBuilding(buildingType)+1];
        
        foreach (var cost in stage.ResourceCosts)
        {
            if (!storage.IsEnough(cost.Resource, cost.Amount))
            {
                return false;
            }
        }
        
        var isUpgraded = info.TryIncreaseLevel(buildingType);
        if (isUpgraded)
        {
            foreach (var cost in stage.ResourceCosts)
            {
                storage.TrySpend(cost.Resource, cost.Amount);
            }
            AnyBuildingUpgradedEvent?.Invoke(playerId, buildingType, info.GetLevelOfBuilding(buildingType));
        }

        return isUpgraded;
    }
}

class PlayerUpgradesInformation
{
    public int PlayerId {get; private set;}
    private List<BuildingLevel> _levels;

    public PlayerUpgradesInformation(int playerId, BuildingUpgragesConfig[] config, int startStage)
    {
        _levels = new List<BuildingLevel>();
        PlayerId =  playerId;

        foreach (var cfg in config)
        {
            _levels.Add(new BuildingLevel(cfg.BuildingType, startStage, cfg.Stages.Length-1));
        }
    }

    public int GetLevelOfBuilding(BuildingType buildingType)
    {
        return _levels.First(x => x.BuildingType == buildingType).Level;
    }
    
    public bool TryIncreaseLevel(BuildingType buildingType)
    {
        return _levels.First( c => c.BuildingType == buildingType).TryIncreaseLevel();
    }
}

class BuildingLevel
{
    public BuildingType BuildingType {get; private set;}
    public int Level {get; private set;}
    public int MaxLevel {get; private set;}

    public BuildingLevel(BuildingType buildingType, int level, int maxLevel)
    {
        BuildingType = buildingType;
        Level = level;
        MaxLevel = maxLevel;
    }

    public bool TryIncreaseLevel()
    {
        if (Level < MaxLevel)
        {
            Level++;
            return true;
        }
        return false;
    }
} 