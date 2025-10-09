using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int ownerId;
    [SerializeField] private PlayerSelectionManager _playerSelectionManager;
    [SerializeField] private PlayerUnitsController _playerUnitsController;
    
    public PlayerSelectionManager PlayerSelectionManager => _playerSelectionManager;
    public PlayerUnitsController PlayerUnitsController => _playerUnitsController;
    public int OwnerId => ownerId;
}