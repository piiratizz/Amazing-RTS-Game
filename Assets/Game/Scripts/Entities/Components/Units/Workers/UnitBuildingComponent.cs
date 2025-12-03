using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitBuildingComponent : EntityComponent
{
    private List<BuildingTypePrefabLink> _availableBuildings;

    private CancellationTokenSource _cancellationTokenSource;
    
    private UnitConfig _unitConfig;
    private UnitMovementComponent _movementComponent;
    private UnitAnimationComponent _animationComponent;
    private UnitWorkerInventoryComponent _inventoryComponent;
    
    public IReadOnlyList<BuildingTypePrefabLink> AvailableBuildings => _availableBuildings;
    
    public bool IsBuilding {get; private set;}
    
    public override void Init(Entity entity)
    {
        _availableBuildings = new List<BuildingTypePrefabLink>();
        
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
        IsBuilding = true;
        _inventoryComponent.AttachTool(WorkerTools.Hammer);
        StartBuildingLoop(buildingEntity, buildComponent).Forget();
    }

    private async UniTask StartBuildingLoop(BuildingEntity buildingEntity, BuildingBuildComponent buildComponent)
    {
        float halfX = buildingEntity.SizeX * 0.5f;
        float halfZ = buildingEntity.SizeZ * 0.5f;

        float px = buildingEntity.transform.position.x;
        float pz = buildingEntity.transform.position.z;
        
        float t = Random.value;
        
        float per = 2f * (halfX + halfZ);
        
        float d = t * per;
        
        float x = Mathf.Clamp(d - halfZ, -halfX, halfX);
        float z = Mathf.Clamp(halfZ - Mathf.Abs(d - halfX - halfZ), -halfZ, halfZ);

        Vector3 targetPoint = new Vector3(px + x, buildingEntity.transform.position.y, pz + z);

        _movementComponent.MoveTo(targetPoint);
        await UniTask.Yield(_cancellationTokenSource.Token);
        await UniTask.WaitWhile(_movementComponent.IsMoving, cancellationToken: _cancellationTokenSource.Token);

        _animationComponent.SetAttack(true);
        transform.DOLookAt(buildingEntity.transform.position, 0.5f, AxisConstraint.Y);
        
        while (!buildComponent.IsFullHp)
        {
            if(_cancellationTokenSource.Token.IsCancellationRequested) 
                break;

            if (buildingEntity.IsDead)
            {
                OnBuildingComplete();
                return;
            }
                
                
            buildComponent.AddBuildProgress(_unitConfig.BuildingRatePerSecond);
            
            await UniTask.WaitForSeconds(1, cancellationToken: _cancellationTokenSource.Token);
        }

        OnBuildingComplete();
    }

    private void OnBuildingComplete()
    {
        IsBuilding = false;
        _animationComponent.SetAttack(false);
    }
    
    public override void OnExit()
    {
        _cancellationTokenSource?.Cancel();
        OnBuildingComplete();
    }
}