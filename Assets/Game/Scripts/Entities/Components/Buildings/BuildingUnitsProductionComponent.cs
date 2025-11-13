using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using GlobalResourceStorageSystem;
using R3;
using UnityEngine;
using Zenject;

public class BuildingUnitsProductionComponent : EntityComponent
{
    [SerializeField] private Transform unitsSpawnPoint;

    [Inject] private ResourcesStoragesManager _storagesManager;
    private GlobalResourceStorage _resourceStorage;

    [Inject] private GlobalUpgradesManager _globalUpgradesManager;
    
    private BuildingConfig _buildingConfig;

    private Queue<ProductionLineItem> _productionQueue;

    private int _ownerId;

    private float _currentUnitProductionValue;
    private float _currentUnitProductionCost;
    private float _productionRatePerSecond;

    private ReactiveProperty<float> _progress;
    public ReadOnlyReactiveProperty<float> Progress => _progress;

    public IReadOnlyCollection<UnitResourceCost> UnitsAvailableToBuild;
    public IReadOnlyCollection<EntityUpgrade> UpgradesAvailableToBuild;
    public IReadOnlyCollection<ProductionLineItem> ProductionQueue => _productionQueue;

    private CancellationTokenSource _cancellationToken;

    [Inject] private UnitFactory _unitFactory;

    private ProductionLineItem _itemCurrentlyProduced;

    private Action _onItemProducedCallback;
    
    public override void Init(Entity entity)
    {
        _productionQueue = new Queue<ProductionLineItem>();
        _progress = new ReactiveProperty<float>();
        _ownerId = entity.OwnerId;
        _resourceStorage = _storagesManager.Get(_ownerId);
    }

    public override void InitializeFields(EntityConfig config)
    {
        _buildingConfig = config as BuildingConfig
            ?? throw new InvalidCastException("Config must be of type BuildingConfig");

        UpgradesAvailableToBuild = _buildingConfig.UpgradesCanProduce;
        _productionRatePerSecond = _buildingConfig.ProductionRatePerSecond;
        UnitsAvailableToBuild = _buildingConfig.UnitsCanProduce;
    }
    
    private bool TryPayResourceCost(IEnumerable<ResourceCost> costs)
    {
        if (!costs.All(cost => _resourceStorage.IsEnough(cost.Resource, cost.Amount)))
            return false;

        foreach (var cost in costs)
            _resourceStorage.TrySpend(cost.Resource, cost.Amount);

        return true;
    }

    private ProductionLineItem CreateProductionItem(
        Sprite icon,
        float productionCost,
        ConfigUnitPrefabLink unit,
        EntityUpgrade upgrade,
        ResourceCost[] cost,
        Action<ProductionLineItem> callback)
    {
        return new ProductionLineItem
        {
            Icon = icon,
            Id = _productionQueue.Count,
            ProductionCost = productionCost,
            Unit = unit,
            Upgrade = upgrade,
            ResourceCost = cost,
            OnProducedCallback = callback
        };
    }

    private void EnqueueAndStartIfNeeded(ProductionLineItem item)
    {
        _productionQueue.Enqueue(item);
        if (_productionQueue.Count == 1)
            StartProduction();
    }
    
    public bool TryAddUpgradeToProductionQueue(EntityUpgrade upgrade, Action onItemProducedCallback)
    {
        if (!TryPayResourceCost(upgrade.ResourceCost))
            return false;

        _onItemProducedCallback = onItemProducedCallback;
        
        var item = CreateProductionItem(
            upgrade.Icon,
            upgrade.ProductionCost,
            null,
            upgrade,
            upgrade.ResourceCost,
            OnUpgradeProductionComplete);
        
        EnqueueAndStartIfNeeded(item);
        return true;
    }

    public bool TryAddUnitToProductionQueue(UnitConfig config, Action onItemProducedCallback)
    {
        var unit = UnitsAvailableToBuild.First(u => u.Unit.Config == config);

        if (!TryPayResourceCost(unit.ResourceCost))
            return false;

        _onItemProducedCallback = onItemProducedCallback;

        var item = CreateProductionItem(
            unit.Unit.Config.Icon,
            unit.Unit.Config.TotalProductionCost,
            unit.Unit,
            null,
            unit.ResourceCost,
            OnUnitProductionComplete);

        EnqueueAndStartIfNeeded(item);
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
            _itemCurrentlyProduced = _productionQueue.Peek();
            _currentUnitProductionCost = _itemCurrentlyProduced.ProductionCost;
            _currentUnitProductionValue = 0f;

            while (_currentUnitProductionValue < _currentUnitProductionCost)
            {
                token.ThrowIfCancellationRequested();

                _currentUnitProductionValue += _productionRatePerSecond * Time.deltaTime;
                _progress.Value = _currentUnitProductionValue / _currentUnitProductionCost;

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            _productionQueue.Dequeue();
            _itemCurrentlyProduced.OnProducedCallback?.Invoke(_itemCurrentlyProduced);

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }

    public void RemoveFromProductionQueue(int queueId)
    {
        var itemToRemove = _productionQueue.First(i => i.Id == queueId);

        Debug.Log(itemToRemove.ResourceCost);
        
        bool wasProducing = _itemCurrentlyProduced?.Id == queueId;

        if (wasProducing)
            _cancellationToken.Cancel();

        _productionQueue = new Queue<ProductionLineItem>(
            _productionQueue.Where(link => link.Id != queueId)
        );

        if (wasProducing && _productionQueue.Count > 0)
            StartProduction();
        
        foreach (var cost in itemToRemove.ResourceCost)
            _resourceStorage.Add(cost.Resource, cost.Amount);
    }

    private void OnUnitProductionComplete(ProductionLineItem item)
    {
        var instance = _unitFactory.Create(_ownerId, item.Unit, unitsSpawnPoint.position);
        _onItemProducedCallback?.Invoke();
    }

    private void OnUpgradeProductionComplete(ProductionLineItem item)
    {
        _globalUpgradesManager.AddUpgrade(_ownerId, item.Upgrade);
        _onItemProducedCallback?.Invoke();
    }
}

public class ProductionLineItem
{
    public int Id;
    public Sprite Icon;
    public ConfigUnitPrefabLink Unit;
    public EntityUpgrade Upgrade;
    public float ProductionCost;
    public ResourceCost[] ResourceCost;
    public Action<ProductionLineItem> OnProducedCallback;
}
