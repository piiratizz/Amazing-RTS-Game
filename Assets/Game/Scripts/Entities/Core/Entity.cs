using System;
using System.Collections.Generic;
using Game.Scripts.GlobalSystems;
using Game.Scripts.Settings;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class Entity : MonoBehaviour, IOwned
{
    [SerializeField] [Tooltip("0 = available for all players")]
    private int ownerId;

    [SerializeField] private GameObject selectionOutlineObject;
    [SerializeField] private List<EntityComponent> entityComponents;
    [SerializeField] private EntityConfig entityConfig;
    [SerializeField] private bool initializeOnStart = true;
    [SerializeField] private float heatMapUpdateInterval = 1f;

    [Inject] private GlobalGrid _globalGrid;
    [Inject] private GameplayHUD _gameplayHUD;
    [Inject] private WorldEntitiesRegistry _entitiesRegistry;

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

    public event Action<Entity> OnEntityDestroyed;

    public int OwnerId
    {
        get => ownerId;
        set => ownerId = value;
    }

    /// <summary>
    /// Change if you need to prohibit selection at runtime
    /// </summary>
    public bool IsAvailableToSelect { get; set; } = true;

    public bool IsDead { get; private set; }

    public PlayerColor PlayerColor {get; private set;}
    
    public virtual void Start()
    {
        if (initializeOnStart)
        {
            Init(ownerId, entityConfig, PlayerColor.Red);
        }
    }

    public void Init(int ownerId, EntityConfig entityConfig, PlayerColor color)
    {
        this.ownerId = ownerId;
        this.entityConfig = entityConfig;

        PlayerColor = color;
        
        foreach (var comp in entityComponents)
        {
            comp.Init(this);
            comp.InitializeFields(entityConfig);
            comp.LateInit(this);
        }

        if (ownerId != 0)
        {
            _globalGrid.HeatMap.GetCell(transform.position, out _lastXHeatMapPosition, out _lastYHeatMapPosition);
            _globalGrid.HeatMap.AddHeat(ownerId, transform.position, entityConfig.HeatWeight);
        }

        _minimapManager = _gameplayHUD.GetModule<MinimapManager>();
        _minimapManager.RegisterEntity(this);
        _entitiesRegistry.Register(this);
    }

    public void UpdateConfig(EntityConfig config)
    {
        _globalGrid.HeatMap.RemoveHeat(ownerId, transform.position, entityConfig.HeatWeight);
        this.entityConfig = config;
        _globalGrid.HeatMap.AddHeat(ownerId, transform.position, entityConfig.HeatWeight);
        
        foreach (var comp in entityComponents)
        {
            comp.InitializeFields(config);
        }
    }

    private float _timer;

    private void Update()
    {
        foreach (var comp in entityComponents)
        {
            comp.OnUpdate();
        }

        if (ownerId == 0) return;

        _timer += Time.deltaTime;

        if (_timer >= heatMapUpdateInterval)
        {
            _timer = 0;
            UpdateHeatMap();
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

    public void OnEntityDead()
    {
        IsDead = true;
        OnEntityDestroyed?.Invoke(this);
        
        if (_minimapManager != null)
        {
            _minimapManager.DeleteEntity(this);
        }

        if (_globalGrid != null && ownerId != 0)
        {
            
            _globalGrid.HeatMap.RemoveHeat(ownerId,
                new Vector3(_lastXHeatMapPosition * _globalGrid.HeatMap.CellSize, 0,
                    _lastYHeatMapPosition * _globalGrid.HeatMap.CellSize),
                entityConfig.HeatWeight);
        }
        
        if (_entitiesRegistry != null)
        {
            _entitiesRegistry.UnRegister(this);
        }
    }

    public GameObject SelectedObject()
    {
        return gameObject;
    }

    public T GetEntityComponent<T>() where T : EntityComponent
    {
        foreach (var comp in entityComponents)
            if (comp is T tComp)
                return tComp;

        return null;
    }

    public T GetFirstComponentByInterface<T>() where T : class
    {
        foreach (var comp in entityComponents)
            if (comp is T tComp)
                return tComp;

        return null;
    }

    public List<T> GetAllComponentsByInterface<T>() where T : class
    {
        var components = new List<T>();

        foreach (var comp in entityComponents)
            if (comp is T tComp)
                components.Add(tComp);

        return components;
    }

    private int _lastXHeatMapPosition, _lastYHeatMapPosition;

    private void UpdateHeatMap()
    {
        _globalGrid.HeatMap.GetCell(transform.position, out int x, out int y);

        if (x != _lastXHeatMapPosition || y != _lastYHeatMapPosition)
        {
            _globalGrid.HeatMap.RemoveHeat(ownerId,
                new Vector3(_lastXHeatMapPosition * _globalGrid.HeatMap.CellSize, 0,
                    _lastYHeatMapPosition * _globalGrid.HeatMap.CellSize),
                entityConfig.HeatWeight);
            _globalGrid.HeatMap.AddHeat(ownerId, transform.position, entityConfig.HeatWeight);

            _lastXHeatMapPosition = x;
            _lastYHeatMapPosition = y;
        }
    }

    private void OnDestroy()
    {
        if (IsDead == false)
        {
            OnEntityDead();
        }
        
        OnEntityDestroyed = null;
    }
}