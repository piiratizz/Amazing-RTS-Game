using System;
using System.Collections.Generic;

public class BuildingEntity : Entity
{
    private BuildingConfig _config;
    
    public float SizeX => _config.SizeX;
    public float SizeZ => _config.SizeZ;
    public BuildingType BuildingType => _config.Type;
    
    public override void Start()
    {
        base.Start();
        _config = Config as BuildingConfig;

        if (_config == null)
        {
            throw new NullReferenceException("Config is null");
        }
    }
}