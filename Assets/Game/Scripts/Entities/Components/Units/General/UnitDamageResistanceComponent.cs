public class UnitDamageResistanceComponent : EntityComponent
{
    private Entity _entity;
    private HealthComponent _healthComponent;
    private UnitConfig _config;
    
    public override void Init(Entity entity)
    {
        _entity = entity;
        _healthComponent = _entity.GetEntityComponent<HealthComponent>();
    }

    public override void InitializeFields(EntityConfig config)
    {
        _config = config as UnitConfig;
    }

    public void TakeDamage(DamageType damageType, float baseDamage)
    {
        foreach (var resist in _config.DamageResists)
        {
            if (resist.DamageType == damageType)
            {
                int effectiveDamage = (int)(baseDamage * (1f - resist.ResistModifier) * (100f / (100f + _config.Armor)));
                _healthComponent.TakeDamage(_entity, effectiveDamage);
                return;
            }
        }
    }
}