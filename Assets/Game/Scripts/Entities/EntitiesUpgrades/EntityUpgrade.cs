using UnityEngine;

public abstract class EntityUpgrade : ScriptableObject
{
    public Sprite Icon;
    public string Name;
    public string Description;
    public EntityType EntityType;

    public abstract bool IsCanBeAppliedOnEntity(Entity entity);
}