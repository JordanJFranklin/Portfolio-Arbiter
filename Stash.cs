using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventoryClass;

[System.Serializable]
public class UnlockList
{
    public bool unlockNotchKick = false;
    public bool unlockSpinningSpool = false;
    public bool unlockCardinalCowl = false;
    public bool unlockRotaryHook = false;
    public bool unlockWakingDescent = false;
    public bool unlockHyphinFeather = false;
}

public class Stash : MonoBehaviour 
{
    public Inventory PlayerInventory;
    public string path;
    public Equipper equipment;
    [Header("Unlocks/Progression Checks")]
    public bool UpdateUnlocks = false;
    public UnlockList unlockList;

    

    //Convert Manager To Singleton
    private static Stash _instance;
    static bool _destroyed;
    public static Stash Instance
    {
        get
        {
            // Prevent re-creation of the singleton during play mode exit.
            if (_destroyed) return null;

            // If the instance is already valid, return it. Needed if called from a
            // derived class that wishes to ensure the instance is initialized.
            if (_instance != null) return _instance;

            // Find the existing instance (across domain reloads).
            if ((_instance = FindObjectOfType<Stash>()) != null) return _instance;

            // Create a new GameObject instance to hold the singleton component.
            var gameObject = new GameObject(typeof(Stash).Name);

            // Move the instance to the DontDestroyOnLoad scene to prevent it from
            // being destroyed when the current scene is unloaded.
            DontDestroyOnLoad(gameObject);

            // Create the MonoBehavior component. Awake() will assign _instance.
            return gameObject.AddComponent<Stash>();
        }
    }

    // Start is called before the first frame update
    private void Awake()
    {
        Debug.Assert(_instance == null || _instance == this, "More than one singleton instance instantiated!", this);

        if (_instance == null || _instance == this)
        {
            _instance = this;
        }

        path = Application.dataPath + "/Inventory.txt";
        equipment = GetComponent<Equipper>();
    }
    void Start()
    {
        //Max Cap is 1 Billion
        double currencyCap = 1000000000000;
        PlayerInventory.BasicItems[0].ItemCap = (int)currencyCap;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAllMovementUnlocks();
    }

    public void UpdateAllMovementUnlocks()
    {
        if(UpdateUnlocks && PlayerInventory.MovementUnlocks.Count > 0)
        {
            foreach (MovementAbilityUnlocks skill in PlayerInventory.MovementUnlocks)
            {
                switch(skill.MovementAbillity)
                {
                    case MovementAbilities.HyphinFeather:
                        unlockList.unlockHyphinFeather = true;
                        break;

                    case MovementAbilities.SpinningSpool:
                        unlockList.unlockSpinningSpool = true;
                        break;

                    case MovementAbilities.CardinalCowl:
                        unlockList.unlockCardinalCowl = true;
                        break;

                    case MovementAbilities.WakingDescent:
                        unlockList.unlockWakingDescent = true;
                        break;

                    case MovementAbilities.RotaryHook:
                        unlockList.unlockRotaryHook = true;
                        break;

                    case MovementAbilities.NotchKick:
                        unlockList.unlockNotchKick = true;
                        break;
                }
            }

            UpdateUnlocks = false;
        }

        if(UpdateUnlocks && PlayerInventory.MovementUnlocks.Count == 0)
        {
            UpdateUnlocks = false;
        }
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
