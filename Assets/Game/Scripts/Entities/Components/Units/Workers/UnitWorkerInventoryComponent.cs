using System.Collections.Generic;
using UnityEngine;

public class UnitWorkerInventoryComponent : EntityComponent
{
    [SerializeField] private GameObject pickaxe;
    [SerializeField] private GameObject hammer;

    private Dictionary<WorkerTools, GameObject> _tools;
    
    private GameObject _attachedTool;
    
    public override void Init(Entity entity)
    {
        _tools = new Dictionary<WorkerTools, GameObject>()
        {
            { WorkerTools.Pickaxe , pickaxe},
            { WorkerTools.Hammer , hammer}
        };

        HideAll();
    }

    public void AttachTool(WorkerTools tool)
    {
        _attachedTool?.SetActive(false);
        
        var toolObject = _tools[tool];
        _attachedTool = toolObject;
        toolObject.SetActive(true);
    }

    private void HideAll()
    {
        foreach (var tool in _tools.Values)
        {
            tool.SetActive(false);
        }
    }
}

public enum WorkerTools
{
    Pickaxe,
    Hammer
}