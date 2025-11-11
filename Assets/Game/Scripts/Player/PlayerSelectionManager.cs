using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerSelectionManager : MonoBehaviour
{
    public event Action<List<Entity>> OnSelectionChanged;
    
    [SerializeField] private new Camera camera;
    [SerializeField] private float boxHeight = 2f;
    [SerializeField] private Color borderColor = new Color(0.3f, 0.6f, 1f);
    [SerializeField] private Color backgroundColor = new Color(0.3f, 0.6f, 1f, 0.2f);
    [SerializeField] private Player player;
    
    public bool IsSelecting => _isSelecting;
    
    private readonly List<Entity> _selectedEntities = new();
    private Vector3 _bottomLeft;
    private Vector3 _bottomRight;
    private Vector2 _startMousePos;
    private Vector2 _currentMousePos;
    private bool _isSelecting;
    
    private void Update()
    {
        if (_isSelecting)
            _currentMousePos = Mouse.current.position.ReadValue();
    }

    public void StartSelection(Vector3 worldPoint)
    {
        ClearSelection();
        _startMousePos = Mouse.current.position.ReadValue();
        _bottomLeft = worldPoint;
        _isSelecting = true;
    }
    
    public void EndSelection(Vector3 worldPoint)
    {
        if(_isSelecting == false) return;
        
        _bottomRight = worldPoint;
        CreateSelectionBox(_bottomLeft, _bottomRight);
        
        OnSelectionChanged?.Invoke(_selectedEntities);
        foreach (var selectable in _selectedEntities)
        {
            selectable.OnEntityDestroyed += SelectionDestroyed;
        }
        _isSelecting = false;
    }

    private void SelectionDestroyed(Entity entity)
    {
        _selectedEntities.Remove(entity);
        OnSelectionChanged?.Invoke(_selectedEntities);
    }

    public void RemoveAllFromSelection(Predicate<Entity> predicate)
    {
        for (var i = 0; i < _selectedEntities.Count; i++)
        {
            if (predicate(_selectedEntities[i]))
            {
                _selectedEntities[i].OnDeselect();
                _selectedEntities.RemoveAt(i);
                i = -1;
            }
        }
        
        OnSelectionChanged?.Invoke(_selectedEntities);
    }
    
    public void ClearSelection()
    {
        for (var i = 0; i < _selectedEntities.Count; i++)
        {
            if (_selectedEntities[i] is UnityEngine.Object unityObj && unityObj != null)
            {
                _selectedEntities[i].OnEntityDestroyed -= SelectionDestroyed;
                _selectedEntities[i].OnDeselect();
            }
        }

        _selectedEntities.Clear();
        OnSelectionChanged?.Invoke(_selectedEntities);
        _isSelecting = false;
    }

    private Vector3 _center;
    private Vector3 _halfExtents;
    
    public void CreateSelectionBox(Vector3 left, Vector3 right)
    {
        float width = Mathf.Abs(left.x - right.x);
        float depth = Mathf.Abs(left.z - right.z);

        Vector3 center = (left + right) * 0.5f + Vector3.up * (boxHeight * 0.5f);
        Vector3 size   = new Vector3(width, boxHeight, depth);

        _center = center;
        _halfExtents = size * 0.5f;

        bool unitWasSelected = false;
        
        Collider[] cols = Physics.OverlapBox(center, _halfExtents, Quaternion.identity);

        foreach (var col in cols)
        {
            if (!col.TryGetComponent(out Entity entity)) continue;
            if (!entity.Selectable) continue;
            if (!entity.IsAvailableToSelect) continue;
            if (entity.OwnerId != player.OwnerId && entity.OwnerId != 0) continue;

            if (entity is UnitEntity && !unitWasSelected)
            {
                _selectedEntities.RemoveAll(e => e is not UnitEntity);
                unitWasSelected = true;
            }

            if (unitWasSelected && entity is not UnitEntity) continue;

            _selectedEntities.Add(entity);
        }

        foreach (var e in _selectedEntities)
            e.OnSelect();
    }

    
    private void OnGUI()
    {
        if (_isSelecting)
        {
            var rect = GetScreenRect(_startMousePos, _currentMousePos);
            
            GUI.color = backgroundColor;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            
            GUI.color = borderColor;
            DrawScreenRectBorder(rect, 2);
        }
    }

    private static Rect GetScreenRect(Vector2 screenPosition1, Vector2 screenPosition2)
    {
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;

        var topLeft = Vector2.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector2.Max(screenPosition1, screenPosition2);

        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    private static void DrawScreenRectBorder(Rect rect, float thickness)
    {
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, thickness), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, thickness, rect.height), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), Texture2D.whiteTexture);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_center, _halfExtents);
    }
}
