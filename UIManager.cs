using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GameSettings;
using InputKeys;
using InventoryClass;
using FileManagement;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.EventSystems;

public enum MouseMoveType {Centered, Free, FreeLimitedToWindow}

public class UIManager : MonoBehaviour
{
    public GameObject Player;
    private PlayerDriver playerPhysics;
    [Header("Mouse Settings")]
    public bool isMouseVisible;
    public MouseMoveType MouseState;

    [Header("Gameplay UI")]
    public GameObject Crosshair;
    public GameObject GameplayUI;
    public GameObject KeyActionGrid;
    public GameObject KeyActionPrefab;
    public TextMeshProUGUI FrameCounter;
    public List<GameObject> ActionsInGrid;
    public bool clearAllDisplayActions;
    public bool displayLockCameraActions;
    public bool displaySwimmingActions;
    public bool displayPreGrappleActions;
    public bool displayGrappleActions;
    public bool displayGrapplePointActions;
    public bool displayAdvancedGrappleActions;
    public bool displayInteraction;
    public bool displayGrapplePoint;
    public bool displayGrappleSwing;

    [Header("Grapple UI")]
    public GameObject GrappleIcon;

    [Header("Pause Menu")]
    public bool isPaused;
    public GameObject PauseWindow;
    public GameObject PauseMenu;
    public GameObject ResumeButton;
    public GameObject SettingsButton;
    public GameObject QuitButton;

    [Header("Settings Menu")]
    public GameObject SettingsWindow;
    public GameObject CameraSettingsWindow;
    public GameObject AudioSettingsWindow;
    public GameObject GraphicalSettingsWindow;
    public bool isListeningForKey;
    public SettingsKeyUI ListenKey;
    public GameObject KeyBindPrompt;
    public GameObject InputSettingsWindow;
    public GameObject InputGrid;
    public GameObject GameplaySettingsWindow;
    public GameObject InputBindingUI;
    public List<GameObject> InputUI;
    public List<GameObject> AllUIElements;
    public PlayerInteractor interactor;

    //Convert Manager To Singleton
    private static UIManager _instance;
    static bool _destroyed;
    public static UIManager Instance
    {
        get
        {
            // Prevent re-creation of the singleton during play mode exit.
            if (_destroyed) return null;

            // If the instance is already valid, return it. Needed if called from a
            // derived class that wishes to ensure the instance is initialized.
            if (_instance != null) return _instance;

            // Find the existing instance (across domain reloads).
            if ((_instance = FindObjectOfType<UIManager>()) != null) return _instance;

            // Create a new GameObject instance to hold the singleton component.
            var gameObject = new GameObject(typeof(UIManager).Name);

            // Move the instance to the DontDestroyOnLoad scene to prevent it from
            // being destroyed when the current scene is unloaded.
            DontDestroyOnLoad(gameObject);

            // Create the MonoBehavior component. Awake() will assign _instance.
            return gameObject.AddComponent<UIManager>();
        }
    }


    protected virtual void Awake()
    {
        Debug.Assert(_instance == null || _instance == this, "More than one singleton instance instantiated!", this);

        if (_instance == null || _instance == this)
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetCursor(MouseMoveType.Centered, false);
        playerPhysics = Player.GetComponent<PlayerDriver>();
    }

    // Update is called once per frame
    void Update()
    {
        PauseScreen();
        GrappleUI();
    }

    void LateUpdate()
    {
        BuildKeyBindings();
        IsListeningForNewKey();
        SaveGame();
        LoadCurrentSettings();
        SpawnKeyActions();
        FrameCount();
        CrosshairUI();
    }

    private void CrosshairUI()
    {
        if(Player.GetComponent<PlayerSettings>().gameplaySettings.Mode.Equals(CameraMode.FPSMode))
        {
            Crosshair.SetActive(true);
        }
        else
        {
            Crosshair.SetActive(false);
        }
    }

    private void FrameCount()
    {
        if(FrameCounter != null)
        {
            FrameCounter.text = "Frames: " + Player.GetComponent<FpsCatcher>().m_lastFramerate;
        }
    }

    private void SpawnKeyActions()
    {
        if(ActionsInGrid != null && !interactor.interactable && !interactor.grappleSwing && !interactor.grapplePoint && !playerPhysics.physicsProperties.isSwimming && !playerPhysics.physicsProperties.swingMode && !playerPhysics.physicsProperties.holdMode && !playerPhysics.physicsProperties.PreGrappling && !playerPhysics.MyCamera.isLockedOn && !playerPhysics|| DialogueManager.Instance.isDialoguePlaying)
        {
            clearAllDisplayActions = true;
        }
        
        if(clearAllDisplayActions)
        {
            foreach (GameObject obj in ActionsInGrid)
            {
                Destroy(obj);
            }

            ActionsInGrid.Clear();

            clearAllDisplayActions = false;
            print("Cleared All UI Actions");
        }

        if(displayInteraction && Player.GetComponent<PlayerInteractor>().interactable)
        {
            foreach (GameObject obj in ActionsInGrid)
            {
                Destroy(obj);
            }

            ActionsInGrid.Clear();

            GameObject Interact = Instantiate(KeyActionPrefab, KeyActionGrid.transform);

            KeyCode Key0 = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.InteractIndex].key;
            Interact.GetComponentInChildren<TextMeshProUGUI>().text = "[" + Key0.ToString() + "]" + " Read / Speak";

            ActionsInGrid.Add(Interact);

            displayInteraction = false;
        }

        if (displayGrapplePoint && Player.GetComponent<PlayerInteractor>().grapplePoint && !Player.GetComponent<PlayerDriver>().physicsProperties.holdMode && !Player.GetComponent<PlayerDriver>().physicsProperties.pullingToGrapplePoint || displayGrappleSwing && Player.GetComponent<PlayerInteractor>().grappleSwing && !Player.GetComponent<PlayerDriver>().physicsProperties.swingMode && !Player.GetComponent<PlayerDriver>().physicsProperties.pullingToSwingPoint)
        {
            foreach (GameObject obj in ActionsInGrid)
            {
                Destroy(obj);
            }

            ActionsInGrid.Clear();

            GameObject Interact = Instantiate(KeyActionPrefab, KeyActionGrid.transform);

            KeyCode Key0 = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.GrappleIndex].key;
            Interact.GetComponentInChildren<TextMeshProUGUI>().text = "[" + Key0.ToString() + "]" + " Grapple";

            ActionsInGrid.Add(Interact);

            displayGrapplePoint = false;
            displayGrappleSwing = false;
        }

        if (displayGrapplePointActions)
        {
            foreach (GameObject obj in ActionsInGrid)
            {
                Destroy(obj);
            }

            ActionsInGrid.Clear();

            GameObject Disengage = Instantiate(KeyActionPrefab, KeyActionGrid.transform);

            KeyCode Key0 = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.JumpIndex].key;
            Disengage.GetComponentInChildren<TextMeshProUGUI>().text = "[" + Key0.ToString() + "]" + " Drop / Disengage";

            ActionsInGrid.Add(Disengage);

            displayGrapplePointActions = false;
        }
        
        if(!displayLockCameraActions && displaySwimmingActions)
        {
            foreach (GameObject obj in ActionsInGrid)
            {
                Destroy(obj);
            }

            ActionsInGrid.Clear();

            GameObject SwimmingSprintMovement = Instantiate(KeyActionPrefab, KeyActionGrid.transform);
            GameObject Dive = Instantiate(KeyActionPrefab, KeyActionGrid.transform);
            GameObject Ascend = Instantiate(KeyActionPrefab, KeyActionGrid.transform);

            KeyCode Key0 = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.DodgeIndex].key;
            SwimmingSprintMovement.GetComponentInChildren<TextMeshProUGUI>().text = "[" + Key0.ToString() + "]" + " Swim Faster";

            KeyCode Key1 = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.CrouchIndex].key;
            Dive.GetComponentInChildren<TextMeshProUGUI>().text = "[" + Key1.ToString() + "]" + " Dive";

            KeyCode Key2 = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.JumpIndex].key;
            Ascend.GetComponentInChildren<TextMeshProUGUI>().text = "[" + Key2.ToString() + "]" + " Surface";

            ActionsInGrid.Add(SwimmingSprintMovement);
            ActionsInGrid.Add(Dive);
            ActionsInGrid.Add(Ascend);

            displaySwimmingActions = false;
        }

        if (displayPreGrappleActions)
        {
            foreach (GameObject obj in ActionsInGrid)
            {
                Destroy(obj);
            }

            ActionsInGrid.Clear();

            GameObject CancelHook = Instantiate(KeyActionPrefab, KeyActionGrid.transform);
            GameObject Hook = Instantiate(KeyActionPrefab, KeyActionGrid.transform);

            KeyCode Key0 = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.AimDownSightsIndex].key;
            CancelHook.GetComponentInChildren<TextMeshProUGUI>().text = "[" + Key0.ToString() + "]" + " Cancel Hook";

            KeyCode Key1 = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.GrappleIndex].key;
            Hook.GetComponentInChildren<TextMeshProUGUI>().text = "[" + Key1.ToString() + "]" + " Release To Grapple";

            ActionsInGrid.Add(CancelHook);
            ActionsInGrid.Add(Hook);

            displayPreGrappleActions = false;
        }

        if (displayGrappleActions)
        {
            print("hit");
            foreach (GameObject obj in ActionsInGrid)
            {
                Destroy(obj);
            }

            ActionsInGrid.Clear();

            GameObject Swing = Instantiate(KeyActionPrefab, KeyActionGrid.transform);
            GameObject Jump = Instantiate(KeyActionPrefab, KeyActionGrid.transform);
            GameObject SlowDown = Instantiate(KeyActionPrefab, KeyActionGrid.transform);

            KeyCode Key0 = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.WalkForwardIndex].key;
            KeyCode Key1 = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.WalkBackwardsIndex].key;
            Swing.GetComponentInChildren<TextMeshProUGUI>().text = "[" + Key0.ToString() + " / " + Key1.ToString() + "]" + " Swing";

            KeyCode Key2 = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.JumpIndex].key;
            Jump.GetComponentInChildren<TextMeshProUGUI>().text = "[" + Key2.ToString() + "]" + " Jump";

            KeyCode Key3 = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.AimDownSightsIndex].key;
            SlowDown.GetComponentInChildren<TextMeshProUGUI>().text = "[" + Key3.ToString() + "]" + " Slow Down / Halt";

            ActionsInGrid.Add(Swing);
            ActionsInGrid.Add(Jump);
            ActionsInGrid.Add(SlowDown);

            displayGrappleActions = false;
        }

        if (displayAdvancedGrappleActions)
        {
            foreach (GameObject obj in ActionsInGrid)
            {
                Destroy(obj);
            }

            ActionsInGrid.Clear();

            GameObject ReelUp = Instantiate(KeyActionPrefab, KeyActionGrid.transform);
            GameObject ReelDown = Instantiate(KeyActionPrefab, KeyActionGrid.transform);

            KeyCode Key0 = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.WalkForwardIndex].key;
            KeyCode Key1 = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.WalkBackwardsIndex].key;
            ReelUp.GetComponentInChildren<TextMeshProUGUI>().text = "[" + Key0.ToString() + "]" + " Reel Up";
            ReelDown.GetComponentInChildren<TextMeshProUGUI>().text = "[" + Key1.ToString() + "]" + " Reel Down";

            ActionsInGrid.Add(ReelUp);
            ActionsInGrid.Add(ReelDown);

            displayAdvancedGrappleActions = false;
        }

        if(displayLockCameraActions)
        {
            KeyCode Key0 = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.ResetCameraIndex].key;

            if (ActionsInGrid == null || ActionsInGrid != null && ActionsInGrid[0].GetComponentInChildren<TextMeshProUGUI>().text == "[" + Key0.ToString() + "]" + " Reset Camera Angle")
            {
                foreach (GameObject obj in ActionsInGrid)
                {
                    Destroy(obj);
                }

                ActionsInGrid.Clear();

                GameObject ResetCameraAngle = Instantiate(KeyActionPrefab, KeyActionGrid.transform);
                ResetCameraAngle.GetComponentInChildren<TextMeshProUGUI>().text = "[" + Key0.ToString() + "]" + " Reset Camera Angle";

                ActionsInGrid.Add(ResetCameraAngle);
            }

            displayLockCameraActions = false;
        }
    }

    private void GrappleUI()
    {
        if (GrappleIcon != null)
        {
            if (playerPhysics.physicsProperties.onGrappleScreen && !playerPhysics.physicsProperties.swingMode && playerPhysics.physicsProperties.inSwingRange || playerPhysics.physicsProperties.inPointRange)
            {
                GrappleIcon.SetActive(true);
            }
            else
            {
                GrappleIcon.SetActive(false);
            }
        }

        if (playerPhysics.physicsProperties.onGrappleScreen && playerPhysics.physicsProperties.inSwingRange && playerPhysics.physicsProperties.GrappleSwingObject != null)
        {
            if(GrappleIcon != null)
            {
                GrappleIcon.GetComponent<RectTransform>().position = PlayerSettings.Instance.gameplaySettings.WorldCamera.WorldToScreenPoint(playerPhysics.physicsProperties.GrappleSwingObject.transform.position + new Vector3(0,3f,0));
            }
        }

        if(playerPhysics.physicsProperties.inPointRange)
        {
            if (GrappleIcon != null)
            {
                GrappleIcon.GetComponent<RectTransform>().position = PlayerSettings.Instance.gameplaySettings.WorldCamera.WorldToScreenPoint(playerPhysics.physicsProperties.GrapplePointObject.transform.position + new Vector3(0, 3f, 0));
            }
        }
    }

    #region UniversalCalls

    public void SetWindowVisabillity(GameObject Window, bool state)
    {
        Window.SetActive(state);
    }
    public void SetCursor(MouseMoveType CursorType, bool visable)
    {
        isMouseVisible = visable;
        MouseState = CursorType;

        if (MouseState.Equals(MouseMoveType.Centered))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (MouseState.Equals(MouseMoveType.Free))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (MouseState.Equals(MouseMoveType.FreeLimitedToWindow))
        {
            Cursor.lockState = CursorLockMode.Confined;
        }

        Cursor.visible = isMouseVisible;
    }
    #endregion

    #region PauseScreen

    public void ClearConflictKey(int index)
    {
        foreach (GameObject UI in InputUI)
        {
            if (UI.GetComponent<SettingsKeyUI>().KeyIndex == index)
            {
                UI.GetComponent<SettingsKeyUI>().KeyText.text = KeyCode.None.ToString();
                return;
            }
        }
    }

    public void IsListeningForNewKey()
    {
        if (isListeningForKey && isPaused && ListenKey != null)
        {
            KeyBindPrompt.SetActive(true);

            foreach (KeyCode newKey in System.Enum.GetValues(typeof(KeyCode)))
            {
                InputManager.Instance.SetNewInput(ListenKey.KeyIndex, InputManager.Instance.PlayerInput.Inputs[ListenKey.KeyIndex]);
                ListenKey.KeyText.text = InputManager.Instance.PlayerInput.Inputs[ListenKey.KeyIndex].key.ToString();
                GameObject EventSystem = GameObject.Find("EventSystem");

                EventSystem.GetComponent<EventSystem>().SetSelectedGameObject(null);
            }
        }
        else
        {
            if (KeyBindPrompt.gameObject.activeSelf && !isListeningForKey)
            {
                KeyBindPrompt.SetActive(false);
            }
        }
    }

    private bool buiidKeys = true;
    private bool defaultKeys = false;
    public void BuildKeyBindings()
    {
        if (buiidKeys && isPaused)
        {
            if (InputUI.Count > 0)
            {
                foreach (GameObject UI in InputUI)
                {
                    Destroy(UI);
                }

                InputUI.Clear();
            }

            for (int i = 0; i < InputManager.Instance.PlayerInput.Inputs.Count; i++)
            {
                GameObject Binding = Instantiate(InputBindingUI, InputGrid.transform);

                RegisterBinding(Binding);

                if (Binding.GetComponent<SettingsKeyUI>() != null && !defaultKeys)
                {
                    Binding.GetComponent<SettingsKeyUI>().KeyIndex = i;
                    Binding.GetComponent<SettingsKeyUI>().Text.text = InputManager.Instance.PlayerInput.Inputs[i].ActionName.ToString();
                    Binding.GetComponent<SettingsKeyUI>().KeyText.text = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();
                    Binding.name = InputManager.Instance.PlayerInput.Inputs[i].ActionName.ToString();
                }

                if (Binding.GetComponent<SettingsKeyUI>() != null && defaultKeys)
                {
                    Binding.GetComponent<SettingsKeyUI>().KeyIndex = i;
                    Binding.GetComponent<SettingsKeyUI>().Text.text = InputManager.Instance.PlayerInput.DefaultInputs[i].ActionName.ToString();
                    Binding.GetComponent<SettingsKeyUI>().KeyText.text = InputManager.Instance.PlayerInput.DefaultInputs[i].key.ToString();
                    Binding.name = InputManager.Instance.PlayerInput.Inputs[i].ActionName.ToString();
                }
            }

            buiidKeys = false;
            defaultKeys = false;
        }
    }
    public void RegisterUIElement(GameObject UIElement)
    {
        if (!AllUIElements.Contains(UIElement))
        {
            AllUIElements.Add(UIElement);
        }
    }
    public void RegisterBinding(GameObject UIElement)
    {
        if (!InputUI.Contains(UIElement))
        {
            InputUI.Add(UIElement);
        }
    }
    private void PauseScreen()
    {
        KeyCode Pause = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.EscapeMenuIndex].key;

        if (Input.GetKeyDown(Pause) && !SettingsWindow.activeSelf)
        {
            isPaused = !isPaused;
            PauseWindow.SetActive(isPaused);
            PauseMenu.SetActive(isPaused);

            if (isPaused)
            {
                SetCursor(MouseMoveType.FreeLimitedToWindow, true);
            }
            else
            {
                SetCursor(MouseMoveType.Centered, false);
            }
        }

        if(isPaused)
        {
            Time.timeScale = 0;

            if(SettingsWindow.activeSelf && Input.GetKeyDown(Pause) && !isListeningForKey)
            {
                ExitSettingsWithoutSaving();
            }
        }
        else
        {
            Time.timeScale = 1;
        }
    }
    private bool loadCameraScreenFirst = false;
    //You need to call a coroutine to update dropdown BIG NOTE!!!!!
    private void LoadCurrentSettings()
    {
        if (loadSettings && isPaused)
        {
            CameraSettingsWindow.SetActive(true);
            GameplaySettingsWindow.SetActive(true);
            InputSettingsWindow.SetActive(true);
            AudioSettingsWindow.SetActive(true);
            GraphicalSettingsWindow.SetActive(true);

            foreach(GameObject UI in AllUIElements)
            {
                if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Max View Distance")
                {
                    UI.GetComponent<UISettingElement>().Element.Slide.value = PlayerSettings.Instance.gameplaySettings.maxViewDistance;
                }

                if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Field Of View")
                {
                    UI.GetComponent<UISettingElement>().Element.Slide.value = PlayerSettings.Instance.gameplaySettings.FieldOfView;
                }

                if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Sensitivity")
                {
                    UI.GetComponent<UISettingElement>().Element.Slide.value = PlayerSettings.Instance.gameplaySettings.sensitivity;
                }

                if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Zoom Sensitivity")
                {
                    UI.GetComponent<UISettingElement>().Element.Slide.value = PlayerSettings.Instance.gameplaySettings.zoomSensitivity;
                }

                if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Invert X Axis")
                {
                    UI.GetComponentInChildren<Toggle>().isOn = PlayerSettings.Instance.gameplaySettings.InvertX;
                }

                if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Invert Y Axis")
                {
                    UI.GetComponentInChildren<Toggle>().isOn = PlayerSettings.Instance.gameplaySettings.InvertY;
                }

                if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Master Volume")
                {
                    UI.GetComponent<UISettingElement>().Element.Slide.value = PlayerSettings.Instance.AudioSet.MasterVolume;
                }

                if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Music Volume")
                {
                    UI.GetComponent<UISettingElement>().Element.Slide.value = PlayerSettings.Instance.AudioSet.MusicVolume;
                }

                if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Sound Effects Volume")
                {
                    UI.GetComponent<UISettingElement>().Element.Slide.value = PlayerSettings.Instance.AudioSet.SoundEffectsVolume;
                }

                if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Ambient Volume")
                {
                    UI.GetComponent<UISettingElement>().Element.Slide.value = PlayerSettings.Instance.AudioSet.AmbientSoundVolume;
                }

                if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Audio Mode")
                {
                    if(PlayerSettings.Instance.AudioSet.Mode == AudioMode.Stereo)
                    {
                        StartCoroutine(ChangeValue(0, UI.GetComponent<UISettingElement>().Element.List));
                    }

                    if (PlayerSettings.Instance.AudioSet.Mode == AudioMode.Mono)
                    {
                        StartCoroutine(ChangeValue(1, UI.GetComponent<UISettingElement>().Element.List));
                    }
                }

                if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Window Type")
                {
                    if(PlayerSettings.Instance.GraphicsSet.WindowStyle == WindowType.Windowed)
                    {
                        StartCoroutine(ChangeValue(0, UI.GetComponent<UISettingElement>().Element.List));
                    }

                    if (PlayerSettings.Instance.GraphicsSet.WindowStyle == WindowType.Borderless)
                    {
                        StartCoroutine(ChangeValue(1, UI.GetComponent<UISettingElement>().Element.List));
                    }

                    if (PlayerSettings.Instance.GraphicsSet.WindowStyle == WindowType.Fullscreen)
                    {
                        StartCoroutine(ChangeValue(2, UI.GetComponent<UISettingElement>().Element.List));
                    }
                }

                if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Window Scale")
                {
                    if(PlayerSettings.Instance.GraphicsSet.WindowScale == WindowSize.NativeResolution)
                    {
                        StartCoroutine(ChangeValue(0, UI.GetComponent<UISettingElement>().Element.List));
                    }

                    if (PlayerSettings.Instance.GraphicsSet.WindowScale == WindowSize.SD)
                    {
                        StartCoroutine(ChangeValue(1, UI.GetComponent<UISettingElement>().Element.List));
                    }

                    if (PlayerSettings.Instance.GraphicsSet.WindowScale == WindowSize.HD)
                    {
                        StartCoroutine(ChangeValue(2, UI.GetComponent<UISettingElement>().Element.List));
                    }

                    if (PlayerSettings.Instance.GraphicsSet.WindowScale == WindowSize.FullHD)
                    {
                        StartCoroutine(ChangeValue(3, UI.GetComponent<UISettingElement>().Element.List));
                    }

                    if (PlayerSettings.Instance.GraphicsSet.WindowScale == WindowSize.QHD)
                    {
                        StartCoroutine(ChangeValue(4, UI.GetComponent<UISettingElement>().Element.List));
                    }

                    if (PlayerSettings.Instance.GraphicsSet.WindowScale == WindowSize.FullUltraHD)
                    {
                        StartCoroutine(ChangeValue(5, UI.GetComponent<UISettingElement>().Element.List));
                    }

                    if (PlayerSettings.Instance.GraphicsSet.WindowScale == WindowSize.UHD)
                    {
                        StartCoroutine(ChangeValue(6, UI.GetComponent<UISettingElement>().Element.List));
                    }
                }

                if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Shadow Resolution")
                {
                    if(PlayerSettings.Instance.GraphicsSet.ShadowQuality == ShadowResolution.Off)
                    {
                        StartCoroutine(ChangeValue(0, UI.GetComponent<UISettingElement>().Element.List));
                    }

                    if (PlayerSettings.Instance.GraphicsSet.ShadowQuality == ShadowResolution.Low)
                    {
                        StartCoroutine(ChangeValue(1, UI.GetComponent<UISettingElement>().Element.List));
                    }

                    if (PlayerSettings.Instance.GraphicsSet.ShadowQuality == ShadowResolution.Medium)
                    {
                        StartCoroutine(ChangeValue(2, UI.GetComponent<UISettingElement>().Element.List));
                    }

                    if (PlayerSettings.Instance.GraphicsSet.ShadowQuality == ShadowResolution.High)
                    {
                        StartCoroutine(ChangeValue(3, UI.GetComponent<UISettingElement>().Element.List));
                    }

                    if (PlayerSettings.Instance.GraphicsSet.ShadowQuality == ShadowResolution.Ultra)
                    {
                        StartCoroutine(ChangeValue(4, UI.GetComponent<UISettingElement>().Element.List));
                    }
                }

                if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Dynamic Shadows")
                {
                    if (PlayerSettings.Instance.GraphicsSet.useDynamicShadows)
                    {
                        StartCoroutine(ChangeValue(1, UI.GetComponent<UISettingElement>().Element.List));
                    }
                    else
                    {
                        StartCoroutine(ChangeValue(0, UI.GetComponent<UISettingElement>().Element.List));
                    }
                }

                if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Particle Effects")
                {
                    if (PlayerSettings.Instance.GraphicsSet.ParticleQuality == ParticleEffectGraphics.Off)
                    {
                        StartCoroutine(ChangeValue(0, UI.GetComponent<UISettingElement>().Element.List));
                    }

                    if (PlayerSettings.Instance.GraphicsSet.ParticleQuality == ParticleEffectGraphics.Low)
                    {
                        StartCoroutine(ChangeValue(1, UI.GetComponent<UISettingElement>().Element.List));
                    }

                    if (PlayerSettings.Instance.GraphicsSet.ParticleQuality == ParticleEffectGraphics.Medium)
                    {
                        StartCoroutine(ChangeValue(2, UI.GetComponent<UISettingElement>().Element.List));
                    }

                    if (PlayerSettings.Instance.GraphicsSet.ParticleQuality == ParticleEffectGraphics.High)
                    {
                        StartCoroutine(ChangeValue(3, UI.GetComponent<UISettingElement>().Element.List));
                    }

                    if (PlayerSettings.Instance.GraphicsSet.ParticleQuality == ParticleEffectGraphics.Ultra)
                    {
                        StartCoroutine(ChangeValue(4, UI.GetComponent<UISettingElement>().Element.List));
                    }
                }

                if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Bloom")
                {
                    if (PlayerSettings.Instance.GraphicsSet.useBloom)
                    {
                        StartCoroutine(ChangeValue(1, UI.GetComponent<UISettingElement>().Element.List));
                    }
                    else
                    {
                        StartCoroutine(ChangeValue(0, UI.GetComponent<UISettingElement>().Element.List));
                    }
                }

                if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Blur")
                {
                    if (PlayerSettings.Instance.GraphicsSet.useBlur)
                    {
                        StartCoroutine(ChangeValue(1, UI.GetComponent<UISettingElement>().Element.List));
                    }
                    else
                    {
                        StartCoroutine(ChangeValue(0, UI.GetComponent<UISettingElement>().Element.List));
                    }
                }

                if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Vignette")
                {
                    if (PlayerSettings.Instance.GraphicsSet.useVignette)
                    {
                        StartCoroutine(ChangeValue(1, UI.GetComponent<UISettingElement>().Element.List));
                    }
                    else
                    {
                        StartCoroutine(ChangeValue(0, UI.GetComponent<UISettingElement>().Element.List));
                    }
                }

                if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Z-Target Mode")
                {
                    if (PlayerSettings.Instance.gameplaySettings.ZTargetType == ZTargetMode.Hold)
                    {
                        StartCoroutine(ChangeValue(0, UI.GetComponent<UISettingElement>().Element.List));
                    }

                    if (PlayerSettings.Instance.gameplaySettings.ZTargetType == ZTargetMode.Toggle)
                    {
                        StartCoroutine(ChangeValue(1, UI.GetComponent<UISettingElement>().Element.List));
                    }
                }

                if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Crouch Mode")
                {
                    if (PlayerSettings.Instance.gameplaySettings.CrouchType == CrouchMode.Hold)
                    {
                        StartCoroutine(ChangeValue(0, UI.GetComponent<UISettingElement>().Element.List));
                    }

                    if (PlayerSettings.Instance.gameplaySettings.CrouchType == CrouchMode.Toggle)
                    {
                        StartCoroutine(ChangeValue(1, UI.GetComponent<UISettingElement>().Element.List));
                    }
                }
            }

            loadCameraScreenFirst = true;
            loadSettings = false;
        }

        if(loadCameraScreenFirst && isPaused)
        {
            ShowSettingsWindow(CameraSettingsWindow);
            loadCameraScreenFirst = false;
        }
    }

    IEnumerator ChangeValue(int newValue, TMP_Dropdown dropdown)
    {
        dropdown.Select();
        yield return new WaitForEndOfFrame();
        dropdown.value = newValue;
        dropdown.RefreshShownValue();
    }
    //Enum Lists Have An Issue With Options Resetting After Saving
    private void SetCurrentSettings()
    {
        foreach (GameObject UI in AllUIElements)
        {
            if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Max View Distance")
            {
                PlayerSettings.Instance.gameplaySettings.maxViewDistance = Single.Parse(UI.GetComponent<UISettingElement>().Element.DisplayResult.text.ToString());
            }

            if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Field Of View")
            {
                PlayerSettings.Instance.gameplaySettings.FieldOfView = Single.Parse(UI.GetComponent<UISettingElement>().Element.DisplayResult.text.ToString());
            }

            if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Sensitivity")
            {
                PlayerSettings.Instance.gameplaySettings.sensitivity = Single.Parse(UI.GetComponent<UISettingElement>().Element.DisplayResult.text.ToString());
            }

            if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Zoom Sensitivity")
            {
                PlayerSettings.Instance.gameplaySettings.zoomSensitivity = Single.Parse(UI.GetComponent<UISettingElement>().Element.DisplayResult.text.ToString());
            }

            if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Invert X Axis")
            {
                PlayerSettings.Instance.gameplaySettings.InvertX = UI.GetComponentInChildren<Toggle>().isOn;
            }

            if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Invert Y Axis")
            {
                PlayerSettings.Instance.gameplaySettings.InvertY = UI.GetComponentInChildren<Toggle>().isOn;
            }

            if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Master Volume")
            {
                PlayerSettings.Instance.AudioSet.MasterVolume = Single.Parse(UI.GetComponent<UISettingElement>().Element.DisplayResult.text.ToString());
            }

            if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Music Volume")
            {
                PlayerSettings.Instance.AudioSet.MusicVolume = Single.Parse(UI.GetComponent<UISettingElement>().Element.DisplayResult.text.ToString());
            }

            if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Sound Effects Volume")
            {
                PlayerSettings.Instance.AudioSet.SoundEffectsVolume = Single.Parse(UI.GetComponent<UISettingElement>().Element.DisplayResult.text.ToString());
            }

            if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Ambient Volume")
            {
                PlayerSettings.Instance.AudioSet.AmbientSoundVolume = Single.Parse(UI.GetComponent<UISettingElement>().Element.DisplayResult.text.ToString());
            }

            if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Audio Mode")
            {
                if (Enum.TryParse<AudioMode>(UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text, out AudioMode yourEnum))
                {
                    AudioMode parsed_enum = (AudioMode)System.Enum.Parse(typeof(AudioMode), UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text);
                    PlayerSettings.Instance.AudioSet.Mode = parsed_enum;
                }
            }

            if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Window Type")
            {
                if (Enum.TryParse<WindowType>(UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text, out WindowType yourEnum))
                {
                    WindowType parsed_enum = (WindowType)System.Enum.Parse(typeof(WindowType), UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text);
                    PlayerSettings.Instance.GraphicsSet.WindowStyle = parsed_enum;
                }
            }

            if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Window Scale")
            {
                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "Native Resolution")
                {
                    PlayerSettings.Instance.GraphicsSet.WindowScale = WindowSize.NativeResolution;
                }

                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "640 x 480")
                {
                    PlayerSettings.Instance.GraphicsSet.WindowScale = WindowSize.SD;
                }

                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "1280 x 720")
                {
                    PlayerSettings.Instance.GraphicsSet.WindowScale = WindowSize.HD;
                }

                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "1920 x 1080")
                {
                    PlayerSettings.Instance.GraphicsSet.WindowScale = WindowSize.FullHD;
                }

                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "2560 x 1440")
                {
                    PlayerSettings.Instance.GraphicsSet.WindowScale = WindowSize.QHD;
                }

                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "3480 x 2160")
                {
                    PlayerSettings.Instance.GraphicsSet.WindowScale = WindowSize.FullUltraHD;
                }

                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "7680 x 4320")
                {
                    PlayerSettings.Instance.GraphicsSet.WindowScale = WindowSize.UHD;
                }
            }

            if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Shadow Resolution")
            {
                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "Off")
                {
                    PlayerSettings.Instance.GraphicsSet.ShadowQuality = ShadowResolution.Off;
                }

                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "Low")
                {
                    PlayerSettings.Instance.GraphicsSet.ShadowQuality = ShadowResolution.Low;
                }

                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "Medium")
                {
                    PlayerSettings.Instance.GraphicsSet.ShadowQuality = ShadowResolution.Medium;
                }

                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "High")
                {
                    PlayerSettings.Instance.GraphicsSet.ShadowQuality = ShadowResolution.High;
                }

                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "Ultra")
                {
                    PlayerSettings.Instance.GraphicsSet.ShadowQuality = ShadowResolution.Ultra;
                }
            }

            if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Dynamic Shadows")
            {
                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "On")
                {
                    PlayerSettings.Instance.GraphicsSet.useDynamicShadows = true;
                }

                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "Off")
                {
                    PlayerSettings.Instance.GraphicsSet.useDynamicShadows = false;
                }
            }

            if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Particle Effects")
            {
                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "Off")
                {
                    PlayerSettings.Instance.GraphicsSet.ParticleQuality = ParticleEffectGraphics.Off;
                }

                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "Low")
                {
                    PlayerSettings.Instance.GraphicsSet.ParticleQuality = ParticleEffectGraphics.Low;
                }

                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "Medium")
                {
                    PlayerSettings.Instance.GraphicsSet.ParticleQuality = ParticleEffectGraphics.Medium;
                }

                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "High")
                {
                    PlayerSettings.Instance.GraphicsSet.ParticleQuality = ParticleEffectGraphics.High;
                }

                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "Ultra")
                {
                    PlayerSettings.Instance.GraphicsSet.ParticleQuality = ParticleEffectGraphics.Ultra;
                }
            }

            if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Bloom")
            {
                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "On")
                {
                    PlayerSettings.Instance.GraphicsSet.useBloom = true;
                }

                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "Off")
                {
                    PlayerSettings.Instance.GraphicsSet.useBloom = false;
                }
            }

            if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Blur")
            {
                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "On")
                {
                    PlayerSettings.Instance.GraphicsSet.useBlur = true;
                }

                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "Off")
                {
                    PlayerSettings.Instance.GraphicsSet.useBlur = false;
                }
            }

            if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Vignette")
            {
                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "On")
                {
                    PlayerSettings.Instance.GraphicsSet.useVignette = true;
                }

                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "Off")
                {
                    PlayerSettings.Instance.GraphicsSet.useVignette = false;
                }
            }

            if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Z-Target Mode")
            {
                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "Toggle")
                {
                    PlayerSettings.Instance.gameplaySettings.ZTargetType = ZTargetMode.Toggle;
                }

                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "Hold")
                {
                    PlayerSettings.Instance.gameplaySettings.ZTargetType = ZTargetMode.Hold;
                }
            }

            if (UI.GetComponent<UISettingElement>().Element.UIText.text == "Crouch Mode")
            {
                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "Toggle")
                {
                    PlayerSettings.Instance.gameplaySettings.CrouchType = CrouchMode.Toggle;
                }

                if (UI.GetComponent<UISettingElement>().Element.List.options[UI.GetComponent<UISettingElement>().Element.List.value].text == "Hold")
                {
                    PlayerSettings.Instance.gameplaySettings.CrouchType = CrouchMode.Hold;
                }
            }
        }
    }

    private bool loadSettings = true;
    public void ShowSettingsWindow(GameObject ExposedWindow)
    {
        //First Hide All Windows
        SetWindowVisabillity(CameraSettingsWindow, false);
        SetWindowVisabillity(GameplaySettingsWindow, false);
        SetWindowVisabillity(InputSettingsWindow, false);
        SetWindowVisabillity(AudioSettingsWindow, false);
        SetWindowVisabillity(GraphicalSettingsWindow, false);

        //Show The New Window
        SetWindowVisabillity(ExposedWindow, true);

        if(ExposedWindow.Equals(InputSettingsWindow))
        {
            buiidKeys = true;
        }
    }
    public void ResumeFromPauseScreen()
    {
        isPaused = false;
        SetCursor(MouseMoveType.Centered, false);
        SetWindowVisabillity(PauseWindow, isPaused);
        SetWindowVisabillity(PauseMenu, true);
    }
    public void SettingsFromPauseScreen()
    {
        //Load Settings
        loadSettings = true;

        SetWindowVisabillity(SettingsWindow, true);
    }
    public void ReturnToPauseScreen()
    {
        SetWindowVisabillity(SettingsWindow, false);
    }
    public void SaveSettingsAndReturn()
    {
        //First Hide All Windows Other Then The Camera Settings
        ShowSettingsWindow(CameraSettingsWindow);

        //Change Settings First On The Player Settings Scripts 
        SetCurrentSettings();

        //Save Set Settings
        SaveGameSettings();
        ReturnToPauseScreen();
    }
    public void ExitSettingsWithoutSaving()
    {
        //First Hide All Windows Other Then The Camera Settings
        ShowSettingsWindow(CameraSettingsWindow);
        ReturnToPauseScreen();
    }

    public void ResetKeyBindings()
    {
        defaultKeys = true;
        buiidKeys = true;
    }

    private bool save = false;
    public void SaveGameSettings()
    {
        save = true;
    }
    private void SaveGame()
    {
        if (save)
        {
            NativeSaveSystem.PerformPartialGameSave();
            save = false;
        }
    }
    public void QuitFromPauseScreen()
    {
        NativeSaveSystem.PerformFullGameSave();
        Application.Quit();
    }
    #endregion

    protected virtual void OnEnable()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }

#if UNITY_EDITOR
    // Called when entering or exiting play mode.
    static void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange stateChange)
    {
        // Reset static _destroyed field. Required when domain reloads are disabled.
        // Note: ExitingPlayMode is called too early.
        if (stateChange == UnityEditor.PlayModeStateChange.EnteredEditMode)
        {
            UnityEditor.EditorApplication.playModeStateChanged -=
                OnPlayModeStateChanged;
            _destroyed = false;
        }
    }
#endif
}
