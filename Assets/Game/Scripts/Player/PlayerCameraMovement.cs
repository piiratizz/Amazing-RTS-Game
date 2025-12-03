using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float thicknessPercent;

    private Vector3 _direction;

    public int _minX = 0;
    public int _maxX = 1000;
    
    public int _minZ = 0;
    public int _maxZ = 1000;
    
    private void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        float zoneX = Screen.width * (thicknessPercent / 100f);
        float zoneY = Screen.height * (thicknessPercent / 100f);

        if (mousePos.x >= Screen.width - zoneX)
            _direction.x = 1;
        else if (mousePos.x <= zoneX)
            _direction.x = -1;
        else
            _direction.x = 0;

        if (mousePos.y >= Screen.height - zoneY)
            _direction.z = 1;
        else if (mousePos.y <= zoneY)
            _direction.z = -1;
        else
            _direction.z = 0;

        Vector3 translation = _direction * speed * Time.deltaTime;

        var nextPos = transform.position + translation;

        if (nextPos.x < _minX ||  nextPos.x >= _maxX)
        {
            return;
        }

        if (nextPos.z < _minZ || nextPos.z >= _maxZ)
        {
            return;
        }
        
        transform.Translate(translation, Space.World);
    }

    public void SetMovementLimit()
    {
        
    }
    
    public void MoveToPosition(Vector3 position)
    {
        transform.position = position;
    }
}