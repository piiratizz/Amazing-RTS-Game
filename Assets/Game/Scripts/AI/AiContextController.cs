using System;
using Game.Scripts.GlobalSystems;
using GlobalResourceStorageSystem;
using UnityEngine;
using Zenject;

namespace Game.Scripts.AI
{
    public class AiContextController : MonoBehaviour
    {
        [SerializeField] private AiPlayer aiPlayer;
        [SerializeField] private float dataUpdateInterval = 2;
        
        private float _elapsedTime;
        
        public AiContext Context {get; private set;}

        [Inject] private ResourcesStoragesManager _storagesManager;
        [Inject] private GlobalWorkersObserver _workersObserver;
        private GlobalResourceStorage _resourceStorage;
        
        private int _ownerId;
        private bool _initialized;
        
        public void Initialize(int ownerId)
        {
            Context = new AiContext();
            _ownerId = ownerId;
            _resourceStorage = _storagesManager.Get(_ownerId);

            Context.AiPlayer = aiPlayer;
            Context.OwnerId = _ownerId;
            
            Context.Food = _resourceStorage.GetResource(ResourceType.Food);
            Context.Wood = _resourceStorage.GetResource(ResourceType.Wood);
            Context.Gold = _resourceStorage.GetResource(ResourceType.Gold);
            
            Context.Workers = 5;
            Context.MaxWorkers = 40;
            
            _initialized = true;
            OnEnable();
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime > dataUpdateInterval)
            {
                UpdateData();
                _elapsedTime = 0;
            }
        }

        private void UpdateData()
        {
            UpdateWorkersData();
        }

        private void UpdateWorkersData()
        {
            Context.FoodWorkers = _workersObserver.Get(_ownerId).Entities[UnitState.GatheringFood].EntitiesList.Count;
            Context.WoodWorkers = _workersObserver.Get(_ownerId).Entities[UnitState.GatheringWood].EntitiesList.Count;
            Context.GoldWorkers = _workersObserver.Get(_ownerId).Entities[UnitState.GatheringGold].EntitiesList.Count;
            Context.LazyWorkers = _workersObserver.Get(_ownerId).Entities[UnitState.Idle].EntitiesList.Count;
        }
        
        private void OnResourceUpdated(ResourceType resourceType, int amount)
        {
            switch (resourceType)
            {
                case ResourceType.Food:
                    Context.Food = amount;
                    break;
                case ResourceType.Wood:
                    Context.Wood = amount;
                    break;
                case ResourceType.Gold:
                    Context.Gold = amount;
                    break;
            }
        }
        
        private void OnEnable()
        {
            if (!_initialized) return;
            
            _resourceStorage.OnResourceChanged += OnResourceUpdated;
        }

        private void OnDisable()
        {
            _resourceStorage.OnResourceChanged -= OnResourceUpdated;
        }
    }
}