using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingConfig",  menuName = "Configs/BuildingConfig")]
public class BuildingConfig : EntityConfig
{
    public BuildingType Type;
    public float SizeX;
    public float SizeZ;

    [Space]
    [Header("UI Data")] 
    public Sprite Preview;
    public ResourceCost[] BuildResourceCost;

    [Space]
    [Header("Units Production")] 
    public UnitResourceCost[] UnitsCanProduce;
    public float ProductionRatePerSecond;

    [Space] 
    [Header("Build")] 
    public GameObject BuildPreviewPrefab;
}

[Serializable]
public class ResourceCost
{
    public ResourceType Resource;
    public int Amount;
}

[Serializable]
public class UnitResourceCost
{
    public ResourceCost[] ResourceCost;
    public ConfigUnitPrefabLink Unit;
}

[Serializable]
public class ConfigUnitPrefabLink
{
    public UnitConfig Config;
    public UnitEntity UnitPrefab;
}