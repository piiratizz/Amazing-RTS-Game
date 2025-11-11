using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingUpgrades",  menuName = "Configs/BuildingUpgrades")]
public class BuildingUpgragesConfig : ScriptableObject
{
    public BuildingType BuildingType;
    public UpgradeStageCost[] Stages;
}

[Serializable]
public class UpgradeStageCost
{
    public ResourceCost[] ResourceCosts;
    public BuildingConfig Stage;
}