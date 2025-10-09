using UnityEngine;

public class UnitResourceGatherCommand : IEntityCommand<ResourceGatherArgs>
{
    public CommandPriorityType Priority { get; set; }

    private UnitResourceGatherComponent _resourceGatherComponent;
    
    public void Init(Entity entity)
    {
        _resourceGatherComponent = entity.GetEntityComponent<UnitResourceGatherComponent>();
    }

    public void Execute(ResourceGatherArgs args)
    {
        if (_resourceGatherComponent != null)
        {
            _resourceGatherComponent.SetNewResourceSource(args.Resource);
        }
    }

    public bool IsComplete()
    {
        return true;
    }
}