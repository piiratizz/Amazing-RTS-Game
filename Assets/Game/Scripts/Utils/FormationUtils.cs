using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Utils
{
    public static class FormationUtils
    {
        private static List<Vector3> _positions = new List<Vector3>();
        
        public static List<Vector3> GetSquareFormationPositions(Vector3 center, List<UnitEntity> unitsInFormation, float formationSpacing)
        {
            _positions.Clear();
            
            int count = unitsInFormation.Count;
            int columns = Mathf.CeilToInt(Mathf.Sqrt(count));
            int rows = Mathf.CeilToInt(count / (float)columns);

            for (int j = 0; j < columns; j++)
            {
                int i = 0;
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < columns; c++)
                    {
                        if (i >= count) return _positions;

                        if (unitsInFormation[i] == null)
                        {
                            i++;
                            continue;
                        }
                
                        if (!unitsInFormation[i].IsAvailableToSelect)
                        {
                            i++;
                            continue;
                        }
                
                        Vector3 offset = new Vector3(
                            (c - columns / 2f) * formationSpacing,
                            0,
                            (r - rows / 2f) * formationSpacing
                        );

                        Vector3 targetPos = center + offset;
                    
                        _positions.Add(targetPos);
                        
                        i++;
                    }
                }
            }

            return _positions;
        }
    }
}