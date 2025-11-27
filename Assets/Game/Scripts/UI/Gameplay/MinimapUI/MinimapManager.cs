using System;
using System.Collections.Generic;
using Game.Scripts.GlobalSystems;
using Game.Scripts.Settings;
using UnityEngine;
using Zenject;

public class MinimapManager : UIModule
{
    [SerializeField] private MinimapMouseEventsHandler minimapMouseEventsHandler;
    [SerializeField] private MinimapView minimapView;
    [SerializeField] private Color backgroundColor;
    [SerializeField] private Color friendlyColor;
    [SerializeField] private Color enemyColor;
    [SerializeField] private float updateInterval;
    [SerializeField][Range(0,1)] private float gridScale;
    [SerializeField] private int gridResolution;
    [SerializeField] private int mapResolution;
    [SerializeField] private int worldSize;

    [Inject] private Player _player;
    
    private HashSet<Entity> _registeredEntities = new HashSet<Entity>();

    private float _elapsedTime = 0;
    
    private Dictionary<PlayerColor, Color> _colors = new Dictionary<PlayerColor, Color>()
    {
        { PlayerColor.Red, Color.red },
        { PlayerColor.Blue, Color.blue },
        { PlayerColor.Yellow, Color.yellow },
        { PlayerColor.Green, Color.green }
    };
    
    private void Update()
    {
        if (_elapsedTime > updateInterval)
        {
            _elapsedTime = 0;
            minimapView.UpdateMinimap(
                _registeredEntities,
                mapResolution,
                backgroundColor,
                _colors,
                worldSize,
                gridScale);
        }
        _elapsedTime += Time.deltaTime;
    }

    public void RegisterEntity(Entity entity)
    {
        _registeredEntities.Add(entity);
    }

    public void DeleteEntity(Entity entity)
    {
        _registeredEntities.Remove(entity);
    }

    private void OnMapClicked(Vector2 pos)
    {
        float worldX = pos.x * worldSize;
        float worldZ = pos.y * worldSize;

        Vector3 worldPos = new Vector3(worldX, _player.transform.position.y, worldZ);

        _player.SetCameraPosition(worldPos);
    }
    
    private void OnEnable()
    {
        minimapMouseEventsHandler.OnMapClicked += OnMapClicked;
    }
    
    private void OnDisable()
    {
        minimapMouseEventsHandler.OnMapClicked -= OnMapClicked;
    }
}