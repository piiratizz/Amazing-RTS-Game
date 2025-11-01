using System;

public class UnitDeadComponent : EntityComponent
{
    private HealthComponent _healthComponent;
    private bool _initialized = false;
    
    public override void Init(Entity entity)
    {
        _healthComponent = entity.GetEntityComponent<HealthComponent>();
        
        _healthComponent.OnDead += OnDead;
        _initialized = true;
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

    private void OnDead()
    {
        Invoke(nameof(DeleteEntity), 3);
    }

    private void DeleteEntity()
    {
        Destroy(gameObject);
    }
}