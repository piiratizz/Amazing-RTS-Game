using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MinimapView : MonoBehaviour
{
    [SerializeField] private RawImage minimapImage;
    [SerializeField] private RawImage gridImage;
    
    [Inject] private Player _player;
    
    private Material _gridMaterial;
    
    private Texture2D _minimapTexture;
    private Color[] _pixels;

    private void Start()
    {
        _gridMaterial = gridImage.material;
    }

    public void UpdateMinimap(
        IReadOnlyCollection<Entity> entitiesToShow,
        int resolution,
        Color backgroundColor,
        Color enemyColor,
        Color friendlyColor,
        int worldSize,
        float gridScale)
    {
        _minimapTexture = new Texture2D(resolution, resolution,  TextureFormat.ARGB32, false);
        _minimapTexture.filterMode = FilterMode.Point;
        minimapImage.texture = _minimapTexture;
        _pixels =  new Color[resolution * resolution];

        for (int i = 0; i < _pixels.Length; i++)
        {
            _pixels[i] = backgroundColor;
        }

        DrawGrid(gridScale, worldSize);
        
        foreach (var entity in entitiesToShow)
        {
            DrawEntity(entity, worldSize,resolution, enemyColor, friendlyColor);
        }
        
        _minimapTexture.SetPixels(_pixels);
        _minimapTexture.Apply(false);
    }
    
    private void DrawGrid(float scale, int worldSize)
    {
        float size = scale * worldSize;
        
        _gridMaterial.SetTextureScale("_BaseMap", new Vector2(size, size));
    }

    
    private void DrawEntity(Entity entity, int worldSize, int resolution, Color enemyColor, Color friendlyColor)
    {
        Vector3 pos = entity.transform.position;

        Color color = entity.OwnerId == _player.OwnerId ? friendlyColor : enemyColor;
        int entitySize = 1;

        if (entity as BuildingEntity != null)
        {
            entitySize = 2;
        }
        else if (entity as ResourceEntity != null)
        {
            color = Color.darkGreen;
        }
        
        float normalizedX = pos.x / worldSize;
        float normalizedZ = pos.z / worldSize;

        int x = Mathf.RoundToInt(normalizedX * (resolution - 1));
        int y = Mathf.RoundToInt(normalizedZ * (resolution - 1));

        if (x < 0 || x >= resolution || y < 0 || y >= resolution)
            return;

        

        int half = entitySize / 2;

        for (int dx = -half; dx <= half; dx++)
        {
            for (int dy = -half; dy <= half; dy++)
            {
                int px = x + dx;
                int py = y + dy;

                if (px >= 0 && px < resolution && py >= 0 && py < resolution)
                    _pixels[py * resolution + px] = color;
            }
        }
    }
}