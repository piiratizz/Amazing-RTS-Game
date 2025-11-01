using UnityEngine;

public class UnitFactory
{
    private int _ownerId;
    
    public UnitFactory(int ownerId)
    {
        _ownerId = ownerId;
    }
    
    public UnitEntity CreateUnit(ConfigUnitPrefabLink unit, Vector3 position)
    {
        var instance = Object.Instantiate(unit.UnitPrefab, position, Quaternion.identity);
        instance.Init(_ownerId, unit.Config);
        return instance;
    }
}