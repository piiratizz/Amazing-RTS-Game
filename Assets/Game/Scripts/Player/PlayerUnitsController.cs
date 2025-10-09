using System;
using System.Collections.Generic;
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
            if (entity is UnitEntity unit)
            {
                if (entity.OwnerId != player.OwnerId)
                {
                    SendAttackCommand(entity);
                    return;
                }
            }
            else
            {
                if (entity is ResourceEntity resource)
                {
                    SendGatherCommand(resource);
                    return;
                }
            }

        }
        
        MoveSelectedUnitsInSquareFormation(hit.point);
    }
    
    private void MoveSelectedUnitsInSquareFormation(Vector3 center)
    {
        int count = _unitsInFormation.Count;
        int columns = Mathf.CeilToInt(Mathf.Sqrt(count));
        int rows = Mathf.CeilToInt(count / (float)columns);

        int i = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (i >= count) return;

                if (_unitsInFormation[i] == null)
                {
                    i++;
                    continue;
                }
                
                if (!_unitsInFormation[i].IsAvailableToSelect)
                {
                    i++;
                    continue;
                }
                
                Vector3 offset = new Vector3(
                    (c - columns / 2f) * formationSpacing,
                    0,
                    (r - rows / 2f) * formationSpacing
                );

                Vector3 targetPos = center + offset;
                
                var dispatcher = _unitsInFormation[i].GetEntityComponent<UnitCommandDispatcher>();
                
                var args = new MoveArgs() { Position = targetPos };
                dispatcher?.ExecuteCommand(UnitCommandsType.Move, args);
                
                i++;
            }
        }
    }

    private void SendGatherCommand(Entity resource)
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