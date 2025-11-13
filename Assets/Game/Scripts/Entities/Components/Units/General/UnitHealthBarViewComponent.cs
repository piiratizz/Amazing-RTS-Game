using System;
using R3;
using UnityEngine;

public class UnitHealthBarViewComponent : EntityComponent
{
    [SerializeField] private MeshRenderer meshRenderer;
    
    private static readonly int Hp = Shader.PropertyToID("_hp");
    private HealthComponent _healthComponent;
    private MaterialPropertyBlock _materialPropertyBlock;
    
    private bool _initialized;
    
    private CompositeDisposable _subscriptions = new CompositeDisposable();
    
    public override void Init(Entity entity)
    {
        _healthComponent = entity.GetEntityComponent<HealthComponent>();

        if (_healthComponent == null) return;
        
        _materialPropertyBlock = new MaterialPropertyBlock();
        UpdateView(_healthComponent.CurrentHealth.CurrentValue);
        
        _initialized = true;
        
        OnEnable();
    }

    private void OnHealthChanged(int newHealth)
    {
        UpdateView(newHealth);
    }

    private void UpdateView(int newHealth)
    {
        _materialPropertyBlock.SetFloat(Hp, newHealth / 100f);
        meshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    private void OnEnable()
    {
        if (_initialized)
        {
            _healthComponent.CurrentHealth.Subscribe(OnHealthChanged).AddTo(_subscriptions);
        }
    }

    private void OnDisable()
    {
        _subscriptions.Clear();
    }
}