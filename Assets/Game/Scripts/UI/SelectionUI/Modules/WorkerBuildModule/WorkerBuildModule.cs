using System.Collections.Generic;
using Game.Scripts.UI;
using UnityEngine;

public class WorkerBuildModule : SelectionPanelModule
{
    [SerializeField] private GameObject background;
    
    private UnitBuildingComponent _unitBuildingComponent;
    
    public override void Show(List<Entity> targets)
    {
        _unitBuildingComponent = null;
        
        foreach (var target in targets)
        {
            var component = target.GetEntityComponent<UnitBuildingComponent>();
            if (component != null)
            {
                _unitBuildingComponent = component;
            }
        }

        if (_unitBuildingComponent != null)
        {
            background.SetActive(true);
        }
    }

    public override void Hide()
    {
        background.SetActive(false);
    }
}
