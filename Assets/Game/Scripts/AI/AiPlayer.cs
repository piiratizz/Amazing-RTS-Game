using System.Collections.Generic;
using Game.Scripts.AI.RuleBasedSystem.Rules;
using GlobalResourceStorageSystem;
using UnityEngine;
using Zenject;

namespace Game.Scripts.AI
{
    public class AiPlayer : MonoBehaviour, IOwned
    {
        [SerializeField] private RulesPerformer rulerPerformer;
        [SerializeField] private AiContextController contextController;
        [SerializeField] private AiEntitiesController entitiesController;
        
        [Inject] private GlobalGrid _globalGrid;
        [Inject] private BuildingFactory _buildingFactory;
        [Inject] private ResourcesStoragesManager _storageManager;
        private AiContextController _aiContextController;
        
        [Inject] private GlobalBuildingsStagesController _globalBuildingsStagesController;
        
        public AiEntitiesController AiEntitiesController => entitiesController;
        
        public int OwnerId { get; set; }
    
        
        public void Initialize(int ownerId)
        {
            OwnerId = ownerId;
            
            var storage = _storageManager.Get(OwnerId);

            contextController.Initialize(OwnerId);
            entitiesController.Initialize(contextController.Context);
            rulerPerformer.Initialize(contextController.Context, new List<IRule>()
            {
                new BuildWorkersRule(),
                new ResourceGatherRule(),
                new WorkersBuildBuildingsRule(_globalGrid.HeatMap),
                new AttackPlayerArmyRule(_globalGrid.HeatMap),
                new BuildArmyRule
                {
                    UnitToBuild = UnitType.Spearman,
                    ProductionBuilding = BuildingType.Barracks,
                    DesiredCount = 50,
                    unitImportance = 1.0f
                },
                new BuildArmyRule
                {
                    UnitToBuild = UnitType.Archer,
                    ProductionBuilding = BuildingType.Archery,
                    DesiredCount = 50,
                    unitImportance = 1.0f
                },
                new BuildArmyRule
                {
                    UnitToBuild = UnitType.Cavalry,
                    ProductionBuilding = BuildingType.Stables,
                    DesiredCount = 50,
                    unitImportance = 1.0f
                },
                new PlaceNewBuildingRule(_globalBuildingsStagesController, _buildingFactory),
                new ReturnArmyToBaseRule(),
                new AttackPlayerBuildingRule(_globalGrid.HeatMap),
                new TradeResourcesRule()
            });
        }
    }
}