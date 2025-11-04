using UnityEngine;

public class GameplayHUD : MonoBehaviour
{
    [SerializeField] private MinimapManager minimapManager;
    
    public MinimapManager MinimapManager => minimapManager;
}