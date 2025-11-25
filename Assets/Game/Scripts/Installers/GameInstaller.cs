using Game.Scripts.GlobalSystems;
using Game.Scripts.GlobalSystems.SceneManagement;
using Game.Scripts.Settings;
using Game.Scripts.UI;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private MatchSettingsManager matchSettingsPrefab;
        [SerializeField] private LoadingScreen loadingScreenPrefab;
        
        public override void InstallBindings()
        {
            Container.Bind<SaveLoadSystem>().FromNew().AsSingle().NonLazy();
            
            var loadingScreenInstance = Container.InstantiatePrefabForComponent<LoadingScreen>(loadingScreenPrefab);
            Container.Bind<LoadingScreen>().FromInstance(loadingScreenInstance).AsSingle();
            
            Container.Bind<SceneManager>().FromNew().AsSingle().NonLazy();
            
            var instance = Container.InstantiatePrefabForComponent<MatchSettingsManager>(matchSettingsPrefab);
            Container.Bind<MatchSettingsManager>().FromInstance(instance).AsSingle();
            
            instance.Initialize();
            
            Debug.Log("Game Installed");
        }
    }
}