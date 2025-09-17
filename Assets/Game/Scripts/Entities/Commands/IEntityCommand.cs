public interface IEntityCommand<TArgs>
{
    CommandPriorityType Priority { get; set; }
    void Init(Entity entity);
    void Execute(TArgs args);
    bool IsComplete();
}