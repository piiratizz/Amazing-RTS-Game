using GlobalResourceStorageSystem;
using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [SerializeField] private Player playerPrefab;
    [SerializeField] private GameplayHUD gameplayUIPrefab;
    [SerializeField] private GlobalGrid gridPrefab;
    
    public override void InstallBindings()
    {
        InstallSignals();
        
        Container.BindInterfacesAndSelfTo<GlobalUpgradesManager>().AsSingle();
        
        var gridInstance = Container.InstantiatePrefabForComponent<GlobalGrid>(gridPrefab);
        Container.Bind<GlobalGrid>().FromInstance(gridInstance).AsSingle();
        
        var resourceStorageInstance = new ResourcesStoragesManager();
        Container.Bind<ResourcesStoragesManager>().FromInstance(resourceStorageInstance).AsSingle();
        
        var globalBuildingsStagesController = new GlobalBuildingsStagesController(resourceStorageInstance);
        Container.Bind<GlobalBuildingsStagesController>().FromInstance(globalBuildingsStagesController).AsSingle();
        
        var storage1 = resourceStorageInstance.Register().FromNew(1);
        storage1.Add(ResourceType.Food, 100);
        storage1.Add(ResourceType.Wood, 100);
        storage1.Add(ResourceType.Gold, 100);
        
        var storage2 = resourceStorageInstance.Register().FromNew(2);
        storage2.Add(ResourceType.Food, 100);
        storage2.Add(ResourceType.Wood, 100);
        storage2.Add(ResourceType.Gold, 100);
        
        Container.Bind<PlayerRegistry>().FromNew().AsSingle();
        
        Container.BindFactory<int, PlayerModes, Player, PlayerFactory>()
            .FromComponentInNewPrefab(playerPrefab)
            .AsSingle();
        Container.BindFactory<int, Vector3, BuildingTypePrefabLink, BuildingEntity, BuildingFactory>();
        
        PlayerFactory playerFactory = Container.Resolve<PlayerFactory>();
        
        Player playerInstance = playerFactory.Create(1, PlayerModes.Default);
        Container.Bind<Player>().FromInstance(playerInstance).AsSingle();
        
        var globalUpgradesManager = Container.Resolve<GlobalUpgradesManager>();
        
        globalUpgradesManager.RegisterPlayer(playerInstance.OwnerId);
        globalBuildingsStagesController.RegisterNewPlayer(playerInstance.OwnerId, 0);
        
        var hudInstance = Container.InstantiatePrefabForComponent<GameplayHUD>(gameplayUIPrefab);
        Container.Bind<GameplayHUD>().FromInstance(hudInstance).AsSingle();
        
        Container.BindFactory<int, ConfigUnitPrefabLink, Vector3, UnitEntity, UnitFactory>()
            .FromMethod(CreateUnit);
    }
    
    private UnitEntity CreateUnit(DiContainer container, int ownerId, ConfigUnitPrefabLink link, Vector3 position)
    {
        var unit = container.InstantiatePrefabForComponent<UnitEntity>(
            link.UnitPrefab,
            position,
            Quaternion.identity,
            null
        );

        unit.Init(ownerId, link.Config);
        return unit;
    }

    private void InstallSignals()
    {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<ChanglePlayerModeSignal>();
        Container.DeclareSignal<UpgradeAddedSignal>();
    }

}