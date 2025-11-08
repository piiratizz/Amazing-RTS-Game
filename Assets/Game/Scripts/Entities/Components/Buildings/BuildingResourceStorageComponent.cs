using GlobalResourceStorageSystem;
using UnityEngine;
using Zenject;

public class BuildingResourceStorageComponent : EntityComponent
{
    [Inject] private ResourcesStoragesManager _storagesManager;
    private GlobalResourceStorage _globalResourceStorage;

    private int _ownerId;
    
    public override void Init(Entity entity)
    {
        _ownerId = entity.OwnerId;
        _globalResourceStorage = _storagesManager.Get(_ownerId);
    }

    public void StoreResource(ResourceType resourceType, int amount)
    {
        _globalResourceStorage.Add(resourceType, amount);
    }
}