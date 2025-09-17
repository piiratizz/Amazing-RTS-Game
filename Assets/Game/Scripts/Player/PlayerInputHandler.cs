using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private Camera camera;
    [SerializeField] private PlayerSelectionManager playerSelectionManager;
    [SerializeField] private PlayerUnitsController playerUnitController;

    private void OnEnable()
    {
        inputActions.FindAction("Attack").performed += StartSelection;
        inputActions.FindAction("Attack").canceled += EndSelection;
        inputActions.FindAction("RightButton").performed += OnRightClick;

        playerSelectionManager.OnSelectionChanged += playerUnitController.SetSelectedUnits;
    }

    private void OnDisable()
    {
        inputActions.FindAction("Attack").performed -= StartSelection;
        inputActions.FindAction("Attack").canceled -= EndSelection;
        inputActions.FindAction("RightButton").performed -= OnRightClick;

        playerSelectionManager.OnSelectionChanged -= playerUnitController.SetSelectedUnits;
    }

    private void StartSelection(InputAction.CallbackContext context)
    {
        if (RaycastMouse(out var hit))
            playerSelectionManager.StartSelection(hit.point);
    }

    private void EndSelection(InputAction.CallbackContext context)
    {
        if (RaycastMouse(out var hit))
            playerSelectionManager.EndSelection(hit.point);
    }

    private void OnRightClick(InputAction.CallbackContext context)
    {
        if (RaycastMouse(out var hit))
            playerUnitController.HandlePlayerCommand(player, hit);
    }

    private bool RaycastMouse(out RaycastHit hit) =>
        Physics.Raycast(camera.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit);
}