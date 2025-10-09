using System;
using UnityEngine;

public class ResourceEntity : Entity
{
    private ResourcesConfig _config;
    
    public float SizeX => _config.SizeX;
    public float SizeZ => _config.SizeZ;

    public override void Start()
    {
        base.Start();
        _config = Config as ResourcesConfig;

        if (_config == null)
        {
            throw new NullReferenceException("Config is null");
        }
    }
}