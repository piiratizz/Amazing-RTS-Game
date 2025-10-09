using ComponentsActionTypes;

public class UnitMoveCommand : IEntityCommand<MoveArgs>
{
    private UnitDetectionComponent _detectionComponent;
    private IMoveable _moveable;
    private IAttackable _attackable;
    private Entity _entity;
    
    public CommandPriorityType Priority { get; set; }

    public void Init(Entity entity)
    {
        _entity = entity;
        _moveable = entity.GetComponentByInterface<IMoveable>();
        _detectionComponent = entity.GetEntityComponent<UnitDetectionComponent>();
        _attackable = entity.GetComponentByInterface<IAttackable>();
    }

    public void Execute(MoveArgs args)
    {
        if(_entity.EntityType != EntityType.Unit) return;

        if (_detectionComponent != null)
        {
            _attackable?.SetAutoAttack(_detectionComponent.ClosestEnemy == null);
        }
        
        
        _moveable.MoveTo(args.Position);
    }

    public bool IsComplete()
    {
        return _moveable.IsMoving();
    }
}