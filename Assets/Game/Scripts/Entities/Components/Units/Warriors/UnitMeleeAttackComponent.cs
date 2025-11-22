using System.Collections;
using ComponentsActionTypes;
using UnityEngine;

public class UnitMeleeAttackComponent : EntityComponent, IAttackable, IUpgradeReceiver<UnitStatsModifierUpgrade>
{
    [SerializeField] private UnitAnimationsEventsHandler unitAnimationsEventsHandler;

    public bool IsCanAutoAttack { get; set; } = true;

    private Entity _entity;
    private UnitMovementComponent _unitMovementComponent;
    private UnitDetectionComponent _unitDetectionComponent;
    private UnitCommandDispatcher _unitCommandDispatcher;
    private UnitStateComponent _unitStateComponent;
    private UnitAnimationComponent _unitAnimationComponent;

    private Entity _target;
    private HealthComponent _targetHealthComponent;
    private UnitDamageResistanceComponent _targetUnitDamageResistanceComponent;

    private float _attackRange = 1f;
    private int _damage = 10;
    private int _bonusDamage = 0;

    public bool IsAttacking { get; set; }

    public int BaseDamage => _damage;

    public int BonusDamage => _bonusDamage;

    private Vector3 _attackPosition;
    private int _unitsInFormation;
    private int _unitOffset;

    private DamageType _damageType;
    private bool _initialized;

    public void SetAutoAttack(bool status)
    {
        IsCanAutoAttack = status;
    }

    public override void Init(Entity entity)
    {
        _entity = entity;
        _unitMovementComponent = entity.GetEntityComponent<UnitMovementComponent>();
        _unitDetectionComponent = entity.GetEntityComponent<UnitDetectionComponent>();
        _unitCommandDispatcher = entity.GetEntityComponent<UnitCommandDispatcher>();
        _unitStateComponent = entity.GetEntityComponent<UnitStateComponent>();
        _unitAnimationComponent = entity.GetEntityComponent<UnitAnimationComponent>();

        _unitStateComponent.OnStateChange += OnStateChanged;
        unitAnimationsEventsHandler.OnHitEvent += PerformDamage;

        _initialized = true;
    }

    public override void InitializeFields(EntityConfig config)
    {
        if (config is UnitConfig unitConfig)
        {
            _attackRange = unitConfig.AttackRange;
            _damage = unitConfig.Damage;
            _damageType = unitConfig.DamageType;
        }
    }

    public override void OnUpdate()
    {
        if (!_initialized || _unitStateComponent.CurrentState == UnitState.Dead)
            return;

        AutoAttackCheck();
        AttackLogicUpdate();
    }

    private void AutoAttackCheck()
    {
        if (!IsCanAutoAttack)
            return;

        var closestEnemy = _unitDetectionComponent?.ClosestEnemy;

        if (closestEnemy == null)
            return;

        if (closestEnemy != _target)
        {
            _unitCommandDispatcher.ExecuteCommand(
                UnitCommandsType.Attack,
                new AttackArgs { Entity = closestEnemy, TotalUnits = 1, UnitOffsetIndex = 0 }
            );
        }
    }

    private void AttackLogicUpdate()
    {
        if (!IsAttacking)
            return;

        if (_target == null || _targetHealthComponent == null || _targetHealthComponent.IsDead)
        {
            OnTargetKilled();
            return;
        }

        Vector3 toTarget = _target.transform.position - transform.position;
        toTarget.y = 0f;

        float radius = 1;

        BuildingEntity buildingEntity = null;
        
        if (_target is BuildingEntity building)
        {
            buildingEntity = building;
            radius = Mathf.Max(buildingEntity.SizeX, buildingEntity.SizeZ);
        }
        
        float angleOffset = 180f + Mathf.Atan2(toTarget.z, toTarget.x) * Mathf.Rad2Deg;
        _attackPosition = GetCirclePosition(
            _target.transform.position,
            _unitsInFormation,
            radius,
            _unitOffset,
            angleOffset
        );

        _unitMovementComponent.MoveTo(_attackPosition);

        if (toTarget.sqrMagnitude > 0f)
        {
            Quaternion rot = Quaternion.LookRotation(toTarget);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                rot,
                360f * Time.deltaTime
            );
        }

        bool inRange = toTarget.sqrMagnitude <= _attackRange * _attackRange;

        
        
        if (buildingEntity != null)
        {
            var biggestSide = Mathf.Max(buildingEntity.SizeX, buildingEntity.SizeZ);
            inRange = toTarget.sqrMagnitude <= biggestSide
                * biggestSide + _attackRange;
        }
        
        _unitAnimationComponent.SetAttack(inRange);
    }

    public void AttackEntity(Entity entity)
    {
        if (entity is UnitEntity unitEntity)
        {
            AttackUnit(unitEntity);
        }
        else if (entity is BuildingEntity buildingEntity)
        {
            AttackBuilding(buildingEntity);
        }
    }

    private void AttackBuilding(BuildingEntity buildingEntity)
    {
        if (buildingEntity == null)
            return;

        if (buildingEntity != _target)
            OnTargetLost();
        
        var health = buildingEntity.GetEntityComponent<HealthComponent>();

        if (health != null && !health.IsDead)
        {
            _target = buildingEntity;
            _targetHealthComponent = health;
            _unitsInFormation = 1;
            _unitOffset = 0;

            IsAttacking = true;
        }
    }

    private void AttackUnit(UnitEntity unitEntity, int unitCircleOffset = 0, int unitsInFormation = 1)
    {
        if (unitEntity == null)
            return;

        if (unitEntity != _target)
            OnTargetLost();

        _targetUnitDamageResistanceComponent = unitEntity.GetEntityComponent<UnitDamageResistanceComponent>();
        var health = unitEntity.GetEntityComponent<HealthComponent>();

        if (health != null && !health.IsDead)
        {
            _target = unitEntity;
            _targetHealthComponent = health;
            _unitsInFormation = unitsInFormation;
            _unitOffset = unitCircleOffset;

            IsAttacking = true;

            unitEntity.AddAttacker();
        }
    }

    private void PerformDamage(AnimationHitArgs hit)
    {
        if (_target == null ||
            _targetHealthComponent == null ||
            _targetHealthComponent.IsDead)
            return;

        if (_targetUnitDamageResistanceComponent != null)
        {
            _targetUnitDamageResistanceComponent.TakeDamage(
                _entity,
                _damageType,
                _damage + _bonusDamage
            );
        }
        else
        {
            _targetHealthComponent.TakeDamage(_entity, _damage + _bonusDamage);
        }
        
        
    }

    private void OnTargetKilled()
    {
        OnExit();
        _unitMovementComponent?.OnExit();
    }

    private void OnTargetLost()
    {
        if (_target is not UnitEntity unitEntity) return;
        
        if (unitEntity.AttackersCount > 0)
        {
            unitEntity.RemoveAttacker();
        }
    }

    public override void OnExit()
    {
        OnTargetLost();

        IsAttacking = false;

        if (_unitAnimationComponent != null)
            _unitAnimationComponent.SetAttack(false);

        _target = null;
        _targetHealthComponent = null;
    }

    private void OnStateChanged(UnitEntity unit, UnitState oldState, UnitState newState)
    {
        if (newState == UnitState.Idle)
            IsCanAutoAttack = true;
    }

    private void OnEnable()
    {
        if (!_initialized) return;

        _unitStateComponent.OnStateChange += OnStateChanged;
        unitAnimationsEventsHandler.OnHitEvent += PerformDamage;
    }

    private void OnDisable()
    {
        if (!_initialized) return;

        _unitStateComponent.OnStateChange -= OnStateChanged;
        unitAnimationsEventsHandler.OnHitEvent -= PerformDamage;
    }

    private Vector3 GetCirclePosition(Vector3 center, int count, float radius, int index, float angleOffset = 0f)
    {
        if (count <= 0) return center;

        float angleStep = 360f / count;
        float angle = angleStep * index + angleOffset;
        float rad = angle * Mathf.Deg2Rad;

        return center + new Vector3(Mathf.Cos(rad) * radius, 0f, Mathf.Sin(rad) * radius);
    }

    public void ReceiveUpgrade(UnitStatsModifierUpgrade upgrade)
    {
        foreach (var s in upgrade.Stats)
        {
            if (s.StatsType == StatsType.Damage)
                _bonusDamage += (int)s.Value;
        }
    }
}