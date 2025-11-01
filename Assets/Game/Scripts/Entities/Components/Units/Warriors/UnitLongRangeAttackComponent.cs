using System;
using System.Threading;
using ComponentsActionTypes;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

public class UnitLongRangeAttackComponent : EntityComponent, IAttackable
{
    [SerializeField] private ArrowVFXTrigger arrowVFXTrigger;
    [SerializeField] private UnitAnimationsEventsHandler unitAnimationsEventsHandler;
    
    private Entity _targetEntity;
    private Entity _thisEntity;
    private UnitConfig _config;
    
    private UnitAnimationComponent _unitAnimationsComponent;
    private UnitMovementComponent _unitMovementComponent;
    private UnitDetectionComponent _unitDetectionComponent;
    private UnitCommandDispatcher _unitCommandDispatcher;
    private HealthComponent _unitHealthComponent;
    private UnitStateComponent _unitStateComponent;
    
    private HealthComponent _targetHealthComponent;
    private UnitDamageResistanceComponent _targetResistanceComponent;
    private UnitMovementComponent _targetMovementComponent;

    private CancellationToken _cancellationToken;

    public bool IsCanAutoAttack { get; set; } = true;

    public bool IsAttacking { get; set; }

    private bool _initialized = false;
    
    public override void Init(Entity entity)
    {
        _cancellationToken = this.GetCancellationTokenOnDestroy();
        
        _thisEntity = entity;
        _unitAnimationsComponent = entity.GetEntityComponent<UnitAnimationComponent>();
        _unitMovementComponent = entity.GetEntityComponent<UnitMovementComponent>();
        _unitDetectionComponent = entity.GetEntityComponent<UnitDetectionComponent>();
        _unitCommandDispatcher = entity.GetEntityComponent<UnitCommandDispatcher>();
        _unitHealthComponent = entity.GetEntityComponent<HealthComponent>();
        _unitStateComponent = entity.GetEntityComponent<UnitStateComponent>();

        _unitStateComponent.OnStateChange += OnStateChanged;
        unitAnimationsEventsHandler.OnProjectileLauncherEvent += LaunchArrow;
        _initialized = true;
    }

    public override void InitializeFields(EntityConfig config)
    {
        _config = config as UnitConfig;
    }

    public override void OnUpdate()
    {
        if (!_initialized)
        {
            return;
        }
        
        if (_unitHealthComponent.IsDead)
        {
            return;
        }
        
        if (IsCanAutoAttack)
        {
            var closestEnemy = _unitDetectionComponent?.ClosestEnemy;
            
            if (closestEnemy != null && closestEnemy != _targetEntity)
            {
                _unitCommandDispatcher.ExecuteCommand(UnitCommandsType.Attack,
                    new AttackArgs()
                        { Entity = closestEnemy, TotalUnits = 1, UnitOffsetIndex = 0 });
            }
        }
        
        if(!IsAttacking) return;
        
        if (_targetHealthComponent != null)
        {
            if (_targetHealthComponent.IsDead)
            {
                OnTargetLost();
                return;
            }
        }
        
        var toTarget = _targetEntity.transform.position - transform.position;
        
        if (toTarget != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(toTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 360f * Time.deltaTime);
        }
        
        var distanceToTarget = toTarget.sqrMagnitude;
        
        if (Math.Round(distanceToTarget, 2) > _config.AttackRange * _config.AttackRange)
        {
            var stopPoint = _targetEntity.transform.position - toTarget.normalized * _config.AttackRange;
            _unitMovementComponent.MoveTo(stopPoint);
            _unitAnimationsComponent.SetAttack(false);
        }
        else
        {
            _unitAnimationsComponent.SetAttack(true);
            _unitMovementComponent.StopMoving();
        }
    }

    public void SetAutoAttack(bool status)
    {
        IsCanAutoAttack = status;
    }

    public void AttackEntity(Entity entity)
    {
        if(entity == null) return;
        
        _targetEntity = entity;
        _targetHealthComponent = entity.GetEntityComponent<HealthComponent>();
        
        if(_targetHealthComponent == null) return;
        
        if(_targetHealthComponent.IsDead) return;
        
        _targetResistanceComponent = _targetEntity.GetEntityComponent<UnitDamageResistanceComponent>();

        _targetMovementComponent = _targetEntity.GetEntityComponent<UnitMovementComponent>();
        
        IsAttacking = true;
    }

    private void LaunchArrow()
    {
        if(_targetEntity == null) return;
        
        var lifetime = CalculateLifetime(_config.ArrowSpeed, transform.position, _targetEntity.transform.position);
        var aimPosition = 
            GetAimPosition(_targetEntity.transform.position,
                _targetMovementComponent.Velocity,
                _config.ArrowSpeed);

        arrowVFXTrigger.Launch(transform.position, aimPosition, _config.ArrowSpeed, lifetime, lifetime);
        ApplyDamageAfterDelay(_targetEntity, lifetime).Forget();
    }

    private async UniTask ApplyDamageAfterDelay(Entity entity, float delay)
    {
        await UniTask.WaitForSeconds(delay, cancellationToken: _cancellationToken);
        
        var damage = CalculateDamageLostOverDistance((entity.transform.position - transform.position).magnitude);

        _targetResistanceComponent.TakeDamage(_thisEntity, _config.DamageType, damage);
    }

    private float CalculateLifetime(float speed, Vector3 startPoint, Vector3 endPoint)
    {
        var distance = (endPoint - startPoint).magnitude;
        return distance / (speed * Time.deltaTime);
    }

    private void OnTargetLost()
    {
        _targetEntity = null;
        _unitAnimationsComponent.SetAttack(false);
        IsAttacking = false;
    }

    private Vector3 GetAimPosition(Vector3 targetPosition, Vector3 targetVelocity, float projectileSpeed)
    {
        Vector3 shooterPos = transform.position;
        Vector3 toTarget = targetPosition - shooterPos;
        float distance = toTarget.magnitude;
        
        Vector3 dir = toTarget.normalized;
        float approachSpeed = Vector3.Dot(targetVelocity, dir);
        float t = distance / Mathf.Max(projectileSpeed - approachSpeed * 0.75f, 0.1f);
        
        for (int i = 0; i < 3; i++)
        {
            Vector3 predictedPos = targetPosition + targetVelocity * t;
            float newDistance = Vector3.Distance(predictedPos, shooterPos);
            t = Mathf.Lerp(t, newDistance / projectileSpeed, 0.8f);
        }
        
        return targetPosition + targetVelocity * t;
    }
    
    private int CalculateDamageLostOverDistance(float distanceToTarget)
    {
        float normalizedDistance = distanceToTarget / _config.AttackRange;
        int damageLost = (int)(_config.Damage * _config.DamageLostDistanceModifierCurve.Evaluate(normalizedDistance));
        
        return _config.Damage - damageLost;
    }

    private void OnEnable()
    {
        if(!_initialized) return;
        
        _unitStateComponent.OnStateChange += OnStateChanged;
        unitAnimationsEventsHandler.OnProjectileLauncherEvent += LaunchArrow;
    }

    private void OnDisable()
    {
        _unitStateComponent.OnStateChange -= OnStateChanged;
        unitAnimationsEventsHandler.OnProjectileLauncherEvent -= LaunchArrow;
    }

    private void OnStateChanged(UnitState state)
    {
        if (state == UnitState.Idle)
        {
            SetAutoAttack(true);
        }
    }
    
    public override void OnExit()
    {
        OnTargetLost();
    }

    private void OnDrawGizmos()
    {
        if (_targetEntity != null)
        {
            Gizmos.DrawWireSphere(_targetEntity.transform.position, 2);
        }
        
    }
}