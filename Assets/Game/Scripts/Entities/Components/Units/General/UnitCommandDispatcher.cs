using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitCommandDispatcher : EntityComponent
{
    private Entity _entity;
    private Dictionary<UnitCommandsType, IEntityCommandWrapperBase> _commands;
    private IEntityCommandWrapperBase _lastCommand;
    
    public override void Init(Entity entity)
    {
        _entity = entity;
        
        _commands = new Dictionary<UnitCommandsType, IEntityCommandWrapperBase>();
        _commands.Add(UnitCommandsType.Move, new EntityCommandWrapper<MoveArgs>(new UnitMoveCommand()));
        _commands.Add(UnitCommandsType.Attack, new EntityCommandWrapper<AttackArgs>(new UnitAttackCommand()));
        _commands.Add(UnitCommandsType.ResourceGather, new EntityCommandWrapper<ResourceGatherArgs>(new UnitResourceGatherCommand()));

        foreach (var command in _commands.Values)
        {
            command.Init(entity);
        }
    }
    
    public void ExecuteCommand<TArgs>(UnitCommandsType commandType, TArgs args, CommandPriorityType priority = CommandPriorityType.Default) where TArgs : struct
    {
        if (_lastCommand != null)
        {
            if (_lastCommand.IsComplete())
            {
                _lastCommand.Priority = CommandPriorityType.Default;
            }
            
            if (priority < _lastCommand?.Priority)
            {
                return;
            }
        }
        
        ExitComponents();
        
        EntityCommandWrapper<TArgs> entityCommand = _commands[commandType] as EntityCommandWrapper<TArgs>;
        entityCommand.Args = args;
        entityCommand.Priority = priority;
        _lastCommand = entityCommand;
        entityCommand.Execute();
    }
    
    public void ExitComponents()
    {
        foreach (var component in _entity.EntityComponents)
        {
            component.OnExit();
        }
    }

}