using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StatClasses;
using MemorySystem;
using System.IO;
using System;

public enum WeaponType { Needle, Scissors, Bow, SpinningRod}
public enum InventoryItemType {NightmareTether, DreamSpool, ScarletThread, Valence, GuardiaCrest, DungeonItem, Mask, Thimble, Memory, Quiltery, AdventureItems, VermillionVessel, SoulweaverSpool, SpiritGrace, SylkeEssence, MovementSkill, QuiltedQuiver, WispOfTheEnders}
public enum InventorySkillSlotType {Needle, Scissors, SpinningRod, Bow, Any}
public enum SpecificAdventureItem {Empty,MagicalLantern, WindGazer}
public enum MovementAbilities {NotchKick, SpinningSpool, CardinalCowl, RotaryHook, WakingDescent, HyphinFeather}
public enum MasksName {Null, WhiteShell,HarkenHornedMask,UmbralMask}
public enum MemoryName {Null, PiercingDart}
public enum QuilteryName {Null, LostKingSatchel}
public enum ThimbleName {Null, SilverCrestChestPlate}

namespace InventoryClass
{
    [System.Serializable]
    public class BasicInventoryItem
    {
        public InventoryItemType Item;
        public int ItemCount = 0;
        public int ItemCap = 999;

        public BasicInventoryItem(InventoryItemType item, int count, int cap)
        {
            this.Item = item;
            this.ItemCount = count;
            this.ItemCap = cap;
        }
    }

    [System.Serializable]
    public class DungeonItem
    {
        public InventoryItemType ItemType = InventoryItemType.DungeonItem;
        public string KeyItemName;
        public GameObject KeyItem;
        public int ItemCount;
    }

    [System.Serializable]
    public class AdventureItem
    {
        public InventoryItemType Item = InventoryItemType.AdventureItems;
        public SpecificAdventureItem adventureItem;
        public GameObject AdventureObject;
    }

    [System.Serializable]
    public class MaskItem
    {
        public GameObject Model;
        public InventoryItemType ItemType = InventoryItemType.Mask;
        public MasksName Mask;

        [Header("Upgrades")]
        public int ThreadCountLevel;
        public int ThreadCountLevelCap = 100;
    }

    [System.Serializable]
    public class ThimbleItem
    {
        public GameObject Model;
        public InventoryItemType ItemType = InventoryItemType.Thimble;
        public ThimbleName Thimble;

        [Header("Upgrades")]
        public int ThreadCountLevel;
        public int ThreadCountLevelCap = 100;
    }

    [System.Serializable]
    public class MemoryItem
    {
        public MemoryName Memory;
        public InventoryItemType ItemType = InventoryItemType.Memory;
        public InventorySkillSlotType SlotType;
        public float SkillDamageScaling;
        public int TensionCost;
        public GameObject SkillObject;

        [Header("Upgrades")]
        public int ThreadCountLevel;
        public int ThreadCountLevelCap = 100;
    }

    [System.Serializable]
    public class QuilteryItem
    {
        public GameObject Model;
        public InventoryItemType ItemType = InventoryItemType.Quiltery;
        public QuilteryName Quilter;
    }

    [System.Serializable]
    public class MovementAbilityUnlocks
    {
        public InventoryItemType ItemType;
        public MovementAbilities MovementAbillity;

        public MovementAbilityUnlocks(InventoryItemType ItemType, MovementAbilities Movement)
        {
            this.ItemType = ItemType;
            this.MovementAbillity = Movement;
        }
    }

    [System.Serializable]
    public class Inventory
    {
        public List<BasicInventoryItem> BasicItems;
        public List<MovementAbilityUnlocks> MovementUnlocks;
        public List<DungeonItem> DungeonItems;
        public List<AdventureItem> AdventureBag;
        public List<MaskItem> Masks;
        public List<ThimbleItem> Thimbles;
        public List<MemoryItem> Memories;
        public List<QuilteryItem> Quiltery;
    }

    [System.Serializable]
    public class MainWeapon
    {
        [Header("Base")]
        public bool unEquipPreventionLock;
        public GameObject WeaponModel;
        public WeaponType Type;

        [Header("Total")]
        public int TotalDamage;
        public float TotalCriticalStrikeChance;
        public float TotalCriticalStrikeDamage;

        [Header("Base")]
        public int BaseDamage;
        public float BaseCriticalStrikeChance;
        public float BaseCriticalStrikeDamage;

        [Header("Upgrades")]
        public int ThreadCountLevel;
        public int ThreadCountLevelCap = 100;
        public int BaseDamageGrowthPerLevel = 1;
        public int CriticalStrikeChancePerLevel;
        public float CriticalStrikeDamagePerLevel;
    }

    [System.Serializable]
    public class Loadout
    {
        public bool LoadoutUnlocked = false;
        public MainWeapon Weapon;
        public List<Memory> Skills = new List<Memory>();
    }

    public class InventoryEvents
    {
        //Adding and Subtracting To Inventory
        public static void AddBasicItemToInventory(BasicInventoryItem Item, Stash Inventory, int AmountToAdd)
        {
            //Loop To Find Typing Match
            for (int i = 0; i < Inventory.PlayerInventory.BasicItems.Count; i++)
            {
                Inventory.PlayerInventory.BasicItems[i].ItemCount = Mathf.Clamp((int)Inventory.PlayerInventory.BasicItems[i].ItemCount, 0, (int) Inventory.PlayerInventory.BasicItems[i].ItemCap);

                //Match Found
                if (Inventory.PlayerInventory.BasicItems[i].Item.Equals(Item.Item))
                {
                    Inventory.PlayerInventory.BasicItems[i].ItemCount += AmountToAdd;
                    Debug.Log("Added " + AmountToAdd + " of " + Item.Item.ToString() + " Total : " + Inventory.PlayerInventory.BasicItems[i].ItemCount);
                    return;
                }
            }
        }

        public static void SubtractBasicItemFromInventory(BasicInventoryItem Item, Stash Inventory, int AmountToSubtract)
        {
            //Loop To Find Typing Match
            for (int i = 0; i < Inventory.PlayerInventory.BasicItems.Count; i++)
            {
                Inventory.PlayerInventory.BasicItems[i].ItemCount = Mathf.Clamp((int)Inventory.PlayerInventory.BasicItems[i].ItemCount, 0, (int)Inventory.PlayerInventory.BasicItems[i].ItemCap);

                //Match Found
                if (Inventory.PlayerInventory.BasicItems[i].Item.Equals(Item.Item))
                {
                    Inventory.PlayerInventory.BasicItems[i].ItemCount -= AmountToSubtract;
                    Debug.Log("Subtracted " + AmountToSubtract + " of " + Item.Item.ToString() + " Total : " + Inventory.PlayerInventory.BasicItems[i].ItemCount);
                    return;
                }
            }
        }

        public static void AddDungeonItem(string ItemName, Stash Inventory, int AmountToAdd, GameObject Object)
        {
            //Loop To Find Typing Match
            for (int i = 0; i < Inventory.PlayerInventory.DungeonItems.Count; i++)
            {
                Inventory.PlayerInventory.BasicItems[i].ItemCount = Mathf.Clamp((int)Inventory.PlayerInventory.BasicItems[i].ItemCount, 0, (int) Inventory.PlayerInventory.BasicItems[i].ItemCap);

                //Match Found
                if (Inventory.PlayerInventory.DungeonItems[i].KeyItemName == ItemName)
                {
                    Inventory.PlayerInventory.DungeonItems[i].ItemCount += AmountToAdd;
                    Debug.Log("Added " + AmountToAdd + " of " + Inventory.PlayerInventory.DungeonItems[i].ItemType.ToString() + " Total : " + Inventory.PlayerInventory.DungeonItems[i].ItemCount);
                    return;
                }
            }

            //Create New Entry If No Copies Found
            DungeonItem Item = new DungeonItem();

            Item.ItemCount = 1;
            Item.ItemType = InventoryItemType.DungeonItem;
            Item.KeyItemName = ItemName;
            Item.KeyItem = Object;

            Inventory.PlayerInventory.DungeonItems.Add(Item);
            return;
        }

        public static void SubtractDungeonItem(string ItemName, Stash Inventory, int AmountToSubtract)
        {
            //Loop To Find Typing Match
            for (int i = 0; i < Inventory.PlayerInventory.DungeonItems.Count; i++)
            {
                //Match Found
                if (Inventory.PlayerInventory.DungeonItems[i].KeyItemName == ItemName)
                {
                    Inventory.PlayerInventory.DungeonItems[i].ItemCount -= AmountToSubtract;
                    Debug.Log("Subtracted " + AmountToSubtract + " of " + Inventory.PlayerInventory.DungeonItems[i].ItemType.ToString() + " Total : " + Inventory.PlayerInventory.DungeonItems[i].ItemCount);
                    
                    if(Inventory.PlayerInventory.DungeonItems[i].ItemCount <= 0)
                    {
                        Inventory.PlayerInventory.DungeonItems.Remove(Inventory.PlayerInventory.DungeonItems[i]);
                    }
                    
                    return;
                }
            }
        }

        public static void AddMemoryToInventory(MemoryItem Skill, Stash Inventory, MemorySkill Memory, GameObject Player, int ThreadCountLevel)
        {
            //Checks Existance
            if (!ContainsMemory(Skill, Inventory))
            {
                Inventory.PlayerInventory.Memories.Add(Skill);

                //YOU LEFT OFF OF MEMORIES!!!
                Inventory.PlayerInventory.Memories[Inventory.PlayerInventory.Memories.IndexOf(Skill)].Memory = Skill.Memory;
                Inventory.PlayerInventory.Memories[Inventory.PlayerInventory.Memories.IndexOf(Skill)].ItemType = InventoryItemType.Memory;
                Inventory.PlayerInventory.Memories[Inventory.PlayerInventory.Memories.IndexOf(Skill)].SlotType = Skill.SlotType;
                Inventory.PlayerInventory.Memories[Inventory.PlayerInventory.Memories.IndexOf(Skill)].SkillDamageScaling = Skill.SkillDamageScaling;
                Inventory.PlayerInventory.Memories[Inventory.PlayerInventory.Memories.IndexOf(Skill)].TensionCost = Skill.TensionCost;
                Inventory.PlayerInventory.Memories[Inventory.PlayerInventory.Memories.IndexOf(Skill)].ThreadCountLevel = ThreadCountLevel;
                Inventory.PlayerInventory.Memories[Inventory.PlayerInventory.Memories.IndexOf(Skill)].ThreadCountLevelCap = 100;
            }
            else
            {
                Debug.Log("Memory Already Exists.");
                return;
            }
        }

        public static void AddQuilteryToInventory(QuilteryItem Accessory, Stash Inventory, GameObject Player)
        {
            //Checks Existance
            if (!ContainsQuiltery(Accessory, Inventory))
            {
                Inventory.PlayerInventory.Quiltery.Add(Accessory);

                //When Adding A New Mask....
                //Copy The Object's Base Stats To The Inventory Entry And Set Default Thread Count Level
                Inventory.PlayerInventory.Quiltery[Inventory.PlayerInventory.Quiltery.IndexOf(Accessory)].ItemType = InventoryItemType.Quiltery;
                //Player.GetComponent<Equipper>().StatUpdater(Inventory.PlayerInventory.Quiltery[Inventory.PlayerInventory.Quiltery.IndexOf(Accessory)].QuilteryStats, Accessory.QuilteryStats, 0);
            }
            else
            {
                Debug.Log("Quiltery Already Exists.");
                return;
            }
        }

        public static void AddThimbleToInventory(ThimbleItem Armor, Stash Inventory, Thimble ThimbleStats, GameObject Player, int ThreadCountLevel)
        {
            //Checks Existance
            if (!ContainsThimble(Armor, Inventory))
            {
                Inventory.PlayerInventory.Thimbles.Add(Armor);

                //When Adding A New Mask....
                //Copy The Object's Base Stats To The Inventory Entry And Set Default Thread Count Level
                Inventory.PlayerInventory.Thimbles[Inventory.PlayerInventory.Thimbles.IndexOf(Armor)].ItemType = InventoryItemType.Thimble;
                //Player.GetComponent<Equipper>().StatUpdater(Inventory.PlayerInventory.Thimbles[Inventory.PlayerInventory.Thimbles.IndexOf(Armor)].BaseThimbleStats, ThimbleStats.Stats, ThreadCountLevel);
                Inventory.PlayerInventory.Thimbles[Inventory.PlayerInventory.Thimbles.IndexOf(Armor)].ThreadCountLevelCap = 100;
            }
            else
            {
                Debug.Log("Thimble Already Exists.");
                return;
            }
        }

        public static void AddMaskToInventory(MaskItem Mask, Stash Inventory, Mask MaskStats, GameObject Player, int ThreadCountLevel)
        {
            //Checks Existance
            if (!ContainsMask(Mask, Inventory))
            {
                Inventory.PlayerInventory.Masks.Add(Mask);

                //When Adding A New Mask....
                //Copy The Object's Base Stats To The Inventory Entry And Set Default Thread Count Level
                Inventory.PlayerInventory.Masks[Inventory.PlayerInventory.Masks.IndexOf(Mask)].ItemType = InventoryItemType.Mask;
                //Player.GetComponent<Equipper>().StatUpdater(Inventory.PlayerInventory.Masks[Inventory.PlayerInventory.Masks.IndexOf(Mask)].MaskStats, MaskStats.Stats, ThreadCountLevel);
                Inventory.PlayerInventory.Masks[Inventory.PlayerInventory.Masks.IndexOf(Mask)].ThreadCountLevelCap = 100;
            }
            else
            {
                Debug.Log("Mask Already Exists.");
                return;
            }
        }

        public static void AddAdventureItemToInventory(AdventureItem ItemToUnlock, Stash Inventory)
        {
            //Checks Existance
            if (!ContainsAdventureItem(ItemToUnlock, Inventory))
            {
                Inventory.PlayerInventory.AdventureBag.Add(ItemToUnlock);

                //Add GameObject Reference
                if (Resources.Load("ChubaiTransformations/" + ItemToUnlock.adventureItem.ToString()) != null)
                {
                    ItemToUnlock.AdventureObject = (GameObject)Resources.Load("ChubaiTransformations/" + ItemToUnlock.AdventureObject.ToString(), typeof(GameObject));
                }
            }
            else
            {
                Debug.Log("Transformation Already Exists. Copy Not Added");
                return;
            }
        }
        //Count Function
        public static int ReturnBasicItemCount(InventoryItemType ItemType, Stash Inventory)
        {
            if (ItemType.Equals(InventoryItemType.DreamSpool))
            {
                for (int i = 0; i < Inventory.PlayerInventory.BasicItems.Count; i++)
                {
                    //Match Found
                    if (Inventory.PlayerInventory.BasicItems[i].Item.Equals(ItemType))
                    {
                        return (int)Inventory.PlayerInventory.BasicItems[i].ItemCount;
                    }
                }
            }

            if (ItemType.Equals(InventoryItemType.GuardiaCrest))
            {
                for (int i = 0; i < Inventory.PlayerInventory.BasicItems.Count; i++)
                {
                    //Match Found
                    if (Inventory.PlayerInventory.BasicItems[i].Item.Equals(ItemType))
                    {
                        return (int)Inventory.PlayerInventory.BasicItems[i].ItemCount;
                    }
                }
            }

            if (ItemType.Equals(InventoryItemType.NightmareTether))
            {
                for (int i = 0; i < Inventory.PlayerInventory.BasicItems.Count; i++)
                {
                    //Match Found
                    if (Inventory.PlayerInventory.BasicItems[i].Item.Equals(ItemType))
                    {
                        return (int)Inventory.PlayerInventory.BasicItems[i].ItemCount;
                    }
                }
            }

            if (ItemType.Equals(InventoryItemType.ScarletThread))
            {
                for (int i = 0; i < Inventory.PlayerInventory.BasicItems.Count; i++)
                {
                    //Match Found
                    if (Inventory.PlayerInventory.BasicItems[i].Item.Equals(ItemType))
                    {
                        return (int)Inventory.PlayerInventory.BasicItems[i].ItemCount;
                    }
                }
            }

            if (ItemType.Equals(InventoryItemType.Valence))
            {
                for (int i = 0; i < Inventory.PlayerInventory.BasicItems.Count; i++)
                {
                    //Match Found
                    if (Inventory.PlayerInventory.BasicItems[i].Item.Equals(ItemType))
                    {
                        return (int)Inventory.PlayerInventory.BasicItems[i].ItemCount;
                    }
                }
            }

            if (ItemType.Equals(InventoryItemType.VermillionVessel))
            {
                for (int i = 0; i < Inventory.PlayerInventory.BasicItems.Count; i++)
                {
                    //Match Found
                    if (Inventory.PlayerInventory.BasicItems[i].Item.Equals(ItemType))
                    {
                        return (int)Inventory.PlayerInventory.BasicItems[i].ItemCount;
                    }
                }
            }

            if (ItemType.Equals(InventoryItemType.SoulweaverSpool))
            {
                for (int i = 0; i < Inventory.PlayerInventory.BasicItems.Count; i++)
                {
                    //Match Found
                    if (Inventory.PlayerInventory.BasicItems[i].Item.Equals(ItemType))
                    {
                        return (int)Inventory.PlayerInventory.BasicItems[i].ItemCount;
                    }
                }
            }

            if (ItemType.Equals(InventoryItemType.SylkeEssence))
            {
                for (int i = 0; i < Inventory.PlayerInventory.BasicItems.Count; i++)
                {
                    //Match Found
                    if (Inventory.PlayerInventory.BasicItems[i].Item.Equals(ItemType))
                    {
                        return (int)Inventory.PlayerInventory.BasicItems[i].ItemCount;
                    }
                }
            }

            if (ItemType.Equals(InventoryItemType.SpiritGrace))
            {
                for (int i = 0; i < Inventory.PlayerInventory.BasicItems.Count; i++)
                {
                    //Match Found
                    if (Inventory.PlayerInventory.BasicItems[i].Item.Equals(ItemType))
                    {
                        return (int)Inventory.PlayerInventory.BasicItems[i].ItemCount;
                    }
                }
            }

            Debug.Log("Count Function Failed To Find The Proper Item For A Count. Error Returned Instead returned -1.");
            return -1;
        }
        //Set Functions
        public static void SetFlaskCapacity(InventoryItemType ItemType, Stash Inventory, int newCapacity)
        {
            if (ItemType.Equals(InventoryItemType.VermillionVessel))
            {
                for (int i = 0; i < Inventory.PlayerInventory.BasicItems.Count; i++)
                {
                    //Match Found
                    if (Inventory.PlayerInventory.BasicItems[i].Item.Equals(ItemType))
                    {
                        Inventory.PlayerInventory.BasicItems[i].ItemCap = newCapacity;
                        Debug.Log("Set Capacity Of Heart Flasks. " + Inventory.PlayerInventory.BasicItems[i].ItemCap);
                        return;
                    }
                }
            }

            if (ItemType.Equals(InventoryItemType.SoulweaverSpool))
            {
                for (int i = 0; i < Inventory.PlayerInventory.BasicItems.Count; i++)
                {
                    //Match Found
                    if (Inventory.PlayerInventory.BasicItems[i].Item.Equals(ItemType))
                    {
                        Inventory.PlayerInventory.BasicItems[i].ItemCap = newCapacity;
                        Debug.Log("Set Capacity Of Crystalline Flasks. " + Inventory.PlayerInventory.BasicItems[i].ItemCap);
                        return;
                    }
                }
            }
        }

        public static void GrantMovementSkill(MovementAbilityUnlocks Ability, Stash Inventory)
        {
            if(ContainsMovementUnlock(Ability,Inventory))
            {
                Debug.LogWarning("You already have this skill");
                return;
            }
            else
            {
                Inventory.PlayerInventory.MovementUnlocks.Add(Ability);
                Debug.Log("You Recieved a new movement skill! Gained " + Ability.MovementAbillity.ToString());
            }
        }
        //Contains Functions
        //This Contains function checks for key name and a quantity to validate if a door/interaction should happen or not
        public static bool ValidateDungeonItem(DungeonItem Item, Stash Inventory, string NameRequirement, int QuantityRequired, bool consumeItem)
        {
            if (Inventory.PlayerInventory.Masks.Count == 0)
            {
                return false;
            }

            for (int i = 0; i < Inventory.PlayerInventory.DungeonItems.Count; i++)
            {
                if (Inventory.PlayerInventory.DungeonItems[i].KeyItemName == NameRequirement && Inventory.PlayerInventory.DungeonItems[i].ItemCount == QuantityRequired)
                {
                    if (consumeItem)
                    {
                        SubtractDungeonItem(NameRequirement, Inventory, QuantityRequired);
                    }

                    return true;
                }
            }

            return false;
        }
        public static bool ContainsMask(MaskItem Mask, Stash Inventory)
        {
            if (Inventory.PlayerInventory.Masks.Count == 0)
            {
                return false;
            }

            if (Inventory.PlayerInventory.Masks.Contains(Mask))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool ContainsMemory(MemoryItem Skill, Stash Inventory)
        {
            if (Inventory.PlayerInventory.Memories.Count == 0)
            {
                return false;
            }

            if (Inventory.PlayerInventory.Memories.Contains(Skill))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool ContainsThimble(ThimbleItem Thimble, Stash Inventory)
        {
            if (Inventory.PlayerInventory.Thimbles.Count == 0)
            {
                return false;
            }

            if (Inventory.PlayerInventory.Thimbles.Contains(Thimble))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool ContainsQuiltery(QuilteryItem Quilt, Stash Inventory)
        {
            if (Inventory.PlayerInventory.Quiltery.Count == 0)
            {
                return false;
            }

            if (Inventory.PlayerInventory.Quiltery.Contains(Quilt))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool ContainsAdventureItem(AdventureItem Item, Stash Inventory)
        {
            if (Inventory.PlayerInventory.AdventureBag.Count == 0)
            {
                return false;
            }

            if (Inventory.PlayerInventory.AdventureBag.Contains(Item))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool ContainsMovementUnlock(MovementAbilityUnlocks Movement, Stash Inventory)
        {
            if (Inventory.PlayerInventory.MovementUnlocks.Count == 0)
            {
                return false;
            }

            foreach (MovementAbilityUnlocks skill in Inventory.PlayerInventory.MovementUnlocks)
            {
                if(skill.MovementAbillity == Movement.MovementAbillity)
                {
                    return true;
                }
            }

            return false;
        }

        static void WriteData(string path, Stash Inv)
        {
            //Write All The Data Into The Inventory
            foreach (BasicInventoryItem Item in Inv.PlayerInventory.BasicItems)
            {
                string itemToSave = Item.Item.ToString() + " " + Item.ItemCount + " \n";
                File.AppendAllText(path, itemToSave);
            }

            foreach (MovementAbilityUnlocks Ability in Inv.PlayerInventory.MovementUnlocks)
            {
                string itemToSave = Ability.ItemType.ToString() + " " + Ability.MovementAbillity.ToString() + " \n";
                File.AppendAllText(path, itemToSave);
            }

            foreach (AdventureItem Item in Inv.PlayerInventory.AdventureBag)
            {
                string itemToSave;

                if (Item.AdventureObject == null)
                {
                    itemToSave = Item.Item.ToString() + " " + Item.adventureItem.ToString() + " \n";
                }
                else
                {
                    itemToSave = Item.Item.ToString() + " " + Item.adventureItem.ToString() + " \n";
                }
                
                File.AppendAllText(path, itemToSave);
            }

            foreach (MaskItem Item in Inv.PlayerInventory.Masks)
            {
                string itemToSave; 

                if(Item.Model == null)
                {
                    itemToSave = Item.ItemType.ToString() + " " + Item.Mask.ToString() + " " + Item.ThreadCountLevel + " " + "NULL" + "\n";
                }
                else
                {
                    itemToSave = Item.ItemType.ToString() + " " + Item.Mask.ToString() + " " + Item.ThreadCountLevel + " " + Item.Model.name + "\n";
                }
                
                File.AppendAllText(path, itemToSave);
            }

            foreach (QuilteryItem Item in Inv.PlayerInventory.Quiltery)
            {
                string itemToSave;

                if (Item.Model == null)
                {
                    itemToSave = Item.ItemType.ToString() + " " + Item.Quilter.ToString() + " " + "NULL" + " \n";
                }
                else
                {
                    itemToSave = Item.ItemType.ToString() + " " + Item.Quilter.ToString() + " " + Item.Model.name + " \n";
                }

                File.AppendAllText(path, itemToSave);
            }

            foreach (ThimbleItem Item in Inv.PlayerInventory.Thimbles)
            {
                string itemToSave;

                if(Item.Model == null)
                {
                    itemToSave = Item.ItemType.ToString() + " " + Item.Thimble.ToString() + " " + Item.ThreadCountLevel + " " + "NULL" + " \n";
                }
                else
                {
                    itemToSave = Item.ItemType.ToString() + " " + Item.Thimble.ToString() + " " + Item.ThreadCountLevel + " " + Item.Model.name + " \n";
                }
             
                File.AppendAllText(path, itemToSave);
            }

            foreach (DungeonItem Item in Inv.PlayerInventory.DungeonItems)
            {
                string itemToSave;

                if (Item.KeyItem == null)
                {
                    itemToSave = Item.ItemType.ToString() + " " + Item.KeyItemName + " " + Item.ItemCount + " " + "NULL" + " \n";
                }
                else
                {
                    itemToSave = Item.ItemType.ToString() + " " + Item.KeyItemName + " " + Item.ItemCount + " " + Item.KeyItem.name + " \n";
                }
                    
                File.AppendAllText(path, itemToSave);
            }

            foreach (MemoryItem Item in Inv.PlayerInventory.Memories)
            {
                string itemToSave;

                if(Item.SkillObject == null)
                {
                    itemToSave = Item.ItemType.ToString() + " " + Item.Memory.ToString() + " " + Item.SlotType.ToString() + " " + Item.ThreadCountLevel + " " + "NULL" + " \n";
                }
                else
                {
                    itemToSave = Item.ItemType.ToString() + " " + Item.Memory.ToString() + " " + Item.SlotType.ToString() + " " + Item.ThreadCountLevel + " " + Item.SkillObject.name + " \n";
                }

                
                File.AppendAllText(path, itemToSave);
            }
        }
        static void LoadData(string path, Stash Inv, EntityStats Stats)
        {
            // Open the file to read from.
            string[] readText = File.ReadAllLines(path);

            //Clear Inventory Of All Items Except Basic Items And Reload Everything From What Is Found In The File
            Inv.PlayerInventory.MovementUnlocks.Clear();
            Inv.PlayerInventory.DungeonItems.Clear();
            Inv.PlayerInventory.AdventureBag.Clear();
            Inv.PlayerInventory.Masks.Clear();
            Inv.PlayerInventory.Thimbles.Clear();
            Inv.PlayerInventory.Memories.Clear();
            Inv.PlayerInventory.Quiltery.Clear();

            //Load In Basic Items From File
            foreach (string s in readText)
            {
                if (s != null)
                {
                    string itemStringName = s.Split(new char[] { ' ' })[0];

                    if (Enum.TryParse<InventoryItemType>(itemStringName, out InventoryItemType yourEnum))
                    {
                        InventoryItemType parsed_enum = (InventoryItemType)System.Enum.Parse(typeof(InventoryItemType), itemStringName);

                        //Loop Through Basic Items
                        for (int i = 0; i < Inv.PlayerInventory.BasicItems.Count; i++)
                        {
                            //Found A Match To Read From And Add To Basic Inventory
                            if (Inv.PlayerInventory.BasicItems[i].Item.Equals(parsed_enum))
                            {
                                string[] separatingStrings = { " " };

                                string[] itemString = s.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

                                int savedCount = Int32.Parse(itemString[1]);

                                Inv.PlayerInventory.BasicItems[i].Item = parsed_enum;
                                Inv.PlayerInventory.BasicItems[i].ItemCount = savedCount;

                                //Potion Caps
                                if (parsed_enum.Equals(InventoryItemType.VermillionVessel))
                                {
                                    Inv.PlayerInventory.BasicItems[i].ItemCap = 1 + (ReturnBasicItemCount(InventoryItemType.VermillionVessel, Inv) * Stats.PlayerSheet.SpiritGracePerBonus);
                                }
                                if (parsed_enum.Equals(InventoryItemType.SoulweaverSpool))
                                {
                                    Inv.PlayerInventory.BasicItems[i].ItemCap = 1 + (ReturnBasicItemCount(InventoryItemType.SoulweaverSpool, Inv) * Stats.PlayerSheet.SylkeEssencePerBonus);
                                }
                                //Valence Cap
                                if (parsed_enum.Equals(InventoryItemType.Valence))
                                {
                                    Inv.PlayerInventory.BasicItems[i].ItemCap = 999999999;
                                }

                                Debug.Log("New Item Loaded Into Inventory: " + Inv.PlayerInventory.BasicItems[i].Item.ToString() + " " + Inv.PlayerInventory.BasicItems[i].ItemCount);
                                break;
                            }
                        }

                        //Get 1st Result And 2nd Parse Result
                        if (parsed_enum.Equals(InventoryItemType.MovementSkill))
                        {
                            string[] separatingStrings = { " " };

                            string[] itemString = s.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

                            MovementAbilityUnlocks Ability = new MovementAbilityUnlocks(parsed_enum, (MovementAbilities)System.Enum.Parse(typeof(MovementAbilities), itemString[1]));

                            Inv.PlayerInventory.MovementUnlocks.Add(Ability);

                            Debug.Log("New Movement Abillity: " + Ability.ToString());
                        }

                        //Get 1st Result And 2nd Parse Result
                        if (parsed_enum.Equals(InventoryItemType.AdventureItems))
                        {
                            AdventureItem NewItem = new AdventureItem();

                            string[] separatingStrings = { " " };

                            string[] itemString = s.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

                            NewItem.Item = parsed_enum;
                            NewItem.adventureItem = (SpecificAdventureItem)System.Enum.Parse(typeof(SpecificAdventureItem), itemString[1]);

                            //Add GameObject Reference
                            if (Resources.Load("AdventureItems/" + itemString[1]) != null)
                            {
                                NewItem.AdventureObject = (GameObject)Resources.Load("AdventureItems/" + itemString[1], typeof(GameObject));
                            }
                            else
                            {
                                Debug.Log("GameObject " + NewItem.Item.ToString() + " Wasn't Found");
                            }

                            Inv.PlayerInventory.AdventureBag.Add(NewItem);

                            Debug.Log("Added New Form To Inventory: " + NewItem.adventureItem.ToString());
                        }

                        //Get 1st Result And 2nd Parse Result
                        if (parsed_enum.Equals(InventoryItemType.DungeonItem))
                        {
                            DungeonItem Item = new DungeonItem();

                            string[] separatingStrings = { " " };
                            string[] itemString = s.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

                            Item.KeyItemName = itemString[1];
                            Item.ItemType = parsed_enum;
                            Item.ItemCount = Int32.Parse(itemString[2]);

                            //Add GameObject Reference
                            if (Resources.Load("DungeonItems/" + itemString[3]) != null)
                            {
                                Item.KeyItem = (GameObject)Resources.Load("DungeonItems/" + itemString[3], typeof(GameObject));
                            }
                            else
                            {
                                Debug.Log("GameObject " + Item.KeyItemName + " Wasn't Found");
                            }

                            Inv.PlayerInventory.DungeonItems.Add(Item);

                            Debug.Log("Added New Dungeon Item To Inventory: " + Item.KeyItemName + " " + Item.ItemCount);
                        }

                        //Get 1st Result And 2nd Parse Result
                        if (parsed_enum.Equals(InventoryItemType.Mask))
                        {
                            MaskItem Item = new MaskItem();

                            string[] separatingStrings = { " " };
                            string[] itemString = s.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

                            Item.Mask = (MasksName)System.Enum.Parse(typeof(MasksName), itemString[1]);
                            Item.ItemType = parsed_enum;
                            Item.ThreadCountLevel = Int32.Parse(itemString[2]);
                            Item.ThreadCountLevelCap = 100;

                            //Add GameObject Reference
                            if (Resources.Load("Masks/" + itemString[3]) != null)
                            {
                                Item.Model = (GameObject)Resources.Load("Masks/" + itemString[3], typeof(GameObject));
                            }
                            else
                            {
                                Debug.Log("GameObject " + Item.Mask.ToString() + " Wasn't Found");
                            }

                            Inv.PlayerInventory.Masks.Add(Item);

                            Debug.Log("Added New Mask Item To Inventory: " + Item.Mask.ToString() + " Thread Count " + Item.ThreadCountLevel);
                        }

                        //Get 1st Result And 2nd Parse Result
                        if (parsed_enum.Equals(InventoryItemType.Memory))
                        {
                            MemoryItem Item = new MemoryItem();

                            string[] separatingStrings = { " " };
                            string[] itemString = s.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

                            Item.Memory = (MemoryName)System.Enum.Parse(typeof(MemoryName), itemString[1]);
                            Item.ItemType = parsed_enum;
                            Item.SlotType = (InventorySkillSlotType)System.Enum.Parse(typeof(InventorySkillSlotType), itemString[2]);
                            Item.ThreadCountLevel = Int32.Parse(itemString[3]);
                            Item.ThreadCountLevelCap = 100;

                            if (Resources.Load("Memories/" + itemString[4]) != null)
                            {
                                Item.SkillObject = (GameObject)Resources.Load("Memories/" + itemString[4], typeof(GameObject));
                            }
                            else
                            {
                                Debug.Log("GameObject " + Item.Memory.ToString() + " Wasn't Found");
                            }

                            Inv.PlayerInventory.Memories.Add(Item);

                            Debug.Log("Added New Memory Item To Inventory: " + Item.Memory.ToString() + " Thread Count " + Item.ThreadCountLevel);
                        }

                        //Get 1st Result And 2nd Parse Result
                        if (parsed_enum.Equals(InventoryItemType.Thimble))
                        {
                            ThimbleItem Item = new ThimbleItem();

                            string[] separatingStrings = { " " };
                            string[] itemString = s.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

                            Item.Thimble = (ThimbleName)System.Enum.Parse(typeof(ThimbleName), itemString[1]);
                            Item.ItemType = parsed_enum;
                            Item.ThreadCountLevel = Int32.Parse(itemString[2]);
                            Item.ThreadCountLevelCap = 100;

                            //Add GameObject Reference
                            if (Resources.Load("Thimbles/" + itemString[3]) != null)
                            {
                                Item.Model = (GameObject)Resources.Load("Thimbles/" + itemString[3], typeof(GameObject));
                            }
                            else
                            {
                                Debug.Log("GameObject " + Item.Thimble.ToString() + " Wasn't Found");
                            }

                            Inv.PlayerInventory.Thimbles.Add(Item);

                            Debug.Log("Added New Thimble Item To Inventory: " + Item.Thimble.ToString() + " Thread Count " + Item.ThreadCountLevel);
                        }

                        //Get 1st Result And 2nd Parse Result
                        if (parsed_enum.Equals(InventoryItemType.Quiltery))
                        {
                            QuilteryItem Item = new QuilteryItem();

                            string[] separatingStrings = { " " };
                            string[] itemString = s.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

                            Item.Quilter = (QuilteryName)System.Enum.Parse(typeof(QuilteryName), itemString[1]);
                            Item.ItemType = parsed_enum;
                            

                            //Add GameObject Reference
                            if(Resources.Load("Quiltery/" + itemString[2]) != null)
                            {
                                Item.Model = (GameObject)Resources.Load("Quiltery/" + itemString[2], typeof(GameObject));
                            }
                            else
                            {
                                Debug.Log("GameObject " + Item.Quilter.ToString() + " Wasn't Found");
                            }

                            Inv.PlayerInventory.Quiltery.Add(Item);

                            Debug.Log("Added New Quiltery Item To Inventory: " + Item.Quilter.ToString());
                        }
                    }

                    //Debug.Log(itemStringName);
                }
            }
        }

        public static void LoadInventory(string path, Stash Inv, EntityStats Stats)
        {
            //Check For File
            if (File.Exists(path))
            {
                Debug.Log("Inventory File Found. Loading In Inventory From File");

                //Check If the document Empty
                if (new FileInfo(path).Length == 0)
                {
                    //File Not Found
                    Debug.LogWarning("WARNING: Inventory File Was Determined To Be Empty. Inventory Is Empty...");

                    //Create Text File To Path
                    File.WriteAllText(path, "");

                    //Write Data To File
                    WriteData(path, Inv);

                    //Load Data From File To Inventory
                    LoadData(path, Inv, Stats);

                    //Load In Resources Via Resources.Load() for gameobject and other detailed data references for each type
                    //Create A Way To Parse The Data and get the correct information back to the right places and put it back into your inventory
                }
                //File Found And Not Empty
                else
                {
                    Debug.Log("Inventory File Data Found. Loading In Data...");

                    //Load Data From File To Inventory
                    LoadData(path, Inv, Stats);
                }
            }
            else
            {
                //File Not Found
                Debug.Log("Inventory File Not Found. Creating Inventory File...");

                //Create Text File To Path
                File.WriteAllText(path, "");

                //Write Data To File
                WriteData(path, Inv);

                //Load Data From File To Inventory
                LoadData(path, Inv, Stats);
            }
        }
        public static void SaveInventory(string path, Stash Inv, EntityStats Stats)
        {
            //Check For File
            if (File.Exists(path))
            {
                //Clear File
                File.WriteAllText(path, string.Empty);

                //Add Text To it
                WriteData(path, Inv);

                Debug.Log("Inventory Saved Successfully");
            }
            else
            {
                //Create Text File To Path
                File.WriteAllText(path, string.Empty);

                //Add Text To it
                WriteData(path, Inv);

                //Create File Fall Back And Then Save
                Debug.Log("Fall Back Method Catch Exception. File Not Found On Save. Creating new file and saving new data set.");
                Debug.Log("Inventory Saved Successfully");
            }
        }
    }
}