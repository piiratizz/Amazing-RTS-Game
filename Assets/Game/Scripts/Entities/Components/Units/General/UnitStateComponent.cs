using System;
using ComponentsActionTypes;
using UnityEngine;

public class UnitStateComponent : EntityComponent
{
    public UnitState CurrentState;
    /// <summary>
    /// Entity, Change from state, To State
    /// </summary>
    public Action<UnitEntity, UnitState, UnitState> OnStateChange;
    
    private UnitEntity _entity;
    
    private IAttackable _attackComponent;
    private UnitMovementComponent _unitMovementComponent;
    private UnitBuildingComponent _unitBuildingComponent;
    private UnitResourceGatherComponent _unitGatherComponent;
    private HealthComponent _healthComponent;
    private bool _initialized;

    public override void Init(Entity entity)
    {
        _entity = entity as UnitEntity;
        _attackComponent = entity.GetFirstComponentByInterface<IAttackable>();
        _unitMovementComponent = entity.GetEntityComponent<UnitMovementComponent>();
        _unitBuildingComponent = entity.GetEntityComponent<UnitBuildingComponent>();
        _unitGatherComponent = entity.GetEntityComponent<UnitResourceGatherComponent>();
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
        else if (_unitGatherComponent != null && _unitGatherComponent.IsGathering.Item1)
        {
            switch (_unitGatherComponent.IsGathering.Item2)
            {
                case ResourceType.Food:
                    CurrentState = UnitState.GatheringFood;
                    break;
                case ResourceType.Wood:
                    CurrentState = UnitState.GatheringWood;
                    break;
                case ResourceType.Gold:
                    CurrentState = UnitState.GatheringGold;
                    break;
            }
            
        }
        else if (_unitBuildingComponent != null && _unitBuildingComponent.IsBuilding)
        {
            CurrentState = UnitState.Building;
        }
        else
        {
            CurrentState = UnitState.Idle;
        }

        if (CurrentState == previousState) return;
        OnStateChange?.Invoke(_entity, previousState, CurrentState);
    }
}

public enum UnitState
{
    Idle,
    Attack,
    Move,
    GatheringFood,
    GatheringWood,
    GatheringGold,
    Building,
    Dead
}