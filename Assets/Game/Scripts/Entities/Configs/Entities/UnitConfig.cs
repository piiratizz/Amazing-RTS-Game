using UnityEngine;
using NaughtyAttributes; 

[CreateAssetMenu(fileName = "UnitConfig",  menuName = "Configs/UnitConfig")]
public class UnitConfig : EntityConfig
{
    // --- VIEW ---
    [Foldout("View Settings")] 
    public string[] ActiveClothParts;
    [Foldout("View Settings")] 
    public string ClothLayer;
    [Foldout("View Settings")]
    public string RightHandLayer;
    [Foldout("View Settings")]
    public string RightHandWeapon;
    [Foldout("View Settings")]
    public string LeftHandLayer;
    [Foldout("View Settings")]
    public string LeftHandWeapon;
    
    // --- STATS ---
    [Foldout("Stats")]
    public int Damage;
    [Foldout("Stats")]
    public float AttackRange;
    [Foldout("Stats")]
    public float Speed;
    [Foldout("Stats")]
    public int Armor;
    [Foldout("Stats")]
    public DamageType DamageType;
    [Foldout("Stats")]
    public DamageResist[] DamageResists;

    // --- RESOURCES ---
    [Foldout("Resources")]
    public float GatherRatePerSecond;
    [Foldout("Resources")]
    public int LiftingCapacity;
    
    // --- BUILDING ---
    [Foldout("Building")]
    public BuildingConfig[] BuildingsAvailableToBuild;
    [Foldout("Building")]
    public int BuildingRatePerSecond;
    
    // --- PRODUCTION ---
    [Foldout("Production")]
    public float TotalProductionCost;
    
    // --- LONG RANGE ATTACK SETTINGS ---
    [Foldout("Long Range Attack Settings")]
    public AnimationCurve DamageLostDistanceModifierCurve;
    [Foldout("Long Range Attack Settings")]
    public AnimationCurve HeightAdvantageRangeCurve;
    [Foldout("Long Range Attack Settings")]
    public float ArrowSpeed;
    [Foldout("Long Range Attack Settings")]
    public float FireRatePerMinute;
    
    [Foldout("Animation Override")]
    public AnimatorOverrideController AnimationOverrideController;
}