using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    public Light flashlight; // Assign the Spot Light in the Inspector
    public GameObject fishingBackground; // Reference to the UI Canvas to toggle the flashlight


    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main; // Get the main camera
    }

    private void FixedUpdate()
    {
        // Check if the UI Canvas is active, and disable flashlight accordingly
        if (fishingBackground.gameObject.activeInHierarchy)
        {
            flashlight.enabled = false; // Turn off flashlight when UI is active
        }
        else
        {
            flashlight.enabled = true; // Turn on flashlight when UI is hidden

            // Move flashlight to follow mouse movement instantly
            FollowMouseWithFlashlight();
        }
    }

    private void FollowMouseWithFlashlight()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        float maxDistance = 100f;

        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance))
        {
            // Get the direction to the hit point
            Vector3 directionToLook = hitInfo.point - flashlight.transform.position;

            // Smoothly rotate the flashlight to face the direction (adjust smoothing speed)
            float smoothingSpeed = 10f;
            Quaternion targetRotation = Quaternion.LookRotation(directionToLook);

            flashlight.transform.rotation = Quaternion.Lerp(
                flashlight.transform.rotation,
                targetRotation,
                Time.deltaTime * smoothingSpeed
            );
        }
    }
}
