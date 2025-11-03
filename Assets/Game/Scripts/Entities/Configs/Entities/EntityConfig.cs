using UnityEngine;

public class EntityConfig : ScriptableObject
{
    public Sprite Icon;
    public string DisplayName;
    public int MaxHealth;
    public int SpawnHealth;
    public float DetectionRadius;
    public EntityType EntityType;
    public bool Selectable = true;
}