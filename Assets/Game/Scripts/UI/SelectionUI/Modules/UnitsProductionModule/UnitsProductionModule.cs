using System.Collections.Generic;
using Game.Scripts.UI;
using NTC.Pool;
using UnityEngine;

public class UnitsProductionModule : SelectionPanelModule
{
    [SerializeField] private ProducedUnitView producedUnitView;
    [SerializeField] private GameObject background;
    [SerializeField] private Transform unitsPanelsContainer;
    
    private BuildingUnitsProductionComponent _productionComponent;
    private List<ProducedUnitView> _unitsInstances = new List<ProducedUnitView>();
    
    public override void Show(List<Entity> targets)
    {
        _productionComponent = null;
        
        foreach (var target in targets)
        {
            if (target is not BuildingEntity) continue;

            _productionComponent = target.GetEntityComponent<BuildingUnitsProductionComponent>();
            
            if(_productionComponent == null) continue;
            
            background.SetActive(true);
        }

        if (_productionComponent != null)
        {
            ShowUnits();
        }
    }

    private void ShowUnits()
    {
        foreach (var unit in _productionComponent.UnitsAvailableToBuild)
        {
            ProducedUnitView instance = NightPool.Spawn(producedUnitView, unitsPanelsContainer);
            instance.Initialize(
                unit.Unit.Config.Icon,
                unit.Unit.Config.DisplayName,
                unit.Unit.Config.Damage,
                unit.Unit.Config.Armor,
                unit.Unit.Config.Speed,
                unit.Unit.Config.AttackRange
                );
            
            _unitsInstances.Add(instance);
        }
        
    }
    
    public override void Hide()
    {
        foreach (var instance in _unitsInstances)
        {
            NightPool.Despawn(instance);
        }
        _unitsInstances.Clear();
        background.SetActive(false);
    }
}