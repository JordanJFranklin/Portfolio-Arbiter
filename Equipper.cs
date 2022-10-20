using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StatClasses;
using InventoryClass;
using StateClasses;
using System;


public class Equipper : MonoBehaviour
{
    [Header("Equipment")]
    public int CurrentlyEquippedIndex = 0;
    public MaskItem Mask;
    public List<Loadout> Loadouts; //3 Weapons in Game so maximum of loadouts is 3
    public ThimbleItem Thimble; //only can wear one at a time regardless of loadout
    public List<QuilteryItem> Quiltery; //Maximum of 12 can be used
    public List<AdventureItem> AdventureItemSlots; //Can Equip To F1-F4 Slots

    [Header("Bones")]
    public GameObject RightHand;
    public GameObject Face;

    private Stash Inv;
    private EntityStats Stats;
    private EntityState States;

    // Start is called before the first frame update
    private void Awake()
    {
        Inv = GetComponent<Stash>();
        Stats = GetComponent<EntityStats>();
        States = GetComponent<EntityState>();
    }

    void Start()
    {
        InventoryEvents.LoadInventory(Inv.path, GetComponent<Stash>(), GetComponent<EntityStats>());
        SpawnAdventureItem();
        Inv.UpdateUnlocks = true;
        //EquipMask(0, Inv, CurrentlyEquippedIndex);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        SwitchWeapon();
    }

    //Upgrade System
    public void UpgradeItem(InventoryItemType ItemType, int inventoryIndex, int skillIndex)
    {
        //Weapons
        /*if (ItemType.Equals(InventoryItemType.Weapon))
        {
            if (InventoryEvents.ReturnBasicItemCount(InventoryItemType.ScarletThread, Inv) > 0 && Inv.PlayerInventory.ChubaiFormations[inventoryIndex].ThreadCountLevel < Inv.PlayerInventory.ChubaiFormations[inventoryIndex].ThreadCountLevelCap)
            {
                BasicInventoryItem ScarletThread = new BasicInventoryItem(InventoryItemType.ScarletThread, 1, 999);

                //Subtract From Inventory
                InventoryEvents.SubtractBasicItemFromInventory(ScarletThread, Inv, 1);

                //Increase Thread Count Level
                Inv.PlayerInventory.AdventureBag[inventoryIndex]. += 1;

                //Refresh All Held Items
                Loadouts[CurrentlyEquippedIndex].Weapon.ThreadCountLevel = Inv.PlayerInventory.ChubaiFormations[inventoryIndex].ThreadCountLevel;
                Loadouts[CurrentlyEquippedIndex].Weapon.ThreadCountLevelCap = Inv.PlayerInventory.ChubaiFormations[inventoryIndex].ThreadCountLevelCap;

                //Update Base Stats
                Loadouts[CurrentlyEquippedIndex].Weapon.TotalDamage = Loadouts[CurrentlyEquippedIndex].Weapon.BaseDamage + (Loadouts[CurrentlyEquippedIndex].Weapon.ThreadCountLevel * Loadouts[CurrentlyEquippedIndex].Weapon.BaseDamageGrowthPerLevel);
                Loadouts[CurrentlyEquippedIndex].Weapon.TotalCriticalStrikeChance = Loadouts[CurrentlyEquippedIndex].Weapon.BaseCriticalStrikeChance + (Loadouts[CurrentlyEquippedIndex].Weapon.ThreadCountLevel * Loadouts[CurrentlyEquippedIndex].Weapon.CriticalStrikeChancePerLevel);
                Loadouts[CurrentlyEquippedIndex].Weapon.TotalCriticalStrikeDamage = Loadouts[CurrentlyEquippedIndex].Weapon.BaseCriticalStrikeDamage + (Loadouts[CurrentlyEquippedIndex].Weapon.ThreadCountLevel * Loadouts[CurrentlyEquippedIndex].Weapon.CriticalStrikeDamagePerLevel);
                
                print("Upgrade Successfull");

                CalculateWeapon();
            }
        }*/
        //Masks
        if (ItemType.Equals(InventoryItemType.Mask))
        {
            if (InventoryEvents.ReturnBasicItemCount(InventoryItemType.ScarletThread, Inv) > 0 && Inv.PlayerInventory.Masks[inventoryIndex].ThreadCountLevel < Inv.PlayerInventory.Masks[inventoryIndex].ThreadCountLevelCap)
            {
                BasicInventoryItem ScarletThread = new BasicInventoryItem(InventoryItemType.ScarletThread, 1, 999);

                //Subtract From Inventory
                InventoryEvents.SubtractBasicItemFromInventory(ScarletThread, Inv, 1);

                //Increase Thread Count Level
                Inv.PlayerInventory.Masks[inventoryIndex].ThreadCountLevel += 1;

                //Refresh All Held Items
                Mask.ThreadCountLevel = Inv.PlayerInventory.Masks[inventoryIndex].ThreadCountLevel;
                Mask.ThreadCountLevelCap = Inv.PlayerInventory.Masks[inventoryIndex].ThreadCountLevelCap;

                //Update Item Stat Details
                Mask.Model.GetComponent<Mask>().UpdateStats(Mask.ThreadCountLevel);

                print("Upgrade Successfull");
            }
        }
        //Skills
        if (ItemType.Equals(InventoryItemType.Memory))
        {
            if (InventoryEvents.ReturnBasicItemCount(InventoryItemType.ScarletThread, Inv) > 0 && Inv.PlayerInventory.Memories[inventoryIndex].ThreadCountLevel < Inv.PlayerInventory.Memories[inventoryIndex].ThreadCountLevelCap)
            {
                BasicInventoryItem ScarletThread = new BasicInventoryItem(InventoryItemType.ScarletThread, 1, 999);

                //Subtract From Inventory
                InventoryEvents.SubtractBasicItemFromInventory(ScarletThread, Inv, 1);

                //Increase Thread Count Level
                Inv.PlayerInventory.Memories[inventoryIndex].ThreadCountLevel += 1;

                //Refresh All Held Items
                Loadouts[CurrentlyEquippedIndex].Skills[skillIndex].ThreadCountLevel = Inv.PlayerInventory.Memories[inventoryIndex].ThreadCountLevel;
                Loadouts[CurrentlyEquippedIndex].Skills[skillIndex].ThreadCountLevelCap = Inv.PlayerInventory.Memories[inventoryIndex].ThreadCountLevelCap;

                Loadouts[CurrentlyEquippedIndex].Skills[skillIndex].SkillObject.GetComponent<MemorySkill>().updateSkill = true;

                print("Upgrade Successfull");
            }
        }
        //Armor
        if (ItemType.Equals(InventoryItemType.Thimble))
        {
            if (InventoryEvents.ReturnBasicItemCount(InventoryItemType.ScarletThread, Inv) > 0 && Inv.PlayerInventory.Thimbles[inventoryIndex].ThreadCountLevel < Inv.PlayerInventory.Thimbles[inventoryIndex].ThreadCountLevelCap)
            {
                BasicInventoryItem ScarletThread = new BasicInventoryItem(InventoryItemType.ScarletThread, 1, 999);

                //Subtract From Inventory
                InventoryEvents.SubtractBasicItemFromInventory(ScarletThread, Inv, 1);

                //Increase Thread Count Level
                Inv.PlayerInventory.Thimbles[inventoryIndex].ThreadCountLevel += 1;

                //Refresh All Held Items
                Thimble.ThreadCountLevel = Inv.PlayerInventory.Thimbles[inventoryIndex].ThreadCountLevel;
                Thimble.ThreadCountLevelCap = Inv.PlayerInventory.Thimbles[inventoryIndex].ThreadCountLevelCap;

                //Update Item Stat Details
                Thimble.Model.GetComponent<Mask>().UpdateStats(Thimble.ThreadCountLevel);

                print("Upgrade Successfull");
            }
            else
            {
                print("Upgrade Failed");
            }
        }
    }
    public void CalculateItems()
    {
        //Now Add Buffs
        if (States.Buffs.BuffEffects != null)
        {
            foreach (BuffEffect Buff in States.Buffs.BuffEffects)
            {
                StateSetter.SendBuff(Buff, States, Stats);
            }
        }

        if (States.Debuffs.DebuffEffects != null)
        {
            foreach (DebuffEffect Debuff in States.Debuffs.DebuffEffects)
            {
                StateSetter.SendDebuff(Debuff, States, Stats);
            }
        }

        //Now Add Thimbles
        if (Thimble.Model != null)
        {
            Thimble.Model.GetComponent<Thimble>().UpdateStats(Thimble.ThreadCountLevel);
        }

        //Now Add Quiltery
        if (Quiltery.Count > 0)
        {
            foreach (QuilteryItem item in Quiltery)
            {
                if (item.Model != null)
                {
                    item.Model.GetComponent<Quiltery>().UpdateStats();
                }
            }
        }

        //Now Add Mask
        if (Mask.Model != null)
        {
            Mask.Model.GetComponent<Mask>().UpdateStats(Mask.ThreadCountLevel);
        }
    }
    //Calculate Weapons
    public void CalculateWeapon()
    {
        try
        {
            if (Loadouts.IndexOf(Loadouts[CurrentlyEquippedIndex]) > -1 && Loadouts[CurrentlyEquippedIndex] != null && Loadouts[CurrentlyEquippedIndex].LoadoutUnlocked)
            {
                //Clear All Loadout Visuals
                if (Loadouts[0].Weapon.WeaponModel != null)
                {
                    Loadouts[0].Weapon.WeaponModel.SetActive(false);
                }

                if (Loadouts[1].Weapon.WeaponModel != null)
                {
                    Loadouts[1].Weapon.WeaponModel.SetActive(false);
                }

                if (Loadouts[2].Weapon.WeaponModel != null)
                {
                    Loadouts[2].Weapon.WeaponModel.SetActive(false);
                }

                for (int i = 0; i < Loadouts.Count; i++)
                {
                    //Set Weapon Stats To Loadout Only
                    Loadouts[i].Weapon.TotalDamage = Loadouts[i].Weapon.BaseDamage + (Loadouts[i].Weapon.ThreadCountLevel * Loadouts[i].Weapon.BaseDamageGrowthPerLevel);
                    Loadouts[i].Weapon.TotalCriticalStrikeChance = Loadouts[i].Weapon.BaseCriticalStrikeChance + (Loadouts[i].Weapon.ThreadCountLevel * Loadouts[i].Weapon.CriticalStrikeChancePerLevel);
                    Loadouts[i].Weapon.TotalCriticalStrikeDamage = Loadouts[i].Weapon.BaseCriticalStrikeDamage + (Loadouts[i].Weapon.ThreadCountLevel * Loadouts[i].Weapon.CriticalStrikeDamagePerLevel);
                }

                //Make One Weapon Visable
                if (Loadouts[CurrentlyEquippedIndex].Weapon.WeaponModel != null)
                {
                    Loadouts[CurrentlyEquippedIndex].Weapon.WeaponModel.SetActive(true);
                }

                print("Updated Loadout Stats");
            }
        }
        catch(NullReferenceException ex)
        {
            print("Prevented Caught Null Reference From Changing Weapons. Error successfully avoided. " + ex.Message);
        }
    }
    public void SpawnAdventureItem()
    {
        if(AdventureItemSlots != null)
        {
            for (int i = 0; i < AdventureItemSlots.Count; i++)
            {
                if(AdventureItemSlots[i].AdventureObject != null)
                {
                    //Spawn Adventure Object And Its Associated Animation Set Here
                    //AdventureItemSlots[i].AdventureObject.GetComponent<ChubaiFormation>().EnableForm();
                }
            }
        }
    }

    //Equip Null To Clear Stats
    public void EquipMask(int MaskIndex, Stash Inventory, int LoadoutIndex)
    {
        //Delete Object
        if (Mask.Model != null)
        {
            Mask.Model.GetComponent<Mask>().RemoveStats(Mask.ThreadCountLevel);
            Destroy(Mask.Model);
        }

        //Held Items
        Mask.ThreadCountLevel = Inv.PlayerInventory.Masks[MaskIndex].ThreadCountLevel;
        Mask.ThreadCountLevelCap = 100;

        //Instantiate New Copy
        Mask.Model = Instantiate(Inventory.PlayerInventory.Masks[MaskIndex].Model, transform);
        Mask.Model.GetComponent<Mask>().Inv = Inv;
        Mask.Model.GetComponent<Mask>().currentEquipment = GetComponent<Equipper>();
        Mask.Model.GetComponent<Mask>().PlayerStats = GetComponent<EntityStats>();

        //Update Current Stats To Thread Count Level First And Add to Player stats
        Mask.Model.GetComponent<Mask>().UpdateStats(Mask.ThreadCountLevel);
    }
    public void ClearMask(int MaskIndex, Stash Inventory, int LoadoutIndex)
    {
        //Delete Object
        if (Inventory.PlayerInventory.Masks[MaskIndex].Model != null)
        {
            Mask.Model.GetComponent<Mask>().RemoveStats(Mask.ThreadCountLevel);
            Destroy(Mask.Model);
        }
    }
    //Equip Null To Clear Stats
    public void EquipQuiltery(QuilteryItem Item, int QuilteryIndex, Stash Inventory, int LoadoutIndex)
    {
        //Delete Object
        for (int i = 0; i < Quiltery.Count; i++)
        {
            if (Quiltery[i].Model != null && Quiltery[i].ItemType.Equals(Item.ItemType))
            {
                Quiltery[i].Model.GetComponent<Quiltery>().RemoveStats();
                Destroy(Quiltery[i].Model);
            }
        }

        //Instantiate New Copy
        Item.Model = Instantiate(Inventory.PlayerInventory.Quiltery[QuilteryIndex].Model, transform);
        Item.Model.GetComponent<Quiltery>().Inv = Inv;
        Item.Model.GetComponent<Quiltery>().currentEquipment = GetComponent<Equipper>();
        Item.Model.GetComponent<Quiltery>().PlayerStats = GetComponent<EntityStats>();

        //Update Current Stats To Thread Count Level First And Add to Player stats
        Item.Model.GetComponent<Quiltery>().UpdateStats();
    }
    public void ClearQuiltery(QuilteryItem Item, int QuilteryIndex, Stash Inventory, int LoadoutIndex)
    {
        //Delete Object
        if (Inventory.PlayerInventory.Quiltery[QuilteryIndex].Model != null)
        {
            for (int i = 0; i < Quiltery.Count; i++)
            {
                if (Quiltery[i].ItemType.Equals(Item.ItemType))
                {
                    Quiltery[i].Model.GetComponent<Quiltery>().RemoveStats();
                    Destroy(Quiltery[i].Model);
                }
            }
        }
    }
    //Equip Null To Clear Stats
    public void EquipThimble (ThimbleItem Item, int ThimbleIndex, Stash Inventory, int LoadoutIndex)
    {
        //Delete Object
        if (Thimble.Model != null)
        {
            Thimble.Model.GetComponent<Thimble>().RemoveStats(Thimble.ThreadCountLevel);
            Destroy(Thimble.Model);
        }

        //Held Items
        Thimble.ThreadCountLevel = Inv.PlayerInventory.Thimbles[ThimbleIndex].ThreadCountLevel;
        Thimble.ThreadCountLevelCap = 100;

        //Instantiate New Copy
        Item.Model = Instantiate(Inventory.PlayerInventory.Thimbles[ThimbleIndex].Model, transform);
        Item.Model.GetComponent<Thimble>().Inv = Inv;
        Item.Model.GetComponent<Thimble>().currentEquipment = GetComponent<Equipper>();
        Item.Model.GetComponent<Thimble>().PlayerStats = GetComponent<EntityStats>();

        //Update Current Stats To Thread Count Level First And Add to Player stats
        Item.Model.GetComponent<Thimble>().UpdateStats(Thimble.ThreadCountLevel);
    }
    public void ClearThimble(ThimbleItem Item, int ThimbleIndex, Stash Inventory, int LoadoutIndex)
    {
        //Delete Object
        if (Inventory.PlayerInventory.Thimbles[ThimbleIndex].Model != null)
        {
            Thimble.Model.GetComponent<Thimble>().RemoveStats(Thimble.ThreadCountLevel);
            Destroy(Thimble.Model);
        }

        //Held Items
        Thimble.ThreadCountLevel = 0;
        Thimble.ThreadCountLevelCap = 100;
        Thimble.ItemType = InventoryItemType.Thimble;
        Thimble.Thimble = ThimbleName.Null;
    }
    //Equip Null To Clear Stats
    public void EquipSkill(MemoryItem SkillToEquip, int SkillSlot , int LoadoutSlot, Stash Inventory)
    {
        Loadouts[LoadoutSlot].Skills[SkillSlot].SkillName = SkillToEquip.ItemType.ToString();
        Loadouts[LoadoutSlot].Skills[SkillSlot].SkillDamageScaling = SkillToEquip.SkillDamageScaling;
        Loadouts[LoadoutSlot].Skills[SkillSlot].TensionCost = SkillToEquip.TensionCost;
        Loadouts[LoadoutSlot].Skills[SkillSlot].SkillObject = Instantiate(SkillToEquip.SkillObject, transform);
        Loadouts[LoadoutSlot].Skills[SkillSlot].ThreadCountLevel = SkillToEquip.ThreadCountLevel;
        Loadouts[LoadoutSlot].Skills[SkillSlot].ThreadCountLevelCap = 100;

        Loadouts[LoadoutSlot].Skills[SkillSlot].SkillObject.GetComponent<MemorySkill>().Inv = Inv;
        Loadouts[LoadoutSlot].Skills[SkillSlot].SkillObject.GetComponent<MemorySkill>().currentEquipment = GetComponent<Equipper>();
    }

    public void ClearSkill(MemoryItem SkillToEquip, int SkillSlot, int LoadoutSlot, Stash Inventory)
    {
        Loadouts[LoadoutSlot].Skills[SkillSlot].SkillName = "";
        Loadouts[LoadoutSlot].Skills[SkillSlot].SkillDamageScaling = 0;
        Loadouts[LoadoutSlot].Skills[SkillSlot].TensionCost = 0;
        Destroy(Loadouts[LoadoutSlot].Skills[SkillSlot].SkillObject);
        Loadouts[LoadoutSlot].Skills[SkillSlot].ThreadCountLevel = 0;
        Loadouts[LoadoutSlot].Skills[SkillSlot].ThreadCountLevelCap = 100;
    }
    //Copy Stats Over From Inventory To Loadout
    //Calls Update Methods all over again upon switching put a small cooldown between switching
    void SwitchWeapon()
    {
        float mouseX = Input.GetAxis("Mouse ScrollWheel");

        if (mouseX > 0)
        {
            CurrentlyEquippedIndex += 1;
            CalculateWeapon();
            print(CurrentlyEquippedIndex);
        }

        if (mouseX < 0)
        {
            CurrentlyEquippedIndex -= 1;
            CalculateWeapon();
            print(CurrentlyEquippedIndex);
        }

        if (CurrentlyEquippedIndex < 0)
        {
            CurrentlyEquippedIndex = 2;
            CalculateWeapon();
            print(CurrentlyEquippedIndex);
        }

        if (CurrentlyEquippedIndex > 2)
        {
            CurrentlyEquippedIndex = 0;
            CalculateWeapon();
            print(CurrentlyEquippedIndex);
        }

        if (mouseX < 0 && CurrentlyEquippedIndex < 0)
        {
            CurrentlyEquippedIndex = 2;
            CalculateWeapon();
            print(CurrentlyEquippedIndex);
        }

        if (mouseX > 0 && CurrentlyEquippedIndex > 2)
        {
            CurrentlyEquippedIndex = 0;
            CalculateWeapon();
            print(CurrentlyEquippedIndex);
        }
    }
}