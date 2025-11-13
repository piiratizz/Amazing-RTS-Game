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
    [SerializeField] private ProductionUpgradeView productionUpgradeViewPrefab;
    [SerializeField] private ProducedUnitView producedUnitView;
    [SerializeField] private ProductionQueueUnitView producedQueueUnitView;
    [SerializeField] private GameObject background;
    [SerializeField] private Transform unitsPanelsContainer;
    [SerializeField] private Transform unitsInProductionQueueContainer;
    [SerializeField] private Transform progressBarParent;
    [SerializeField] private Image radialFillProgressBar;

    private BuildingUpgradeComponent _upgradeComponent;
    private BuildingUnitsProductionComponent _productionComponent;
    private List<ProducedUnitView> _unitsInstances = new List<ProducedUnitView>();
    private List<ProductionUpgradeView> _upgradesInstances = new List<ProductionUpgradeView>();
    private List<ProductionQueueUnitView> _unitsInProductionQueueInstances = new List<ProductionQueueUnitView>();

    private CompositeDisposable _disposables = new CompositeDisposable();
    
    private bool _isOpen;

    private List<Entity> _selectedTargets;
    
    public override void Show(List<Entity> targets)
    {
        _productionComponent = null;
        _selectedTargets = targets;

        if(targets.Count > 1 || targets.Count == 0) return;
        
        var target = targets[0];
        
        if (target is not BuildingEntity) return;

        _productionComponent = target.GetEntityComponent<BuildingUnitsProductionComponent>();

        if(_productionComponent == null) return;
            
        var buildComponent = target.GetEntityComponent<BuildingBuildComponent>();

        if (buildComponent != null)
        {
            if (!buildComponent.IsBuilded.CurrentValue)
            {
                buildComponent.IsBuilded
                    .Where(isBuilt => isBuilt)
                    .Subscribe(_ => UpdateView())
                    .AddTo(_disposables);
                return;
            }
        }
        
        _upgradeComponent = target.GetEntityComponent<BuildingUpgradeComponent>();

        if (_upgradeComponent != null)
        {
            Debug.Log("SUBSCRIBED");
            _upgradeComponent.BuildingUpgradedEvent += UpdateView;
        }
        
        BindProgressBar();
        ShowUnits();
        ShowUpgrades();
        ShowProductionQueue();
        background.SetActive(true);
        _isOpen = true;
    }
    
    private void UpdateView()
    {
        Debug.Log("UPDATED");
        Hide();
        Show(_selectedTargets);
    }
    
    private void BindProgressBar()
    {
        _productionComponent.Progress.Subscribe(OnProgressUpdated).AddTo(_disposables);
    }

    private void OnProgressUpdated(float progress)
    {
        radialFillProgressBar.fillAmount = progress;
    }

    private void ShowUpgrades()
    {
        foreach (var upgrade in _productionComponent.UpgradesAvailableToBuild)
        {
            var upgradeInstance = NightPool.Spawn(productionUpgradeViewPrefab, unitsPanelsContainer);
            upgradeInstance.Initialize(upgrade, TryProduceUpgrade);
            
            _upgradesInstances.Add(upgradeInstance);
        }
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
        foreach (var item in _productionComponent.ProductionQueue)
        {
            var instance = NightPool.Spawn(
                producedQueueUnitView,
                unitsInProductionQueueContainer
                );

            instance.Initialize(item.Id, item.Icon, OnProductionCancelled);
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
        if (_productionComponent.TryAddUnitToProductionQueue(config, UpdateProductionQueue))
        {
            UpdateProductionQueue();
        }
        
    }

    private void TryProduceUpgrade(EntityUpgrade upgrade)
    {
        if (_productionComponent.TryAddUpgradeToProductionQueue(upgrade, UpdateProductionQueue))
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
    
    public override void Hide()
    {
        if (_upgradeComponent != null)
        {
            _upgradeComponent.BuildingUpgradedEvent -= UpdateView;
        }
        
        foreach (var instance in _unitsInstances)
        {
            NightPool.Despawn(instance);
        }
        
        foreach (var instance in _upgradesInstances)
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