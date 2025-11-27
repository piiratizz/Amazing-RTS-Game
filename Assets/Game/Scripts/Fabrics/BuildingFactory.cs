using System.Linq;
using Game.Scripts.GlobalSystems;
using UnityEngine;
using Zenject;

public class BuildingFactory : PlaceholderFactory<int, Vector3, BuildingTypePrefabLink, BuildingEntity>
{
    private readonly DiContainer _container;
    private GlobalBuildingsStagesController _globalBuildingsStagesController;
    
    private MatchSettingsManager _matchSettingsManager;
    
    public BuildingFactory(DiContainer container)
    {
        _container = container;
    }
    
    public override BuildingEntity Create(int ownerId, Vector3 position, BuildingTypePrefabLink link)
    {
        if (_globalBuildingsStagesController == null)
        {
            _globalBuildingsStagesController = _container.Resolve<GlobalBuildingsStagesController>();
        }

        if (_matchSettingsManager == null)
        {
            _matchSettingsManager = _container.Resolve<MatchSettingsManager>();
        }
        
        var building = _container.InstantiatePrefabForComponent<BuildingEntity>(
            link.Prefab,
            position,
            Quaternion.identity,
            null
        );

        var config = _globalBuildingsStagesController.GetActualConfig(ownerId, link.Type);
        var color = _matchSettingsManager.Settings.MatchPlayers.First(s => s.OwnerId ==  ownerId).Color;
        
        building.Init(ownerId, config, color);
        return building;
    }
}