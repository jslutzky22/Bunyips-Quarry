using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    [Header("InputActions")]
    public PlayerInput PlayerControls;
    private InputAction TurnAround;
    private InputAction Left;
    private InputAction Right;
    private InputAction Interact;
    private InputAction LeftClick;
    private InputAction RightClick;

    [Header("GameObjects")]
    [SerializeField] private GameObject fishingCanvas;

    void Start()
    {
        TurnAround = PlayerControls.currentActionMap.FindAction("TurnAround");
        Left = PlayerControls.currentActionMap.FindAction("Left");
        Right = PlayerControls.currentActionMap.FindAction("Right");
        Interact = PlayerControls.currentActionMap.FindAction("Interact");
        LeftClick = PlayerControls.currentActionMap.FindAction("LeftClick");
        RightClick = PlayerControls.currentActionMap.FindAction("RightClick");
    }

    void OnTurnAround()
    {
        if (fishingCanvas.activeSelf == true)
        {
            fishingCanvas.SetActive(false);
        }
        if (fishingCanvas.activeSelf == false)
        {
            fishingCanvas.SetActive(true);
        }
    }

    void Update()
    {
        
    }
}
