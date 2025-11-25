using System.Collections.Generic;
using System.Linq;
using Game.Scripts.AI;
using Game.Scripts.GlobalSystems;
using Game.Scripts.Signals;
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

        [Inject] private MatchSettingsManager _matchSettingsManager;


        public override void InstallBindings()
        {
            InstallSignals();

            Container.BindInterfacesAndSelfTo<GlobalUpgradesManager>().AsSingle();

            Container.Bind<MatchResultController>().FromNew().AsSingle();
            
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

            Container.BindFactory<int, PlayerModes, Player, PlayerFactory>()
                .FromComponentInNewPrefab(playerPrefab)
                .AsSingle();
            Container.BindFactory<int, Vector3, BuildingTypePrefabLink, BuildingEntity, BuildingFactory>();

            var globalUpgradesManager = Container.Resolve<GlobalUpgradesManager>();
            
            PlayerFactory playerFactory = Container.Resolve<PlayerFactory>();
            
            List<Player> players = new List<Player>();
            List<int> ownerIds = new List<int>();
            
            foreach (var playerData in _matchSettingsManager.Settings.MatchPlayers)
            {
                ownerIds.Add(playerData.OwnerId);
                
                var storage1 = resourceStorageInstance.Register().FromNew(playerData.OwnerId);
                storage1.Add(ResourceType.Food, 100);
                storage1.Add(ResourceType.Wood, 100);
                storage1.Add(ResourceType.Gold, 100);

                globalWorkersObserverInstance.RegisterPlayer(playerData.OwnerId);
                globalUpgradesManager.RegisterPlayer(playerData.OwnerId);
                globalBuildingsStagesController.RegisterNewPlayer(playerData.OwnerId, 0);
                gridInstance.RegisterPlayer(playerData.OwnerId);

                if (playerData.IsAI)
                {
                    AiPlayer aiPlayer = Container.InstantiatePrefabForComponent<AiPlayer>(aiPlayerPrefab);
                    aiPlayer.Initialize(playerData.OwnerId);
                    Container.Bind<AiPlayer>().FromInstance(aiPlayer).AsSingle();
                }
                else
                {
                    Player playerInstance = playerFactory.Create(playerData.OwnerId, PlayerModes.Default);
                    Container.Bind<Player>().FromInstance(playerInstance).AsSingle();
                    players.Add(playerInstance);
                }
            }

            var hudInstance = Container.InstantiatePrefabForComponent<GameplayHUD>(gameplayUIPrefab);
            Container.Bind<GameplayHUD>().FromInstance(hudInstance).AsSingle();
            
            Container.BindFactory<int, ConfigUnitPrefabLink, Vector3, UnitEntity, UnitFactory>()
                .FromMethod(CreateUnit);
            
            var maps = Resources.LoadAll<Game.Scripts.Map.Map>($"Maps");
            var mapInstance = Container.InstantiatePrefabForComponent<Game.Scripts.Map.Map>(
                maps.First(m => m.Name == _matchSettingsManager.Settings.MapName).gameObject,
                Vector3.zero,
                Quaternion.identity,
                null);
            
            mapInstance.PlayersBasesSpawner.Initialize(
                _matchSettingsManager.Settings.MatchPlayers.Count,
                ownerIds,
                players);
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
            Container.DeclareSignal<PlayerWinSignal>();
        }
    }
}