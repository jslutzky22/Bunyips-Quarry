using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
    [SerializeField] private Slider fishingBar;
    [SerializeField] private GameObject minigameOneUI;
    [SerializeField] private GameObject minigameOneA;
    [SerializeField] private GameObject minigameOneD;
    [SerializeField] private GameObject minigameTwoUI;
    [SerializeField] private Slider minigameTwoSlider;


    [Header("Values")]
    [SerializeField] private bool minigameOneUIWasActive;
    [SerializeField] private bool minigameOneAWasActive;
    [SerializeField] private bool minigameOneDWasActive;
    [SerializeField] private bool minigameTwoUIWasActive;
    [SerializeField] private bool minigameTwoSliderWasActive;
    [SerializeField] private bool activeSceneIs2D;
    [SerializeField] private bool transitioning;
    [SerializeField] private float transitionOpacity; //is a number from 0-1
    [SerializeField] private float transitionLerp; //is a number from 0-1
    [SerializeField] private float fishingBarProgress;
    [SerializeField] public int fishCaught;
    [SerializeField] private int minigameRNGNumber;
    [SerializeField] private int minigameOneProgress;
    [SerializeField] private string lastHitKey;
    [SerializeField] private float minigameOneTimer;
    [SerializeField] private float minigameTwoValue;
    [SerializeField] private bool interactActive;
    [SerializeField] private bool interactDisabled;
    [SerializeField] private float minigameTwoTimer;
    [SerializeField] private bool minigameTwoWon;
    [SerializeField] private bool minigameTwoReverse;
    [SerializeField] private float winFishAmount; //How Many Fish to win

    void Start()
    {
        activeSceneIs2D = true;
        interactActive = false;
        interactDisabled = false;
        fishCaught = 0;
        fishingBarProgress = 0;
        TurnAround = PlayerControls.currentActionMap.FindAction("TurnAround");
        Left = PlayerControls.currentActionMap.FindAction("Left");
        Right = PlayerControls.currentActionMap.FindAction("Right");
        Interact = PlayerControls.currentActionMap.FindAction("Interact");
        LeftClick = PlayerControls.currentActionMap.FindAction("LeftClick");
        RightClick = PlayerControls.currentActionMap.FindAction("RightClick");

        StartCoroutine(Minigames());
    }

    void OnTurnAround()
    {
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

    void OnLeft()
    {
        lastHitKey = "A";
    }

    void OnRight()
    {
        lastHitKey = "D";
    }

    void OnInteract()
    {
        if (!interactActive && !interactDisabled)
        {
            StartCoroutine(InteractPushed());
        }
    }

    IEnumerator InteractPushed()
    {
        interactActive = true;
        yield return new WaitForSecondsRealtime(0.01f);
        interactActive = false;
        interactDisabled = true;
        yield return new WaitForSecondsRealtime(0.25f);
        interactDisabled = false;
    }

    IEnumerator Minigames()
    {
        yield return new WaitForSecondsRealtime(3f);
        while (!activeSceneIs2D)
        {
            yield return new WaitForSecondsRealtime(0.1f);
        }
        minigameRNGNumber = Random.Range(1, 3);
        if (minigameRNGNumber == 1)
        {
            StartCoroutine(MinigameOne());
        }
        if (minigameRNGNumber == 2)
        {
            StartCoroutine(MinigameTwo());
        }
    }

    IEnumerator MinigameOne()
    {
        minigameOneProgress = 0;
        minigameOneUI.SetActive(true);
        minigameOneA.SetActive(true);
        lastHitKey = "none";
        while (minigameOneProgress < 20)
        {
            yield return new WaitForSecondsRealtime(0.01f);
            if (activeSceneIs2D)
            {
                if (lastHitKey == "A" && !(!minigameOneA.activeSelf))
                {
                    minigameOneA.SetActive(false);
                    minigameOneD.SetActive(true);
                    minigameOneProgress++;
                }
                if (lastHitKey == "D" && !(!minigameOneD.activeSelf))
                {
                    minigameOneA.SetActive(true);
                    minigameOneD.SetActive(false);
                    minigameOneProgress++;
                }

                minigameOneTimer += 0.01f;
                if (minigameOneTimer > 10)
                {
                    minigameOneTimer = 0;
                    break;
                }
            }
        }
        minigameOneUI.SetActive(false);
        minigameOneA.SetActive(false);
        minigameOneD.SetActive(false);
        if (minigameOneProgress >= 20)
        {
            fishingBarProgress += 0.30f;
        }
        StartCoroutine(Minigames());
    }

    IEnumerator MinigameTwo()
    {
        minigameTwoUI.SetActive(true);
        minigameTwoTimer = 0;
        minigameTwoValue = 0;
        minigameTwoWon = false;
        while (minigameTwoTimer < 20 && !minigameTwoWon)
        {
            yield return new WaitForSecondsRealtime(0.001f);
            while (minigameTwoValue < 0.4f && minigameTwoTimer < 20 && !minigameTwoWon && !minigameTwoReverse && activeSceneIs2D)
            {
                minigameTwoValue += 0.03f;
                minigameTwoSlider.GetComponent<Slider>().value = minigameTwoValue;
                yield return new WaitForSecondsRealtime(0.01f);
                minigameTwoTimer =+ 0.01f;
                if (interactActive)
                {
                    interactActive = false;
                    interactDisabled = true;
                    yield return new WaitForSecondsRealtime(0.5f);
                    interactDisabled = false;
                }
            }
            while (minigameTwoValue >= 0.4f && minigameTwoValue <= 0.6f && minigameTwoTimer < 20 && !minigameTwoWon && !minigameTwoReverse && activeSceneIs2D)
            {
                minigameTwoValue += 0.03f;
                minigameTwoSlider.GetComponent<Slider>().value = minigameTwoValue;
                yield return new WaitForSecondsRealtime(0.01f);
                minigameTwoTimer = +0.01f;
                if (interactActive)
                {
                    minigameTwoWon = true;
                    break;
                }
            }
            while (minigameTwoValue > 0.6f && minigameTwoTimer < 20 && !minigameTwoWon && !minigameTwoReverse && activeSceneIs2D)
            {
                minigameTwoValue += 0.03f;
                minigameTwoSlider.GetComponent<Slider>().value = minigameTwoValue;
                yield return new WaitForSecondsRealtime(0.01f);
                minigameTwoTimer = +0.01f;
                if (interactActive)
                {
                    interactActive = false;
                    interactDisabled = true;
                    yield return new WaitForSecondsRealtime(0.5f);
                    interactDisabled = false;
                }
                if (minigameTwoValue >= 1)
                {
                    minigameTwoReverse = true;
                }
            }
            while (minigameTwoValue > 0.6f && minigameTwoTimer < 20 && !minigameTwoWon && minigameTwoReverse && activeSceneIs2D)
            {
                minigameTwoValue -= 0.03f;
                minigameTwoSlider.GetComponent<Slider>().value = minigameTwoValue;
                yield return new WaitForSecondsRealtime(0.01f);
                minigameTwoTimer = +0.01f;
                if (interactActive)
                {
                    interactActive = false;
                    interactDisabled = true;
                    yield return new WaitForSecondsRealtime(0.5f);
                    interactDisabled = false;
                }
            }
            while (minigameTwoValue >= 0.4f && minigameTwoValue <= 0.6f && minigameTwoTimer < 20 && !minigameTwoWon && minigameTwoReverse && activeSceneIs2D)
            {
                minigameTwoValue -= 0.03f;
                minigameTwoSlider.GetComponent<Slider>().value = minigameTwoValue;
                yield return new WaitForSecondsRealtime(0.01f);
                minigameTwoTimer = +0.01f;
                if (interactActive)
                {
                    minigameTwoWon = true;
                    break;
                }
            }
            while (minigameTwoValue < 0.4f && minigameTwoTimer < 20 && !minigameTwoWon && minigameTwoReverse && activeSceneIs2D)
            {
                minigameTwoValue -= 0.03f;
                minigameTwoSlider.GetComponent<Slider>().value = minigameTwoValue;
                yield return new WaitForSecondsRealtime(0.01f);
                minigameTwoTimer = +0.01f;
                if (interactActive)
                {
                    interactActive = false;
                    interactDisabled = true;
                    yield return new WaitForSecondsRealtime(0.5f);
                    interactDisabled = false;
                }
                if (minigameTwoValue <= 0)
                {
                    minigameTwoReverse = false;
                }
            }
        }
        if (minigameTwoWon)
        {
            fishingBarProgress += 0.30f;
            minigameTwoWon = false;
        }
        minigameTwoUI.SetActive(false);
        StartCoroutine(Minigames());
    }

    IEnumerator TransitionFromFishing()
    {
        activeSceneIs2D = false;
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
        fishingBar.gameObject.SetActive(false);
        if (!(!minigameOneUI.activeSelf))
        {
            minigameOneUI.SetActive(false);
            minigameOneUIWasActive = true;
        }
        if (!(!minigameOneA.activeSelf))
        {
            minigameOneA.SetActive(false);
            minigameOneAWasActive = true;
        }
        if (!(!minigameOneD.activeSelf))
        {
            minigameOneD.SetActive(false);
            minigameOneDWasActive = true;
        }
        if (!(!minigameTwoUI.activeSelf))
        {
            minigameTwoUI.SetActive(false);
            minigameTwoUIWasActive = true;
        }
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
        fishingBar.gameObject.SetActive(true);
        if (minigameOneUIWasActive)
        {
            minigameOneUI.SetActive(true);
            minigameOneUIWasActive = false;
        }
        if (minigameOneAWasActive)
        {
            minigameOneA.SetActive(true);
            minigameOneAWasActive = false;
        }
        if (minigameOneDWasActive)
        {
            minigameOneD.SetActive(true);
            minigameOneDWasActive = false;
        }
        if (minigameTwoUIWasActive)
        {
            minigameTwoUI.SetActive(true);
            minigameTwoUIWasActive = false;
        }
        while (transitionOpacity > 0)
        {
            transition.GetComponent<UnityEngine.UI.Image>().color = new Vector4(255, 255, 255, transitionOpacity);
            yield return new WaitForSecondsRealtime(0.01f);
            transitionOpacity -= 0.01f;
        }
        transition.SetActive(false);
        activeSceneIs2D = true;
        transitioning = false;
    }

    void FixedUpdate()
    {
        fishingBar.GetComponent<Slider>().value = fishingBarProgress;
        if (activeSceneIs2D)
        {
            fishingBarProgress += 0.00005f;
            if (fishingBarProgress >= 1f)
            {
                fishCaught++;
                fishingBarProgress -= 1f;
                if (fishCaught >= winFishAmount)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
                }
            }
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
