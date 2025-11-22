using System;
using System.Collections.Generic;
using BeastConsole;
using Game.Scripts.GlobalSystems;
using GlobalResourceStorageSystem;
using UnityEngine;
using Zenject;

[ConsoleParse]
public class ConsoleCommands : MonoBehaviour
{
    [SerializeField] private List<EntityUpgrade> upgrades;
    
    [Inject] private GlobalWorkersObserver _workersObserver;
    [Inject] private GlobalUpgradesManager _globalUpgradesManager;
    [Inject] private ResourcesStoragesManager _resourceManager;
    
    [ConsoleCommand("upgrades.add", "", false)]
    public void AddUpgrade(string upgradeName)
    {
        var upgrade = upgrades.Find(x => x.name == upgradeName);
        _globalUpgradesManager.AddUpgrade(1, upgrade);
        Debug.Log($"Adding upgrade {upgradeName}");
    }

    [ConsoleCommand("resources.add", "", false)]
    public void AddResource(int playerId, string resourceName, int amount)
    {
        var storage = _resourceManager.Get(playerId);
        
        var resource = Enum.Parse<ResourceType>(resourceName);
        
        storage.Add(resource, amount);
    }
    
    [ConsoleCommand("resources.spend", "", false)]
    public void SpendResource(int playerId, string resourceName, int amount)
    {
        var storage = _resourceManager.Get(playerId);
        
        var resource = Enum.Parse<ResourceType>(resourceName);
        
        storage.TrySpend(resource, amount);
    }
    
    [ConsoleCommand("resources.show", "", false)]
    public void ShowResource(int playerId)
    {
        var storage = _resourceManager.Get(playerId);
        
        Debug.Log($"Resources of player {playerId}\n" +
                  $"Food: {storage.GetResource(ResourceType.Food)}\n" +
                  $"Wood: {storage.GetResource(ResourceType.Wood)}\n" +
                  $"Gold: {storage.GetResource(ResourceType.Gold)}\n");
    }
    
    [ConsoleCommand("resources.showWorkers", "", false)]
    public void ShowWorkersOnResources(int playerId)
    {
        Debug.Log($"Resources of player {playerId}\n" +
                  $"Workers on Food: {_workersObserver.Get(playerId).Entities[UnitState.GatheringFood].EntitiesList.Count}\n" +
                  $"Workers on Wood: {_workersObserver.Get(playerId).Entities[UnitState.GatheringWood].EntitiesList.Count}\n" +
                  $"Workers on Gold: {_workersObserver.Get(playerId).Entities[UnitState.GatheringGold].EntitiesList.Count}\n");
    }
    
}