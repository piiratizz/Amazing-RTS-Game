using System;
using System.Linq;
using UnityEngine;

public class UnitSkinAttachComponent : EntityComponent
{
    private UnitConfig _config;
    
    public override void InitializeFields(EntityConfig config)
    {
        _config = config as UnitConfig;
        if (_config == null) return;
        
        ApplyConfigToLayer(_config.ClothLayer,
            child => _config.ActiveClothParts.Contains(child.name));
        
        ApplyConfigToLayer(_config.RightHandLayer,
            child => child.name == _config.RightHandWeapon);
        
        ApplyConfigToLayer(_config.LeftHandLayer,
            child => child.name == _config.LeftHandWeapon);
    }
    
    private Transform FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            var result = FindChildRecursive(child, name);
            if (result != null)
                return result;
        }
        return null;
    }

    private void ApplyConfigToLayer(string layerName, Func<Transform, bool> shouldBeActive)
    {
        if (layerName == string.Empty)
        {
            return;
        }
        
        var layer = FindChildRecursive(transform, layerName);
        if (layer == null)
        {
            return;
        }

        foreach (Transform child in layer)
        {
            child.gameObject.SetActive(shouldBeActive(child));
        }
    }
    
}