using System;
using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private new Camera camera;
    [SerializeField] private PlayerSelectionManager playerSelectionManager;
    [SerializeField] private PlayerUnitsController playerUnitController;
    [SerializeField] private PlayerBuildingManager playerBuildingManager;
    [SerializeField] private PlayerModeManager playerModeManager;
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private float doubleClickTime = 0.25f;

    public event Action<RaycastHit> OnDoubleClick;
    public event Action OnSelectionCancelled;
    public event Action<Vector3> OnSelectionStarted;
    public event Action<Vector3>  OnSelectionEnded;
    
    private float _lastClickTime;
    public bool IsDoubleClick { get; private set; }
    
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    
    private void SubscribeModeChangeHandling()
    {
        UnsubscribeAllActions();
        
        playerModeManager.Mode.Where(m => m == PlayerModes.Default)
            .Subscribe(_ => SubscribeDefaultActions())
            .AddTo(_disposables);

        playerModeManager.Mode.Where(m => m == PlayerModes.Building)
            .Subscribe(_ => SubscribeBuildingActions())
            .AddTo(_disposables);
    }

    private void OnEnable()
    {
        SubscribeModeChangeHandling();
    }

    private void OnDisable()
    {
        UnsubscribeAllActions();
        ClearSubscription();
    }

    private void SubscribeDefaultActions()
    {
        UnsubscribeAllActions();

        inputActions.FindAction("Attack").performed += StartSelection;
        inputActions.FindAction("Attack").canceled += EndSelection;
        inputActions.FindAction("RightButton").performed += TriggerPlayerCommandHandling;

        playerSelectionManager.OnSelectionChanged += playerUnitController.SetSelectedUnits;
    }

    private void SubscribeBuildingActions()
    {
        UnsubscribeAllActions();

        inputActions.FindAction("Attack").performed += PlaceBuilding;
        inputActions.FindAction("RightButton").canceled += StopBuilding;
    }

    private void PlaceBuilding(InputAction.CallbackContext context)
    {
        if (IsPointerOverUI(Mouse.current.position.ReadValue()))
        {
            return;
        }
        
        playerBuildingManager.PlaceBuilding().Forget();
    }

    private void StopBuilding(InputAction.CallbackContext context)
    {
        playerBuildingManager.StopBuilding();
    }

    private void UnsubscribeAllActions()
    {
        inputActions.FindAction("Attack").performed -= StartSelection;
        inputActions.FindAction("Attack").canceled -= EndSelection;
        inputActions.FindAction("RightButton").performed -= TriggerPlayerCommandHandling;
        playerSelectionManager.OnSelectionChanged -= playerUnitController.SetSelectedUnits;

        inputActions.FindAction("Attack").performed -= PlaceBuilding;
        inputActions.FindAction("RightButton").canceled -= StopBuilding;
    }

    private void ClearSubscription()
    {
        _disposables?.Clear();
    }
    
    private void StartSelection(InputAction.CallbackContext context)
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            float time = Time.time;

            if (time - _lastClickTime <= doubleClickTime)
            {
                if (RaycastMouse(out RaycastHit hit1))
                {
                    IsDoubleClick = true;
                    OnDoubleClick?.Invoke(hit1);
                }
            }
            else
            {
                IsDoubleClick = false;
            }
            
            _lastClickTime = time;
        }
        
        if (RaycastMouse(out var hit))
        {
            OnSelectionStarted?.Invoke(hit.point);
        }
    }

    private void EndSelection(InputAction.CallbackContext context)
    {
        if (RaycastMouse(out var hit))
        {
            OnSelectionEnded?.Invoke(hit.point);
        }
        else
        {
            OnSelectionCancelled?.Invoke();
        }
    }

    private void TriggerPlayerCommandHandling(InputAction.CallbackContext context)
    {
        if (RaycastMouse(out var hit, false))
            playerUnitController.HandlePlayerCommand(player, hit);
    }

    private bool RaycastMouse(out RaycastHit hit, bool useTerrainLayer = true)
    {
        var mousePosition = Mouse.current.position.ReadValue();

        if (mousePosition.x < 0 ||
            mousePosition.y < 0 ||
            mousePosition.x > Screen.width ||
            mousePosition.y > Screen.height)
        {
            hit = default;
            return false;
        }

        if (IsPointerOverUI(mousePosition))
        {
            hit = default;
            return false;
        }

        if (useTerrainLayer)
        {
            return Physics.Raycast(camera.ScreenPointToRay(mousePosition), out hit, Mathf.Infinity, terrainLayer);
        }
        
        return Physics.Raycast(camera.ScreenPointToRay(mousePosition), out hit, Mathf.Infinity);
    }

    private bool IsPointerOverUI(Vector2 mousePosition)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = mousePosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        return results.Count > 0;
    }
}