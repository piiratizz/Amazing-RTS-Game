using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.GlobalSystems.GridSystem
{
    public class HeatMap
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public float CellSize { get; private set; }
        
        private readonly Dictionary<int, int[,]> _heatPerPlayer = new();
    
        public HeatMap(int width, int height, float cellSize)
        {
            Width = width;
            Height = height;
            CellSize = cellSize;
        }
        
        public void RegisterPlayer(int ownerId)
        {
            if (!_heatPerPlayer.ContainsKey(ownerId))
            {
                _heatPerPlayer[ownerId] = new int[Width, Height];
            }
        }

        public void UnregisterPlayer(int ownerId)
        {
            _heatPerPlayer.Remove(ownerId);
        }
        
        public void AddHeat(int ownerId, Vector3 pos, int heat)
        {
            GetCell(pos, out var x, out var y);
            _heatPerPlayer[ownerId][x, y] += heat;
        }

        public void RemoveHeat(int ownerId, Vector3 pos, int heat)
        {
            GetCell(pos, out var x, out var z);
            _heatPerPlayer[ownerId][x, z] -= heat;
            if (_heatPerPlayer[ownerId][x, z] < 0)
                _heatPerPlayer[ownerId][x, z] = 0;
        }

        public int GetHeat(int ownerId, int x, int z)
        {
            return _heatPerPlayer[ownerId][x, z];
        }

        public void GetCell(Vector3 worldPos, out int x, out int z)
        {
            x = Mathf.Clamp(Mathf.FloorToInt(worldPos.x / CellSize), 0, Width - 1);
            z = Mathf.Clamp(Mathf.FloorToInt(worldPos.z / CellSize), 0, Height - 1);
        }
        
        public int GetEnemyHeat(int selfOwnerId, int x, int z)
        {
            int heat = 0;
            foreach (var kv in _heatPerPlayer)
            {
                if (kv.Key == selfOwnerId)
                    continue;

                heat += kv.Value[x, z];
            }

            return heat;
        }
        
        public bool IsInside(int x, int z)
        {
            return x >= 0 && z >= 0 && x < Width && z < Height;
        }
        
        public Vector3 FindClosestEnemyCellWorld(
            Vector3 fromWorldPos,
            int selfOwnerId,
            float searchRadiusWorld,
            float heatWeight = 2f,
            float distanceWeight = 0.1f,
            float closeRangeBonus = 25f,
            float closeRangeMeters = 10f)
        {
            GetCell(fromWorldPos, out int startX, out int startZ);

            int r = Mathf.CeilToInt(searchRadiusWorld / CellSize);

            float bestScore = float.MinValue;
            int bestX = -1;
            int bestZ = -1;
            
            float closeRangeSqr = closeRangeMeters * closeRangeMeters / (CellSize * CellSize);

            for (int x = startX - r; x <= startX + r; x++)
            {
                for (int z = startZ - r; z <= startZ + r; z++)
                {
                    if (!IsInside(x, z))
                        continue;

                    int enemyHeat = GetEnemyHeat(selfOwnerId, x, z);
                    if (enemyHeat <= 0)
                        continue;
                    
                    int dx = x - startX;
                    int dz = z - startZ;
                    float distSqr = dx * dx + dz * dz;
                    
                    float score = enemyHeat * heatWeight - distSqr * distanceWeight;
                    
                    if (distSqr < closeRangeSqr)
                    {
                        score += closeRangeBonus;
                    }
                    
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestX = x;
                        bestZ = z;
                    }
                }
            }

            if (bestX == -1)
                return Vector3.negativeInfinity;

            return CellToWorld(bestX, bestZ);
        }
        
        public Vector3 CellToWorld(int cellX, int cellZ)
        {
            float half = CellSize * 0.5f;

            return new Vector3(
                cellX * CellSize + half,
                0,
                cellZ * CellSize + half
            );
        }
    }
}