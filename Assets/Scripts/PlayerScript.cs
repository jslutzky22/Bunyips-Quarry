using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializeField] private GameObject fishingCanvasBackground;
    [SerializeField] private GameObject transition;

    [Header("Values")]
    [SerializeField] private bool transitioning;
    [SerializeField] private float transitionOpacity; //is a number from 0-1
    [SerializeField] private float transitionLerp; //is a number from 0-1
    [SerializeField] private float fishingBarProgress;
    [SerializeField] private int fishCaught;

    void Start()
    {
        fishCaught = 0;
        fishingBarProgress = 0;
        TurnAround = PlayerControls.currentActionMap.FindAction("TurnAround");
        Left = PlayerControls.currentActionMap.FindAction("Left");
        Right = PlayerControls.currentActionMap.FindAction("Right");
        Interact = PlayerControls.currentActionMap.FindAction("Interact");
        LeftClick = PlayerControls.currentActionMap.FindAction("LeftClick");
        RightClick = PlayerControls.currentActionMap.FindAction("RightClick");
    }

    void OnTurnAround()
    {
        Debug.Log("space pressed");
        if (!transitioning)
        {
            if (!fishingCanvasBackground.activeSelf)
            {
                StartCoroutine(TransitionToFishing());
            }
            else
            {
                StartCoroutine(TransitionFromFishing());
            }
        }
    }

    IEnumerator TransitionFromFishing()
    {
        transitioning = true;
        transitionLerp = 0f;
        transitionOpacity = 0f;
        transition.SetActive(true);
        while (transitionOpacity < 1)
        {
            transition.GetComponent<UnityEngine.UI.Image>().color = new Vector4(255, 255, 255, transitionOpacity);
            yield return new WaitForSecondsRealtime(0.01f);
            transitionOpacity += 0.01f;
        }
        while (transitionLerp < 1)
        {
            transition.GetComponent<UnityEngine.UI.Image>().color = Color.Lerp(Color.white, Color.black, transitionLerp);
            yield return new WaitForSecondsRealtime(0.01f);
            transitionLerp += 0.01f;
        }
        fishingCanvasBackground.SetActive(false);
        while (transitionOpacity > 0)
        {
            transition.GetComponent<UnityEngine.UI.Image>().color = new Vector4(0, 0, 0, transitionOpacity);
            yield return new WaitForSecondsRealtime(0.01f);
            transitionOpacity -= 0.01f;
        }
        transition.SetActive(false);
        transitioning = false;
    }

    IEnumerator TransitionToFishing()
    {
        transitioning = true;
        transitionLerp = 1f;
        transitionOpacity = 0f;
        transition.SetActive(true);
        while (transitionOpacity < 1)
        {
            transition.GetComponent<UnityEngine.UI.Image>().color = new Vector4(0, 0, 0, transitionOpacity);
            yield return new WaitForSecondsRealtime(0.01f);
            transitionOpacity += 0.01f;
        }
        while (transitionLerp > 0)
        {
            transition.GetComponent<UnityEngine.UI.Image>().color = Color.Lerp(Color.white, Color.black, transitionLerp);
            yield return new WaitForSecondsRealtime(0.01f);
            transitionLerp -= 0.01f;
        }
        fishingCanvasBackground.SetActive(true);
        while (transitionOpacity > 0)
        {
            transition.GetComponent<UnityEngine.UI.Image>().color = new Vector4(255, 255, 255, transitionOpacity);
            yield return new WaitForSecondsRealtime(0.01f);
            transitionOpacity -= 0.01f;
        }
        transition.SetActive(false);
        transitioning = false;
    }

    void Update()
    {
        fishingBarProgress += 0.01f;
        if (fishingBarProgress > 0.99f)
        {
            fishCaught++;
        }

        if (transitionOpacity < 0)
        {
            transitionOpacity = 0;
        }
        if (transitionOpacity > 1)
        {
            transitionOpacity = 1;
        }
        if (transitionLerp < 0)
        {
            transitionLerp = 0;
        }
        if (transitionLerp > 1)
        {
            transitionLerp = 1;
        }
    }
}
