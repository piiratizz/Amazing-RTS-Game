using ComponentsActionTypes;

public class UnitAttackCommand : IEntityCommand<AttackArgs>
{
    private Entity _entity;

    public CommandPriorityType Priority { get; set; }

    public void Init(Entity entity)
    {
        _entity = entity;
    }

    public void Execute(AttackArgs args)
    {
        if (_entity.EntityType == EntityType.Unit)
        {
            var attackable = _entity.GetComponentByInterface<IAttackable>();
            if (attackable != null)
            {
                attackable.SetAutoAttack(true);
                attackable.AttackEntity(args.Entity);
            }
        }
    }

    public bool IsComplete()
    {
        return true;
    }
}