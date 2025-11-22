using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Scripts.GlobalSystems
{
    public class GlobalWorkersObserver
    {
        private Dictionary<int, PlayerWorkersData> _workersData = new Dictionary<int, PlayerWorkersData>();
        
        public Action<int> WorkerStateChanged;
        
        public void RegisterPlayer(int ownerId)
        {
            _workersData.Add(ownerId, new PlayerWorkersData()
            {
                OwnerId = ownerId
            });
            _workersData[ownerId].Entities.Add(UnitState.Idle, new WorkersList());
            _workersData[ownerId].Entities.Add(UnitState.Building, new WorkersList());
            _workersData[ownerId].Entities.Add(UnitState.GatheringFood, new WorkersList());
            _workersData[ownerId].Entities.Add(UnitState.GatheringWood, new WorkersList());
            _workersData[ownerId].Entities.Add(UnitState.GatheringGold, new WorkersList());
        }

        public void RegisterWorker(UnitEntity worker)
        {
            if (!_workersData.ContainsKey(worker.OwnerId)) return;
            
            var state = worker.GetEntityComponent<UnitStateComponent>();
            if (state != null)
            {
                if (!_workersData[worker.OwnerId].Entities.ContainsKey(state.CurrentState))
                {
                    return;
                }
                    
                state.OnStateChange += OnWorkerStateChanged;

                _workersData[worker.OwnerId].Entities[state.CurrentState].EntitiesList.Add(worker);
            }
        }

        public void UnregisterWorker(UnitEntity worker)
        {
            if (_workersData.ContainsKey(worker.OwnerId))
            {
                var state = worker.GetEntityComponent<UnitStateComponent>();
                if (state != null)
                {
                    state.OnStateChange -= OnWorkerStateChanged;

                    _workersData[worker.OwnerId].Entities[state.CurrentState].EntitiesList.Remove(worker);
                }
            }
        }

        private void OnWorkerStateChanged(UnitEntity worker, UnitState oldState, UnitState newState)
        {
            bool changed = false;
            
            if (_workersData[worker.OwnerId].Entities.ContainsKey(oldState))
            {
                _workersData[worker.OwnerId].Entities[oldState].EntitiesList.Remove(worker);
                changed = true;
            }
            
            if (_workersData[worker.OwnerId].Entities.ContainsKey(newState))
            {
                _workersData[worker.OwnerId].Entities[newState].EntitiesList.Add(worker);
                changed = true;
            }

            if (changed)
            {
                WorkerStateChanged?.Invoke(worker.OwnerId);
            }
           
        }
        
        public PlayerWorkersData Get(int ownerId)
        {
            return _workersData[ownerId];
        }
    }

    public class PlayerWorkersData
    {
        public int OwnerId { get; set; }
        public Dictionary<UnitState, WorkersList> Entities = new Dictionary<UnitState, WorkersList>();
    }

    public class WorkersList
    {
        public List<UnitEntity> EntitiesList = new List<UnitEntity>();
    }
}