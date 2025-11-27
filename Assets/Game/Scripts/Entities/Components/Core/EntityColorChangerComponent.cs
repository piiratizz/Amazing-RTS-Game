using System.Collections.Generic;
using Game.Scripts.GlobalSystems;
using Game.Scripts.Settings;
using UnityEngine;
using Zenject;

public class EntityColorChangerComponent : EntityComponent
{
    [SerializeField] private List<Renderer> meshRenderersWillBeChanged;
    
    private Material _colorMaterial;
    
    public override void Init(Entity entity)
    {
        if (entity is UnitEntity unitEntity)
        {
            SetUnitMaterial(unitEntity);
        }
        else if (entity is BuildingEntity buildingEntity)
        {
            SetBuildingMaterial(buildingEntity);
        }

        foreach (var meshRenderer in meshRenderersWillBeChanged)
        {
            meshRenderer.material = _colorMaterial;
        }
    }

    private void SetUnitMaterial(UnitEntity entity)
    {
        switch (entity.PlayerColor)
        {
            case PlayerColor.Red:
                _colorMaterial = Resources.Load<Material>("ColorsMaterials/Units/WK_Standard_Units_red");
                break;
            case PlayerColor.Blue:
                _colorMaterial = Resources.Load<Material>("ColorsMaterials/Units/WK_Standard_Units_blue");
                break;
            case PlayerColor.Yellow:
                _colorMaterial = Resources.Load<Material>("ColorsMaterials/Units/WK_Standard_Units_tan");
                break;
            case PlayerColor.Green:
                _colorMaterial = Resources.Load<Material>("ColorsMaterials/Units/WK_Standard_Units_green");
                break;
        }
    }
    
    private void SetBuildingMaterial(BuildingEntity entity)
    {
        switch (entity.PlayerColor)
        {
            case PlayerColor.Red:
                _colorMaterial = Resources.Load<Material>("ColorsMaterials/Buildings/TT_RTS_buildings_red");
                break;
            case PlayerColor.Blue:
                _colorMaterial = Resources.Load<Material>("ColorsMaterials/Buildings/TT_RTS_buildings_blue");
                break;
            case PlayerColor.Yellow:
                _colorMaterial = Resources.Load<Material>("ColorsMaterials/Buildings/TT_RTS_buildings_tan");
                break;
            case PlayerColor.Green:
                _colorMaterial = Resources.Load<Material>("ColorsMaterials/Buildings/TT_RTS_buildings_green");
                break;
        }
    }
}