using System.Collections;
using System.Collections.Generic;
using TMPro; // For handling TextMeshPro UI
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BunyipControl : MonoBehaviour
{
    public GameObject[] monsterPositions; // Array of positions/models the monster will move through
    public int currentMonsterIndex = 0; // Tracks the monster's current position

    // Set min and max time for movement intervals
    public float minTimeBetweenMoves = 3f; // Minimum time between moves
    public float maxTimeBetweenMoves = 8f; // Maximum time between moves
    private float timeBetweenMoves; // Random time between moves

    private float moveTimer = 0f; // Tracks time since last move

    public GameObject fishingBackground; // Reference to the UI Canvas to track if the monster should move
    public Light flashlight; // Reference to the player's flashlight
    public PlayerScript player; // Reference to PlayerScript to access the player's fish count and check for death

    // Reference to the TextMeshPro UI for showing the fish-eaten message
    public TextMeshProUGUI fishEatenText;
    private float textDisplayTime = 2f; // How long the text should be displayed

    [SerializeField] private float holdTimeToReset = 3f; // Time in seconds the flashlight must shine on the Bunyip to reset it
    private float flashlightHoldTimer = 0f; // Timer to track how long the flashlight has been shining on the Bunyip

    private Camera mainCamera; // Reference to the main camera

    PlayerScript player_script;

    private void Start()
    {
        mainCamera = Camera.main; // Cache the Main Camera at the start
        ResetMonsterPosition(); // Set the monster to the starting position

        // Set the initial random time between moves
        SetRandomTimeBetweenMoves();

        // Ensure the text is initially hidden
        fishEatenText.text = "";

        player_script = PlayerScript.Instance;
    }

    private void Update()
    {
        // Only try moving the monster when the fishing UI is active and the player isn't transitioning
        if (fishingBackground.gameObject.activeInHierarchy && !player_script.gamePaused)
        {
            player.BatteryDecrease();
            // Increment the timer
            moveTimer += Time.deltaTime;

           

            // Try moving the monster after the timer reaches the randomly generated timeBetweenMoves
            if (moveTimer >= timeBetweenMoves)
            {
                TryMoveMonster();
                moveTimer = 0f; // Reset the timer

                // Set a new random time between moves
                SetRandomTimeBetweenMoves();
            }
        }

        // Check if the monster is hit by the flashlight's raycast
        CheckFlashlightHit();
    }

    private void SetRandomTimeBetweenMoves()
    {
        // Generate a random time between minTimeBetweenMoves and maxTimeBetweenMoves
        timeBetweenMoves = Random.Range(minTimeBetweenMoves, maxTimeBetweenMoves);
    }

    private void TryMoveMonster()
    {
        // If the monster is at the last position, check the player's fish count
        if (currentMonsterIndex >= monsterPositions.Length - 1)
        {
            CheckPlayerFish(); // Check whether the player survives or dies
        }
        else
        {
            // Move the monster to the next position if there are remaining positions
            MoveMonster();
        }
    }

    private void MoveMonster()
    {
        // Disable the current position model
        monsterPositions[currentMonsterIndex].SetActive(false);

        // Move to the next position and enable the new model
        currentMonsterIndex++;
        monsterPositions[currentMonsterIndex].SetActive(true);
    }

    private void CheckPlayerFish()
    {
        if (player.fishCaught <= 0)
        {
            // If player has no fish, the monster kills the player (trigger game over or restart)
            Debug.Log("The monster killed the player!");
            SceneManager.LoadScene("LoseScene"); // Load game over scene
        }
        else
        {
            // If player has fish, the monster takes 1 fish and the player survives
            player.fishCaught--; // Take 1 fish from the player
            Debug.Log("The monster took 1 fish! Remaining fish: " + player.fishCaught);

            // Show the message that a fish was eaten
            StartCoroutine(ShowFishEatenMessage());

            // Reset the monster to the starting position after taking a fish
            ResetMonsterPosition();
        }
    }

    private IEnumerator ShowFishEatenMessage()
    {
        // Show the text
        fishEatenText.text = "Bunyip ate 1 fish!";

        // Wait for a few seconds
        yield return new WaitForSeconds(textDisplayTime);

        // Hide the text again
        fishEatenText.text = "";
    }

    private void CheckFlashlightHit()
    {
        // **Only perform the raycast if the flashlight is enabled**
        if (flashlight.enabled)
        {
            // Use the Main Camera for raycasting
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                // If the monster is hit by the flashlight, start the hold timer
                if (hitInfo.collider.gameObject == monsterPositions[currentMonsterIndex])
                {
                    flashlightHoldTimer += Time.deltaTime; // Accumulate the time flashlight is on the Bunyip

                    // If the flashlight has been held on the Bunyip for enough time, reset the monster
                    if (flashlightHoldTimer >= holdTimeToReset)
                    {
                        ResetMonsterPosition();
                        flashlightHoldTimer = 0f; // Reset the hold timer after reset
                    }
                }
                else
                {
                    flashlightHoldTimer = 0f; // Reset the timer if the flashlight moves off the Bunyip
                }
            }
            else
            {
                flashlightHoldTimer = 0f; // Reset the timer if the raycast doesn't hit the Bunyip
            }
        }
        else
        {
            flashlightHoldTimer = 0f; // Reset the timer if the flashlight is off
        }
    }

    public void ResetMonsterPosition()
    {
        // Reset the monster to the starting position
        if (monsterPositions[currentMonsterIndex].activeSelf)
        {
            monsterPositions[currentMonsterIndex].SetActive(false);
        }
        currentMonsterIndex = 0; // Go back to the starting position
        monsterPositions[currentMonsterIndex].SetActive(true);
    }
}
