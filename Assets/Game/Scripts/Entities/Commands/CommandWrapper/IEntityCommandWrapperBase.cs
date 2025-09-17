public interface IEntityCommandWrapperBase
{
    CommandPriorityType Priority { get; set; }
    void Init(Entity entity);
    void Execute();
    bool IsComplete();
}