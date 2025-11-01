using System;
using UnityEngine;

public class UnitHealthBarViewComponent : EntityComponent
{
    [SerializeField] private MeshRenderer meshRenderer;
    
    private static readonly int Hp = Shader.PropertyToID("_hp");
    private HealthComponent _healthComponent;
    private MaterialPropertyBlock _materialPropertyBlock;
    
    private bool _initialized;
    
    public override void Init(Entity entity)
    {
        _healthComponent = entity.GetEntityComponent<HealthComponent>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        UpdateView(_healthComponent.CurrentHealth);
        _healthComponent.OnHealthChanged += OnHealthChanged;
        _initialized = true;
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
        if(!_initialized) return;
        
        _healthComponent.OnHealthChanged += OnHealthChanged;
    }
    
    private void OnDisable()
    {
        if(!_initialized) return;
        
        _healthComponent.OnHealthChanged -= OnHealthChanged;
    }
}