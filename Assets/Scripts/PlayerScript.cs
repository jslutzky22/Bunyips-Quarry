using System.Collections;
using System.Collections.Generic;
using System.Transactions;
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
    private InputAction Up;
    private InputAction Up2;
    private InputAction Left;
    private InputAction Left2;
    private InputAction Down;
    private InputAction Down2;
    private InputAction Right;
    private InputAction Right2;
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
    [SerializeField] private AudioSource fishingAudio;
    private float fishingAudioTime;
    [SerializeField] private AudioSource forestAudio;
    private float forestAudioTime;
    [SerializeField] private UnityEngine.UI.Image batteryBar;
    public GameObject fishSprite;
    [SerializeField] private GameObject minigameFourUI;
    [SerializeField] private GameObject minigameFourInput1Object;
    [SerializeField] private GameObject minigameFourInput2Object;
    [SerializeField] private GameObject minigameFourInput3Object;
    [SerializeField] private GameObject minigameFourInput4Object;


    AudioSource audioSource;

    //[SerializeField] private Slider batteryBar;

    [Header("Values")]
    private float fishingBarProgress;
    [SerializeField] private float fishingBarGain;
    public int fishCaught;
    [SerializeField] private float winFishAmount; //How Many Fish to win
    public bool transitioning;
    private float transitionOpacity; //is a number from 0-1
    private float transitionLerp; //is a number from 0-1
    private int lastMinigamePlayed;
    private int minigameRNGNumber;
    private float minigameChooseTimer;
    [SerializeField] private float minigameChooseTimerLength;
    private bool minigameOneUIWasActive;
    private bool minigameOneAWasActive;
    private bool minigameOneDWasActive;
    private float minigameOneTimer;
    [SerializeField] private float minigameOneTimerAmount;
    private int minigameOneProgress;
    [SerializeField] private int minigameOneMaxProgress;
    private bool minigameTwoUIWasActive;
    private bool minigameTwoSliderWasActive;
    private bool minigameTwoWon;
    private bool minigameTwoReverse;
    private float minigameTwoValue;
    private float minigameTwoTimer;
    private bool minigameThreeActive;
    private bool minigameThreeUIWasActive;
    private float minigameThreeTimer;
    private float minigameThreeValue;
    private int minigameFourProgress;
    private int minigameFourInput1;
    private int minigameFourInput2;
    private int minigameFourInput3;
    private int minigameFourInput4;
    private float minigameFourTimer;
    public bool activeSceneIs2D;
    private string lastHitKey;
    private bool interactActive;
    private bool interactDisabled;
    public bool gamePaused;
    private bool gameWasPausedDuringMinigameTimer;
    public float batteryPercentage;
    [SerializeField] private float batteryPercentageGain;
    [SerializeField] private float batteryDrain;
    private bool batteryDraining;


    [Header("Sounds")]
    [SerializeField] private AudioClip lightClick;
    [SerializeField] private AudioClip offClick;
    [SerializeField] private AudioClip minigameSuccess;
    [SerializeField] private AudioClip reelIn;
    [SerializeField] private AudioClip fishSplash;
    [SerializeField] private AudioClip transitionSound;


    [HideInInspector] public static PlayerScript Instance;

    private void Awake()
    {
        Instance = this;
    }

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
        if (minigameChooseTimerLength == 0)
        {
            minigameChooseTimerLength = 3f;
        }
        if (minigameOneMaxProgress == 0)
        {
            minigameOneMaxProgress = 20;
        }
        if (minigameOneTimerAmount == 0)
        {
            minigameOneTimerAmount = 10;
        }
        if (fishingBarGain == 0)
        {
            fishingBarGain = 0.15f;
        }
        if (batteryPercentageGain == 0)
        {
            batteryPercentageGain = 0.01f;
        }
        TurnAround = PlayerControls.currentActionMap.FindAction("TurnAround");
        Up = PlayerControls.currentActionMap.FindAction("Up");
        Up2 = PlayerControls.currentActionMap.FindAction("Up2");
        Left = PlayerControls.currentActionMap.FindAction("Left");
        Left2 = PlayerControls.currentActionMap.FindAction("Left2");
        Down = PlayerControls.currentActionMap.FindAction("Down");
        Down2 = PlayerControls.currentActionMap.FindAction("Down2");
        Right = PlayerControls.currentActionMap.FindAction("Right");
        Right2 = PlayerControls.currentActionMap.FindAction("Right2");
        Interact = PlayerControls.currentActionMap.FindAction("Interact");
        LeftClick = PlayerControls.currentActionMap.FindAction("LeftClick");
        RightClick = PlayerControls.currentActionMap.FindAction("RightClick");
        Pause = PlayerControls.currentActionMap.FindAction("Pause");

        StartCoroutine(Minigames());


        //MAKE FISHING SIDE START PLAYING
        forestAudioTime = forestAudio.time;
        forestAudio.Pause();
        forestAudio.mute = true;
        audioSource = GetComponent<AudioSource>();

    }

    void OnTurnAround()
    {
        if (!transitioning)
        {
            if (!fishingCanvasBackground.activeSelf)
            {
                StartCoroutine(TransitionToFishing());
                audioSource.PlayOneShot(transitionSound, 1F);
                forestAudioTime = forestAudio.time;
                fishingAudio.UnPause();
                fishingAudio.time = fishingAudioTime;
                forestAudio.Pause();

                //MAKE FISH SIDE AUDIO PLAY, RECORD FOREST SIDE AUDIO POINT, PAUSE FOREST SIDE AUDIO POINT 
            }
            else
            {
                StartCoroutine(TransitionFromFishing());
                audioSource.PlayOneShot(transitionSound, 1F);
                fishingAudioTime = fishingAudio.time;
                forestAudio.UnPause();
                forestAudio.mute = false;
                forestAudio.time = forestAudioTime;
                fishingAudio.Pause();

                //MAKE FOREST SIDE AUDIO PLAY, RECORD FOREST SIDE AUDIO POINT, FOREST FISH SIDE AUDIO POINT 
            }
        }
    }

    void OnUp()
    {
        lastHitKey = "W";
    }
    void OnUp2()
    {
        lastHitKey = "W";
    }

    void OnLeft()
    {
        lastHitKey = "A";
    }
    void OnLeft2()
    {
        lastHitKey = "A";
    }

    void OnDown()
    {
        lastHitKey = "S";
    }
    void OnDown2()
    {
        lastHitKey = "S";
    }

    void OnRight()
    {
        lastHitKey = "D";
    }
    void OnRight2()
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
            gamePaused = true;
            Cursor.visible = true;
            activeSceneIs2D = false;
            transitioning = true;
            pauseMenu.SetActive(true);
            
        }
        else if (gamePaused)
        {
            gamePaused = false;
            if (!(!fishingCanvasBackground.activeSelf))
            {
                activeSceneIs2D = true;
                Cursor.visible = true;
            }
            else
            {
                activeSceneIs2D = false;
                Cursor.visible = false;
            }
            transitioning = false;
            pauseMenu.SetActive(false);
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
        minigameChooseTimer = minigameChooseTimerLength;
        while (minigameChooseTimer > 0)
        {
            while (minigameChooseTimer > 0 && activeSceneIs2D && !gamePaused) 
            {
                yield return new WaitForSecondsRealtime(0.1f);
                minigameChooseTimer -= 0.1f;
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
        minigameRNGNumber = Random.Range(1, 5);
        while (minigameRNGNumber == lastMinigamePlayed)
        {
            minigameRNGNumber = Random.Range(1, 5);
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
        if (minigameRNGNumber == 4)
        {
            StartCoroutine(MinigameFour());
        }
    }

    IEnumerator MinigameOne()
    {
        minigameOneProgress = 0;
        minigameOneUI.SetActive(true);
        minigameOneA.SetActive(true);
        lastHitKey = "none";
        while (minigameOneProgress < minigameOneMaxProgress)
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
                if (minigameOneTimer > minigameOneTimerAmount)
                {
                    minigameOneTimer = 0;
                    break;
                }
            }
        }
        minigameOneUI.SetActive(false);
        minigameOneA.SetActive(false);
        minigameOneD.SetActive(false);
        if (minigameOneProgress >= minigameOneMaxProgress)
        {
            fishingBarProgress += fishingBarGain;
            batteryPercentage += batteryPercentageGain * Random.Range(1, 3);
            batteryBar.fillAmount = batteryPercentage;
            audioSource.PlayOneShot(reelIn, 1F);
            audioSource.PlayOneShot(minigameSuccess, 1F);
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
            batteryPercentage += 0.01f * Random.Range(1, 3);
            batteryBar.fillAmount = batteryPercentage;
            audioSource.PlayOneShot(reelIn, 1F);
            audioSource.PlayOneShot(minigameSuccess, 1F);
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
            batteryPercentage += 0.01f * Random.Range(1, 3);
            batteryBar.fillAmount = batteryPercentage;
            audioSource.PlayOneShot(reelIn, 1F);
            audioSource.PlayOneShot(minigameSuccess, 1F);
        }
        minigameThreeValue = 0;
        minigameThreeTimer = 0;
        lastMinigamePlayed = 3;
        StartCoroutine(Minigames());
    }

    IEnumerator MinigameFour()
    {
        minigameFourProgress = 0;
        minigameFourUI.SetActive(true);
        lastHitKey = "none";
        minigameFourInput1 = Random.Range(1, 5);
        minigameFourInput2 = Random.Range(1, 5);
        minigameFourInput3 = Random.Range(1, 5);
        minigameFourInput4 = Random.Range(1, 5);

        if (minigameFourInput1 == 1)
        {
            minigameFourInput1Object.transform.eulerAngles = new Vector3(0, 0, 270);
        }
        if (minigameFourInput1 == 2)
        {
            minigameFourInput1Object.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (minigameFourInput1 == 3)
        {
            minigameFourInput1Object.transform.eulerAngles = new Vector3(0, 0, 90);
        }
        if (minigameFourInput1 == 4)
        {
            minigameFourInput1Object.transform.eulerAngles = new Vector3(0, 0, 180);
        }
        if (minigameFourInput2 == 1)
        {
            minigameFourInput2Object.transform.eulerAngles = new Vector3(0, 0, 270);
        }
        if (minigameFourInput2 == 2)
        {
            minigameFourInput2Object.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (minigameFourInput2 == 3)
        {
            minigameFourInput2Object.transform.eulerAngles = new Vector3(0, 0, 90);
        }
        if (minigameFourInput2 == 4)
        {
            minigameFourInput2Object.transform.eulerAngles = new Vector3(0, 0, 180);
        }
        if (minigameFourInput3 == 1)
        {
            minigameFourInput3Object.transform.eulerAngles = new Vector3(0, 0, 270);
        }
        if (minigameFourInput3 == 2)
        {
            minigameFourInput3Object.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (minigameFourInput3 == 3)
        {
            minigameFourInput3Object.transform.eulerAngles = new Vector3(0, 0, 90);
        }
        if (minigameFourInput3 == 4)
        {
            minigameFourInput3Object.transform.eulerAngles = new Vector3(0, 0, 180);
        }
        if (minigameFourInput4 == 1)
        {
            minigameFourInput4Object.transform.eulerAngles = new Vector3(0, 0, 270);
        }
        if (minigameFourInput4 == 2)
        {
            minigameFourInput4Object.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (minigameFourInput4 == 3)
        {
            minigameFourInput4Object.transform.eulerAngles = new Vector3(0, 0, 90);
        }
        if (minigameFourInput4 == 4)
        {
            minigameFourInput4Object.transform.eulerAngles = new Vector3(0, 0, 180);
        }
        while (minigameFourProgress < 4)
        {
            yield return new WaitForSecondsRealtime(0.01f);
            if (activeSceneIs2D)
            {
                if (minigameFourProgress == 0)
                {
                    minigameFourInput1Object.SetActive(true);
                    if (minigameFourInput1 == 1)
                    {
                        while (lastHitKey == "none")
                        {
                            yield return new WaitForSecondsRealtime(0.001f);
                        }
                        if (lastHitKey == "W")
                        {
                            minigameFourProgress = 1;
                            lastHitKey = "none";
                            minigameFourInput1Object.SetActive(false);
                        }
                        else
                        {
                            minigameFourProgress = 0;
                        }
                    }
                    if (minigameFourInput1 == 2)
                    {
                        while (lastHitKey == "none")
                        {
                            yield return new WaitForSecondsRealtime(0.001f);
                        }
                        if (lastHitKey == "A")
                        {
                            minigameFourProgress = 1;
                            lastHitKey = "none";
                            minigameFourInput1Object.SetActive(false);
                        }
                        else
                        {
                            minigameFourProgress = 0;
                        }
                    }
                    if (minigameFourInput1 == 3)
                    {
                        while (lastHitKey == "none")
                        {
                            yield return new WaitForSecondsRealtime(0.001f);
                        }
                        if (lastHitKey == "S")
                        {
                            minigameFourProgress = 1;
                            lastHitKey = "none";
                            minigameFourInput1Object.SetActive(false);
                        }
                        else
                        {
                            minigameFourProgress = 0;
                        }
                    }
                    if (minigameFourInput1 == 4)
                    {
                        while (lastHitKey == "none")
                        {
                            yield return new WaitForSecondsRealtime(0.001f);
                        }
                        if (lastHitKey == "D")
                        {
                            minigameFourProgress = 1;
                            lastHitKey = "none";
                            minigameFourInput1Object.SetActive(false);
                        }
                        else
                        {
                            minigameFourProgress = 0;
                        }
                    }
                }
                if (minigameFourProgress == 1)
                {
                    minigameFourInput2Object.SetActive(true);
                    if (minigameFourInput2 == 1)
                    {
                        while (lastHitKey == "none")
                        {
                            yield return new WaitForSecondsRealtime(0.001f);
                        }
                        if (lastHitKey == "W")
                        {
                            minigameFourProgress = 2;
                            lastHitKey = "none";
                            minigameFourInput2Object.SetActive(false);
                        }
                        else
                        {
                            minigameFourProgress = 0;
                        }
                    }
                    if (minigameFourInput2 == 2)
                    {
                        while (lastHitKey == "none")
                        {
                            yield return new WaitForSecondsRealtime(0.001f);
                        }
                        if (lastHitKey == "A")
                        {
                            minigameFourProgress = 2;
                            lastHitKey = "none";
                            minigameFourInput2Object.SetActive(false);
                        }
                        else
                        {
                            minigameFourProgress = 0;
                        }
                    }
                    if (minigameFourInput2 == 3)
                    {
                        while (lastHitKey == "none")
                        {
                            yield return new WaitForSecondsRealtime(0.001f);
                        }
                        if (lastHitKey == "S")
                        {
                            minigameFourProgress = 2;
                            lastHitKey = "none";
                            minigameFourInput2Object.SetActive(false);
                        }
                        else
                        {
                            minigameFourProgress = 0;
                        }
                    }
                    if (minigameFourInput2 == 4)
                    {
                        while (lastHitKey == "none")
                        {
                            yield return new WaitForSecondsRealtime(0.001f);
                        }
                        if (lastHitKey == "D")
                        {
                            minigameFourProgress = 2;
                            lastHitKey = "none";
                            minigameFourInput2Object.SetActive(false);
                        }
                        else
                        {
                            minigameFourProgress = 0;
                        }
                    }
                }
                if (minigameFourProgress == 2)
                {
                    minigameFourInput3Object.SetActive(true);
                    if (minigameFourInput3 == 1)
                    {
                        while (lastHitKey == "none")
                        {
                            yield return new WaitForSecondsRealtime(0.001f);
                        }
                        if (lastHitKey == "W")
                        {
                            minigameFourProgress = 3;
                            lastHitKey = "none";
                            minigameFourInput3Object.SetActive(false);
                        }
                        else
                        {
                            minigameFourProgress = 0;
                        }
                    }
                    if (minigameFourInput3 == 2)
                    {
                        while (lastHitKey == "none")
                        {
                            yield return new WaitForSecondsRealtime(0.001f);
                        }
                        if (lastHitKey == "A")
                        {
                            minigameFourProgress = 3;
                            lastHitKey = "none";
                            minigameFourInput3Object.SetActive(false);
                        }
                        else
                        {
                            minigameFourProgress = 0;
                        }
                    }
                    if (minigameFourInput3 == 3)
                    {
                        while (lastHitKey == "none")
                        {
                            yield return new WaitForSecondsRealtime(0.001f);
                        }
                        if (lastHitKey == "S")
                        {
                            minigameFourProgress = 3;
                            lastHitKey = "none";
                            minigameFourInput3Object.SetActive(false);
                        }
                        else
                        {
                            minigameFourProgress = 0;
                        }
                    }
                    if (minigameFourInput3 == 4)
                    {
                        while (lastHitKey == "none")
                        {
                            yield return new WaitForSecondsRealtime(0.001f);
                        }
                        if (lastHitKey == "D")
                        {
                            minigameFourProgress = 3;
                            lastHitKey = "none";
                            minigameFourInput3Object.SetActive(false);
                        }
                        else
                        {
                            minigameFourProgress = 0;
                        }
                    }
                }
                if (minigameFourProgress == 3)
                {
                    minigameFourInput4Object.SetActive(true);
                    if (minigameFourInput4 == 1)
                    {
                        while (lastHitKey == "none")
                        {
                            yield return new WaitForSecondsRealtime(0.001f);
                        }
                        if (lastHitKey == "W")
                        {
                            minigameFourProgress = 4;
                            lastHitKey = "none";
                            minigameFourInput4Object.SetActive(false);
                        }
                        else
                        {
                            minigameFourProgress = 0;
                        }
                    }
                    if (minigameFourInput4 == 2)
                    {
                        while (lastHitKey == "none")
                        {
                            yield return new WaitForSecondsRealtime(0.001f);
                        }
                        if (lastHitKey == "A")
                        {
                            minigameFourProgress = 4;
                            lastHitKey = "none";
                            minigameFourInput4Object.SetActive(false);
                        }
                        else
                        {
                            minigameFourProgress = 0;
                        }
                    }
                    if (minigameFourInput4 == 3)
                    {
                        while (lastHitKey == "none")
                        {
                            yield return new WaitForSecondsRealtime(0.001f);
                        }
                        if (lastHitKey == "S")
                        {
                            minigameFourProgress = 4;
                            lastHitKey = "none";
                            minigameFourInput4Object.SetActive(false);
                        }
                        else
                        {
                            minigameFourProgress = 0;
                        }
                    }
                    if (minigameFourInput4 == 4)
                    {
                        while (lastHitKey == "none")
                        {
                            yield return new WaitForSecondsRealtime(0.001f);
                        }
                        if (lastHitKey == "D")
                        {
                            minigameFourProgress = 4;
                            lastHitKey = "none";
                            minigameFourInput4Object.SetActive(false);
                        }
                        else
                        {
                            minigameFourProgress = 0;
                        }
                    }
                }
                minigameFourTimer += 0.01f;
                if (minigameFourTimer > 10)
                {
                    minigameFourTimer = 0;
                    break;
                }
            }
        }
        minigameFourUI.SetActive(false);
        if (minigameFourProgress >= 4)
        {
            fishingBarProgress += 0.15f;
            batteryPercentage += 0.01f * Random.Range(1, 3);
            batteryBar.fillAmount = batteryPercentage;
            audioSource.PlayOneShot(reelIn, 1F);
            audioSource.PlayOneShot(minigameSuccess, 1F);
        }
        lastMinigamePlayed = 4;
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
            transitionOpacity += 0.04f;
        }
        transitionOpacity = 1f;
        Cursor.visible = false;
        while (transitionLerp < 1)
        {
            transition.GetComponent<UnityEngine.UI.Image>().color = Color.Lerp(Color.white, Color.black, transitionLerp);
            yield return new WaitForSecondsRealtime(0.01f);
            transitionLerp += 0.04f;
        }
        transitionLerp = 1f;
        fishingCanvasBackground.SetActive(false);
        audioSource.PlayOneShot(lightClick, 1F);
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
        if (!(!minigameThreeUI.activeSelf))
        {
            minigameThreeUI.SetActive(false);
            minigameThreeUIWasActive = true;
        }
        while (transitionOpacity > 0)
        {
            transition.GetComponent<UnityEngine.UI.Image>().color = new Vector4(0, 0, 0, transitionOpacity);
            yield return new WaitForSecondsRealtime(0.01f);
            transitionOpacity -= 0.04f;
        }
        transitionOpacity = 0f;
        transition.SetActive(false);
        transitioning = false;   
    }

    public IEnumerator TransitionForAttack()
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
            transitionOpacity += 0.04f;
        }
        transitionOpacity = 1f;
        Cursor.visible = false;
        while (transitionLerp < 1)
        {
            transition.GetComponent<UnityEngine.UI.Image>().color = Color.Lerp(Color.white, Color.black, transitionLerp);
            yield return new WaitForSecondsRealtime(0.01f);
            transitionLerp += 0.04f;
        }
        transitionLerp = 1f;
        fishingCanvasBackground.SetActive(false);
        audioSource.PlayOneShot(lightClick, 1F);
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
        if (!(!minigameThreeUI.activeSelf))
        {
            minigameThreeUI.SetActive(false);
            minigameThreeUIWasActive = true;
        }
        while (transitionOpacity > 0)
        {
            transition.GetComponent<UnityEngine.UI.Image>().color = new Vector4(0, 0, 0, transitionOpacity);
            yield return new WaitForSecondsRealtime(0.01f);
            transitionOpacity -= 0.04f;
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
        audioSource.PlayOneShot(offClick, 1F);
        while (transitionOpacity < 1)
        {
            transition.GetComponent<UnityEngine.UI.Image>().color = new Vector4(0, 0, 0, transitionOpacity);
            yield return new WaitForSecondsRealtime(0.01f);
            transitionOpacity += 0.04f;
        }
        transitionOpacity = 1f;
        while (transitionLerp > 0)
        {
            transition.GetComponent<UnityEngine.UI.Image>().color = Color.Lerp(Color.white, Color.black, transitionLerp);
            yield return new WaitForSecondsRealtime(0.01f);
            transitionLerp -= 0.04f;
        }
        transitionLerp = 0f;
        Cursor.visible = true;
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
        if (minigameThreeUIWasActive)
        {
            minigameThreeUI.SetActive(true);
            minigameThreeUIWasActive = false;
        }
        while (transitionOpacity > 0)
        {
            transition.GetComponent<UnityEngine.UI.Image>().color = new Vector4(255, 255, 255, transitionOpacity);
            yield return new WaitForSecondsRealtime(0.01f);
            transitionOpacity -= 0.04f;
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

    public IEnumerator BatteryDecrease()
    {
        if (batteryDrain == 0)
        {
            batteryDrain = 0.001f;
        }
        while (!activeSceneIs2D && !transitioning && batteryPercentage > 0)
        {
            batteryDraining = true;
            yield return new WaitForSecondsRealtime(0.1f);
            batteryPercentage -= batteryDrain;
            batteryBar.fillAmount = batteryPercentage;
            //batteryBar.GetComponent<Slider>().value = batteryPercentage / 1;
        }
        batteryDraining = false;
    }

    public void ResumeGame()
    {
        gamePaused = false;
        if (!(!fishingCanvasBackground.activeSelf))
        {
            activeSceneIs2D = true;
            Cursor.visible = true;
        }
        else
        {
            activeSceneIs2D = false;
            Cursor.visible = false;
        }
        transitioning = false;
        pauseMenu.SetActive(false);
    }

    public void BackToMenu() 
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator FishSpriteShow()
    {
        fishSprite.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);
        fishSprite.SetActive(false);
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
                StartCoroutine(FishSpriteShow());
                fishingBarProgress -= 1f;
                audioSource.PlayOneShot(fishSplash, 1F);
                if (fishCaught >= winFishAmount)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
                }
            }
        }

        if (!activeSceneIs2D && !batteryDraining)
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
