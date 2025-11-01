using System;
using UnityEngine;
using Zenject;

public class Player : MonoBehaviour
{
    [SerializeField] private int ownerId;
    [SerializeField] private PlayerSelectionManager _playerSelectionManager;
    [SerializeField] private PlayerUnitsController _playerUnitsController;
    
    private PlayerModes _playerMode;
    
    public PlayerSelectionManager PlayerSelectionManager => _playerSelectionManager;
    public PlayerUnitsController PlayerUnitsController => _playerUnitsController;
    public int OwnerId => ownerId;
    public PlayerModes Mode => _playerMode;
    
    [Inject]
    public void Construct(int ownerId, PlayerModes mode)
    {
        this.ownerId = ownerId;
        _playerMode = mode;
    }

    public void SetMode(PlayerModes mode)
    {
        _playerMode = mode;
    }
}