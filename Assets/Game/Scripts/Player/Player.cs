using System;
using UnityEngine;
using Zenject;

public class Player : MonoBehaviour
{
    [SerializeField] private int ownerId;
    [SerializeField] private PlayerSelectionManager playerSelectionManager;
    [SerializeField] private PlayerUnitsController playerUnitsController;
    [SerializeField] private PlayerCameraMovement playerCameraMovement;
    [SerializeField] private PlayerModeManager playerModeManager;
    
    public PlayerSelectionManager PlayerSelectionManager => playerSelectionManager;
    public PlayerUnitsController PlayerUnitsController => playerUnitsController;
    public int OwnerId => ownerId;
    
    [Inject]
    public void Construct(int ownerId, PlayerModes mode)
    {
        this.ownerId = ownerId;
        playerModeManager.SetMode(mode);
    }

    public void ClampMovementInWorld(int worldSizeX, int worldSizeY)
    {
        
    }
    
    public void SetCameraPosition(Vector3 position)
    {
        playerCameraMovement.MoveToPosition(position);
    }
}