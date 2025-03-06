using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    public Light flashlight; // Assign the Spot Light in the Inspector
    public GameObject fishingBackground; // Reference to the UI Canvas to toggle the flashlight

    private Camera mainCamera;
    public float castRadius = 0.5f; // Radius of the capsule cast
    public float castLength = 5f; // Length of the capsule cast
    public float maxDistance = 100f; // Maximum distance of the flashlight cast

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

            // Ensure the flashlight follows the mouse movement
            FollowMouseWithFlashlight();
        }
    }

    private void FollowMouseWithFlashlight()
    {
        // Get the mouse position in screen space
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        // Calculate the direction the flashlight should look at (based on the mouse position)
        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance))
        {
            // Make the flashlight point towards the hit point
            Vector3 directionToLook = hitInfo.point - flashlight.transform.position;
            flashlight.transform.rotation = Quaternion.LookRotation(directionToLook);
        }

        // Perform the capsule cast for collision detection
        PerformCapsuleCast();
    }

    private void PerformCapsuleCast()
    {
        // Calculate capsule cast points (start and end points of the capsule)
        Vector3 startPoint = flashlight.transform.position;
        Vector3 endPoint = flashlight.transform.position + flashlight.transform.forward * castLength;

        // Perform the capsule cast
        if (Physics.CapsuleCast(startPoint, endPoint, castRadius, flashlight.transform.forward, 
            out RaycastHit hitInfo, maxDistance))
        {
            // Optionally: Do something when the capsule cast hits something
            Debug.Log("Flashlight hit: " + hitInfo.collider.gameObject.name);
        }
    }

    // Draw Gizmos to visualize the CapsuleCast in the Scene view
    private void OnDrawGizmosSelected()
    {
        // Set the color for the Gizmo
        Gizmos.color = Color.yellow;

        // Calculate capsule cast points (start and end points of the capsule)
        Vector3 startPoint = flashlight.transform.position;
        Vector3 endPoint = flashlight.transform.position + flashlight.transform.forward * castLength;

        // Draw the capsule to represent the cast
        Gizmos.DrawWireSphere(startPoint, castRadius); // Draw the start of the capsule
        Gizmos.DrawWireSphere(endPoint, castRadius); // Draw the end of the capsule
        Gizmos.DrawLine(startPoint + flashlight.transform.up * castRadius, 
            endPoint + flashlight.transform.up * castRadius); // Draw the top connection line
        Gizmos.DrawLine(startPoint - flashlight.transform.up * castRadius, 
            endPoint - flashlight.transform.up * castRadius); // Draw the bottom connection line
    }
}
