using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class FlashlightController : MonoBehaviour
{
    public Light flashlight; // Assign the Spot Light in the Inspector
    public GameObject fishingBackground; // Reference to the UI Canvas to toggle the flashlight

    private Camera mainCamera;
    public float castRadius = 0.5f; // Radius of the capsule cast
    public float castLength = 5f; // Length of the capsule cast
    public float maxDistance = 100f; // Maximum distance of the flashlight cast


    private bool isShaking = false; // Track if the flashlight is currently shaking
    private bool isFlickering = false; // To track if the flashlight is flickering
    private bool isOnCooldown = false; // Track if the flashlight is on cooldown
    private bool flashlightActive = true; // Tracks if the flashlight is active

    public float shakeDuration = 0.5f; // Duration of the shake effect
    public float shakeMagnitude = 0.1f; // How much the flashlight shakes
    public float flickerDuration = 0.5f; // How long the flashlight flickers when Bunyip is scared
    public float flickerSpeed = 0.05f; // The speed of the flickering effect
    public float cooldownDuration = 3f; // Cooldown time between flickers

    public BunyipControl bunyipController; // Reference to the Bunyip controller script
    public PlayerScript player; // Reference to track game pause state
    AudioSource audioSource;
    [SerializeField] private AudioClip jumpscareStinger;

    private void Awake()
    {
        mainCamera = Camera.main; // Get the main camera
        audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        // Check if game is paused or the fishing background is active
        if (player.gamePaused || fishingBackground.gameObject.activeInHierarchy)
        {
            flashlight.enabled = false;
            flashlightActive = false;
            return; // Skip further updates if paused or in fishing mode
        }

        // Drain battery when flashlight is active

        // Ensure the flashlight follows the mouse movement during any state (flicker, shake, or normal)
        if (flashlightActive && !isFlickering)
        {
            FollowMouseWithFlashlight();
        }

        // Disable the flashlight and capsule cast if the battery runs out
        if (player.batteryPercentage <= 0f)
        {
            flashlight.enabled = false;
            flashlightActive = false;
            Debug.Log("Out of Battery");
        }
        else if (!isShaking && !isFlickering && player.batteryPercentage > 0)
        {
            flashlight.enabled = true;
            flashlightActive = true;

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
            // Make the flashlight point towards the hit point, always update the rotation
            Vector3 directionToLook = hitInfo.point - flashlight.transform.position;
            flashlight.transform.rotation = Quaternion.LookRotation(directionToLook);

            // Perform the capsule cast for collision detection only if flashlight is active
            if (flashlight.enabled)
            {
                PerformCapsuleCheck();
            }
        }
    }

    private void PerformCapsuleCheck()
    {
        // If the flashlight is not active, skip the capsule cast
        if (!flashlight.enabled) return;

        // Calculate capsule cast points (start and end points of the capsule)
        Vector3 startPoint = flashlight.transform.position;
        Vector3 endPoint = flashlight.transform.position + flashlight.transform.forward * castLength;

        // Perform the OverlapCapsule to check for all colliders within the capsule
        Collider[] hitColliders = Physics.OverlapCapsule(startPoint, endPoint, castRadius);

        // Loop through all the colliders hit by the capsule
        foreach (Collider hitCollider in hitColliders)
        {
            // Check if the hit object is the Bunyip and if the flashlight is not on cooldown
            if (hitCollider.gameObject.CompareTag("Bunyip") && !isShaking && !isOnCooldown)
            {
                // Start the shake and flicker effect if the Bunyip is hit
                StartCoroutine(ShakeAndFlickerFlashlight());
                audioSource.PlayOneShot(jumpscareStinger, 1F);
                break;
            }
        }
    }

    // Coroutine to shake and flicker the flashlight when scaring the Bunyip away
    private IEnumerator ShakeAndFlickerFlashlight()
    {
        isShaking = true; // Set the shaking state to true
        Vector3 originalPosition = flashlight.transform.localPosition; // Store the original position
        float elapsedTime = 0f;

        // Perform the shake, but allow rotation to continue
        while (elapsedTime < shakeDuration)
        {
            // Generate a small random offset for the shake (only modify position, not rotation)
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

            // Apply the shake to the flashlight's local position (rotation remains free)
            flashlight.transform.localPosition = new Vector3(originalPosition.x + offsetX,
                originalPosition.y + offsetY, originalPosition.z);

            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            yield return null; // Wait until the next frame
        }

        // Reset the flashlight's position after shaking (rotation stays unaffected)
        flashlight.transform.localPosition = originalPosition;

        // Start the flickering effect
        yield return StartCoroutine(FlickerFlashlight());

        // Trigger the Bunyip to be scared away
        bunyipController.ResetMonsterPosition(); // Call Bunyip reset from BunyipControl script

        isShaking = false; // Reset the shaking state
        StartCoroutine(Cooldown()); // Start the cooldown timer after flicker
    }

    // Coroutine to flicker the flashlight for a short duration
    private IEnumerator FlickerFlashlight()
    {
        isFlickering = true; // Start flickering

        float flickerElapsedTime = 0f;

        while (flickerElapsedTime < flickerDuration)
        {
            // Toggle flashlight on/off rapidly, but keep following the mouse (rotation)
            flashlight.enabled = !flashlight.enabled;

            // Wait for the flicker speed interval
            yield return new WaitForSeconds(flickerSpeed);

            flickerElapsedTime += flickerSpeed;
        }

        // Ensure the flashlight is turned on after flickering ends, only if the battery is not empty
        flashlight.enabled = player.batteryPercentage > 0f;
        isFlickering = false; // Stop flickering
    }

    // Cooldown Coroutine to prevent repeated shaking and flickering
    private IEnumerator Cooldown()
    {
        isOnCooldown = true; // Start cooldown
        yield return new WaitForSeconds(cooldownDuration); // Wait for cooldown duration
        isOnCooldown = false; // Reset cooldown state
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
