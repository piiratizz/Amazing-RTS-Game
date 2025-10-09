using System;
using R3;
using UnityEngine;

public class ResourceStorageComponent : EntityComponent
{
    private ResourceType _resourceType;
    private int _baseAmount;
    private ReactiveProperty<int> _currentAmount;
    private ReactiveProperty<bool> _isEmpty;
    
    public ResourceType ResourceType => _resourceType;
    public int BaseAmount => _baseAmount;
    public ReadOnlyReactiveProperty<int> Amount => _currentAmount;
    public ReadOnlyReactiveProperty<bool> IsEmpty => _isEmpty.ToReadOnlyReactiveProperty();
    
    public override void InitializeFields(EntityConfig config)
    {
        var resourcesConfig = config as ResourcesConfig;

        if (resourcesConfig == null)
        {
            throw new NullReferenceException("ResourcesConfig must be of type ResourcesConfig");
        }
        
        _resourceType = resourcesConfig.ResourceType;
        _baseAmount = resourcesConfig.Amount;
        _currentAmount = new ReactiveProperty<int>(resourcesConfig.Amount);
        _isEmpty = new ReactiveProperty<bool>(false);
    }

    /// <summary>
    /// Returns amount that was collected, 0 = storage is empty
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public int TryExtractResource(int amount)
    {
        if (amount > _currentAmount.Value)
        {
            var current = _currentAmount.Value;
            _currentAmount.Value = 0;
            _isEmpty.Value = true;
            return current;
        }
        
        _currentAmount.Value -= amount;
        return amount;
    }
}