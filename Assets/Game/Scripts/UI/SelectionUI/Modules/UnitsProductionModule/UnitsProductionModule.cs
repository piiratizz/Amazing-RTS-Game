using R3;
using System.Collections.Generic;
using Game.Scripts.UI;
using NTC.Pool;
using UnityEngine;
using UnityEngine.UI;

public class UnitsProductionModule : SelectionPanelModule
{
    [SerializeField] private ProducedUnitView producedUnitView;
    [SerializeField] private ProductionQueueUnitView producedQueueUnitView;
    [SerializeField] private GameObject background;
    [SerializeField] private Transform unitsPanelsContainer;
    [SerializeField] private Transform unitsInProductionQueueContainer;
    [SerializeField] private Image radialFillProgressBar;
    
    private BuildingUnitsProductionComponent _productionComponent;
    private List<ProducedUnitView> _unitsInstances = new List<ProducedUnitView>();
    private List<ProductionQueueUnitView> _unitsInProductionQueueInstances = new List<ProductionQueueUnitView>();

    private CompositeDisposable _disposables = new CompositeDisposable();
    
    public override void Show(List<Entity> targets)
    {
        _productionComponent = null;
        
        foreach (var target in targets)
        {
            if (target is not BuildingEntity) continue;

            _productionComponent = target.GetEntityComponent<BuildingUnitsProductionComponent>();
            
            if(_productionComponent == null) continue;
            
            background.SetActive(true);
        }

        if (_productionComponent != null)
        {
            BindProgressBar();
            ShowUnits();
            ShowProductionQueue();
        }
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
            instance.Initialize(unit.Config.Icon);
            _unitsInProductionQueueInstances.Add(instance);
        }

        if (_productionComponent.ProductionQueue.Count > 0)
        {
            radialFillProgressBar.gameObject.SetActive(true);
        }
        
    }
    
    private void HideProductionQueue()
    {
        foreach (var instance in _unitsInProductionQueueInstances)
        {
            NightPool.Despawn(instance);
        }
        _unitsInProductionQueueInstances.Clear();
        radialFillProgressBar.gameObject.SetActive(false);
    }

    private void TryProduceUnit(UnitConfig config)
    {
        _productionComponent.AddUnitToProductionQueue(config, OnUnitProduced);
        UpdateProductionQueue();
    }

    private void UpdateProductionQueue()
    {
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
        radialFillProgressBar.gameObject.SetActive(false);
        background.SetActive(false);
        _disposables.Clear();
    }
}