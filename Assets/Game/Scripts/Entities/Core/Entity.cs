using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IOwned, ISelectable
{
    [SerializeField] private int ownerId;
    [SerializeField] private GameObject selectionOutlineObject;
    [SerializeField] private List<EntityComponent> unitComponents;
    [SerializeField] private EntityConfig entityConfig;
    
    public IReadOnlyList<EntityComponent> UnitComponents => unitComponents;
    public EntityType EntityType => entityConfig.EntityType;
    public string DisplayName => entityConfig.DisplayName;
    public Sprite Icon => entityConfig.Icon;
    
    public int OwnerId { get => ownerId; set => ownerId = value; }

    public bool IsAvailableToSelect { get; set; } = true;
    
    public virtual void Start()
    {
        foreach (var comp in unitComponents)
        {
            comp.Init(this);
            comp.InitializeFields(entityConfig);
        }
    }

    private void Update()
    {
        foreach (var comp in unitComponents)
        {
            comp.OnUpdate();
        }
    }
    
    public void OnSelect(Player selecter)
    {
        if (selecter.OwnerId == OwnerId)
        {
            selectionOutlineObject.SetActive(true);
        }
    }
    
    public void OnDeselect()
    {
        selectionOutlineObject.SetActive(false);
    }

    public GameObject SelectedObject()
    {
        return gameObject;
    }
    
    public T GetEntityComponent<T>() where T : EntityComponent
    {
        foreach (var comp in unitComponents)
            if (comp is T tComp) return tComp;

        return null;
    }
    
    public T GetComponentByInterface<T>() where T : class
    {
        foreach (var comp in unitComponents)
            if (comp is T tComp)
                return tComp;

        return null;
    }
}