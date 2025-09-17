using UnityEngine;
using System;
using System.Collections.Generic;
using Game.Scripts.UI;
using UnityEngine.InputSystem;

public class PlayerSelectionManager : MonoBehaviour
{
    public event Action<List<ISelectable>> OnSelectionChanged;

    //DEBUG
    [SerializeField] private SelectionPanel selectionPanel;
    //
    
    [SerializeField] private Camera camera;
    [SerializeField] private float boxHeight = 2f;
    [SerializeField] private Color borderColor = new Color(0.3f, 0.6f, 1f);
    [SerializeField] private Color backgroundColor = new Color(0.3f, 0.6f, 1f, 0.2f);
    [SerializeField] private Player player;
    
    private readonly List<ISelectable> _selectedObjects = new();
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
        _bottomRight = worldPoint;
        CreateSelectionBox(_bottomLeft, _bottomRight);
        OnSelectionChanged?.Invoke(_selectedObjects);
        _isSelecting = false;

        //DEBUG
        if(_selectedObjects.Count > 0)
            selectionPanel.Show(_selectedObjects);
        else
            selectionPanel.Hide();
        //
    }

    private void ClearSelection()
    {
        for (var i = 0; i < _selectedObjects.Count; i++)
        {
            if (_selectedObjects[i] is UnityEngine.Object unityObj && unityObj != null)
            {
                _selectedObjects[i].OnDeselect();
            }
        }

        _selectedObjects.Clear();
    }

    private void CreateSelectionBox(Vector3 left, Vector3 right)
    {
        var width = Vector3.Distance(new Vector3(left.x, 0, left.z), new Vector3(right.x, 0, right.z));
        var center = (left + right) / 2f + Vector3.up * (boxHeight / 2f);
        var size = new Vector3(width, boxHeight, Mathf.Abs(left.z - right.z));
        
        foreach (var col in Physics.OverlapBox(center, size / 2f))
        {
            if (col.TryGetComponent(out ISelectable selectable))
            {
                if (selectable.IsAvailableToSelect && selectable.OwnerId == player.OwnerId)
                {
                    _selectedObjects.Add(selectable);
                }
            }
        }
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
}
