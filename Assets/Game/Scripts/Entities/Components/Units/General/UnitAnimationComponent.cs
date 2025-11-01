using System;
using UnityEngine;

public class UnitAnimationComponent : EntityComponent
{
    [SerializeField] private Animator animator;
    
    private HealthComponent _healthComponent;
    private static readonly int Dead = Animator.StringToHash("Dead");
    private static readonly int Start = Animator.StringToHash("Start");
    private static readonly int Attacking = Animator.StringToHash("Attacking");
    private static readonly int CarryingWood = Animator.StringToHash("CarryingWood");
    private static readonly int CarryingBag = Animator.StringToHash("CarryingBag");
    private bool _initialized;
    
    public override void Init(Entity entity)
    {
        _healthComponent = entity.GetEntityComponent<HealthComponent>();
        _healthComponent.OnDead += OnDead;
        _initialized = true;
    }

    public override void InitializeFields(EntityConfig config)
    {
        var unitConfig = config as UnitConfig;

        if (unitConfig == null) return;
        
        if (unitConfig.AnimationOverrideController != null)
        {
            animator.runtimeAnimatorController = unitConfig.AnimationOverrideController;
        }
    }

    public void SetMove(bool status)
    {
        animator.SetBool(Start, status);
    }
    
    public void SetAttack(bool status)
    {
        animator.SetBool(Attacking, status);
    }
    
    public void SetCarryingWood(bool status)
    {
        animator.SetBool(CarryingWood, status);
    }
    
    public void SetCarryingBag(bool status)
    {
        animator.SetBool(CarryingBag, status);
    }
    
    private void OnDead()
    {
        animator.SetTrigger(Dead);
    }
    
    private void OnEnable()
    {
        if(!_initialized) return;
            
        _healthComponent.OnDead += OnDead;
    }

    private void OnDisable()
    {
        if(!_initialized) return;
        
        _healthComponent.OnDead -= OnDead;
    }
}