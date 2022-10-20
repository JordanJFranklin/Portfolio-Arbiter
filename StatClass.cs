using UnityEngine;
using HealthClasses;
using StateClasses;
using DamageClass;

public enum EnemyAffix {Normal, Dream, Nightmare}
public enum EnemyClass {Normal , Elite, MiniBoss, WorldBoss}
public enum EnemySize {Small, Normal, Large }

namespace StatClasses
{
    [System.Serializable]
    public class PlayerStatMap
    {
        [Header("Base Stats")]
        [Tooltip("Vitality")]
        public int Health = 0;
        [Tooltip("Power")]
        public int PhysicalAtk = 0;
        public int CurrWeaponDamage;
        [Tooltip("Spirit")]
        public int MagicalAtk = 0;
        [Tooltip("Dextrous")]
        public float AttackSpeed;
        [Tooltip("Pin Cushion")]
        public int GuardIntegrity = 0;
        [Tooltip("Form")]
        public int GuardRegeneration = 0;
        [Tooltip("PatchWork")]
        public int PhysicalDamageFlatNumberReduction;
        [Tooltip("Embrioder")]
        public int MagicalDamageFlatNumberReduction;
        [Tooltip("Spool")]
        public int TensionLimit = 0;
        public float CurrentTension = 0;
        [Tooltip("Hem")]
        public float TensionReductionSpeed = 0f;
        [Tooltip("Mend")]
        public int TensionCostReduction;
        [Tooltip("Prick")]
        public int StatusChance = 0;
        [Tooltip("Critical Guard")]
        public float CriticalChanceReduction;
        [Tooltip("Bulwark")]
        public float ShieldDamageAbsorption;
        [Tooltip("Sew")]
        public int PhysicalCriticalStrikeChance;
        [Tooltip("Cross Stitch")]
        public float PhysicalCriticalStrikeDamage = 0;
        [Tooltip("Seamstress")]
        public int MagicalCriticalStrikeChance;
        [Tooltip("Crochet")]
        public float MagicalCriticalStrikeDamage = 0;
        [Tooltip("Momentum")]
        public float MovementSpeedBonus;
        [Tooltip("Back Stitch")]
        public float ParryDamageBonus;
        [Tooltip("Needle Point")]
        public float NeedleDamageBonus;
        [Tooltip("Seam Ripper")]
        public float ScissorDamageBonus;
        [Tooltip("Suture")]
        public float SpinningRodDamageBonus;
        [Tooltip("ArrowHead")]
        public float NestleDamageBonus;
        [Tooltip("Nighmarish")]
        public float NightmareDamageBonus;
        [Tooltip("Day Dream")]
        public float DreamDamageBonus;
        [Tooltip("World Weaver")]
        public float AllElementalDamageBonus;
        [Tooltip("Combustion")]
        public float FireDamageBonus;
        [Tooltip("Hypothermic")]
        public float FrostDamageBonus;
        [Tooltip("Spark")]
        public float ThunderDamageBonus;
        [Tooltip("Deluge")]
        public float WaterDamageBonus;
        [Tooltip("Gust")]
        public float WindDamageBonus;
        [Tooltip("Form")]
        public int StaggerResistance;
        [Tooltip("Soul Weaver")]
        public float ElementalDamageResistance;
        [Tooltip("Flamara")]
        public float FireChanceResistance;
        [Tooltip("Frizin")]
        public float FrostChanceResistance;
        [Tooltip("Sparth")]
        public float ThunderChanceResistance;
        [Tooltip("Aquium")]
        public float WaterChanceResistance;
        [Tooltip("Aeri")]
        public float WindChanceResistance;
        [Tooltip("Leash")]
        public int SummoningPoints;


        [Header("Item Stat Bonuses")]
        [Tooltip("Permenant Maximum Health Bonus")]
        public int NightmareTethersPerBonus = 5;
        [Tooltip("Permenant Maximum Tension Limit Bonus")]
        public int DreamSpoolPerBonus = 5;
        [Tooltip("Permenant Guard Health Bonus")]
        public int GuardiaCrestsPerBonus = 10;
        [Tooltip("Adds Additional Heart Flasks")]
        public int SpiritGracePerBonus = 1;
        [Tooltip("Adds Additional Crystalline Flasks")]
        public int SylkeEssencePerBonus = 1;
        [Tooltip("Adds Additional Arrows To All Types")]
        public int QuiltedQuiverPerBonus = 10;
        [Tooltip("Adds Additional Charges For Spinning Rods")]
        public int WispOfTheEndersPerBonus = 3;
    }

    [System.Serializable]
    public class ItemStatBonus
    {
        [Header("Base Stats")]
        [Tooltip("Vitality")]
        public int TotalHealth = 0;
        public int BaseHealth = 0;
        public int PerUpgradeHealth = 0;
        [Tooltip("Power")]
        public int TotalPhysicalAtk = 0;
        public int BasePhysicalAtk = 0;
        public int PerUpgradePhysicalAtk = 0;
        [Tooltip("Spirit")]
        public int TotalMagicalAtk = 0;
        public int BaseMagicalAtk = 0;
        public int PerUpgradeMagicalAtk = 0;
        [Tooltip("Dextrous")]
        public float TotalAttackSpeed;
        public float BaseAttackSpeed;
        public float PerUpgradeAttackSpeed;
        [Tooltip("Pin Cushion")]
        public int TotalGuardIntegrity = 0;
        public int BaseGuardIntegrity = 0;
        public int PerUpgradeGuardIntegrity = 0;
        [Tooltip("Form")]
        public int TotalGuardRegeneration = 0;
        public int BaseGuardRegeneration = 0;
        public int PerUpgradeGuardRegeneration = 0;
        [Tooltip("PatchWork")]
        public int TotalPhysicalDamageFlatNumberReduction;
        public int BasePhysicalDamageFlatNumberReduction;
        public int PerUpgradePhysicalDamageFlatNumberReduction;
        [Tooltip("Embrioder")]
        public int TotalMagicalDamageFlatNumberReduction;
        public int BaseMagicalDamageFlatNumberReduction;
        public int PerUpgradeMagicalDamageFlatNumberReduction;
        [Tooltip("Spool")]
        public int TotalTensionLimit = 0;
        public int BaseTensionLimit = 0;
        public int PerUpgradeTensionLimit = 0;
        [Tooltip("Hem")]
        public float TotalTensionReductionSpeed = 0f;
        public float BaseTensionReductionSpeed = 0f;
        public float PerUpgradeTensionReductionSpeed = 0f;
        [Tooltip("Mend")]
        public int TotalTensionCostReduction;
        public int BaseTensionCostReduction;
        public int PerUpgradeTensionCostReduction;
        [Tooltip("Sew")]
        public int TotalPhysicalCriticalStrikeChance;
        public int BasePhysicalCriticalStrikeChance;
        public int PerUpgradePhysicalCriticalStrikeChance;
        [Tooltip("Cross Stitch")]
        public float TotalPhysicalCriticalStrikeDamage = 0;
        public float BasePhysicalCriticalStrikeDamage = 0;
        public float PerUpgradePhysicalCriticalStrikeDamage = 0;
        [Tooltip("Seamstress")]
        public int TotalMagicalCriticalStrikeChance;
        public int BaseMagicalCriticalStrikeChance;
        public int PerUpgradeMagicalCriticalStrikeChance;
        [Tooltip("Crochet")]
        public float TotalMagicalCriticalStrikeDamage = 0;
        public float BaseMagicalCriticalStrikeDamage = 0;
        public float PerUpgradeMagicalCriticalStrikeDamage = 0;
        [Tooltip("Momentum")]
        public float TotalMovementSpeedBonus;
        public float BaseMovementSpeedBonus;
        public float PerUpgradeMovementSpeedBonus;
        [Tooltip("Back Stitch")]
        public float TotalParryDamageBonus;
        public float BaseParryDamageBonus;
        public float PerUpgradeParryDamageBonus;
        [Tooltip("Needle Point")]
        public float TotalNeedleDamageBonus;
        public float BaseNeedleDamageBonus;
        public float PerUpgradeNeedleDamageBonus;
        [Tooltip("Seam Ripper")]
        public float TotalScissorDamageBonus;
        public float BaseScissorDamageBonus;
        public float PerUpgradeScissorDamageBonus;
        [Tooltip("Suture")]
        public float TotalSpinningRodDamageBonus;
        public float BaseSpinningRodDamageBonus;
        public float PerUpgradeSpinningRodDamageBonus;
        [Tooltip("ArrowHead")]
        public float TotalNestleDamageBonus;
        public float BaseNestleDamageBonus;
        public float PerUpgradeNestleDamageBonus;
        [Tooltip("Nighmarish")]
        public float TotalNightmareDamageBonus;
        public float BaseNightmareDamageBonus;
        public float PerUpgradeNightmareDamageBonus;
        [Tooltip("Day Dream")]
        public float TotalDreamDamageBonus;
        public float BaseDreamDamageBonus;
        public float PerUpgradeDreamDamageBonus;
        [Tooltip("Prick")]
        public int TotalStatusChance;
        public int BaseStatusChance;
        public int PerUpgradeStatusChance;
        [Tooltip("Form")]
        public int TotalStaggerResistance;
        public int BaseStaggerResistance;
        public int PerUpgradeStaggerResistance;
        [Tooltip("Critical Guard")]
        public float TotalCriticalChanceReduction;
        public float BaseCriticalChanceReduction;
        public float PerUpgradeCriticalChanceReduction;
        [Tooltip("Bulwark")]
        public float TotalShieldDamageAbsorption;
        public float BaseShieldDamageAbsorption;
        public float PerUpgradeShieldDamageAbsorption;
        [Tooltip("World Weaver")]
        public float TotalAllElementalDamageBonus;
        public float BaseAllElementalDamageBonus;
        public float PerUpgradeAllElementalDamageBonus;
        [Tooltip("Combustion")]
        public float TotalFireDamageBonus;
        public float BaseFireDamageBonus;
        public float PerUpgradeFireDamageBonus;
        [Tooltip("Hypothermic")]
        public float TotalFrostDamageBonus;
        public float BaseFrostDamageBonus;
        public float PerUpgradeFrostDamageBonus;
        [Tooltip("Spark")]
        public float TotalThunderDamageBonus;
        public float BaseThunderDamageBonus;
        public float PerUpgradeThunderDamageBonus;
        [Tooltip("Deluge")]
        public float TotalWaterDamageBonus;
        public float BaseWaterDamageBonus;
        public float PerUpgradeWaterDamageBonus;
        [Tooltip("Gust")]
        public float TotalWindDamageBonus;
        public float BaseWindDamageBonus;
        public float PerUpgradeWindDamageBonus;
        [Tooltip("Soul Weaver")]
        public float TotalElementalDamageResistance;
        public float BaseElementalDamageResistance;
        public float PerUpgradeElementalDamageResistance;
        [Tooltip("Flamara")]
        public float TotalFireChanceResistance;
        public float BaseFireChanceResistance;
        public float PerUpgradeFireChanceResistance;
        [Tooltip("Frizin")]
        public float TotalFrostChanceResistance;
        public float BaseFrostChanceResistance;
        public float PerUpgradeFrostChanceResistance;
        [Tooltip("Sparth")]
        public float TotalThunderChanceResistance;
        public float BaseThunderChanceResistance;
        public float PerUpgradeThunderChanceResistance;
        [Tooltip("Aquium")]
        public float TotalWaterChanceResistance;
        public float BaseWaterChanceResistance;
        public float PerUpgradeWaterChanceResistance;
        [Tooltip("Aeri")]
        public float TotalWindChanceResistance;
        public float BaseWindChanceResistance;
        public float PerUpgradeWindChanceResistance;
        [Tooltip("Leash")]
        public int TotalSummoningPoints;
        public int BaseSummoningPoints;
        public int PerUpgradeSummoningPoints;

    }

    [System.Serializable]
    public class BonusBuffSheet
    {
        [Tooltip("Vitality")]
        public int Health;
        [Tooltip("Power")]
        public int PhysicalAtk;
        [Tooltip("Spirit")]
        public int MagicalAtk;
        [Tooltip("Dextrous")]
        public float AttackSpeed;
        [Tooltip("Pin Cushion")]
        public int GuardIntegrity;
        [Tooltip("Form")]
        public int GuardRegeneration;
        [Tooltip("PatchWork")]
        public int PhysicalDamageFlatNumberReduction;
        [Tooltip("Embrioder")]
        public int MagicalDamageFlatNumberReduction;
        [Tooltip("Spool")]
        public int TensionLimitBonus = 0;
        [Tooltip("Hem")]
        public float TensionReductionSpeed = 0f;
        [Tooltip("Sew")]
        public float PhysicalCriticalStrikeChance;
        [Tooltip("Cross Stitch")]
        public float PhysicalCriticalStrikeDamage;
        [Tooltip("Seamstress")]
        public float MagicalCriticalStrikeChance;
        [Tooltip("Crochet")]
        public float MagicalCriticalStrikeDamage;
        [Tooltip("Momentum")]
        public float MovementSpeedBonus;
        [Tooltip("Back Stitch")]
        public float ParryDamageBonus;
        [Tooltip("Needle Point")]
        public float NeedleDamageBonus;
        [Tooltip("Seam Ripper")]
        public float ScissorDamageBonus;
        [Tooltip("Suture")]
        public float SpinningRodDamageBonus;
        [Tooltip("ArrowHead")]
        public float NestelDamageBonus;
        [Tooltip("Nighmarish")]
        public float NightmareDamageBonus;
        [Tooltip("Day Dream")]
        public float DreamDamageBonus;
        [Tooltip("World Weaver")]
        public float AllElementalDamageBonus;
        [Tooltip("Combustion")]
        public float FireDamageBonus;
        [Tooltip("Hypothermic")]
        public float FrostDamageBonus;
        [Tooltip("Spark")]
        public float ThunderDamageBonus;
        [Tooltip("Deluge")]
        public float WaterDamageBonus;
        [Tooltip("Gust")]
        public float WindDamageBonus;
        [Tooltip("Form")]
        public int StaggerResistance;
        [Tooltip("Soul Weaver")]
        public float ElementalDamageResistance;
        [Tooltip("Flamara")]
        public float FireChanceResistance;
        [Tooltip("Frizin")]
        public float FrostChanceResistance;
        [Tooltip("Sparth")]
        public float ThunderChanceResistance;
        [Tooltip("Aquium")]
        public float WaterChanceResistance;
        [Tooltip("Aeri")]
        public float WindChanceResistance;
        [Tooltip("Leash")]
        public int SummoningPoints;
        [Tooltip("Prick")]
        public int StatusChance = 0;
        [Tooltip("Critical Guard")]
        public float CriticalChanceReduction;
        [Tooltip("Bulwark")]
        public float ShieldDamageAbsorption;
    }

    //Enemy Stat Maps Will Not Be Updated Until Phase 4 of Development
    [System.Serializable]
    public class EnemyStatMap
    {
        [Header("Enemy Classifications")]
        public EnemyAffix EnemyElement;
        public EnemyClass Class;
        public EnemySize Size;

        [Header("Survivability")]
        public int HealthMax;
        public int ArmorMax;
        public int GuardMax;
        public int ShieldMax;
        public int GuardRegen;
        public int GuardRecoverDelay;
        public int StaggerResistance;
        public float GeneralDamageReductionPercentage;
        public float CutDamageReduction;
        public float StrikeDamageReduction;
        public float DreamDamageReduction;
        public float NightmareDamageReduction;
        public float AllElementalDamageReduction;
        public float FireDamageReduction;
        public float WaterDamageReduction;
        public float WindDamageReduction;
        public float FrostDamageReduction;
        public float ThunderDamageReduction;
        public float WorldDamageReduction;
        public float CriticalChanceReduction;

        [Header("Damage")]
        public int ElementalDamage;
        public int DreamDamage;
        public int NightmareDamage;
        public int StatusChance;
        public int PhysicalDamage;
        public int MagicalDamage;
        public int CriticalChance;
        public int CriticalDamage;

        [Header("Mobility")]
        public int BaseMovementSpeed;
        public int JumpHeight;

        [Header("Enemy States")]
        public bool isGuarding;
        public bool isStaggered;
    }

    [System.Serializable]
    public class DebuffSheet
    {
        [Header("Debuff Scaling")]
        public float MovementSpeedReduction = 0;
        public float AttackSpeedReduction = 0;
        public float DamageAmplification = 0;
    }

    public class StatInteractions
    {
        public static void ReduceTension(int TensionReduction, EntityStats Stats)
        {
            Stats.PlayerSheet.CurrentTension -= TensionReduction;
        }
    }
}