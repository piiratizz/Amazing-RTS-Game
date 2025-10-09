using UnityEngine;
using Zenject;

public class BuildingResourceStorageComponent : EntityComponent
{
    [Inject] private GlobalResourceStorage _globalResourceStorage;
    
    public void StoreResource(ResourceType resourceType, int amount)
    {
        _globalResourceStorage.Add(resourceType, amount);
    }
}