using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

public class BuildingUnitsProductionComponent : EntityComponent
{
    [SerializeField] private Transform unitsSpawnPoint;

    [Inject] private GlobalResourceStorage _resourceStorage;
    
    private BuildingConfig _buildingConfig;

    private Queue<ProductionLineUnit> _productionQueue;

    private float _currentUnitProductionValue;
    private float _currentUnitProductionCost;
    private float _productionRatePerSecond;

    private ReactiveProperty<float> _progress;
    public ReadOnlyReactiveProperty<float> Progress => _progress;

    public IReadOnlyCollection<UnitResourceCost> UnitsAvailableToBuild;
    public IReadOnlyCollection<ProductionLineUnit> ProductionQueue => _productionQueue;

    private CancellationTokenSource _cancellationToken;

    private UnitFactory _unitFactory;

    private ProductionLineUnit _unitCurrentlyProduced;
    
    private Action<ConfigUnitPrefabLink> _onUnitProducedCallback;
    
    public override void Init(Entity entity)
    {
        _productionQueue = new Queue<ProductionLineUnit>();
        _progress = new ReactiveProperty<float>();
        _unitFactory = new UnitFactory(entity.OwnerId);
    }

    public override void InitializeFields(EntityConfig config)
    {
        _buildingConfig = config as BuildingConfig;

        if (_buildingConfig == null)
        {
            throw new InvalidCastException("Config must be of type BuildingConfig");
        }

        _productionRatePerSecond = _buildingConfig.ProductionRatePerSecond;
        UnitsAvailableToBuild = _buildingConfig.UnitsCanProduce;
    }

    public bool TryAddUnitToProductionQueue(UnitConfig config, Action<ConfigUnitPrefabLink> onUnitProducedCallback)
    {
        var unit = UnitsAvailableToBuild.First(u => u.Unit.Config == config);
        
        foreach (var cost in unit.ResourceCost)
        {
            if (!_resourceStorage.IsEnough(cost.Resource, cost.Amount))
            {
                return false;
            }
        }
        
        foreach (var cost in unit.ResourceCost)
        {
            _resourceStorage.TrySpend(cost.Resource, cost.Amount);
        }
        
        _onUnitProducedCallback = onUnitProducedCallback;
        
        var productionLineUnit = new ProductionLineUnit()
        {
            UnitId = _productionQueue.Count,
            Unit = unit.Unit,
        };
        
        _productionQueue.Enqueue(productionLineUnit);

        if (_productionQueue.Count == 1)
        {
            StartProduction();
        }

        return true;
    }

    private void StartProduction()
    {
        _cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(
            new CancellationTokenSource().Token,
            this.GetCancellationTokenOnDestroy()
        );

        StartProduction(_cancellationToken.Token).Forget();
    }
    
    private async UniTask StartProduction(CancellationToken token)
    {
        while (_productionQueue.Count > 0)
        {
            _unitCurrentlyProduced = _productionQueue.Peek();
            _currentUnitProductionCost = _unitCurrentlyProduced.Unit.Config.TotalProductionCost;
            _currentUnitProductionValue = 0f;

            while (_currentUnitProductionValue < _currentUnitProductionCost)
            {
                token.ThrowIfCancellationRequested();

                _currentUnitProductionValue += _productionRatePerSecond * Time.deltaTime;

                _progress.Value = _currentUnitProductionValue / _currentUnitProductionCost;

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            _productionQueue.Dequeue();

            OnProductionComplete(_unitCurrentlyProduced, _onUnitProducedCallback);

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }


    public void RemoveFromProductionQueue(int queueId)
    {
        var unitToRemove = _productionQueue.First(i => i.UnitId == queueId);
        
        if (_unitCurrentlyProduced.UnitId == unitToRemove.UnitId)
        {
            _cancellationToken.Cancel();
        }
        
        _productionQueue = new Queue<ProductionLineUnit>(
            _productionQueue.Where(link => link.UnitId != queueId)
        );

        if (_unitCurrentlyProduced.UnitId == unitToRemove.UnitId)
        {
            if (_productionQueue.Count >= 1)
            {
                StartProduction();
            }
        }
        
        var resources = UnitsAvailableToBuild.First(u => u.Unit.Config == unitToRemove.Unit.Config).ResourceCost;

        foreach (var cost in resources)
        {
            _resourceStorage.Add(cost.Resource, cost.Amount);
        }
    }

    private void OnProductionComplete(ProductionLineUnit unit, Action<ConfigUnitPrefabLink> onUnitProduced)
    {
        var instance = _unitFactory.CreateUnit(unit.Unit, unitsSpawnPoint.position);
        onUnitProduced?.Invoke(unit.Unit);
    }
}

public class ProductionLineUnit
{
    public int UnitId;
    public ConfigUnitPrefabLink Unit;
}