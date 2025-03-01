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
    public float timeBetweenMoves = 5f; // Time in seconds between each move attempt
    private float moveTimer = 0f; // Tracks time since last move

    public Canvas uiCanvas; // Reference to the UI Canvas to track if the monster should move
    public Light flashlight; // Reference to the player's flashlight
    public PlayerScript player; // Reference to PlayerScript to access the player's fish count and check for death

    // Reference to the TextMeshPro UI for showing the fish-eaten message
    public TextMeshProUGUI fishEatenText;
    private float textDisplayTime = 2f; // How long the text should be displayed

    private Camera mainCamera; // Reference to the main camera

    private void Start()
    {
        mainCamera = Camera.main; // Cache the Main Camera at the start
        ResetMonsterPosition(); // Set the monster to the starting position

        // Ensure the text is initially hidden
        fishEatenText.text = "";
    }

    private void Update()
    {
        // Only try moving the monster when the UI Canvas is active (i.e., the player isn't looking)
        if (uiCanvas.gameObject.activeInHierarchy)
        {
            // Increment the timer
            moveTimer += Time.deltaTime;

            // Try moving the monster after the timer reaches the timeBetweenMoves
            if (moveTimer >= timeBetweenMoves)
            {
                TryMoveMonster();
                moveTimer = 0f; // Reset the timer
            }
        }

        // Check if the monster is hit by the flashlight's raycast
        CheckFlashlightHit();
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
        fishEatenText.text = "The monster ate 1 fish!";

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
                // If the monster is hit by the flashlight, reset to the starting position
                if (hitInfo.collider.gameObject == monsterPositions[currentMonsterIndex])
                {
                    ResetMonsterPosition();
                }
            }
        }
    }

    private void ResetMonsterPosition()
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
