public class UnitBuildCommand : IEntityCommand<BuildArgs>
{
    public CommandPriorityType Priority { get; set; }
    
    private Entity _entity;
    
    public void Init(Entity entity)
    {
        _entity = entity;
    }

    public void Execute(BuildArgs args)
    {
        var buildComponent = args.Building.GetEntityComponent<BuildingBuildComponent>();
        
        if(buildComponent == null)
            return;

        if(buildComponent.IsFullHp)
            return;
        
        var unitBuildingComponent = _entity.GetEntityComponent<UnitBuildingComponent>();
        
        if(unitBuildingComponent == null)
            return;
        
        unitBuildingComponent.BuildSelected(args.Building);
    }

    public bool IsComplete()
    {
        return true;
    }
}