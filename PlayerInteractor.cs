using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventoryClass;
using StatClasses;

public class PlayerInteractor : MonoBehaviour
{
    public bool interactable;
    public bool grappleSwing;
    public bool grapplePoint;
    public GameObject currentNPC;
    public LayerMask NPCMask;
    RaycastHit hit;
    private Rigidbody rb;
    private PlayerDriver player;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<PlayerDriver>();
    }

    private void FixedUpdate()
    {
        Interact();
    }
    private bool uiLock;
    private void Interact()
    {
        
        //Pressing Interact Key While Looking At A NPC with a NPC component and file present will allow you to intiate speaking to them
        if(!player.physicsProperties.readyToWallKick && !player.physicsProperties.isWallKicking && !player.physicsProperties.dashing && !PlayerSettings.Instance.gameplaySettings.Mode.Equals(CameraMode.DialogueMode) && !PlayerSettings.Instance.gameplaySettings.Mode.Equals(CameraMode.TargetMode) && !DialogueManager.Instance.isDialoguePlaying && Physics.Raycast(transform.position, transform.forward, out hit, 5f, NPCMask) && hit.transform.GetComponent<NPC>() != null && !hit.transform.GetComponent<NPC>().isCommunicating && hit.transform.GetComponent<NPC>().DialogueFile != null && Input.GetKey(InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.InteractIndex].key))
        {
            //Start Conversation
            hit.transform.GetComponent<NPC>()._player = gameObject;
            hit.transform.GetComponent<NPC>().StartConversation(gameObject);
            player.ResetGroundDetection();
        }

        if (!PlayerSettings.Instance.gameplaySettings.Mode.Equals(CameraMode.TargetMode) && !DialogueManager.Instance.isDialoguePlaying && Physics.Raycast(transform.position, transform.forward, out hit, 5f, NPCMask) && hit.transform.GetComponent<NPC>() != null && !hit.transform.GetComponent<NPC>().isCommunicating && hit.transform.GetComponent<NPC>().DialogueFile != null)
        {
            interactable = true;
            uiLock = false;
        }
        else
        {
            if (!uiLock)
            {
                interactable = false;
                UIManager.Instance.displayInteraction = true;

                if (!interactable && UIManager.Instance.displayInteraction && !grapplePoint && !grappleSwing)
                {
                    UIManager.Instance.clearAllDisplayActions = true;

                }

                uiLock = true;
            }
        }

        if (!player.physicsProperties.dashing && GetComponent<PlayerSettings>().gameplaySettings.Mode.Equals(CameraMode.TargetMode) && GetComponent<PlayerDriver>().MyCamera.LockOnTarget.GetComponent<NPC>() != null && !DialogueManager.Instance.isDialoguePlaying && !GetComponent<PlayerDriver>().MyCamera.LockOnTarget.GetComponent<NPC>().isCommunicating && GetComponent<PlayerDriver>().MyCamera.LockOnTarget.GetComponent<NPC>().DialogueFile != null && Input.GetKey(InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.InteractIndex].key))
        {
            //Start Conversation
            GetComponent<PlayerDriver>().MyCamera.LockOnTarget.transform.GetComponent<NPC>()._player = gameObject;
            GetComponent<PlayerDriver>().MyCamera.LockOnTarget.transform.GetComponent<NPC>().StartConversation(gameObject);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "GuardiaCrest")
        {
            BasicInventoryItem Item = new BasicInventoryItem(InventoryItemType.GuardiaCrest, 1, 999);

            //Add One To Inventory
            InventoryEvents.AddBasicItemToInventory(Item, GetComponent<Stash>(), 1);
            GetComponent<EntityStats>().ResetBaseStats();
            Destroy(col.gameObject);
        }

        if (col.gameObject.tag == "QuiltedQuiver")
        {
            BasicInventoryItem Item = new BasicInventoryItem(InventoryItemType.QuiltedQuiver, 1, 999);

            //Add One To Inventory
            InventoryEvents.AddBasicItemToInventory(Item, GetComponent<Stash>(), 1);
            GetComponent<EntityStats>().ResetBaseStats();
            Destroy(col.gameObject);
        }

        if (col.gameObject.tag == "WispOfTheEnders")
        {
            BasicInventoryItem Item = new BasicInventoryItem(InventoryItemType.WispOfTheEnders, 1, 999);

            //Add One To Inventory
            InventoryEvents.AddBasicItemToInventory(Item, GetComponent<Stash>(), 1);
            GetComponent<EntityStats>().ResetBaseStats();
            Destroy(col.gameObject);
        }

        if (col.gameObject.tag == "Sylke")
        {
            BasicInventoryItem Item = new BasicInventoryItem(InventoryItemType.SylkeEssence, 1, 999);

            StatInteractions.ReduceTension(GetComponent<EntityStats>().PlayerSheet.TensionLimit, GetComponent<EntityStats>());

            //Add One To Inventory
            InventoryEvents.AddBasicItemToInventory(Item, GetComponent<Stash>(), 1);
            GetComponent<EntityStats>().ResetBaseStats();
            Destroy(col.gameObject);
        }

        if (col.gameObject.tag == "Valence")
        {
            double currencyCap = 1000000000000;
            BasicInventoryItem Item = new BasicInventoryItem(InventoryItemType.Valence, col.gameObject.GetComponent<Valence>().CurrencyValue, (int)currencyCap);

            Item.Item = InventoryItemType.Valence;
            Item.ItemCount = col.gameObject.GetComponent<Valence>().CurrencyValue;

            //Add Currency
            InventoryEvents.AddBasicItemToInventory(Item, GetComponent<Stash>(), col.gameObject.GetComponent<Valence>().CurrencyValue);
            Destroy(col.gameObject);
        }

        if(col.gameObject.tag == "ScarletThread")
        {
            BasicInventoryItem Item = new BasicInventoryItem(InventoryItemType.ScarletThread, 1, 999);

            //Add One To Inventory
            InventoryEvents.AddBasicItemToInventory(Item, GetComponent<Stash>(), 1);

            Destroy(col.gameObject);
        }

        if (col.gameObject.tag == "DreamSpool")
        {
            BasicInventoryItem Item = new BasicInventoryItem(InventoryItemType.DreamSpool, 1, 999);

            //Add One To Inventory
            InventoryEvents.AddBasicItemToInventory(Item, GetComponent<Stash>(), 1);
            GetComponent<EntityStats>().ResetBaseStats();
            Destroy(col.gameObject);
        }

        if (col.gameObject.tag == "NightmareTether")
        {
            BasicInventoryItem Item = new BasicInventoryItem(InventoryItemType.NightmareTether, 1, 999);

            //Add One To Inventory
            InventoryEvents.AddBasicItemToInventory(Item, GetComponent<Stash>(), 1);
            GetComponent<EntityStats>().ResetBaseStats();
            Destroy(col.gameObject);
        }

        if (col.gameObject.tag == "SylkeEssence")
        {
            BasicInventoryItem Item = new BasicInventoryItem(InventoryItemType.SylkeEssence, 1, 999);

            StatInteractions.ReduceTension(GetComponent<EntityStats>().PlayerSheet.TensionLimit, GetComponent<EntityStats>());

            //Add One To Inventory
            InventoryEvents.AddBasicItemToInventory(Item, GetComponent<Stash>(), 1);
            GetComponent<EntityStats>().ResetBaseStats();
            Destroy(col.gameObject);
        }

        if (col.gameObject.tag == "SpiritGrace")
        {
            BasicInventoryItem Item = new BasicInventoryItem(InventoryItemType.SpiritGrace, 1, 999);

            //Apply a 100% heal value to player health and guard healing sources
            col.gameObject.GetComponent<SpiritGrace>().HealApplication.HealingValue = GetComponent<EntityHealth>().Health[GetComponent<EntityHealth>().BaseHealthIndex].HealthCap;
            col.gameObject.GetComponent<SpiritGrace>().GuardHealApplication.HealingValue = GetComponent<EntityHealth>().Health[GetComponent<EntityHealth>().GuardHealthIndex].HealthCap;

            //Heal Player
            col.gameObject.GetComponent<SpiritGrace>().HealPlayer(col.gameObject.GetComponent<SpiritGrace>().HealApplication, GetComponent<EntityHealth>());
            col.gameObject.GetComponent<SpiritGrace>().HealPlayer(col.gameObject.GetComponent<SpiritGrace>().GuardHealApplication, GetComponent<EntityHealth>());

            //Add One To Inventory
            InventoryEvents.AddBasicItemToInventory(Item, GetComponent<Stash>(), 1);
            GetComponent<EntityStats>().ResetBaseStats();

            Destroy(col.gameObject);
        }

        if(col.gameObject.tag == "Sylke")
        {
            StatInteractions.ReduceTension(col.gameObject.GetComponent<Sylke>().TensionReduction, GetComponent<EntityStats>());
            print("Reduced Player Tension by " + col.gameObject.GetComponent<Sylke>().TensionReduction);
            Destroy(col.gameObject);
        }

        if(col.gameObject.tag == "Bead")
        {
            col.gameObject.GetComponent<Bead>().HealPlayer(GetComponent<EntityHealth>());
            Destroy(col.gameObject);
        }
    }


}
