using System;
using ComponentsActionTypes;
using UnityEngine;

public class UnitStateComponent : EntityComponent
{
    public UnitState CurrentState;
    public Action<UnitState> OnStateChange;
    
    private IAttackable _attackComponent;
    private UnitMovementComponent _unitMovementComponent;
    private HealthComponent _healthComponent;
    private bool _initialized;

    public override void Init(Entity entity)
    {
        _attackComponent = entity.GetComponentByInterface<IAttackable>();
        _unitMovementComponent = entity.GetEntityComponent<UnitMovementComponent>();
        _healthComponent = entity.GetEntityComponent<HealthComponent>();

        _initialized = true;
    }

    public override void OnUpdate()
    {
        if(!_initialized) return;
        
        UnitState previousState = CurrentState;
        
        
        if (_healthComponent != null && _healthComponent.IsDead)
        {
            CurrentState = UnitState.Dead;
        }
        else if(_attackComponent != null && _attackComponent.IsAttacking)
        {
            CurrentState = UnitState.Attack;
        }
        else if (_attackComponent != null && _unitMovementComponent.IsMoving())
        {
            CurrentState = UnitState.Move;
        }
        else
        {
            CurrentState = UnitState.Idle;
        }

        if (CurrentState == previousState) return;
        OnStateChange?.Invoke(CurrentState);
    }
}

public enum UnitState
{
    Idle,
    Attack,
    Move,
    Dead
}