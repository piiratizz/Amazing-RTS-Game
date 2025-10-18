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
    private BuildingConfig _buildingConfig;
    
    private Queue<ConfigUnitPrefabLink> _productionQueue;
    private float _currentUnitProductionValue;
    private float _currentUnitProductionCost;
    private float _productionRatePerSecond;

    private ReactiveProperty<float> _progress;
    public ReadOnlyReactiveProperty<float> Progress => _progress;
    
    public IReadOnlyCollection<UnitResourceCost> UnitsAvailableToBuild;
    public IReadOnlyCollection<ConfigUnitPrefabLink> ProductionQueue => _productionQueue;
    
    private CancellationTokenSource _cancellationToken;
    
    [Inject] private UnitsFactory _unitsFactory;
    
    public override void Init(Entity entity)
    {
        _productionQueue = new Queue<ConfigUnitPrefabLink>();
        _progress = new ReactiveProperty<float>();
    }

    public override void InitializeFields(EntityConfig config)
    {
        _buildingConfig  = config as BuildingConfig;

        if (_buildingConfig == null)
        {
            throw new InvalidCastException("Config must be of type BuildingConfig");
        }
        _productionRatePerSecond = _buildingConfig.ProductionRatePerSecond;
        UnitsAvailableToBuild = _buildingConfig.UnitsCanProduce;
    }

    public void AddUnitToProductionQueue(UnitConfig config, Action<ConfigUnitPrefabLink> onUnitProduced)
    {
        var unit = UnitsAvailableToBuild.First(u => u.Unit.Config == config).Unit;
        _productionQueue.Enqueue(unit);

        if (_productionQueue.Count == 1)
        {
            _cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(
                new CancellationTokenSource().Token,
                this.GetCancellationTokenOnDestroy()
                );
            
            StartProduction(_cancellationToken.Token, onUnitProduced).Forget();
        }
    }

    private async UniTask StartProduction(CancellationToken token, Action<ConfigUnitPrefabLink> onUnitProduced)
    {
        while (_productionQueue.Count > 0)
        {
            var unitToProduce = _productionQueue.Peek();
            _currentUnitProductionCost = unitToProduce.Config.TotalProductionCost;
            _currentUnitProductionValue = 0f;

            while (_currentUnitProductionValue < _currentUnitProductionCost)
            {
                token.ThrowIfCancellationRequested();
            
                _currentUnitProductionValue += _productionRatePerSecond * Time.deltaTime;
            
                _progress.Value = _currentUnitProductionValue / _currentUnitProductionCost;

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
            
            _productionQueue.Dequeue();
            OnProductionComplete(unitToProduce, onUnitProduced);
            
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }
    
    
    private void OnProductionComplete(ConfigUnitPrefabLink unit, Action<ConfigUnitPrefabLink> onUnitProduced)
    {
        //var instance = _unitsFactory.Create();
        onUnitProduced?.Invoke(unit);
    }
}