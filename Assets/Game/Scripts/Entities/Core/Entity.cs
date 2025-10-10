using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Entity : MonoBehaviour, IOwned
{
    [SerializeField][Tooltip("0 = available for all players")] private int ownerId;
    [SerializeField] private GameObject selectionOutlineObject;
    [SerializeField] private List<EntityComponent> entityComponents;
    [SerializeField] private EntityConfig entityConfig;
    
    protected EntityConfig Config => entityConfig;
    
    public IReadOnlyList<EntityComponent> EntityComponents => entityComponents;
    public EntityType EntityType => entityConfig.EntityType;
    public string DisplayName => entityConfig.DisplayName;
    public Sprite Icon => entityConfig.Icon;

    public Action<Entity> OnEntityDestroyed { get; set; }
    public int OwnerId { get => ownerId; set => ownerId = value; }

    public bool IsAvailableToSelect { get; set; } = true;
    
    public virtual void Start()
    {
        foreach (var comp in entityComponents)
        {
            comp.Init(this);
            comp.InitializeFields(entityConfig);
        }
    }

    private void Update()
    {
        foreach (var comp in entityComponents)
        {
            comp.OnUpdate();
        }
    }
    
    public void OnSelect()
    {
        selectionOutlineObject.SetActive(true);
    }
    
    public void OnDeselect()
    {
        selectionOutlineObject.SetActive(false);
    }

    public void InvokeSelectionDestroyed()
    {
        OnEntityDestroyed?.Invoke(this);
    }

    public GameObject SelectedObject()
    {
        return gameObject;
    }
    
    public T GetEntityComponent<T>() where T : EntityComponent
    {
        foreach (var comp in entityComponents)
            if (comp is T tComp) return tComp;

        return null;
    }
    
    public T GetComponentByInterface<T>() where T : class
    {
        foreach (var comp in entityComponents)
            if (comp is T tComp)
                return tComp;

        return null;
    }

    private void OnDestroy()
    {
        OnEntityDestroyed = null;
    }
}