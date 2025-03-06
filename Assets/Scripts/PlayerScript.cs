using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private InputAction Pause;

    [Header("GameObjects")]
    [SerializeField] private GameObject fishingCanvasBackground;
    [SerializeField] private TMP_Text fishCaughtText;
    [SerializeField] private Slider fishingBar;
    [SerializeField] private GameObject transition;
    [SerializeField] private GameObject minigameOneUI;
    [SerializeField] private GameObject minigameOneA;
    [SerializeField] private GameObject minigameOneD;
    [SerializeField] private GameObject minigameTwoUI;
    [SerializeField] private Slider minigameTwoSlider;
    [SerializeField] private GameObject minigameThreeButton;
    [SerializeField] private GameObject minigameThreeUI;
    [SerializeField] private Slider minigameThreeSlider;
    [SerializeField] private GameObject pauseMenu;

    [Header("Values")]
    [SerializeField] private float fishingBarProgress;
    [SerializeField] public int fishCaught;
    [SerializeField] private float winFishAmount; //How Many Fish to win
    [SerializeField] private bool transitioning;
    [SerializeField] private float transitionOpacity; //is a number from 0-1
    [SerializeField] private float transitionLerp; //is a number from 0-1
    [SerializeField] private int lastMinigamePlayed;
    [SerializeField] private int minigameRNGNumber;
    [SerializeField] private bool minigameOneUIWasActive;
    [SerializeField] private bool minigameOneAWasActive;
    [SerializeField] private bool minigameOneDWasActive;
    [SerializeField] private float minigameOneTimer;
    [SerializeField] private int minigameOneProgress;
    [SerializeField] private bool minigameTwoUIWasActive;
    [SerializeField] private bool minigameTwoSliderWasActive;
    [SerializeField] private bool minigameTwoWon;
    [SerializeField] private bool minigameTwoReverse;
    [SerializeField] private float minigameTwoValue;
    [SerializeField] private float minigameTwoTimer;
    [SerializeField] private bool minigameThreeActive;
    [SerializeField] private float minigameThreeTimer;
    [SerializeField] private float minigameThreeValue;
    [SerializeField] private bool activeSceneIs2D;
    [SerializeField] private string lastHitKey;
    [SerializeField] private bool interactActive;
    [SerializeField] private bool interactDisabled;
    [SerializeField] private bool leftClickHeld;
    [SerializeField] private bool gamePaused;
    [SerializeField] private float batteryPercentage;
    [SerializeField] private float batteryDrain;

    void Start()
    {
        gamePaused = false;
        batteryPercentage = 1f;
        lastMinigamePlayed = 0;
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
        Pause = PlayerControls.currentActionMap.FindAction("Pause");

        LeftClick.started += Handle_LeftClickStarted;
        LeftClick.canceled += Handle_LeftClickCanceled;

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

    void OnPause()
    {
        Debug.Log("OnPause");
        if (!gamePaused && !transitioning)
        {
            Debug.Log("!gamePaused and !transitioning");
            gamePaused = true;
            activeSceneIs2D = false;
            transitioning = true;
            pauseMenu.SetActive(true);
            
        }
        else if (gamePaused)
        {
            Debug.Log("gamePaused");
            gamePaused = false;
            if (!(!fishingCanvasBackground.activeSelf))
            {
                activeSceneIs2D = true;
            }
            else
            {
                activeSceneIs2D = false;
            }
            transitioning = false;
            pauseMenu.SetActive(false);
        }
    }

    private void Handle_LeftClickStarted(InputAction.CallbackContext obj)
    {
        leftClickHeld = true;
    }

    private void Handle_LeftClickCanceled(InputAction.CallbackContext obj)
    {
        leftClickHeld = false;
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
        minigameRNGNumber = Random.Range(1, 4);
        while (minigameRNGNumber == lastMinigamePlayed)
        {
            minigameRNGNumber = Random.Range(1, 4);
        }
        if (minigameRNGNumber == 1)
        {
            StartCoroutine(MinigameOne());
        }
        if (minigameRNGNumber == 2)
        {
            StartCoroutine(MinigameTwo());
        }
        if (minigameRNGNumber == 3)
        {
            StartCoroutine(MinigameThree());
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
            fishingBarProgress += 0.15f;
        }
        lastMinigamePlayed = 1;
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
            fishingBarProgress += 0.15f;
            minigameTwoWon = false;
        }
        minigameTwoUI.SetActive(false);
        lastMinigamePlayed = 2;
        StartCoroutine(Minigames());
    }

    IEnumerator MinigameThree()
    {
        minigameThreeValue = 0;
        minigameThreeTimer = 0;
        minigameThreeActive = true;
        minigameThreeUI.SetActive(true);
        while (minigameThreeValue < 1 && minigameThreeTimer < 20)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            minigameThreeTimer += 0.1f;
            minigameThreeValue -= 0.025f;
            
            while (!activeSceneIs2D)
            {
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }
        minigameThreeUI.SetActive(false);
        minigameThreeActive = false;
        if (minigameThreeValue >= 1)
        {
            fishingBarProgress += 0.15f;
        }
        minigameThreeValue = 0;
        minigameThreeTimer = 0;
        lastMinigamePlayed = 3;
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
            transitionOpacity += 0.02f;
        }
        transitionOpacity = 1f;
        while (transitionLerp < 1)
        {
            transition.GetComponent<UnityEngine.UI.Image>().color = Color.Lerp(Color.white, Color.black, transitionLerp);
            yield return new WaitForSecondsRealtime(0.01f);
            transitionLerp += 0.02f;
        }
        transitionLerp = 1f;
        fishingCanvasBackground.SetActive(false);
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
            transitionOpacity -= 0.02f;
        }
        transitionOpacity = 0f;
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
            transitionOpacity += 0.02f;
        }
        transitionOpacity = 1f;
        while (transitionLerp > 0)
        {
            transition.GetComponent<UnityEngine.UI.Image>().color = Color.Lerp(Color.white, Color.black, transitionLerp);
            yield return new WaitForSecondsRealtime(0.01f);
            transitionLerp -= 0.02f;
        }
        transitionLerp = 0f;
        fishingCanvasBackground.SetActive(true);
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
            transitionOpacity -= 0.02f;
        }
        transitionOpacity = 0f;
        transition.SetActive(false);
        activeSceneIs2D = true;
        transitioning = false;
    }

    public void ButtonPressedDown()
    {
        if (minigameThreeActive)
        {
            minigameThreeValue += 0.15f;
        }
    }

    IEnumerator BatteryDecrease()
    {
        if (batteryDrain == 0)
        {
            batteryDrain = 0.001f;
        }
        while (!activeSceneIs2D && !transitioning)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            batteryPercentage -= batteryDrain;
        }
    }

    public void ResumeGame()
    {
        gamePaused = false;
        if (!(!fishingCanvasBackground.activeSelf))
        {
            activeSceneIs2D = true;
        }
        else
        {
            activeSceneIs2D = false;
        }
        transitioning = false;
        pauseMenu.SetActive(false);
    }

    public void BackToMenu() 
    {
        SceneManager.GetSceneByBuildIndex(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void FixedUpdate()
    {
        fishingBar.GetComponent<Slider>().value = fishingBarProgress;
        if (activeSceneIs2D)
        {
            fishingBarProgress += 0.000065f;

            minigameThreeSlider.value = minigameThreeValue;

            if (fishingBarProgress >= 1f)
            {
                fishCaught++;
                fishingBarProgress -= 1f;
                batteryPercentage += 0.01f * Random.Range(1, 3);
                if (fishCaught >= winFishAmount)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
                }
            }
        }

        if (!activeSceneIs2D)
        {
            StartCoroutine(BatteryDecrease());
        }

        if (!activeSceneIs2D && !transitioning)
        {
            fishingBarProgress -= 0.0000325f;
        }

        if (batteryPercentage > 1f)
        {
            batteryPercentage = 1f;
        }

        fishCaughtText.text = "Fish Caught: " + fishCaught;

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
