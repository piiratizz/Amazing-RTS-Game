using System;
using System.Collections;
using R3;
using UnityEngine;

public class HealthComponent : EntityComponent, IUpgradeReceiver<UnitStatsModifierUpgrade>
{
    [SerializeField] private bool canCounterAttack;
    
    [SerializeField] private float recentlyDamagedDuration = 0.5f; 
    
    private bool _isRecentlyDamaged;
    private Coroutine _recentDamageCoroutine;

    private ReactiveProperty<int> _health;
    private int _maxHealth;
    private Entity _entity;
    private UnitCommandDispatcher _unitCommandDispatcher;
    private BuildingBuildComponent _buildingBuildComponent;
    
    public bool IsDead { get; private set; }
    public int OwnerId { get; set; }

    public ReadOnlyReactiveProperty<int> CurrentHealth => _health;
    public int MaxHealth => _maxHealth;
    
    public bool IsRecentlyDamaged => _isRecentlyDamaged;

    public Action OnDead;

    public override void Init(Entity entity)
    {
        _entity = entity;
        OwnerId = entity.OwnerId;
        
        _health = new ReactiveProperty<int>(0);
        
        _unitCommandDispatcher = _entity.GetEntityComponent<UnitCommandDispatcher>();
        _buildingBuildComponent = _entity.GetEntityComponent<BuildingBuildComponent>();
    }

    public override void InitializeFields(EntityConfig config)
    {
        if (_buildingBuildComponent != null)
        {
            _health.Value = _buildingBuildComponent.IsBuilded.CurrentValue
                ? config.MaxHealth
                : config.SpawnHealth;
        }
        else
        {
            _health.Value = config.SpawnHealth;
        }
        
        _maxHealth = config.MaxHealth;
    }
    
    public void TakeDamage(Entity sender, int finalDamage)
    {
        if (IsDead) return;

        int newHp = _health.CurrentValue - finalDamage;

        _health.Value = newHp > 0 ? newHp : 0;

        if (newHp <= 0)
        {
            Dead();
            return;
        }

        ActivateRecentlyDamaged();

        if (canCounterAttack && _unitCommandDispatcher != null)
        {
            _unitCommandDispatcher.ExecuteCommand(
                UnitCommandsType.Attack,
                new AttackArgs()
                {
                    Entity = sender,
                    TotalUnits = 1,
                    UnitOffsetIndex = 0
                });
        }
    }
    
    private void ActivateRecentlyDamaged()
    {
        _isRecentlyDamaged = true;

        if (_recentDamageCoroutine != null)
            StopCoroutine(_recentDamageCoroutine);

        _recentDamageCoroutine = StartCoroutine(RecentlyDamagedTimer());
    }

    private IEnumerator RecentlyDamagedTimer()
    {
        yield return new WaitForSeconds(recentlyDamagedDuration);
        _isRecentlyDamaged = false;
        _recentDamageCoroutine = null;
    }

    public void ApplyHealing(int amount)
    {
        if (_health.CurrentValue + amount > _maxHealth)
            return;
        
        _health.Value += amount;
    }
    
    private void Dead()
    {
        IsDead = true;
        _isRecentlyDamaged = false;

        _entity.IsAvailableToSelect = false;
        _entity.OnDeselect();
        _entity.OnEntityDead();

        if (_unitCommandDispatcher != null)
        {
            _unitCommandDispatcher.ExitComponents();
        }
        
        foreach (var component in _entity.EntityComponents)
        {
            component.OnKillComponent();
        }
        
        OnDead?.Invoke();
    }

    public void ReceiveUpgrade(UnitStatsModifierUpgrade upgrade)
    {
        upgrade.Stats.ForEach(s =>
        {
            if (s.StatsType == StatsType.Health)
            {
                var oldMax = _maxHealth;
                _maxHealth += (int)s.Value;

                if (_health.CurrentValue == oldMax)
                {
                    _health.Value = _maxHealth;
                }
            }
        });
    }
}
