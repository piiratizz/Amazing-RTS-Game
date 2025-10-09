using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [SerializeField] private Player playerPrefab;
    [SerializeField] private GameObject gameplayUIPrefab;
    
    
    public override void InstallBindings()
    {
        var resourceStorageInstance = new GlobalResourceStorage();
        Container.Bind<GlobalResourceStorage>().FromInstance(resourceStorageInstance).AsSingle();
        resourceStorageInstance.Add(ResourceType.Food, 100);
        resourceStorageInstance.Add(ResourceType.Gold, 100);
        resourceStorageInstance.Add(ResourceType.Wood, 100);
        
        var playerInstance = Container.InstantiatePrefabForComponent<Player>(playerPrefab);
        Container.Bind<Player>().FromInstance(playerInstance).AsSingle();
        
        Container.InstantiatePrefab(gameplayUIPrefab);
    }
}