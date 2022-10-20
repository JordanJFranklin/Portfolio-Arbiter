using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HealthClasses;
using StateClasses;
using StatClasses;
using DamageClass;
using InventoryClass;

public class EntityStats : MonoBehaviour
{
    public PlayerStatMap PlayerSheet;
    public EnemyStatMap EnemySheet;
    public BonusBuffSheet BuffSheet;
    public DebuffSheet DebuffSheet;

    private EntityHealth HealthHandler;
    private Stash Inventory;
    private Equipper Equipment;
    
    // Start is called before the first frame update
    void Start()
    {
        HealthHandler = GetComponent<EntityHealth>();
        Inventory = GetComponent<Stash>();
        Equipment = GetComponent<Equipper>();
        SetBaseStats();
        HealthHandler.IndexFlags();
        HealthHandler.SetHealth();
    }

    // Update is called once per frame
    void Update()
    {
        LimitStats();
    }

    private void SetBaseStats()
    {
        if (HealthHandler.Identity.Equals(EntityTarget.Player))
        {
            //Health Stats
            HealthHandler.Health[HealthHandler.BaseHealthIndex].HealthCap = 25 + (InventoryEvents.ReturnBasicItemCount(InventoryItemType.NightmareTether, Inventory) * PlayerSheet.NightmareTethersPerBonus);
            PlayerSheet.Health = HealthHandler.Health[HealthHandler.BaseHealthIndex].HealthCap;
            HealthHandler.Health[HealthHandler.GuardHealthIndex].HealthCap = 10 + (InventoryEvents.ReturnBasicItemCount(InventoryItemType.GuardiaCrest, Inventory) * PlayerSheet.GuardiaCrestsPerBonus);
            PlayerSheet.GuardIntegrity = HealthHandler.Health[HealthHandler.GuardHealthIndex].HealthCap;
            HealthHandler.Health[HealthHandler.DreamHealthIndex].HealthCap = Mathf.RoundToInt((25 + (InventoryEvents.ReturnBasicItemCount(InventoryItemType.NightmareTether, Inventory) * PlayerSheet.NightmareTethersPerBonus)) * 0.25f);
            HealthHandler.Health[HealthHandler.NightmareHealthIndex].HealthCap = Mathf.RoundToInt((25 + (InventoryEvents.ReturnBasicItemCount(InventoryItemType.NightmareTether, Inventory) * PlayerSheet.NightmareTethersPerBonus)) * 0.25f);
            
            //Survivabillity
            PlayerSheet.GuardRegeneration = 1;
            PlayerSheet.PhysicalDamageFlatNumberReduction = 0;
            PlayerSheet.MagicalDamageFlatNumberReduction = 0;


            //Damage Stats
            //Critical Stats Load In on a by item basis when needed instead of here
            PlayerSheet.PhysicalAtk = PlayerSheet.CurrWeaponDamage;
            PlayerSheet.MagicalAtk = PlayerSheet.CurrWeaponDamage;
            PlayerSheet.AttackSpeed = 0;
            PlayerSheet.TensionLimit = 10 + (InventoryEvents.ReturnBasicItemCount(InventoryItemType.DreamSpool, Inventory) * PlayerSheet.DreamSpoolPerBonus);
            PlayerSheet.PhysicalCriticalStrikeChance = 0;
            PlayerSheet.PhysicalCriticalStrikeDamage = 0;
            PlayerSheet.MagicalCriticalStrikeChance = 0;
            PlayerSheet.MagicalCriticalStrikeDamage = 0;


            //Additive Bonuses
            //PlayerSheet.ParryDamageBonus = BuffSheet.ParryDamageBonus;
            //PlayerSheet.NeedleDamageBonus = BuffSheet.NeedleDamageBonus;
            //PlayerSheet.ScissorDamageBonus = BuffSheet.ScissorDamageBonus;
            //PlayerSheet.SpundleDamageBonus = BuffSheet.SpundleDamageBonus;
            //PlayerSheet.NightmareDamageBonus = BuffSheet.NightmareDamageBonus;
            //PlayerSheet.DreamDamageBonus = BuffSheet.DreamDamageBonus;

            //Utility Stats
            PlayerSheet.MovementSpeedBonus = 5;
            PlayerSheet.TensionReductionSpeed = 1;

            //Flasks
            InventoryEvents.SetFlaskCapacity(InventoryItemType.VermillionVessel, Inventory, 1 + (InventoryEvents.ReturnBasicItemCount(InventoryItemType.SpiritGrace, Inventory) * PlayerSheet.SpiritGracePerBonus));
            InventoryEvents.SetFlaskCapacity(InventoryItemType.SoulweaverSpool, Inventory, 1 + (InventoryEvents.ReturnBasicItemCount(InventoryItemType.SylkeEssence, Inventory) * PlayerSheet.SylkeEssencePerBonus));

            //Add All Current Loadout Information
            Equipment.CalculateItems();

            print("SET PLAYER STATS");
        }
    }

    public void ResetBaseStats()
    {
        if (HealthHandler.Identity.Equals(EntityTarget.Player))
        {
            //Health Stats
            HealthHandler.Health[HealthHandler.BaseHealthIndex].HealthCap = 25 + (InventoryEvents.ReturnBasicItemCount(InventoryItemType.NightmareTether, Inventory) * PlayerSheet.NightmareTethersPerBonus);
            HealthHandler.Health[HealthHandler.GuardHealthIndex].HealthCap = 10 + (InventoryEvents.ReturnBasicItemCount(InventoryItemType.GuardiaCrest, Inventory) * PlayerSheet.GuardiaCrestsPerBonus);
            HealthHandler.Health[HealthHandler.DreamHealthIndex].HealthCap = Mathf.RoundToInt((25 + (InventoryEvents.ReturnBasicItemCount(InventoryItemType.NightmareTether, Inventory) * PlayerSheet.NightmareTethersPerBonus)) * 0.25f);
            HealthHandler.Health[HealthHandler.NightmareHealthIndex].HealthCap = Mathf.RoundToInt((25 + (InventoryEvents.ReturnBasicItemCount(InventoryItemType.NightmareTether, Inventory) * PlayerSheet.NightmareTethersPerBonus)) * 0.25f);

            //Survivabillity
            PlayerSheet.GuardRegeneration = 1;
            PlayerSheet.PhysicalDamageFlatNumberReduction = 0;
            PlayerSheet.MagicalDamageFlatNumberReduction = 0;


            //Damage Stats
            //Critical Stats Load In on a by item basis when needed instead of here
            PlayerSheet.PhysicalAtk = 5;
            PlayerSheet.MagicalAtk = 5;
            PlayerSheet.AttackSpeed = 0;
            PlayerSheet.TensionLimit = 10 + (InventoryEvents.ReturnBasicItemCount(InventoryItemType.DreamSpool, Inventory) * PlayerSheet.DreamSpoolPerBonus);
            PlayerSheet.PhysicalCriticalStrikeChance = 0;
            PlayerSheet.PhysicalCriticalStrikeDamage = 0;
            PlayerSheet.MagicalCriticalStrikeChance = 0;
            PlayerSheet.MagicalCriticalStrikeDamage = 0;


            //Additive Bonuses
            /*PlayerSheet.ParryDamageBonus = BuffSheet.ParryDamageBonus;
            PlayerSheet.NeedleDamageBonus = BuffSheet.NeedleDamageBonus;
            PlayerSheet.ScissorDamageBonus = BuffSheet.ScissorDamageBonus;
            PlayerSheet.SpundleDamageBonus = BuffSheet.SpundleDamageBonus;
            PlayerSheet.NightmareDamageBonus = BuffSheet.NightmareDamageBonus;
            PlayerSheet.DreamDamageBonus = BuffSheet.DreamDamageBonus;*/

            //Utility Stats
            PlayerSheet.MovementSpeedBonus = 5;
            PlayerSheet.TensionReductionSpeed = 1;

            //Flasks
            InventoryEvents.SetFlaskCapacity(InventoryItemType.VermillionVessel, Inventory, 1 + (InventoryEvents.ReturnBasicItemCount(InventoryItemType.SpiritGrace, Inventory) * PlayerSheet.SpiritGracePerBonus));
            InventoryEvents.SetFlaskCapacity(InventoryItemType.SoulweaverSpool, Inventory, 1 + (InventoryEvents.ReturnBasicItemCount(InventoryItemType.SylkeEssence, Inventory) * PlayerSheet.SylkeEssencePerBonus));

            //Add All Current Loadout Information
            Equipment.CalculateItems();

            print("SET PLAYER STATS");
        }
    }

    void LimitStats()
    {
        if(HealthHandler.Identity.Equals(EntityTarget.Player))
        {
            PlayerSheet.CurrentTension = Mathf.Clamp(PlayerSheet.CurrentTension, 0, PlayerSheet.TensionLimit);
        }
    }
}
