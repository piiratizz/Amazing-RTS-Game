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
        
        var playerInstance = Container.InstantiatePrefabForComponent<Player>(playerPrefab);
        Container.Bind<Player>().FromInstance(playerInstance).AsSingle();
        
        var hudInstance = Container.InstantiatePrefabForComponent<GameplayHUD>(gameplayUIPrefab);
        Container.QueueForInject(hudInstance);
        
        BindFactories();
    }

    private void BindFactories()
    {
        Container.BindFactory<UnitEntity, UnitsFactory>();
    }
}