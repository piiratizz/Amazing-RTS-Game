using System;
using System.Collections.Generic;
using BeastConsole;
using GlobalResourceStorageSystem;
using UnityEngine;
using Zenject;

[ConsoleParse]
public class ConsoleCommands : MonoBehaviour
{
    [SerializeField] private List<EntityUpgrade> upgrades;
    
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
    
}