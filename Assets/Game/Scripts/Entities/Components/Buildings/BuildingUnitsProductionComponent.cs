using System;
using System.Collections.Generic;

public class BuildingUnitsProductionComponent : EntityComponent
{
    private BuildingConfig _buildingConfig;

    public IReadOnlyCollection<UnitResourceCost> UnitsAvailableToBuild;
    
    public override void InitializeFields(EntityConfig config)
    {
        _buildingConfig  = config as BuildingConfig;

        if (_buildingConfig == null)
        {
            throw new InvalidCastException("Config must be of type BuildingConfig");
        }

        UnitsAvailableToBuild = _buildingConfig.UnitsCanProduce;
    }
}