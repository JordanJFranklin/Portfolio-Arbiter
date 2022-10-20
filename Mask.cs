using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StatClasses;

public class Mask : MonoBehaviour
{
    [Header("Mask")]
    public ItemStatBonus Stats;
    public Stash Inv;
    public Equipper currentEquipment;
    public EntityStats PlayerStats;

    public void UpdateStats(int Level)
    {
        //Calculate Stats
        Stats.TotalHealth = Stats.BaseHealth + (Level * Stats.PerUpgradeHealth);
        Stats.TotalPhysicalAtk = Stats.BasePhysicalAtk + (Level * Stats.PerUpgradePhysicalAtk);
        Stats.TotalMagicalAtk = Stats.BaseMagicalAtk + (Level * Stats.PerUpgradeMagicalAtk);
        Stats.TotalAttackSpeed = Stats.BaseAttackSpeed + (Level * Stats.PerUpgradeAttackSpeed);
        Stats.TotalGuardIntegrity = Stats.BaseGuardIntegrity + (Level * Stats.PerUpgradeGuardIntegrity);
        Stats.TotalGuardRegeneration = Stats.BaseGuardRegeneration + (Level * Stats.PerUpgradeGuardRegeneration);
        Stats.TotalPhysicalDamageFlatNumberReduction = Stats.BasePhysicalDamageFlatNumberReduction + (Level * Stats.PerUpgradePhysicalDamageFlatNumberReduction);
        Stats.TotalMagicalDamageFlatNumberReduction = Stats.BaseMagicalDamageFlatNumberReduction + (Level * Stats.PerUpgradeMagicalDamageFlatNumberReduction);
        Stats.TotalTensionLimit = Stats.BaseTensionLimit + (Level * Stats.PerUpgradeTensionLimit);
        Stats.TotalTensionReductionSpeed = Stats.BaseTensionReductionSpeed + (Level * Stats.PerUpgradeTensionReductionSpeed);
        Stats.TotalTensionCostReduction = Stats.BaseTensionCostReduction + (Level * Stats.PerUpgradeTensionCostReduction);
        Stats.TotalPhysicalCriticalStrikeChance = Stats.BasePhysicalCriticalStrikeChance + (Level * Stats.PerUpgradePhysicalCriticalStrikeChance);
        Stats.TotalPhysicalCriticalStrikeDamage = Stats.BasePhysicalCriticalStrikeDamage + (Level * Stats.PerUpgradePhysicalCriticalStrikeDamage);
        Stats.TotalMagicalCriticalStrikeChance = Stats.BaseMagicalCriticalStrikeChance + (Level * Stats.PerUpgradeMagicalCriticalStrikeChance);
        Stats.TotalMagicalCriticalStrikeDamage = Stats.BaseMagicalCriticalStrikeDamage + (Level * Stats.PerUpgradeMagicalCriticalStrikeDamage);
        Stats.TotalMovementSpeedBonus = Stats.BaseMovementSpeedBonus + (Level * Stats.PerUpgradeMovementSpeedBonus);
        Stats.TotalParryDamageBonus = Stats.BaseParryDamageBonus + (Level * Stats.PerUpgradeParryDamageBonus);
        Stats.TotalNeedleDamageBonus = Stats.BaseNeedleDamageBonus + (Level * Stats.PerUpgradeNeedleDamageBonus);
        Stats.TotalScissorDamageBonus = Stats.BaseScissorDamageBonus + (Level * Stats.PerUpgradeScissorDamageBonus);
        Stats.TotalSpinningRodDamageBonus = Stats.BaseSpinningRodDamageBonus + (Level * Stats.PerUpgradeSpinningRodDamageBonus);
        Stats.TotalNightmareDamageBonus = Stats.BaseNightmareDamageBonus + (Level * Stats.PerUpgradeNightmareDamageBonus);
        Stats.TotalDreamDamageBonus = Stats.BaseDreamDamageBonus + (Level * Stats.PerUpgradeDreamDamageBonus);

        Stats.TotalStatusChance = Stats.BaseStatusChance + (Level * Stats.PerUpgradeStatusChance);
        Stats.TotalCriticalChanceReduction = Stats.BaseCriticalChanceReduction + (Level * Stats.PerUpgradeCriticalChanceReduction);
        Stats.TotalShieldDamageAbsorption = Stats.BaseShieldDamageAbsorption + (Level * Stats.PerUpgradeShieldDamageAbsorption);
        Stats.TotalAllElementalDamageBonus = Stats.BaseAllElementalDamageBonus + (Level * Stats.PerUpgradeAllElementalDamageBonus);
        Stats.TotalFireDamageBonus = Stats.BaseFireDamageBonus + (Level * Stats.PerUpgradeFireDamageBonus);
        Stats.TotalFrostDamageBonus = Stats.BaseFrostDamageBonus + (Level * Stats.PerUpgradeFrostDamageBonus);
        Stats.TotalThunderDamageBonus = Stats.BaseThunderDamageBonus + (Level * Stats.PerUpgradeThunderDamageBonus);
        Stats.TotalWaterDamageBonus = Stats.BaseWaterDamageBonus + (Level * Stats.PerUpgradeWaterDamageBonus);
        Stats.TotalWindDamageBonus = Stats.BaseWindDamageBonus + (Level * Stats.PerUpgradeWindDamageBonus);
        Stats.TotalElementalDamageResistance = Stats.BaseElementalDamageResistance + (Level * Stats.PerUpgradeElementalDamageResistance);
        Stats.TotalFireChanceResistance = Stats.BaseFireChanceResistance + (Level * Stats.PerUpgradeFireChanceResistance);
        Stats.TotalFrostChanceResistance = Stats.BaseFrostChanceResistance + (Level * Stats.PerUpgradeFrostChanceResistance);
        Stats.TotalThunderChanceResistance = Stats.BaseThunderChanceResistance + (Level * Stats.PerUpgradeThunderChanceResistance);
        Stats.TotalWaterChanceResistance = Stats.BaseWaterChanceResistance + (Level * Stats.PerUpgradeWaterChanceResistance);
        Stats.TotalWindChanceResistance = Stats.BaseWindChanceResistance + (Level * Stats.PerUpgradeWindChanceResistance);
        Stats.TotalSummoningPoints = Stats.BaseSummoningPoints + (Level * Stats.PerUpgradeSummoningPoints);
        Stats.TotalNestleDamageBonus = Stats.BaseNestleDamageBonus + (Level * Stats.PerUpgradeNestleDamageBonus);

        //Add Stats
        PlayerStats.PlayerSheet.Health += Stats.TotalHealth;
        PlayerStats.PlayerSheet.PhysicalAtk += Stats.TotalPhysicalAtk;
        PlayerStats.PlayerSheet.MagicalAtk += Stats.TotalMagicalAtk;
        PlayerStats.PlayerSheet.AttackSpeed += Stats.TotalAttackSpeed;
        PlayerStats.PlayerSheet.GuardIntegrity += Stats.TotalGuardIntegrity;
        PlayerStats.PlayerSheet.GuardRegeneration += Stats.TotalGuardRegeneration;
        PlayerStats.PlayerSheet.PhysicalDamageFlatNumberReduction += Stats.TotalPhysicalDamageFlatNumberReduction;
        PlayerStats.PlayerSheet.MagicalDamageFlatNumberReduction += Stats.TotalMagicalDamageFlatNumberReduction;
        PlayerStats.PlayerSheet.TensionLimit += Stats.TotalTensionLimit;
        PlayerStats.PlayerSheet.TensionReductionSpeed += Stats.TotalTensionReductionSpeed;
        PlayerStats.PlayerSheet.TensionCostReduction += Stats.TotalTensionCostReduction;
        PlayerStats.PlayerSheet.PhysicalCriticalStrikeChance += Stats.TotalPhysicalCriticalStrikeChance;
        PlayerStats.PlayerSheet.PhysicalCriticalStrikeDamage += Stats.TotalPhysicalCriticalStrikeDamage;
        PlayerStats.PlayerSheet.MagicalCriticalStrikeChance += Stats.TotalMagicalCriticalStrikeChance;
        PlayerStats.PlayerSheet.MagicalCriticalStrikeDamage += Stats.TotalMagicalCriticalStrikeDamage;
        PlayerStats.PlayerSheet.MovementSpeedBonus += Stats.TotalMovementSpeedBonus;
        PlayerStats.PlayerSheet.ParryDamageBonus += Stats.TotalParryDamageBonus;
        PlayerStats.PlayerSheet.NeedleDamageBonus += Stats.TotalNeedleDamageBonus;
        PlayerStats.PlayerSheet.ScissorDamageBonus += Stats.TotalScissorDamageBonus;
        PlayerStats.PlayerSheet.SpinningRodDamageBonus += Stats.TotalSpinningRodDamageBonus;
        PlayerStats.PlayerSheet.NightmareDamageBonus += Stats.TotalNightmareDamageBonus;
        PlayerStats.PlayerSheet.DreamDamageBonus += Stats.TotalDreamDamageBonus;

        PlayerStats.PlayerSheet.NestleDamageBonus += Stats.TotalNestleDamageBonus;
        PlayerStats.PlayerSheet.StatusChance += Stats.TotalStatusChance;
        PlayerStats.PlayerSheet.CriticalChanceReduction += Stats.TotalCriticalChanceReduction;
        PlayerStats.PlayerSheet.ShieldDamageAbsorption += Stats.TotalShieldDamageAbsorption;
        PlayerStats.PlayerSheet.AllElementalDamageBonus += Stats.TotalAllElementalDamageBonus;
        PlayerStats.PlayerSheet.FireDamageBonus += Stats.TotalFireDamageBonus;
        PlayerStats.PlayerSheet.FrostDamageBonus += Stats.TotalFrostDamageBonus;
        PlayerStats.PlayerSheet.ThunderDamageBonus += Stats.TotalThunderDamageBonus;
        PlayerStats.PlayerSheet.WaterDamageBonus += Stats.TotalWaterDamageBonus;
        PlayerStats.PlayerSheet.WindDamageBonus += Stats.TotalWindDamageBonus;
        PlayerStats.PlayerSheet.ElementalDamageResistance += Stats.TotalElementalDamageResistance;
        PlayerStats.PlayerSheet.FireChanceResistance += Stats.TotalFireChanceResistance;
        PlayerStats.PlayerSheet.FrostChanceResistance += Stats.TotalFrostChanceResistance;
        PlayerStats.PlayerSheet.ThunderChanceResistance += Stats.TotalThunderChanceResistance;
        PlayerStats.PlayerSheet.WaterChanceResistance += Stats.TotalWaterChanceResistance;
        PlayerStats.PlayerSheet.WindChanceResistance += Stats.TotalWindChanceResistance;
        PlayerStats.PlayerSheet.SummoningPoints += Stats.TotalSummoningPoints;
    }

    public void RemoveStats(int Level)
    {
        //Calculate Stats
        Stats.TotalHealth = Stats.BaseHealth + (Level * Stats.PerUpgradeHealth);
        Stats.TotalPhysicalAtk = Stats.BasePhysicalAtk + (Level * Stats.PerUpgradePhysicalAtk);
        Stats.TotalMagicalAtk = Stats.BaseMagicalAtk + (Level * Stats.PerUpgradeMagicalAtk);
        Stats.TotalAttackSpeed = Stats.BaseAttackSpeed + (Level * Stats.PerUpgradeAttackSpeed);
        Stats.TotalGuardIntegrity = Stats.BaseGuardIntegrity + (Level * Stats.PerUpgradeGuardIntegrity);
        Stats.TotalGuardRegeneration = Stats.BaseGuardRegeneration + (Level * Stats.PerUpgradeGuardRegeneration);
        Stats.TotalPhysicalDamageFlatNumberReduction = Stats.BasePhysicalDamageFlatNumberReduction + (Level * Stats.PerUpgradePhysicalDamageFlatNumberReduction);
        Stats.TotalMagicalDamageFlatNumberReduction = Stats.BaseMagicalDamageFlatNumberReduction + (Level * Stats.PerUpgradeMagicalDamageFlatNumberReduction);
        Stats.TotalTensionLimit = Stats.BaseTensionLimit + (Level * Stats.PerUpgradeTensionLimit);
        Stats.TotalTensionReductionSpeed = Stats.BaseTensionReductionSpeed + (Level * Stats.PerUpgradeTensionReductionSpeed);
        Stats.TotalTensionCostReduction = Stats.BaseTensionCostReduction + (Level * Stats.PerUpgradeTensionCostReduction);
        Stats.TotalPhysicalCriticalStrikeChance = Stats.BasePhysicalCriticalStrikeChance + (Level * Stats.PerUpgradePhysicalCriticalStrikeChance);
        Stats.TotalPhysicalCriticalStrikeDamage = Stats.BasePhysicalCriticalStrikeDamage + (Level * Stats.PerUpgradePhysicalCriticalStrikeDamage);
        Stats.TotalMagicalCriticalStrikeChance = Stats.BaseMagicalCriticalStrikeChance + (Level * Stats.PerUpgradeMagicalCriticalStrikeChance);
        Stats.TotalMagicalCriticalStrikeDamage = Stats.BaseMagicalCriticalStrikeDamage + (Level * Stats.PerUpgradeMagicalCriticalStrikeDamage);
        Stats.TotalMovementSpeedBonus = Stats.BaseMovementSpeedBonus + (Level * Stats.PerUpgradeMovementSpeedBonus);
        Stats.TotalParryDamageBonus = Stats.BaseParryDamageBonus + (Level * Stats.PerUpgradeParryDamageBonus);
        Stats.TotalNeedleDamageBonus = Stats.BaseNeedleDamageBonus + (Level * Stats.PerUpgradeNeedleDamageBonus);
        Stats.TotalScissorDamageBonus = Stats.BaseScissorDamageBonus + (Level * Stats.PerUpgradeScissorDamageBonus);
        Stats.TotalSpinningRodDamageBonus = Stats.BaseSpinningRodDamageBonus + (Level * Stats.PerUpgradeSpinningRodDamageBonus);
        Stats.TotalNightmareDamageBonus = Stats.BaseNightmareDamageBonus + (Level * Stats.PerUpgradeNightmareDamageBonus);
        Stats.TotalDreamDamageBonus = Stats.BaseDreamDamageBonus + (Level * Stats.PerUpgradeDreamDamageBonus);

        Stats.TotalStatusChance = Stats.BaseStatusChance + (Level * Stats.PerUpgradeStatusChance);
        Stats.TotalCriticalChanceReduction = Stats.BaseCriticalChanceReduction + (Level * Stats.PerUpgradeCriticalChanceReduction);
        Stats.TotalShieldDamageAbsorption = Stats.BaseShieldDamageAbsorption + (Level * Stats.PerUpgradeShieldDamageAbsorption);
        Stats.TotalAllElementalDamageBonus = Stats.BaseAllElementalDamageBonus + (Level * Stats.PerUpgradeAllElementalDamageBonus);
        Stats.TotalFireDamageBonus = Stats.BaseFireDamageBonus + (Level * Stats.PerUpgradeFireDamageBonus);
        Stats.TotalFrostDamageBonus = Stats.BaseFrostDamageBonus + (Level * Stats.PerUpgradeFrostDamageBonus);
        Stats.TotalThunderDamageBonus = Stats.BaseThunderDamageBonus + (Level * Stats.PerUpgradeThunderDamageBonus);
        Stats.TotalWaterDamageBonus = Stats.BaseWaterDamageBonus + (Level * Stats.PerUpgradeWaterDamageBonus);
        Stats.TotalWindDamageBonus = Stats.BaseWindDamageBonus + (Level * Stats.PerUpgradeWindDamageBonus);
        Stats.TotalElementalDamageResistance = Stats.BaseElementalDamageResistance + (Level * Stats.PerUpgradeElementalDamageResistance);
        Stats.TotalFireChanceResistance = Stats.BaseFireChanceResistance + (Level * Stats.PerUpgradeFireChanceResistance);
        Stats.TotalFrostChanceResistance = Stats.BaseFrostChanceResistance + (Level * Stats.PerUpgradeFrostChanceResistance);
        Stats.TotalThunderChanceResistance = Stats.BaseThunderChanceResistance + (Level * Stats.PerUpgradeThunderChanceResistance);
        Stats.TotalWaterChanceResistance = Stats.BaseWaterChanceResistance + (Level * Stats.PerUpgradeWaterChanceResistance);
        Stats.TotalWindChanceResistance = Stats.BaseWindChanceResistance + (Level * Stats.PerUpgradeWindChanceResistance);
        Stats.TotalSummoningPoints = Stats.BaseSummoningPoints + (Level * Stats.PerUpgradeSummoningPoints);
        Stats.TotalNestleDamageBonus = Stats.BaseNestleDamageBonus + (Level * Stats.PerUpgradeNestleDamageBonus);

        //Add Stats
        PlayerStats.PlayerSheet.Health -= Stats.TotalHealth;
        PlayerStats.PlayerSheet.PhysicalAtk -= Stats.TotalPhysicalAtk;
        PlayerStats.PlayerSheet.MagicalAtk -= Stats.TotalMagicalAtk;
        PlayerStats.PlayerSheet.AttackSpeed -= Stats.TotalAttackSpeed;
        PlayerStats.PlayerSheet.GuardIntegrity -= Stats.TotalGuardIntegrity;
        PlayerStats.PlayerSheet.GuardRegeneration -= Stats.TotalGuardRegeneration;
        PlayerStats.PlayerSheet.PhysicalDamageFlatNumberReduction -= Stats.TotalPhysicalDamageFlatNumberReduction;
        PlayerStats.PlayerSheet.MagicalDamageFlatNumberReduction -= Stats.TotalMagicalDamageFlatNumberReduction;
        PlayerStats.PlayerSheet.TensionLimit -= Stats.TotalTensionLimit;
        PlayerStats.PlayerSheet.TensionReductionSpeed -= Stats.TotalTensionReductionSpeed;
        PlayerStats.PlayerSheet.TensionCostReduction -= Stats.TotalTensionCostReduction;
        PlayerStats.PlayerSheet.PhysicalCriticalStrikeChance -= Stats.TotalPhysicalCriticalStrikeChance;
        PlayerStats.PlayerSheet.PhysicalCriticalStrikeDamage -= Stats.TotalPhysicalCriticalStrikeDamage;
        PlayerStats.PlayerSheet.MagicalCriticalStrikeChance -= Stats.TotalMagicalCriticalStrikeChance;
        PlayerStats.PlayerSheet.MagicalCriticalStrikeDamage -= Stats.TotalMagicalCriticalStrikeDamage;
        PlayerStats.PlayerSheet.MovementSpeedBonus -= Stats.TotalMovementSpeedBonus;
        PlayerStats.PlayerSheet.ParryDamageBonus -= Stats.TotalParryDamageBonus;
        PlayerStats.PlayerSheet.NeedleDamageBonus -= Stats.TotalNeedleDamageBonus;
        PlayerStats.PlayerSheet.ScissorDamageBonus -= Stats.TotalScissorDamageBonus;
        PlayerStats.PlayerSheet.SpinningRodDamageBonus -= Stats.TotalSpinningRodDamageBonus;
        PlayerStats.PlayerSheet.NightmareDamageBonus -= Stats.TotalNightmareDamageBonus;
        PlayerStats.PlayerSheet.DreamDamageBonus -= Stats.TotalDreamDamageBonus;

        PlayerStats.PlayerSheet.NestleDamageBonus -= Stats.TotalNestleDamageBonus;
        PlayerStats.PlayerSheet.StatusChance -= Stats.TotalStatusChance;
        PlayerStats.PlayerSheet.CriticalChanceReduction -= Stats.TotalCriticalChanceReduction;
        PlayerStats.PlayerSheet.ShieldDamageAbsorption -= Stats.TotalShieldDamageAbsorption;
        PlayerStats.PlayerSheet.AllElementalDamageBonus -= Stats.TotalAllElementalDamageBonus;
        PlayerStats.PlayerSheet.FireDamageBonus -= Stats.TotalFireDamageBonus;
        PlayerStats.PlayerSheet.FrostDamageBonus -= Stats.TotalFrostDamageBonus;
        PlayerStats.PlayerSheet.ThunderDamageBonus -= Stats.TotalThunderDamageBonus;
        PlayerStats.PlayerSheet.WaterDamageBonus -= Stats.TotalWaterDamageBonus;
        PlayerStats.PlayerSheet.WindDamageBonus -= Stats.TotalWindDamageBonus;
        PlayerStats.PlayerSheet.ElementalDamageResistance -= Stats.TotalElementalDamageResistance;
        PlayerStats.PlayerSheet.FireChanceResistance -= Stats.TotalFireChanceResistance;
        PlayerStats.PlayerSheet.FrostChanceResistance -= Stats.TotalFrostChanceResistance;
        PlayerStats.PlayerSheet.ThunderChanceResistance -= Stats.TotalThunderChanceResistance;
        PlayerStats.PlayerSheet.WaterChanceResistance -= Stats.TotalWaterChanceResistance;
        PlayerStats.PlayerSheet.WindChanceResistance -= Stats.TotalWindChanceResistance;
        PlayerStats.PlayerSheet.SummoningPoints -= Stats.TotalSummoningPoints;
    }
}
