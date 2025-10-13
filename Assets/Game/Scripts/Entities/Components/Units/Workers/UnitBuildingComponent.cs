using System;
using System.Collections.Generic;

public class UnitBuildingComponent : EntityComponent
{
    private List<BuildingConfig> _availableBuildings;

    public IReadOnlyList<BuildingConfig> AvailableBuildings => _availableBuildings;
    
    public override void Init(Entity entity)
    {
        _availableBuildings = new List<BuildingConfig>();
    }

    public override void InitializeFields(EntityConfig config)
    {
        UnitConfig unitConfig = config as UnitConfig;

        if (unitConfig == null)
        {
            throw new InvalidCastException("EntityConfig cannot cast to UnitConfig!");
        }
        
        foreach (var building in unitConfig.BuildingsAvailableToBuild)
        {
            _availableBuildings.Add(building);
        }
    }
}