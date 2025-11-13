using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitEntity : Entity
{
    private UnitConfig _unitConfig;
    
    public int AttackersCount { get; private set; } = 0;

    public int Damage => _unitConfig.Damage;
    public int Armor => _unitConfig.Armor;
    public float Speed => _unitConfig.Speed;
    public float Range => _unitConfig.AttackRange;
    public UnitType UnitType => _unitConfig.UnitType;
    
    public override void Start()
    {
        base.Start();
        _unitConfig = Config as UnitConfig;

        if (_unitConfig == null)
        {
            throw new InvalidCastException("Config must be UnitConfig");
        }
        
    }

    public void AddAttacker()
    {
        AttackersCount++;
    }

    public void RemoveAttacker()
    {
        AttackersCount--;
    }
}