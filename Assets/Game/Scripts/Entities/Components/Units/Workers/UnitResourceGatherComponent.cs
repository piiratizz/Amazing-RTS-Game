using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitResourceGatherComponent : EntityComponent
{
    [SerializeField] [Range(0, 1)] private float rotationToTargetSpeed;

    private UnitEntity _unitEntity;

    private UnitMovementComponent _movementComponent;
    private UnitAnimationComponent _animationComponent;

    private ResourceEntity _resourceEntity;
    private ResourceStorageComponent _resourceSource;

    private BuildingEntity _storageBuilding;
    private BuildingResourceStorageComponent _resourceStorage;

    private CancellationTokenSource _cancellationToken;

    private int _liftingCapacity;
    private float _timeToGather;

    private int _liftedResources;
    
    private readonly CompositeDisposable _resourceGatheringRoutineDisposable = new();

    public override void Init(Entity entity)
    {
        _unitEntity = entity as UnitEntity;
        _movementComponent = entity.GetEntityComponent<UnitMovementComponent>();
        _animationComponent = entity.GetEntityComponent<UnitAnimationComponent>();
    }

    public override void InitializeFields(EntityConfig config)
    {
        var unitConfig = config as UnitConfig;

        if (unitConfig == null) return;

        _liftingCapacity = unitConfig.LiftingCapacity;
        _timeToGather = _liftingCapacity / unitConfig.GatherRatePerSecond;
    }

    private async UniTask StartResourceGatheringLoop(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await FollowToSource(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                _movementComponent.StopMoving();
                RotateToTarget(_resourceSource.transform.position);
                _animationComponent.SetAttack(true);

                float t = 0f;
                while (t < _timeToGather)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    t += Time.deltaTime;
                    await UniTask.Yield(cancellationToken);
                }

                _liftedResources = _resourceSource.TryExtractResource(_liftingCapacity);
                _animationComponent.SetAttack(false);

                FindNearestStorage();
                await FollowToStorage(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                _resourceStorage.StoreResource(_resourceSource.ResourceType, _liftedResources);
                _liftedResources = 0;
            }
        }
        catch (OperationCanceledException)
        {
            _resourceGatheringRoutineDisposable.Clear();
            _animationComponent.SetAttack(false);
        }
    }


    private void FindNearestStorage()
    {
        BuildingResourceStorageComponent nearestStorage = null;
        BuildingEntity nearestStorageBuilding = null;

        float nearestStorageDistance = 500f;
        var buildings = Physics.OverlapSphere(transform.position, 100f, LayerMask.GetMask("Buildings"));
        foreach (var building in buildings)
        {
            if (building.TryGetComponent<BuildingEntity>(out var storage))
            {
                if (storage.OwnerId != _unitEntity.OwnerId)
                    continue;

                var distance = Vector3.Distance(building.transform.position, transform.position);
                if (distance < nearestStorageDistance)
                {
                    var component = storage.GetEntityComponent<BuildingResourceStorageComponent>();
                    if (component != null)
                    {
                        nearestStorageBuilding = storage;
                        nearestStorage = component;
                        nearestStorageDistance = distance;
                    }
                }
            }
        }

        _storageBuilding = nearestStorageBuilding;
        _resourceStorage = nearestStorage;
    }

    private async UniTask FollowToSource(CancellationToken cancellationToken)
    {
        float angle = Random.Range(0f, 360f);

        float r = Mathf.Max(_resourceEntity.SizeX, _resourceEntity.SizeZ);
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * r;
        Vector3 targetPoint = _resourceSource.transform.position + offset;

        _movementComponent.MoveTo(targetPoint);

        await UniTask.WaitWhile(() => _movementComponent.IsMoving(), cancellationToken: cancellationToken);
    }

    private async UniTask FollowToStorage(CancellationToken cancellationToken)
    {
        float angle = Random.Range(0f, 360f);

        float r = Mathf.Max(_storageBuilding.SizeX, _storageBuilding.SizeZ);
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * r;
        Vector3 targetPoint = _storageBuilding.transform.position + offset;

        _movementComponent.MoveTo(targetPoint);
        
        await UniTask.WaitWhile(() => _movementComponent.IsMoving(), cancellationToken: cancellationToken);
    }

    private void RotateToTarget(Vector3 target)
    {
        transform.DOLookAt(target, rotationToTargetSpeed, AxisConstraint.Y);
    }

    public void SetNewResourceSource(Entity resourceStorage)
    {
        _resourceEntity = resourceStorage as ResourceEntity;

        if (_resourceEntity == null)
        {
            throw new NullReferenceException("Resource entity is not a resource entity");
        }

        _resourceSource = resourceStorage.GetEntityComponent<ResourceStorageComponent>();


        if (_resourceSource.IsEmpty.CurrentValue == true)
        {
            return;
        }


        _resourceSource.IsEmpty.Where(s => s == true).Subscribe(OnResourceEmpty)
            .AddTo(_resourceGatheringRoutineDisposable);
        _cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource().Token, this.GetCancellationTokenOnDestroy());
        StartResourceGatheringLoop(_cancellationToken.Token).Forget();
    }

    private void OnResourceEmpty(bool state = true)
    {
        StopGathering();
    }
    
    private void StopGathering()
    {
        _cancellationToken?.Cancel();
    }

    public override void OnExit()
    {
        StopGathering();
        _cancellationToken?.Dispose();
        _cancellationToken = null;
    }
}