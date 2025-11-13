using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingUpgrades",  menuName = "Configs/BuildingUpgrades")]
public class BuildingUpgragesConfig : ScriptableObject
{
    public BuildingType BuildingType;
    public BuildingUpgradeStageCost[] Stages;
}

[Serializable]
public class BuildingUpgradeStageCost
{
    public ResourceCost[] ResourceCosts;
    public BuildingConfig Stage;
}