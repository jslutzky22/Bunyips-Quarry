using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BunyipControl : MonoBehaviour
{
    public GameObject[] monsterPositions; // Array of positions/models the monster will move through
    public int currentMonsterIndex = 0; // Tracks the monster's current position

    public float timeBetweenMoves = 5f; // Time in seconds between each move attempt
    private float moveTimer = 0f; // Tracks time since last move

    public Canvas uiCanvas; // Reference to the UI Canvas to track if the monster should move
    public Light flashlight; // Reference to the player's flashlight

    private Camera mainCamera; // Reference to the main camera

    private void Start()
    {
        mainCamera = Camera.main; // Cache the Main Camera at the start
        ResetMonsterPosition(); // Set the monster to the starting position
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
        // Move the monster to the next position if there are remaining positions
        if (currentMonsterIndex < monsterPositions.Length - 1)
        {
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
