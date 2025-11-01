using System.Collections.Generic;
using UnityEngine;

public class UnitVisualResourceHoldingComponent : EntityComponent
{
    [SerializeField] private GameObject bagObject;
    [SerializeField] private GameObject woodObject;
    
    private UnitAnimationComponent _unitAnimationComponent;
    
    public override void Init(Entity entity)
    {
        _unitAnimationComponent = entity.GetEntityComponent<UnitAnimationComponent>();
    }

    public void ShowResource(ResourceType resourceType)
    {
        Clear();

        switch (resourceType)
        {
            case ResourceType.Food:
            case ResourceType.Gold:
                bagObject.SetActive(true);
                _unitAnimationComponent.SetCarryingBag(true);
                break;
            case ResourceType.Wood:
                woodObject.SetActive(true);
                _unitAnimationComponent.SetCarryingWood(true);
                break;
        }
    }

    public override void OnExit()
    {
        Clear();
    }

    public void Clear()
    {
        bagObject.SetActive(false);
        woodObject.SetActive(false);
        _unitAnimationComponent.SetCarryingWood(false);
        _unitAnimationComponent.SetCarryingBag(false);
    }
}