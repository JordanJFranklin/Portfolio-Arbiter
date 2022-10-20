using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventoryClass;
using HealthClasses;
using DamageClass;
using StatClasses;

public class Flasks : MonoBehaviour
{
    public float HealingPercentage = 0.25f;
    public float TensionPercentageRestoration = 0.25f;

    private EntityStats stats;
    private EntityHealth healthHandler;

    void Start()
    {
        stats = GetComponent<EntityStats>();
        healthHandler = GetComponent<EntityHealth>();
    }

    void Update()
    {
        //Create Trigger From Input System Here
        //Heal
        if(Input.GetKey(InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.UseHealthPotionIndex].key))
        {
            UseVermillionVessel();
        }
        //Reduce Tension
        if (Input.GetKey(InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.UseTensionPotionIndex].key))
        {
            UseSoulweaverSpool();
        }
    }

    public void UseVermillionVessel()
    {
        BasicInventoryItem HFlask = new BasicInventoryItem(InventoryItemType.VermillionVessel, 1, InventoryEvents.ReturnBasicItemCount(InventoryItemType.SpiritGrace, Stash.Instance));
        HealingProperty Heal = new HealingProperty(EntityTarget.Player, HealthTypes.Base, Mathf.RoundToInt(healthHandler.Health[healthHandler.BaseHealthIndex].HealthCap * HealingPercentage));

        if (InventoryEvents.ReturnBasicItemCount(InventoryItemType.VermillionVessel, GetComponent<Stash>()) > 0)
        {
            CallHealthEvent.SendHealingEvent(Heal, GetComponent<EntityHealth>());
            InventoryEvents.SubtractBasicItemFromInventory(HFlask, GetComponent<Stash>(), 1);
        }
    }

    public void UseSoulweaverSpool()
    {
        BasicInventoryItem CFlask = new BasicInventoryItem(InventoryItemType.SoulweaverSpool, 1, InventoryEvents.ReturnBasicItemCount(InventoryItemType.DreamSpool, Stash.Instance));

        if (InventoryEvents.ReturnBasicItemCount(InventoryItemType.SoulweaverSpool, GetComponent<Stash>()) > 0)
        {
            StatInteractions.ReduceTension(Mathf.RoundToInt(stats.PlayerSheet.TensionLimit * TensionPercentageRestoration), stats);
            InventoryEvents.SubtractBasicItemFromInventory(CFlask, GetComponent<Stash>(), 1);
        }
    }
}