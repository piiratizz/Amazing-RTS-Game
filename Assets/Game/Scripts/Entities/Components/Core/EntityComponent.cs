using UnityEngine;

public abstract class EntityComponent : MonoBehaviour
{
    public virtual void Init(Entity entity) { }
    
    public virtual void LateInit(Entity entity) { }
    
    public virtual void OnUpdate() { }
    
    public virtual void InitializeFields(EntityConfig config) { }
    public virtual void OnExit() { }
    
    public virtual void OnKillComponent() { }
}