using System;
using System.Collections;
using UnityEngine;

public class HealthComponent : EntityComponent
{
    [SerializeField] private bool canCounterAttack;
    
    private int _health;
    private int _maxHealth;
    private Entity _entity;
    private UnitCommandDispatcher _unitCommandDispatcher;
    
    public bool IsDead { get; private set; }
    public int OwnerId { get; set; }

    public int CurrentHealth => _health;
    public int MaxHealth => _maxHealth;

    public Action OnDead;
    public Action<int> OnHealthChanged;

    public override void Init(Entity entity)
    {
        _entity = entity;
        OwnerId = entity.OwnerId;
        _unitCommandDispatcher = _entity.GetEntityComponent<UnitCommandDispatcher>();
    }

    public override void InitializeFields(EntityConfig config)
    {
        _health = config.Health;
        _maxHealth = config.Health;
    }
    
    public void TakeDamage(Entity sender, int finalDamage)
    {
        if(IsDead) return;
        
        if (_health - finalDamage > 0)
        {
            _health -= finalDamage;
            OnHealthChanged?.Invoke(_health);
        }
        else
        {
            _health -= finalDamage;
            OnHealthChanged?.Invoke(_health);
            Dead();
        }

        if (canCounterAttack)
        {
            _unitCommandDispatcher.ExecuteCommand(
                UnitCommandsType.Attack,
                new AttackArgs() { Entity = sender , TotalUnits = 1, UnitOffsetIndex = 0});
        }
    }
    
    private void Dead()
    {
        IsDead = true;
        _entity.IsAvailableToSelect = false;
        _entity.OnDeselect();
        _entity.InvokeSelectionDestroyed();
        
        _unitCommandDispatcher.ExitComponents();

        foreach (var component in _entity.EntityComponents)
        {
            component.OnKillComponent();
        }
        
        OnDead?.Invoke();
    }
}