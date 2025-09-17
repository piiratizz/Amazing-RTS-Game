using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float thickness;

    private Vector3 _direction;
    
    private void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        
        if (mousePos.x >= Screen.width - thickness)
        {
            _direction.x = 1;
        }
        else if (mousePos.x <= thickness)
        {
            _direction.x = -1;
        }
        else
        {
            _direction.x = 0;
        }
        
        if (mousePos.y <= thickness)
        {
            _direction.z = -1;
        }
        else if (mousePos.y >= Screen.height - thickness)
        {
            _direction.z = 1;
        }
        else
        {
            _direction.z = 0;
        }
        
        transform.Translate(_direction * speed * Time.deltaTime, Space.World);
        
    }
}
