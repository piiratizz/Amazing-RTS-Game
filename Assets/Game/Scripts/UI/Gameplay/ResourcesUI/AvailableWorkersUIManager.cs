using System;
using Game.Scripts.GlobalSystems;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Gameplay.ResourcesUI
{
    public class AvailableWorkersUIManager : UIModule
    {
        [SerializeField] private TextMeshProUGUI foodText;
        [SerializeField] private TextMeshProUGUI woodText;
        [SerializeField] private TextMeshProUGUI goldText;
        
        [Inject] private GlobalWorkersObserver _workersObserver;

        private int _ownerId;
        private bool _initialized;
        
        public override void Initialize(int ownerId)
        {
            _ownerId = ownerId;
            UpdateView();
            _initialized = true;
            OnEnable();
        }
        
        private void WorkerStateChanged(int ownerId)
        {
            if (_ownerId == ownerId)
            {
                UpdateView();
            }
        }

        private void UpdateView()
        {
            var data = _workersObserver.Get(_ownerId);
            
            var foodWorkers = data.Entities[UnitState.GatheringFood].EntitiesList.Count;
            foodText.text = foodWorkers.ToString();
                
            var woodWorkers = data.Entities[UnitState.GatheringWood].EntitiesList.Count;
            woodText.text = woodWorkers.ToString();
                
            var goldWorkers = data.Entities[UnitState.GatheringGold].EntitiesList.Count;
            goldText.text = goldWorkers.ToString();
        }
        
        private void OnEnable()
        {
            if(!_initialized)
                return;
            
            _workersObserver.WorkerStateChanged += WorkerStateChanged;
        }
        
        private void OnDisable()
        {
            _workersObserver.WorkerStateChanged -= WorkerStateChanged;
        }
    }
}