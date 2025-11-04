using System;
using System.Collections.Generic;
using UnityEngine;

public class MinimapManager : MonoBehaviour
{
    [SerializeField] private MinimapView minimapView;
    [SerializeField] private Color backgroundColor;
    [SerializeField] private Color friendlyColor;
    [SerializeField] private Color enemyColor;
    [SerializeField] private float updateInterval;
    [SerializeField] private int resolution;
    [SerializeField] private int worldSize;
    
    private HashSet<Entity> _registeredEntities = new HashSet<Entity>();

    private float _elapsedTime = 0;
    
    private void Update()
    {
        if (_elapsedTime > updateInterval)
        {
            _elapsedTime = 0;
            minimapView.UpdateMinimap(
                _registeredEntities,
                resolution,
                backgroundColor,
                enemyColor,
                friendlyColor,
                worldSize);
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
}