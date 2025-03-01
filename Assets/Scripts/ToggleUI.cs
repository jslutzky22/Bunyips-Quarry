using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleUI : MonoBehaviour
{
    public Canvas uiCanvas; // Assign in Inspector
    private bool isUIActive = true;
    private InputAction toggleAction;

    private void Awake()
    {
        // Get PlayerInput component (must be attached to the GameObject)
        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            toggleAction = playerInput.actions["Turn Around"]; // Reference the action by name
        }
        else
        {
            Debug.LogError("PlayerInput component not found! Make sure it's added to the GameObject.");
        }
    }

    private void OnEnable()
    {
        if (toggleAction != null)
        {
            toggleAction.performed += ToggleCanvas;
            toggleAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (toggleAction != null)
        {
            toggleAction.performed -= ToggleCanvas;
            toggleAction.Disable();
        }
    }

    private void ToggleCanvas(InputAction.CallbackContext context)
    {
        isUIActive = !isUIActive;
        uiCanvas.gameObject.SetActive(isUIActive); // Directly enable/disable the Canvas
    }
}
