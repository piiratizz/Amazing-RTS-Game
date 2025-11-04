using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MinimapView : MonoBehaviour
{
    [SerializeField] private RawImage minimapImage;

    [Inject] private Player _player;
    
    private Texture2D _minimapTexture;
    private Color[] _pixels;
    
    public void UpdateMinimap(
        IReadOnlyCollection<Entity> entitiesToShow,
        int resolution,
        Color backgroundColor,
        Color enemyColor,
        Color friendlyColor,
        int worldSize)
    {
        _minimapTexture = new Texture2D(resolution, resolution,  TextureFormat.ARGB32, false);
        _minimapTexture.filterMode = FilterMode.Point;
        minimapImage.texture = _minimapTexture;
        _pixels =  new Color[resolution * resolution];

        for (int i = 0; i < _pixels.Length; i++)
        {
            _pixels[i] = backgroundColor;
        }

        foreach (var entity in entitiesToShow)
        {
            DrawEntity(entity, worldSize,resolution, enemyColor, friendlyColor);
        }
        
        _minimapTexture.SetPixels(_pixels);
        _minimapTexture.Apply(false);
    }
    
    private void DrawEntity(Entity entity, int worldSize, int resolution, Color enemyColor, Color friendlyColor)
    {
        Vector3 pos = entity.transform.position;
        
        float normalizedX = (pos.x / worldSize) + 0.5f;
        float normalizedZ = (pos.z / worldSize) + 0.5f;

        int x = Mathf.RoundToInt(normalizedX * (resolution - 1));
        int y = Mathf.RoundToInt(normalizedZ * (resolution - 1));
        
        if (x < 0 || x >= resolution || y < 0 || y >= resolution)
            return;
        
        Color color = entity.OwnerId == _player.OwnerId ? friendlyColor : enemyColor;
        _pixels[y * resolution + x] = color;
    }
}