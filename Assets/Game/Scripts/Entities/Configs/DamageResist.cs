using System;
using UnityEngine;

[Serializable]
public class DamageResist
{
    public DamageType DamageType;
    
    [Range(0,1)]
    public float ResistModifier;
}