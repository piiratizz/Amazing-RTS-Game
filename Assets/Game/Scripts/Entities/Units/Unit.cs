using System.Collections.Generic;
using UnityEngine;

public class Unit : Entity
{
    public int AttackersCount { get; private set; } = 0;
    
    public void AddAttacker()
    {
        AttackersCount++;
    }

    public void RemoveAttacker()
    {
        AttackersCount--;
    }
}