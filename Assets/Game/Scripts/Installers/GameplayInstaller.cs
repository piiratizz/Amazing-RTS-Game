using System.Collections.Generic;
using Game.Scripts.AI;
using Game.Scripts.GlobalSystems;
using GlobalResourceStorageSystem;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Installers
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private Player playerPrefab;
        [SerializeField] private AiPlayer aiPlayerPrefab;
        [SerializeField] private GameplayHUD gameplayUIPrefab;
        [SerializeField] private GlobalGrid gridPrefab;
    
        private List<int> _playerIds = new List<int>()
        {
            1,
            2
        };    
        
        public override void InstallBindings()
        {
            InstallSignals();
        
            Container.BindInterfacesAndSelfTo<GlobalUpgradesManager>().AsSingle();

            var entityRegistryInstance = new WorldEntitiesRegistry();
            Container.Bind<WorldEntitiesRegistry>().FromInstance(entityRegistryInstance).AsSingle();
            
            var gridInstance = Container.InstantiatePrefabForComponent<GlobalGrid>(gridPrefab);
            gridInstance.Initialize();
            Container.Bind<GlobalGrid>().FromInstance(gridInstance).AsSingle();
        
            var globalWorkersObserverInstance = new GlobalWorkersObserver();
            Container.Bind<GlobalWorkersObserver>().FromInstance(globalWorkersObserverInstance).AsSingle();
            
            var resourceStorageInstance = new ResourcesStoragesManager();
            Container.Bind<ResourcesStoragesManager>().FromInstance(resourceStorageInstance).AsSingle();
        
            var globalBuildingsStagesController = new GlobalBuildingsStagesController(resourceStorageInstance);
            Container.Bind<GlobalBuildingsStagesController>().FromInstance(globalBuildingsStagesController).AsSingle();

            var globalUpgradesManager = Container.Resolve<GlobalUpgradesManager>();
            
            foreach (var id in _playerIds)
            {
                var storage1 = resourceStorageInstance.Register().FromNew(id);
                storage1.Add(ResourceType.Food, 100);
                storage1.Add(ResourceType.Wood, 100);
                storage1.Add(ResourceType.Gold, 100);
                
                globalWorkersObserverInstance.RegisterPlayer(id);
                globalUpgradesManager.RegisterPlayer(id);
                globalBuildingsStagesController.RegisterNewPlayer(id, 0);
                gridInstance.RegisterPlayer(id);
            }
            
            Container.Bind<PlayerRegistry>().FromNew().AsSingle();
        
            Container.BindFactory<int, PlayerModes, Player, PlayerFactory>()
                .FromComponentInNewPrefab(playerPrefab)
                .AsSingle();
            Container.BindFactory<int, Vector3, BuildingTypePrefabLink, BuildingEntity, BuildingFactory>();
        
            PlayerFactory playerFactory = Container.Resolve<PlayerFactory>();
        
            Player playerInstance = playerFactory.Create(1, PlayerModes.Default);
            Container.Bind<Player>().FromInstance(playerInstance).AsSingle();
        
            AiPlayer aiPlayer = Container.InstantiatePrefabForComponent<AiPlayer>(aiPlayerPrefab);
            aiPlayer.Initialize(2);
            Container.Bind<AiPlayer>().FromInstance(aiPlayer).AsSingle();
            
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
}