using R3;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.UI;
using NTC.Pool;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UnitsProductionModule : SelectionPanelModule
{
    [SerializeField] private ProducedUnitView producedUnitView;
    [SerializeField] private ProductionQueueUnitView producedQueueUnitView;
    [SerializeField] private GameObject background;
    [SerializeField] private Transform unitsPanelsContainer;
    [SerializeField] private Transform unitsInProductionQueueContainer;
    [SerializeField] private Transform progressBarParent;
    [SerializeField] private Image radialFillProgressBar;
    
    private BuildingUnitsProductionComponent _productionComponent;
    private List<ProducedUnitView> _unitsInstances = new List<ProducedUnitView>();
    private List<ProductionQueueUnitView> _unitsInProductionQueueInstances = new List<ProductionQueueUnitView>();

    private CompositeDisposable _disposables = new CompositeDisposable();
    
    private bool _isOpen;
    
    public override void Show(List<Entity> targets)
    {
        _productionComponent = null;
        
        foreach (var target in targets)
        {
            if (target is not BuildingEntity) return;

            _productionComponent = target.GetEntityComponent<BuildingUnitsProductionComponent>();
            
            if(_productionComponent == null) return;
        }

        if (_productionComponent == null) return;
        
        BindProgressBar();
        ShowUnits();
        ShowProductionQueue();
        background.SetActive(true);
        _isOpen = true;
    }

    private void BindProgressBar()
    {
        _productionComponent.Progress.Subscribe(OnProgressUpdated).AddTo(_disposables);
    }

    private void OnProgressUpdated(float progress)
    {
        radialFillProgressBar.fillAmount = progress;
    }
    
    private void ShowUnits()
    {
        foreach (var unit in _productionComponent.UnitsAvailableToBuild)
        {
            ProducedUnitView instance = NightPool.Spawn(producedUnitView, unitsPanelsContainer);
            instance.Initialize(
                unit.Unit.Config,
                unit.ResourceCost,
                TryProduceUnit
                );
            
            _unitsInstances.Add(instance);
        }
    }

    private void ShowProductionQueue()
    {
        foreach (var unit in _productionComponent.ProductionQueue)
        {
            var instance = NightPool.Spawn(
                producedQueueUnitView,
                unitsInProductionQueueContainer
                );
            instance.Initialize(unit.UnitId, unit.Unit.Config, OnProductionCancelled);
            _unitsInProductionQueueInstances.Add(instance);
        }

        if (_productionComponent.ProductionQueue.Count > 0)
        {
            progressBarParent.gameObject.SetActive(true);
        }
        
    }

    private void OnProductionCancelled(int queueId)
    {
        _productionComponent.RemoveFromProductionQueue(queueId);
        UpdateProductionQueue();
    }
    
    private void HideProductionQueue()
    {
        foreach (var instance in _unitsInProductionQueueInstances)
        {
            NightPool.Despawn(instance);
        }
        _unitsInProductionQueueInstances.Clear();
        progressBarParent.gameObject.SetActive(false);
    }

    private void TryProduceUnit(UnitConfig config)
    {
        if (_productionComponent.TryAddUnitToProductionQueue(config, OnUnitProduced))
        {
            UpdateProductionQueue();
        }
        
    }

    private void UpdateProductionQueue()
    {
        if(!_isOpen) return;
        
        HideProductionQueue();
        ShowProductionQueue();
    }
    
    private void OnUnitProduced(ConfigUnitPrefabLink unit)
    {
        UpdateProductionQueue();
    }
    
    
    public override void Hide()
    {
        foreach (var instance in _unitsInstances)
        {
            NightPool.Despawn(instance);
        }
        
        _unitsInstances.Clear();
        HideProductionQueue();
        progressBarParent.gameObject.SetActive(false);
        background.SetActive(false);
        _disposables.Clear();
        _isOpen = false;
    }
}