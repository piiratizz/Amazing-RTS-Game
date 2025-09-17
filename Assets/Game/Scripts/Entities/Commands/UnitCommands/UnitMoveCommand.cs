using ComponentsActionTypes;

public class UnitMoveCommand : IEntityCommand<MoveArgs>
{
    private UnitDetectionComponent _detectionComponent;
    private IMoveable _moveable;
    private IAttackable _attackable;

    public CommandPriorityType Priority { get; set; }

    public void Init(Entity entity)
    {
        _moveable = entity.GetComponentByInterface<IMoveable>();
        _detectionComponent = entity.GetEntityComponent<UnitDetectionComponent>();
        _attackable = entity.GetComponentByInterface<IAttackable>();
    }

    public void Execute(MoveArgs args)
    {
        if (_detectionComponent.ClosestEnemy != null)
        {
            _attackable.SetAutoAttack(false);
        }
        else
        {
            _attackable.SetAutoAttack(true);
        }
        
        _moveable.MoveTo(args.Position);
    }

    public bool IsComplete()
    {
        return _moveable.IsMoving();
    }
}