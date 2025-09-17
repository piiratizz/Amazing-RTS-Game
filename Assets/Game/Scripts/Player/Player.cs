using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int ownerId;
    
    public int OwnerId => ownerId;
}