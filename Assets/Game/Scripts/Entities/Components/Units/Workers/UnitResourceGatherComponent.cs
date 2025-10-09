using System;
using System.Collections;
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
    
    private Coroutine _resourceGatheringRoutine;
    
    private int _liftingCapacity;
    private float _timeToGather;
    
    private readonly CompositeDisposable _resourceGatheringRoutineDisposable = new();

    private bool _requestedCoroutineStop;
    
    public override void Init(Entity entity)
    {
        _unitEntity = entity as UnitEntity;
        _movementComponent = entity.GetEntityComponent<UnitMovementComponent>();
        _animationComponent = entity.GetEntityComponent<UnitAnimationComponent>();
    }

    public override void InitializeFields(EntityConfig config)
    {
        var unitConfig = config as UnitConfig;
        
        if(unitConfig == null) return;
        
        _liftingCapacity = unitConfig.LiftingCapacity;
        _timeToGather = _liftingCapacity / unitConfig.GatherRatePerSecond;
    }

    private IEnumerator ResourceGatheringLoop()
    {
        while (!_requestedCoroutineStop)
        {
            FollowToSource();
            yield return null;
            yield return new WaitWhile(() => _movementComponent.IsMoving());
            
            if(_requestedCoroutineStop) 
                yield break;
            
            _movementComponent.StopMoving();
            RotateToTarget(_resourceSource.transform.position);
            _animationComponent.SetAttack(true);
            
            float t = 0f;
            while (t < _timeToGather)
            {
                if (_requestedCoroutineStop) 
                    yield break;
                
                t += Time.deltaTime;
                yield return null;
            }
            
            var extractedResources = _resourceSource.TryExtractResource(_liftingCapacity);
            _animationComponent.SetAttack(false);
            
            FindNearestStorage();
            FollowToStorage();
            yield return null;
            yield return new WaitWhile(() => _movementComponent.IsMoving());
            
            if (_requestedCoroutineStop) 
                yield break;
            
            _resourceStorage.StoreResource(_resourceSource.ResourceType, extractedResources);
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
            if(building.TryGetComponent<BuildingEntity>(out var storage))
            {
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
    
    private void FollowToSource()
    {
        float angle = Random.Range(0f, 360f);

        float r = Mathf.Max(_resourceEntity.SizeX, _resourceEntity.SizeZ);
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * r;
        Vector3 targetPoint = _resourceSource.transform.position + offset;
        
        _movementComponent.MoveTo(targetPoint);
    }
    
    private void FollowToStorage()
    {
        _movementComponent.MoveTo(_resourceStorage.transform.position);
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
        
        if (_resourceGatheringRoutine != null)
        {
            _requestedCoroutineStop = true;
        }

        if (_resourceSource.IsEmpty.CurrentValue == true)
        {
            return;
        }
        
        _requestedCoroutineStop = false;
        _resourceSource.IsEmpty.Where(s => s == true).Subscribe(StopGathering).AddTo(_resourceGatheringRoutineDisposable);
        _resourceGatheringRoutine = StartCoroutine(ResourceGatheringLoop());
    }

    private void StopGathering(bool state = true)
    {
        if (_resourceGatheringRoutine != null)
        {
            _requestedCoroutineStop = true;
            StopCoroutine(_resourceGatheringRoutine);
        }
        
        _resourceGatheringRoutineDisposable.Clear();
        _animationComponent.SetAttack(false);
        _movementComponent.StopMoving();
    }
    
    public override void OnExit()
    {
        StopGathering();
    }
}