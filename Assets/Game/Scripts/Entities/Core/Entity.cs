using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class Entity : MonoBehaviour, IOwned
{
    [SerializeField][Tooltip("0 = available for all players")] private int ownerId;
    [SerializeField] private GameObject selectionOutlineObject;
    [SerializeField] private List<EntityComponent> entityComponents;
    [SerializeField] private EntityConfig entityConfig;
    [SerializeField] private bool initializeOnStart = true;
    
    [Inject] private GameplayHUD _gameplayHUD;
    private MinimapManager _minimapManager;
    
    protected EntityConfig Config => entityConfig;
    
    public IReadOnlyList<EntityComponent> EntityComponents => entityComponents;
    public EntityType EntityType => entityConfig.EntityType;
    public string DisplayName => entityConfig.DisplayName;
    public Sprite Icon => entityConfig.Icon;
    /// <summary>
    /// Changes in the config if you want to prohibit the selection from the very beginning
    /// </summary>
    public bool Selectable => entityConfig.Selectable;
    
    public Action<Entity> OnEntityDestroyed { get; set; }
    public int OwnerId { get => ownerId; set => ownerId = value; }

    /// <summary>
    /// Change if you need to prohibit selection at runtime
    /// </summary>
    public bool IsAvailableToSelect { get; set; } = true;
    
    public virtual void Start()
    {
        if (initializeOnStart)
        {
            Init(ownerId, entityConfig);
        }
    }

    public void Init(int ownerId, EntityConfig entityConfig)
    {
        this.ownerId = ownerId;
        this.entityConfig = entityConfig;
        
        foreach (var comp in entityComponents)
        {
            comp.Init(this);
            comp.InitializeFields(entityConfig);
        }
        
        _minimapManager = _gameplayHUD.GetModule<MinimapManager>();
        _minimapManager.RegisterEntity(this);
    }

    public void UpdateConfig(EntityConfig config)
    {
        this.entityConfig = config;
        
        foreach (var comp in entityComponents)
        {
            comp.InitializeFields(config);
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
        _minimapManager.DeleteEntity(this);
    }
}