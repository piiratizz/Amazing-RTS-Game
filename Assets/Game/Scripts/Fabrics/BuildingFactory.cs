using UnityEngine;
using Zenject;

public class BuildingFactory : PlaceholderFactory<int, Vector3, BuildingTypePrefabLink, BuildingEntity>
{
    private readonly DiContainer _container;
    private GlobalBuildingsStagesController _globalBuildingsStagesController;
    
    public BuildingFactory(DiContainer container)
    {
        _container = container;
    }
    
    public override BuildingEntity Create(int id, Vector3 position, BuildingTypePrefabLink link)
    {
        if (_globalBuildingsStagesController == null)
        {
            _globalBuildingsStagesController = _container.Resolve<GlobalBuildingsStagesController>();
        }
        
        var building = _container.InstantiatePrefabForComponent<BuildingEntity>(
            link.Prefab,
            position,
            Quaternion.identity,
            null
        );

        var config = _globalBuildingsStagesController.GetActualConfig(id, link.Type);
        
        building.Init(id, config);
        return building;
    }
}