using System;
using UnityEngine;

public class UnitAnimationComponent : EntityComponent
{
    [SerializeField] private Animator animator;
    
    private HealthComponent _healthComponent;
    private static readonly int Dead = Animator.StringToHash("Dead");
    private static readonly int Start = Animator.StringToHash("Start");
    private static readonly int Attacking = Animator.StringToHash("Attacking");
    private bool _initialized;
    
    public override void Init(Entity entity)
    {
        _healthComponent = entity.GetEntityComponent<HealthComponent>();
        _healthComponent.OnDead += OnDead;
        _initialized = true;
    }

    public void SetMove(bool status)
    {
        animator.SetBool(Start, status);
    }
    
    public void SetAttack(bool status)
    {
        animator.SetBool(Attacking, status);
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
        _healthComponent.OnDead -= OnDead;
    }
}