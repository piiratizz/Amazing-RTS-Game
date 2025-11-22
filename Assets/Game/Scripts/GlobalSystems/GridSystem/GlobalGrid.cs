using System;
using Game.Scripts.GlobalSystems.GridSystem;
using UnityEngine;

public class GlobalGrid : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private int worldWidth;
    [SerializeField] private int worldHeight;
    [SerializeField] private int cellSize;
    
    private HeatMap _heatMap;
    public HeatMap HeatMap => _heatMap;
    
    public void Initialize()
    {
        _heatMap = new HeatMap(worldWidth, worldHeight, cellSize);
    }

    public void RegisterPlayer(int ownerId)
    {
        _heatMap.RegisterPlayer(ownerId);
    }
    
    public Vector3 WorldToCell(Vector3 position) => grid.WorldToCell(position);
    
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        for (int x = 0; x < _heatMap.Width; x++)
        for (int y = 0; y < _heatMap.Height; y++)
        {
            int heat = _heatMap.GetHeat(1, x,y);
            if (heat == 0) continue;

            float intensity = Mathf.Clamp01(heat / 10f);
            Gizmos.color = new Color(intensity, 0, 0, 0.5f);

            Vector3 pos = new Vector3(x * _heatMap.CellSize + _heatMap.CellSize/2, 0, y * _heatMap.CellSize + _heatMap.CellSize/2);
            Gizmos.DrawCube(pos, new Vector3(_heatMap.CellSize, 0.1f, _heatMap.CellSize));
        }
    }
}