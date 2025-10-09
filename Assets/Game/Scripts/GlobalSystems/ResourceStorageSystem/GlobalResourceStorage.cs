using System;
using System.Collections.Generic;
using R3.Collections;

public class GlobalResourceStorage
{
    private Dictionary<ResourceType, int> _resources;

    public Action<ResourceType, int> OnResourceChanged;
    
    public GlobalResourceStorage()
    {
        _resources = new Dictionary<ResourceType, int>();
    }

    public void Add(ResourceType resourceType, int amount)
    {
        if (!_resources.ContainsKey(resourceType))
        {
            _resources.Add(resourceType, amount);
            return;
        }
        
        int currentAmount = _resources[resourceType];
        _resources[resourceType] = currentAmount + amount;
        OnResourceChanged?.Invoke(resourceType, currentAmount + amount);
    }
    
    public bool TrySpend(ResourceType resourceType, int amount)
    {
        int currentAmount = _resources[resourceType];
        
        if (currentAmount - amount < 0)
        {
            return false;
        }
        
        _resources[resourceType] = currentAmount - amount;
        OnResourceChanged?.Invoke(resourceType, currentAmount);
        return true;
    }

    public int GetResource(ResourceType resourceType)
    {
        return _resources[resourceType];
    }
}