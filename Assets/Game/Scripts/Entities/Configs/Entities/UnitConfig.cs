using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitConfig",  menuName = "Configs/UnitConfig")]
public class UnitConfig : EntityConfig
{
    [Header("View")]
    public string[] ActiveClothParts;
    public string ClothLayer;
    public string RightHandLayer;
    public string RightHandWeapon;
    public string LeftHandLayer;
    public string LeftHandWeapon;
    
    [Header("Stats")]
    public int Damage;
    public float AttackRange;
    public float Speed;
    public int Armor;
    public DamageType DamageType;
    public DamageResist[] DamageResists;

    [Header("Resources")]
    public float GatherRatePerSecond;
    public int LiftingCapacity;
    
    [Header("Building")]
    public BuildingConfig[] BuildingsAvailableToBuild;
    
    [Header("Production")]
    public float TotalProductionCost;
}