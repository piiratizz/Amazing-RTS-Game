using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingConfig",  menuName = "Configs/BuildingConfig")]
public class BuildingConfig : EntityConfig
{
    public float SizeX;
    public float SizeZ;

    [Space]
    [Header("UI Data")] 
    public Sprite Preview;
    public ResourceCost[] ResourceCosts;
}

[Serializable]
public class ResourceCost
{
    public ResourceType Resource;
    public int Amount;
}