using UnityEngine;
using Zenject;

public class BuildingFactory : PlaceholderFactory<int, Vector3, BuildingConfigPrefabLink, BuildingEntity>
{
    private readonly DiContainer _container;

    public BuildingFactory(DiContainer container)
    {
        _container = container;
    }
    
    public override BuildingEntity Create(int id, Vector3 position, BuildingConfigPrefabLink link)
    {
        var building = _container.InstantiatePrefabForComponent<BuildingEntity>(
            link.Prefab,
            position,
            Quaternion.identity,
            null
        );

        building.Init(id, link.Config);
        return building;
    }
}