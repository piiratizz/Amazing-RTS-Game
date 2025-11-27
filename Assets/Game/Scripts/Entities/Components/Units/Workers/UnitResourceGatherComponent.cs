using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitResourceGatherComponent : EntityComponent
{
    [SerializeField] [Range(0, 1)] private float rotationToTargetSpeed;
    
    [InfoBox("Optional: can be null if no animation handler is used.")]
    [SerializeField] private UnitAnimationsEventsHandler eventsHandler;
    
    private UnitEntity _unitEntity;

    private UnitMovementComponent _movementComponent;
    private UnitAnimationComponent _animationComponent;
    private UnitWorkerInventoryComponent _inventoryComponent;
    private UnitVisualResourceHoldingComponent _resourceHoldingComponent;
    
    private ResourceEntity _resourceEntity;
    private ResourceSourceComponent _resourceSource;
    private Vector3 _sourcePosition;
    
    private BuildingEntity _storageBuilding;
    private BuildingResourceStorageComponent _resourceStorage;

    private CancellationTokenSource _cancellationToken;

    private int _liftingCapacity;
    private float _timeToGather;

    private int _liftedResources;
    private ResourceType _lifterResourcesType;
    
    private readonly CompositeDisposable _resourceGatheringRoutineDisposable = new();
    
    public (bool, ResourceType) IsGathering { get; private set; }
    
    public override void Init(Entity entity)
    {
        _unitEntity = entity as UnitEntity;
        _movementComponent = entity.GetEntityComponent<UnitMovementComponent>();
        _animationComponent = entity.GetEntityComponent<UnitAnimationComponent>();
        _resourceHoldingComponent = entity.GetEntityComponent<UnitVisualResourceHoldingComponent>();
        _inventoryComponent = entity.GetEntityComponent<UnitWorkerInventoryComponent>();
    }

    private void OnHitEvent(AnimationHitArgs args)
    {
        if (_resourceSource != null)
        {
            _resourceSource.TakeHit(args);
        }
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
            IsGathering = (true, _resourceSource.ResourceType);
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_resourceSource == null || _resourceSource.IsEmpty.CurrentValue)
                {
                    TryFindNearestSource();
                }
                
                await FollowToSource(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                if (_resourceSource == null || _resourceSource.IsEmpty.CurrentValue)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                    continue;
                }
                
                _movementComponent.StopMoving();
                RotateToTarget(_resourceSource.transform.position);
                _animationComponent.SetAttack(true);

                float t = 0f;
                while (t < _timeToGather)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    t += Time.deltaTime;
                    if (_resourceSource == null)
                    {
                        break;
                    }

                    if (_resourceSource.IsEmpty.CurrentValue)
                    {
                        break;
                    }
                    
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                }

                if (_resourceSource != null)
                {
                    if (!_resourceSource.IsEmpty.CurrentValue)
                    {
                        _lifterResourcesType = _resourceSource.ResourceType;
                        _liftedResources = _resourceSource.TryExtractResource(_liftingCapacity);
                        _resourceHoldingComponent?.ShowResource(_lifterResourcesType);
                    }
                    else
                    {
                        _animationComponent.SetAttack(false);
                        continue;
                    }
                }
                
                _animationComponent.SetAttack(false);

                if (_liftedResources != 0)
                {
                    if (!TryFindNearestStorage())
                    {
                        throw new OperationCanceledException("No storage is near");
                    }
                    await FollowToStorage(cancellationToken);
                }
               
                cancellationToken.ThrowIfCancellationRequested();

                if (_liftedResources != 0)
                {
                    _resourceHoldingComponent?.Clear();
                    _resourceStorage.StoreResource(_lifterResourcesType, _liftedResources);
                }
                
                _liftedResources = 0;
            }
        }
        catch (OperationCanceledException)
        {
            _resourceGatheringRoutineDisposable.Clear();
            if (_animationComponent != null)
            {
                _animationComponent.SetAttack(false);
            }
            IsGathering = (false, default);
        }
    }
    
    private bool TryFindNearestStorage()
    {
        BuildingResourceStorageComponent nearestStorage = null;
        BuildingEntity nearestStorageBuilding = null;

        float nearestStorageDistance = 500f;
        var buildings = Physics.OverlapSphere(transform.position, 100f, LayerMask.GetMask("Buildings"));
        foreach (var building in buildings)
        {
            if (building.TryGetComponent<BuildingEntity>(out var storage))
            {
                var buildingComponent = storage.GetEntityComponent<BuildingBuildComponent>();

                if (buildingComponent != null)
                {
                    if (!buildingComponent.IsBuilded.CurrentValue)
                    {
                        continue;
                    }
                }
                
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

        return _storageBuilding != null;
    }

    private bool TryFindNearestSource()
    {
        ResourceSourceComponent nearestSource = null;
        ResourceEntity nearestResourceEntity = null;

        float sqrNearestSourceDistance = 500f;
        var resources = Physics.OverlapSphere(transform.position, 100f, LayerMask.GetMask("Resources"));
        foreach (var resource in resources)
        {
            if (resource.TryGetComponent<ResourceEntity>(out var storage))
            {
                var component = storage.GetEntityComponent<ResourceSourceComponent>();
                if (component == null)
                {
                    continue;
                }
                
                if (component.IsEmpty.CurrentValue)
                {
                    continue;
                }
                
                var distance = (resource.transform.position - _sourcePosition).sqrMagnitude;
                if (distance < sqrNearestSourceDistance)
                {
                    nearestResourceEntity = storage;
                    nearestSource = component;
                    sqrNearestSourceDistance = distance;
                }
            }
        }

        if (resources.Length == 0)
        {
            return false;
        }
        
        _resourceEntity = nearestResourceEntity;
        _resourceSource = nearestSource;
        _sourcePosition = nearestSource.transform.position;
        return true;
    }
    
    private async UniTask FollowToSource(CancellationToken cancellationToken)
    {
        float angle = Random.Range(0f, 360f);

        float r = Mathf.Max(_resourceEntity.SizeX, _resourceEntity.SizeZ);
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * r;
        Vector3 targetPoint = _resourceSource.transform.position + offset;

        _movementComponent.MoveTo(targetPoint);

        await UniTask.WaitWhile(() => (targetPoint - transform.position).sqrMagnitude > 0.3f, cancellationToken: cancellationToken);
    }

    private async UniTask FollowToStorage(CancellationToken cancellationToken)
    {
        float angle = Random.Range(0f, 360f);

        float r = Mathf.Max(_storageBuilding.SizeX, _storageBuilding.SizeZ);
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * r;
        Vector3 targetPoint = _storageBuilding.transform.position + offset;

        _movementComponent.MoveTo(targetPoint);
        
        await UniTask.WaitWhile(() => (targetPoint - transform.position).sqrMagnitude > 0.3f, cancellationToken: cancellationToken);
    }

    private void RotateToTarget(Vector3 target)
    {
        transform.DOLookAt(target, rotationToTargetSpeed, AxisConstraint.Y);
    }

    public void OrderToGather(Entity resourceStorage)
    {
        _inventoryComponent.AttachTool(WorkerTools.Pickaxe);
        SetNewResourceSource(resourceStorage);
        StartGathering();
    }

    private void SetNewResourceSource(Entity resourceStorage)
    {
        _resourceEntity = resourceStorage as ResourceEntity;

        if (_resourceEntity == null)
        {
            throw new NullReferenceException("Resource entity is not a resource entity");
        }

        _resourceSource = resourceStorage.GetEntityComponent<ResourceSourceComponent>();
        _sourcePosition = _resourceSource.transform.position;

        if (_resourceSource.IsEmpty.CurrentValue)
        {
            return;
        }
        
        _resourceSource.IsEmpty.Where(s => s == true).Subscribe(OnResourceEmpty)
            .AddTo(_resourceGatheringRoutineDisposable);
        
        _resourceSource.SelectResourceToGather();
    }
    
    private void OnResourceEmpty(bool state = true)
    {
        _resourceEntity = null;
        _resourceSource = null;
        _animationComponent.SetAttack(false);
    }
    
    private void StartGathering()
    {
        _cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource().Token, this.GetCancellationTokenOnDestroy());
        StartResourceGatheringLoop(_cancellationToken.Token).Forget();
    }
    
    private void StopGathering()
    {
        _cancellationToken?.Cancel();
        IsGathering = (false, default);
        _resourceSource = null;
        _animationComponent.SetAttack(false);
    }

    public override void OnExit()
    {
        StopGathering();
        _cancellationToken?.Dispose();
        _cancellationToken = null;
    }

    private void OnEnable()
    {
        if (eventsHandler != null)
        {
            eventsHandler.OnHitEvent += OnHitEvent;
        }
    }

    private void OnDisable()
    {
        if (eventsHandler != null)
        {
            eventsHandler.OnHitEvent -= OnHitEvent;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_storageBuilding != null && _sourcePosition != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_storageBuilding.transform.position, 0.3f);
            Gizmos.DrawLine(_storageBuilding.transform.position, _sourcePosition);
        }

        if (_sourcePosition != Vector3.zero)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_sourcePosition, 0.3f);
        }
        
        if (_resourceSource != null && _sourcePosition != Vector3.zero)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_sourcePosition, 0.3f);
            Gizmos.DrawLine(_sourcePosition, _resourceSource.transform.position);
        }

        if (_resourceSource != null && _storageBuilding != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_resourceSource.transform.position, 0.3f);
            Gizmos.DrawLine(_resourceSource.transform.position, _storageBuilding.transform.position);
        }
        
    }
}