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
        
        [Inject] private ResourcesStoragesManager _storageManager;
        private AiContextController _aiContextController;
        
        public AiEntitiesController AiEntitiesController => entitiesController;
        
        public int OwnerId { get; set; }
    
        
        public void Initialize(int ownerId)
        {
            OwnerId = ownerId;
            
            var storage = _storageManager.Get(OwnerId);

            contextController.Initialize(OwnerId);
            rulerPerformer.Initialize(contextController.Context, new List<IRule>()
            {
                new BuildWorkersRule(),
                new ResourceGatherRule(),
                new WorkersBuildBuldingsRule(),
                new AttackPlayerArmyRule(),
                new BuildArmyRule
                {
                    UnitToBuild = UnitType.Spearman,
                    ProductionBuilding = BuildingType.Barracks,
                    DesiredCount = 20,
                    unitImportance = 1.0f
                },
                new BuildArmyRule
                {
                    UnitToBuild = UnitType.Archer,
                    ProductionBuilding = BuildingType.Barracks,
                    DesiredCount = 15,
                    unitImportance = 1.0f
                },
                new BuildArmyRule
                {
                    UnitToBuild = UnitType.Cavalry,
                    ProductionBuilding = BuildingType.Stables,
                    DesiredCount = 10,
                    unitImportance = 1.0f
                },
            });
            
            entitiesController.Initialize(contextController.Context);
        }
    }
}