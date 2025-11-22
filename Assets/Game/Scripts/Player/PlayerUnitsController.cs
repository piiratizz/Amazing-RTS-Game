using System;
using System.Collections.Generic;
using Game.Scripts.Utils;
using UnityEngine;

public class PlayerUnitsController : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float formationSpacing = 1f;
    private List<Entity> _selectedEntities = new();
    private readonly List<UnitEntity> _unitsInFormation = new();
    
    public void SetSelectedUnits(List<Entity> units)
    {
        _selectedEntities = units;
        
        _unitsInFormation.Clear();
        
        foreach (var selectedObject in _selectedEntities)
        {
            if (selectedObject.SelectedObject().TryGetComponent<UnitEntity>(out var unit))
            { 
                if (player.OwnerId == unit.OwnerId)
                {
                    _unitsInFormation.Add(unit);
                }
            }
        }
    }

    public void HandlePlayerCommand(Player sender, RaycastHit hit)
    {
        if (hit.collider.TryGetComponent(out Entity entity))
        {
            switch (entity)
            {
                case UnitEntity unit when entity.OwnerId != player.OwnerId:
                    SendAttackCommand(unit);
                    return;
                case ResourceEntity resource:
                    SendGatherCommand(resource);
                    return;
                case BuildingEntity building:
                    if (entity.OwnerId != player.OwnerId)
                    {
                        SendAttackCommand(building);
                        return;
                    }
                    SendBuildCommand(building);
                    return;
            }
        }

        var positions = FormationUtils.GetSquareFormationPositions(
            hit.point,
            _unitsInFormation,
            formationSpacing);
        
        for (var i = 0; i < _unitsInFormation.Count; i++)
        {
            var unit = _unitsInFormation[i];
            var dispatcher = unit.GetEntityComponent<UnitCommandDispatcher>();

            var args = new MoveArgs() { Position = positions[i] };
            dispatcher?.ExecuteCommand(UnitCommandsType.Move, args);
        }
    }

    public void SendBuildCommand(BuildingEntity building)
    {
        foreach (var unit in _unitsInFormation)
        {
            if(!unit.IsAvailableToSelect || unit == null) 
                continue;
            
            var component = unit.GetEntityComponent<UnitCommandDispatcher>();
            if (component != null)
            {
                component.ExecuteCommand(
                    UnitCommandsType.Build,
                    new BuildArgs() { Building = building }
                );
            }
            
        }
    }

    public void SendGatherCommand(ResourceEntity resource)
    {
        foreach (var unit in _unitsInFormation)
        {
            if(!unit.IsAvailableToSelect || unit == null) 
                continue;
            
            var component = unit.GetEntityComponent<UnitCommandDispatcher>();
            if (component != null)
            {
                component.ExecuteCommand(
                    UnitCommandsType.ResourceGather,
                    new ResourceGatherArgs() { Resource = resource }
                );
            }
        }
    }
    
    private void SendAttackCommand(Entity target)
    {
        for (var i = 0; i < _unitsInFormation.Count; i++)
        {
            if (_unitsInFormation[i] == null)
            {
                continue;
            }
            
            if (!_unitsInFormation[i].IsAvailableToSelect)
            {
                continue;
            }
            
            var dispatcher = _unitsInFormation[i].GetEntityComponent<UnitCommandDispatcher>();
            dispatcher.ExecuteCommand(
                UnitCommandsType.Attack,
                new AttackArgs() {Entity = target, TotalUnits = i, UnitOffsetIndex = _unitsInFormation.Count});
        }
    }
}