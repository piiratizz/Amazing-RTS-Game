using System;
using UnityEngine;

public class UnitStateComponent : EntityComponent
{
    public UnitState CurrentState;
    public Action<UnitState> OnStateChange;
    
    private UnitMeleeAttackComponent _attackComponent;
    private UnitMovementComponent _unitMovementComponent;
    private HealthComponent _healthComponent;

    public override void Init(Entity entity)
    {
        _attackComponent = entity.GetEntityComponent<UnitMeleeAttackComponent>();
        _unitMovementComponent = entity.GetEntityComponent<UnitMovementComponent>();
        _healthComponent = entity.GetEntityComponent<HealthComponent>();
    }

    public override void OnUpdate()
    {
        UnitState previousState = CurrentState;
        
        if (_healthComponent.IsDead)
        {
            CurrentState = UnitState.Dead;
        }
        else if (_attackComponent.IsAttacking)
        {
            CurrentState = UnitState.Attack;
        }
        else if (_unitMovementComponent.IsMoving())
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
    Retreat,
    Dead
}