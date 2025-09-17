public class EntityCommandWrapper<TArgs> : IEntityCommandWrapperBase where TArgs : struct
{
    private readonly IEntityCommand<TArgs> _command;
    public TArgs Args { get; set; }

    public CommandPriorityType Priority { get => _command.Priority; set  => _command.Priority = value; }
    
    public EntityCommandWrapper(IEntityCommand<TArgs> command)
    {
        _command = command;
    }
    
    public void Init(Entity entity)
    {
        _command.Init(entity);
    }
    
    public void Execute()
    {
        _command.Execute(Args);
    }

    public bool IsComplete()
    {
        return _command.IsComplete();
    }
}