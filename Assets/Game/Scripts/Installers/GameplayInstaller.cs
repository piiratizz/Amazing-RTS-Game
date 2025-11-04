using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [SerializeField] private Player playerPrefab;
    [SerializeField] private GameplayHUD gameplayUIPrefab;
    
    public override void InstallBindings()
    {
        var resourceStorageInstance = new GlobalResourceStorage();
        Container.Bind<GlobalResourceStorage>().FromInstance(resourceStorageInstance).AsSingle();
        resourceStorageInstance.Add(ResourceType.Food, 100);
        resourceStorageInstance.Add(ResourceType.Gold, 100);
        resourceStorageInstance.Add(ResourceType.Wood, 100);
        
        Container.Bind<PlayerRegistry>().FromNew().AsSingle();
        
        Container.BindFactory<int, PlayerModes, Player, PlayerFactory>()
            .FromComponentInNewPrefab(playerPrefab)
            .AsSingle();
        
        PlayerFactory playerFactory = Container.Resolve<PlayerFactory>();
        
        Player playerInstance = playerFactory.Create(1, PlayerModes.Default);
        Container.Bind<Player>().FromInstance(playerInstance).AsSingle();
        
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
}