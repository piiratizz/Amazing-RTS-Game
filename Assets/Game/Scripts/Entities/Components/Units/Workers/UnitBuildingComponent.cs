using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitBuildingComponent : EntityComponent
{
    private List<BuildingConfigPrefabLink> _availableBuildings;

    private CancellationTokenSource _cancellationTokenSource;
    
    private UnitConfig _unitConfig;
    private UnitMovementComponent _movementComponent;
    private UnitAnimationComponent _animationComponent;
    private UnitWorkerInventoryComponent _inventoryComponent;
    
    public IReadOnlyList<BuildingConfigPrefabLink> AvailableBuildings => _availableBuildings;
    
    public override void Init(Entity entity)
    {
        _availableBuildings = new List<BuildingConfigPrefabLink>();
        
        _movementComponent = entity.GetEntityComponent<UnitMovementComponent>();
        _animationComponent = entity.GetEntityComponent<UnitAnimationComponent>();
        _inventoryComponent = entity.GetEntityComponent<UnitWorkerInventoryComponent>();
    }

    public override void InitializeFields(EntityConfig config)
    {
        UnitConfig unitConfig = config as UnitConfig;

        if (unitConfig == null)
        {
            throw new InvalidCastException("EntityConfig cannot cast to UnitConfig!");
        }
        
        _unitConfig = unitConfig;
        
        foreach (var building in unitConfig.BuildingsAvailableToBuild)
        {
            _availableBuildings.Add(building);
        }
    }

    public void BuildSelected(Entity entity)
    {
        var buildingEntity = entity as BuildingEntity;
        if(buildingEntity == null) 
            return;

        var buildComponent = entity.GetEntityComponent<BuildingBuildComponent>();
        
        if(buildComponent == null)
            return;

        if (buildComponent.IsFullHp) return;
        
        _cancellationTokenSource?.Cancel();
        
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource
        (
            this.GetCancellationTokenOnDestroy(),
            new CancellationTokenSource().Token
        );
        
        _inventoryComponent.AttachTool(WorkerTools.Hammer);
        StartBuildingLoop(buildingEntity, buildComponent).Forget();
    }

    private async UniTask StartBuildingLoop(BuildingEntity buildingEntity, BuildingBuildComponent buildComponent)
    {
        float angle = Random.Range(0f, 360f);

        float r = Mathf.Max(buildingEntity.SizeX, buildingEntity.SizeZ);
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * r;
        Vector3 targetPoint = buildingEntity.transform.position + offset;

        _movementComponent.MoveTo(targetPoint);
        await UniTask.Yield(_cancellationTokenSource.Token);
        await UniTask.WaitWhile(_movementComponent.IsMoving, cancellationToken: _cancellationTokenSource.Token);

        _animationComponent.SetAttack(true);
        transform.DOLookAt(buildingEntity.transform.position, 0.5f, AxisConstraint.Y);
        
        while (!buildComponent.IsFullHp)
        {
            if(_cancellationTokenSource.Token.IsCancellationRequested) 
                break;
            
            buildComponent.AddBuildProgress(_unitConfig.BuildingRatePerSecond);
            
            await UniTask.WaitForSeconds(1, cancellationToken: _cancellationTokenSource.Token);
        }

        OnBuildingComplete();
    }

    private void OnBuildingComplete()
    {
        _animationComponent.SetAttack(false);
    }
    
    public override void OnExit()
    {
        _cancellationTokenSource?.Cancel();
        _animationComponent.SetAttack(false);
    }
}