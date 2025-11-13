using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatsModifierUpgrade",  menuName = "Upgrades/StatsModifierUpgrade")]
public class UnitStatsModifierUpgrade : EntityUpgrade
{
    public List<UnitType> UnitsWillUpgrade;
    public List<StatsTypeModifierLink> Stats;
    
    
    public override bool IsCanBeAppliedOnEntity(Entity entity)
    {
        var unit = entity as UnitEntity;
        
        if (unit == null)
        {
            return false;
        }
        
        return UnitsWillUpgrade.Contains(unit.UnitType);
    }
}

