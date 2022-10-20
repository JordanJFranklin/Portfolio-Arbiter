using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InputKeys;
using FileManagement;
using System.IO;
using System;
using DialogueClass;
using Ink.Runtime;

public class InputManager : MonoBehaviour
{
    public InputScheme PlayerInput;
    public bool updateKeys = true;
    public bool debugKeys;
    public string path;
    
    //Convert Manager To Singleton
    private static InputManager _instance;
    static bool _destroyed;
    public static InputManager Instance
    {
        get
        {
            // Prevent re-creation of the singleton during play mode exit.
            if (_destroyed) return null;

            // If the instance is already valid, return it. Needed if called from a
            // derived class that wishes to ensure the instance is initialized.
            if (_instance != null) return _instance;

            // Find the existing instance (across domain reloads).
            if ((_instance = FindObjectOfType<InputManager>()) != null) return _instance;

            // Create a new GameObject instance to hold the singleton component.
            var gameObject = new GameObject(typeof(InputManager).Name);

            // Move the instance to the DontDestroyOnLoad scene to prevent it from
            // being destroyed when the current scene is unloaded.
            DontDestroyOnLoad(gameObject);

            // Create the MonoBehavior component. Awake() will assign _instance.
            return gameObject.AddComponent<InputManager>();
        }
    }


    protected virtual void Awake()
    {
        Debug.Assert(_instance == null || _instance == this, "More than one singleton instance instantiated!", this);

        if (_instance == null || _instance == this)
        {
            _instance = this;
        }

        path = Application.dataPath + "/ArbiterInputs.txt";
    }

    void Start()
    {
        LoadKeyScheme();
        UpdateIndexes();
    }

    // Update is called once per frame
    void Update()
    {
        DebugKeys();
        KeyMovement();
        UpdateIndexes();

        if(searchForConflict)
        {
            CheckForInvalidInputs(conflictKey, conflictIndex);
        }
    }

    void LoadKeyScheme()
    {
        //Check For File
        if (File.Exists(path))
        {
            print("Input File Found. Loading In KeyInputs From File");

            // Open the file to read from.
            string[] readText = File.ReadAllLines(path);

            //Check If the document Empty
            if (new FileInfo(path).Length == 0)
            {
                //File Not Found
                print("WARNING: Input File Was Determined To Be Empty. Resetting File With Default Inputs...");

                //Create Text File To Path
                File.WriteAllText(path, string.Empty);

                //Add Text To it
                foreach (InputKey Key in PlayerInput.DefaultInputs)
                {
                    string keycode = Key.BoundAction.ToString() + "  [ " + Key.key.ToString() + " ] " + " \n";
                    File.AppendAllText(path, keycode);
                }

                foreach (string s in readText)
                {
                    if (s != null)
                    {
                        string boundActionStringName = s.Split(new char[] { ' ' })[0];

                        if (System.Enum.TryParse<KeyActions>(boundActionStringName, out KeyActions yourEnum))
                        {
                            KeyActions parsed_enum = (KeyActions)System.Enum.Parse(typeof(KeyActions), boundActionStringName);

                            //Loop Through Default Inputs
                            for (int i = 0; i < PlayerInput.DefaultInputs.Count; i++)
                            {
                                //Found A Match To Read From And Add To PlayerInput.Input
                                if (PlayerInput.DefaultInputs[i].BoundAction.Equals(parsed_enum))
                                {
                                    InputKey NewKey = new InputKey();

                                    string[] separatingStrings = { " ", "  [ ", " ] " };

                                    string[] keystring = s.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

                                    KeyCode newKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), keystring[2]);

                                    NewKey.ActionName = parsed_enum.ToString();
                                    NewKey.BoundAction = parsed_enum;
                                    NewKey.key = newKey;

                                    //Created New Key Profile
                                    PlayerInput.Inputs.Add(NewKey);

                                    if (debugKeys)
                                    {
                                        print("New Key: " + NewKey.ActionName + NewKey.BoundAction.ToString());
                                    }
                                    break;
                                }
                            }
                        }

                        //print(s);
                    }
                }
            }
            //File Not Found To Be Empty
            else
            {
                //Load In Data...
                foreach (string s in readText)
                {
                    if (s != null)
                    {
                        string boundActionStringName = s.Split(new char[] { ' ' })[0];

                        if (System.Enum.TryParse<KeyActions>(boundActionStringName, out KeyActions yourEnum))
                        {
                            KeyActions parsed_enum = (KeyActions)System.Enum.Parse(typeof(KeyActions), boundActionStringName);

                            //Loop Through Default Inputs
                            for (int i = 0; i < PlayerInput.DefaultInputs.Count; i++)
                            {
                                //Found A Match To Read From And Add To PlayerInput.Input
                                if (PlayerInput.DefaultInputs[i].BoundAction.Equals(parsed_enum))
                                {
                                    string[] separatingStrings = { " ", "  [ ", " ] " };

                                    string[] keystring = s.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

                                    KeyCode newKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), keystring[2]);

                                    InputKey NewKey = new InputKey();

                                    NewKey.ActionName = parsed_enum.ToString();
                                    NewKey.BoundAction = parsed_enum;
                                    NewKey.key = newKey;

                                    //Created New Key Profile
                                    PlayerInput.Inputs.Add(NewKey);
                                    if (debugKeys)
                                    {
                                        print("Loaded In And Set Key Binding To The " + PlayerInput.DefaultInputs[i].key.ToString() + " - " + PlayerInput.DefaultInputs[i].BoundAction.ToString());
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }
        //No Such File Exists
        else
        {
            //File Not Found
            print("Input File Not Found. Creating Keybinding File...");

            //Create Text File To Path
            File.WriteAllText(path, string.Empty);

            //Add Text To it
            foreach (InputKey Key in PlayerInput.DefaultInputs)
            {
                string keycode = Key.BoundAction.ToString() + "  [ " + Key.key.ToString() + " ] " + " \n";
                File.AppendAllText(path, keycode);
            }

            // Open the file to read from.
            string[] readText = File.ReadAllLines(path);

            foreach (string s in readText)
            {
                if (s != null)
                {
                    string boundActionStringName = s.Split(new char[] { ' ' })[0];

                    if (System.Enum.TryParse<KeyActions>(boundActionStringName, out KeyActions yourEnum))
                    {
                        KeyActions parsed_enum = (KeyActions)System.Enum.Parse(typeof(KeyActions), boundActionStringName);

                        //Loop Through Default Inputs
                        for (int i = 0; i < PlayerInput.DefaultInputs.Count; i++)
                        {
                            //Found A Match To Read From And Add To PlayerInput.Input
                            if (PlayerInput.DefaultInputs[i].BoundAction.Equals(parsed_enum))
                            {
                                InputKey NewKey = new InputKey();

                                string[] separatingStrings = { " ", "  [ ", " ] " };

                                string[] keystring = s.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

                                KeyCode newKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), keystring[2]);

                                NewKey.ActionName = parsed_enum.ToString();
                                NewKey.BoundAction = parsed_enum;
                                NewKey.key = newKey;

                                //Created New Key Profile
                                PlayerInput.Inputs.Add(NewKey);

                                if (debugKeys)
                                {
                                    print("New Key: " + NewKey.ActionName + NewKey.BoundAction.ToString());
                                }

                                break;
                            }
                        }
                    }

                    //print(s);
                }
            }
        }

        print("Completed Populated List For Key Bindings.");
    }

    public void SaveKeyScheme()
    {
        //Check For File
        if (File.Exists(path))
        {
            File.WriteAllText(path, "Key Bindings" + "\n \n");

            //Add Text To it
            foreach (InputKey Key in PlayerInput.Inputs)
            {
                string keycode = Key.BoundAction.ToString() + "  [ " + Key.key.ToString() + " ] " + " \n";
                File.AppendAllText(path, keycode);
            }

            updateKeys = true;
        }
        else
        {
            //Create File Fall Back And Then Save
            print("Fall Back Method. File Not Found On Save. Creating new file and saving new data set.");
            //Create Text File To Path
            File.WriteAllText(path, "Key Bindings" + "\n \n");

            //Add Text To it
            foreach (InputKey Key in PlayerInput.Inputs)
            {
                string keycode = Key.BoundAction.ToString() + "  [ " + Key.key.ToString() + " ] " + " \n";
                File.AppendAllText(path, keycode);
            }

            updateKeys = true;
        }
    }
    public void UpdateIndexes()
    {
        if (updateKeys)
        {
            Story story = new Story(DialogueManager.Instance.globalsInkFile.text);
            print(story.currentText);
            DialogueManager.Instance.dialogueVariables.StartListening(story);

            for (int i = 0; i < PlayerInput.Inputs.Count; i++)
            {
                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.WalkForward))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.WalkForwardIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["ForwardKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.WalkForwardIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.WalkBackwards))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.WalkBackwardsIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["BackwardKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.WalkBackwardsIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.WalkRight))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.WalkRightIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["RightKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();


                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.WalkRightIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.WalkLeft))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.WalkLeftIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["LeftKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.WalkLeftIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.SkillA))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.SkillAIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["SkillAKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.SkillAIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.SkillB))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.SkillBIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["SkillBKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.SkillBIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.SkillC))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.SkillCIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["SkillCKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.SkillCIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.SkillD))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.SkillDIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["SkillDKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.SkillDIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.Dodge))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.DodgeIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["DodgeKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.DodgeIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.Crouch))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.CrouchIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["CrouchKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.CrouchIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.UseHealthPotion))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.UseHealthPotionIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["HealthPotionKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.UseHealthPotionIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.UseTensionPotion))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.UseTensionPotionIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["MagicPotionKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.UseTensionPotionIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.TargetEnemy))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.TargetEnemyIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["TargetEnemyKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.TargetEnemyIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.Jump))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.JumpIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["JumpKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.JumpIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.Grapple))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.GrappleIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["GrappleKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.GrappleIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                //HyperThreadings will change to Dreamweaving in Phase 2
                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.HyperThreading))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.HyperThreadIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.HyperThreadIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.LightAttack))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.LightAttackIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["LightAttackKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.LightAttackIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.HeavyAttack))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.HeavyAttackIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["HeavyAttackKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();


                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.HeavyAttackIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.SwitchArrowType))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.SwitchArrowIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["SwitchArrowsKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.SwitchArrowIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.SwitchWeapon))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.SwitchWeaponIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["SwitchWeaponKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.SwitchWeaponIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.Interact))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.InteractIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["InteractKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.InteractIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.EscapeMenu))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.EscapeMenuIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["EscapeKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.EscapeMenuIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.UI_Confirm))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.UI_ConfirmIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["UI_ConfirmKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.UI_ConfirmIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.UI_Cancel))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.UI_CancelIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["UI_CancelKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.UI_CancelIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.UI_Equip))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.UI_EquipIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["UI_EquipKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.UI_EquipIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.Guard))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.GuardIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["GuardKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.GuardIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.ResetCamera))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.ResetCameraIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["ResetCameraKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.ResetCameraIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }

                if (PlayerInput.Inputs[i].BoundAction.Equals(KeyActions.AimDownSights))
                {
                    PlayerInput.Inputs[i].ActionName = PlayerInput.Inputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.AimDownSightsIndex = PlayerInput.Inputs.IndexOf(PlayerInput.Inputs[i]);
                    story.variablesState["AimDownSightsKey"] = InputManager.Instance.PlayerInput.Inputs[i].key.ToString();

                    PlayerInput.DefaultInputs[i].ActionName = PlayerInput.DefaultInputs[i].BoundAction.ToString();
                    PlayerInput.KeyIndex.AimDownSightsIndex = PlayerInput.DefaultInputs.IndexOf(PlayerInput.DefaultInputs[i]);
                }
            }

            DialogueManager.Instance.dialogueVariables.StopListening(story);
            print("Completed Index Update.");
            updateKeys = false;
        }
    }

    void KeyMovement()
    {
        //Clamp
        PlayerInput.Horizontal = Mathf.Clamp(PlayerInput.Horizontal, -1, 1);
        PlayerInput.Vertical = Mathf.Clamp(PlayerInput.Vertical, -1, 1);
        PlayerInput.Elevation = Mathf.Clamp(PlayerInput.Elevation, -1, 1);

        if (PlayerInput.Horizontal > 1)
        {
            PlayerInput.Horizontal = 1;
        }

        if (PlayerInput.Horizontal < -1)
        {
            PlayerInput.Horizontal = -1;
        }

        if (PlayerInput.Vertical > 1)
        {
            PlayerInput.Vertical = 1;
        }

        if (PlayerInput.Vertical < -1)
        {
            PlayerInput.Vertical = -1;
        }

        if (PlayerInput.Elevation > 1)
        {
            PlayerInput.Elevation = 1;
        }

        if (PlayerInput.Elevation < -1)
        {
            PlayerInput.Elevation = -1;
        }

        //Keys
        KeyCode keyForward = PlayerInput.Inputs[PlayerInput.KeyIndex.WalkForwardIndex].key;
        KeyCode keyBackward = PlayerInput.Inputs[PlayerInput.KeyIndex.WalkBackwardsIndex].key;
        KeyCode keyLeft = PlayerInput.Inputs[PlayerInput.KeyIndex.WalkLeftIndex].key;
        KeyCode keyRight = PlayerInput.Inputs[PlayerInput.KeyIndex.WalkRightIndex].key;
        KeyCode keyUp = PlayerInput.Inputs[PlayerInput.KeyIndex.JumpIndex].key;
        KeyCode keyDown = PlayerInput.Inputs[PlayerInput.KeyIndex.CrouchIndex].key;

        if (Input.GetKey(keyForward) && PlayerInput.Vertical < 1)
        {
            PlayerInput.Vertical += Time.deltaTime * PlayerInput.acceleration;
        }

        if (Input.GetKey(keyBackward) && PlayerInput.Vertical > -1)
        {
            PlayerInput.Vertical -= Time.deltaTime * PlayerInput.acceleration;
        }

        if (Input.GetKey(keyRight) && PlayerInput.Horizontal < 1)
        {
            PlayerInput.Horizontal += Time.deltaTime * PlayerInput.acceleration;
        }

        if (Input.GetKey(keyLeft) && PlayerInput.Horizontal > -1)
        {
            PlayerInput.Horizontal -= Time.deltaTime * PlayerInput.acceleration;
        }

        if (Input.GetKey(keyUp) && PlayerInput.Horizontal < 1)
        {
            PlayerInput.Elevation += Time.deltaTime * PlayerInput.acceleration;
        }

        if (Input.GetKey(keyDown) && PlayerInput.Horizontal > -1)
        {
            PlayerInput.Elevation -= Time.deltaTime * PlayerInput.acceleration;
        }


        if (!Input.GetKey(keyForward) && !Input.GetKey(keyBackward))
        {
            if (PlayerInput.Vertical > 0)
            {
                PlayerInput.Vertical -= Time.deltaTime * PlayerInput.deacceleration;
            }
            if (PlayerInput.Vertical < 0)
            {
                PlayerInput.Vertical += Time.deltaTime * PlayerInput.deacceleration;
            }
            if (PlayerInput.Vertical < 1f && PlayerInput.Vertical > -1f)
            {
                PlayerInput.Vertical = 0;
            }

        }

        if (!Input.GetKey(keyLeft) && !Input.GetKey(keyRight))
        {
            if (PlayerInput.Horizontal > 0)
            {
                PlayerInput.Horizontal -= Time.deltaTime * PlayerInput.deacceleration;
            }
            if (PlayerInput.Horizontal < 0)
            {
                PlayerInput.Horizontal += Time.deltaTime * PlayerInput.deacceleration;
            }
            if (PlayerInput.Horizontal < 1f && PlayerInput.Horizontal > -1f)
            {
                PlayerInput.Horizontal = 0;
            }
        }

        if (!Input.GetKey(keyUp) && !Input.GetKey(keyDown))
        {
            if (PlayerInput.Elevation > 0)
            {
                PlayerInput.Elevation -= Time.deltaTime * PlayerInput.deacceleration;
            }
            if (PlayerInput.Elevation < 0)
            {
                PlayerInput.Elevation += Time.deltaTime * PlayerInput.deacceleration;
            }
            if (PlayerInput.Elevation < 1f && PlayerInput.Elevation > -1f)
            {
                PlayerInput.Elevation = 0;
            }
        }

        PlayerInput.MovementVector = new Vector3(PlayerInput.Horizontal, PlayerInput.Elevation, PlayerInput.Vertical);
    }

    void DebugKeys()
    {
        if (debugKeys)
        {
            foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(vKey))
                {
                    //your code here
                    print(vKey.ToString());
                }
            }
        }
    }

    private int conflictIndex = 0;
    private InputKey conflictKey = new InputKey();
    private bool searchForConflict = false;
    public void SetNewInput(int newkeyIndex, InputKey Newkey)
    {
        foreach (KeyCode newKey in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(newKey))
            {
                //Add New Key To Action Binding
                PlayerInput.Inputs[newkeyIndex].key = newKey;
                
                UIManager.Instance.isListeningForKey = false;
            }
        }

        //Clear Conflicts
        conflictIndex = newkeyIndex;
        conflictKey.ActionName = PlayerInput.Inputs[newkeyIndex].ActionName;
        conflictKey.key = Newkey.key;
        searchForConflict = true;
    }

    private void CheckForInvalidInputs(InputKey NewKey, int newkeyIndex)
    {
        for(int i = 0; i < PlayerInput.Inputs.Count; i++)
        {
            //This catches duplicate uses of the same key and prevents them by deleting the duplicate found and keybinding this one instead leaving it null
            if(PlayerInput.Inputs[i].key == NewKey.key && PlayerInput.Inputs[i].ActionName != NewKey.ActionName)
            {
                PlayerInput.Inputs[i].key = KeyCode.None;
                UIManager.Instance.ClearConflictKey(i);
                searchForConflict = false;
                return;
            }
        }

        searchForConflict = false;
    }

    // Called when the singleton is created *or* after a domain reload in the editor.
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

