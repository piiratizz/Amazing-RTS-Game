using UnityEngine;

public abstract class EntityUpgrade : ScriptableObject
{
    public int Id;
    public Sprite Icon;
    public string Name;
    public string DisplayName;
    public string Description;
    public EntityType EntityType;
    public ResourceCost[] ResourceCost;
    public float ProductionCost;
    public abstract bool IsCanBeAppliedOnEntity(Entity entity);
}