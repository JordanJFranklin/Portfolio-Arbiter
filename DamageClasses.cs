using UnityEngine;
using System.Collections.Generic;
using HealthClasses;
using StateClasses;

//Enum
public enum DamageClassification {Physical, Magical}
public enum DamageTypes {CutDamage, StrikeDamage, NestleDamage, SpinningRodDamage, DreamDamage, NightmareDamage, ScarletFateDamage, ElementalDamage}
public enum ElementalAffix {Elementless, Fire, Wind, Thunder, Frost, Water, ShadowFlame, SinisterWinds, NightStrikes, PhantomFreeze,AbyssalCurrent, HolyFire, HeavenlyGale, SkywardCry, Cryopathy, CleansingTide, World}
public enum EntityTarget {Player, Enemy}

[System.Serializable]
public class DamageProperty
{
    [Header("Base Properties")]
    public GameObject Sender;
    public EntityTarget Target;
    public string SpecialFlag;
    public int BaseDamage;
    public float DamageScaleFactor;
    public DamageClassification DamageClass;
    public DamageTypes DamageType;
    public ElementalAffix Element;
    public float StatusChance;
    public float CriticalChance;
    public float CriticalDamage;
    public int CalculatedDamage;

    [Header("Damage Bonuses")]
    [Tooltip("Back Stitch")]
    public float ParryDamageBonus;
    [Tooltip("Needle Point")]
    public float NeedleDamageBonus;
    [Tooltip("Seam Ripper")]
    public float ScissorDamageBonus;
    [Tooltip("ArrowHead")]
    public float NestleDamageBonus;
    [Tooltip("Suture")]
    public float SpinningRodDamageBonus;
    [Tooltip("Nighmarish")]
    public float NightmareDamageBonus;
    [Tooltip("Day Dream")]
    public float DreamDamageBonus;

    [Header("Elemental Damage Bonuses")]
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

    public DamageProperty (GameObject Sender, EntityTarget Target, string Flag, int CalculatedDamage, int BaseDamage, float statusChance, float DamageScaleFactor, DamageClassification DamageClass, DamageTypes DamageType, ElementalAffix Element, int CriticalChance, float CriticalDamage)
    {
        this.Sender = Sender;
        this.Target = Target;
        this.SpecialFlag = Flag;
        this.CalculatedDamage = CalculatedDamage; 
        this.BaseDamage = BaseDamage;
        this.DamageScaleFactor = DamageScaleFactor;
        this.DamageClass = DamageClass;
        this.DamageType = DamageType;
        this.CriticalChance = CriticalChance; 
        this.CriticalDamage = CriticalDamage;
        this.StatusChance = statusChance;
        this.Element = Element;
    }
}


namespace DamageClass
{
    public class CallHealthEvent
    {
        static string ParryFlag = "Parry";
        static string IgnoreArmorFlag = "IgnoreArmor";
        static string IgnoreShieldFlag = "IgnoreShield";
        static string IgnoreGuardFlag = "IgnoreGuard";
        static string GuardBreakFlag = "GuardBreak";

        public static void SendDamageEvent(DamageProperty IncomingDamage, EntityHealth OtherEntity)
        {
            //Set Damage Property Up For Player
            if (IncomingDamage.Sender.GetComponent<EntityHealth>().Identity.Equals(EntityTarget.Player))
            {
                //Physical
                if (IncomingDamage.DamageClass.Equals(DamageClassification.Physical))
                {
                    IncomingDamage.BaseDamage = IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.PhysicalAtk + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.PhysicalAtk;
                    IncomingDamage.CriticalChance = IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.PhysicalCriticalStrikeChance + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.PhysicalCriticalStrikeChance;
                    IncomingDamage.CriticalDamage = IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.PhysicalCriticalStrikeDamage + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.PhysicalCriticalStrikeDamage;
                }

                //Magical
                if (IncomingDamage.DamageClass.Equals(DamageClassification.Magical))
                {
                    IncomingDamage.BaseDamage = IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.MagicalAtk + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.MagicalAtk;
                    IncomingDamage.CriticalChance = IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.MagicalCriticalStrikeChance + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.MagicalCriticalStrikeChance;
                    IncomingDamage.CriticalDamage = IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.MagicalCriticalStrikeDamage + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.MagicalCriticalStrikeDamage;
                }

                //Set Damage Bonuses
                IncomingDamage.ParryDamageBonus = IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.ParryDamageBonus + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.ParryDamageBonus;
                IncomingDamage.NeedleDamageBonus = IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.NeedleDamageBonus + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.NeedleDamageBonus;
                IncomingDamage.ScissorDamageBonus = IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.ScissorDamageBonus + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.ScissorDamageBonus;
                IncomingDamage.SpinningRodDamageBonus = IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.SpinningRodDamageBonus + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.SpinningRodDamageBonus;
                IncomingDamage.NestleDamageBonus = IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.NestleDamageBonus + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.NestelDamageBonus;
                IncomingDamage.DreamDamageBonus = IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.DreamDamageBonus + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.DreamDamageBonus;
                IncomingDamage.NightmareDamageBonus = IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.NightmareDamageBonus + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.NightmareDamageBonus;

                //Set Elemental Damage Bonus
                IncomingDamage.AllElementalDamageBonus = IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.AllElementalDamageBonus;
                IncomingDamage.FireDamageBonus = IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.AllElementalDamageBonus + IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.FireDamageBonus + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.FireDamageBonus;
                IncomingDamage.WaterDamageBonus = IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.AllElementalDamageBonus + IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.WaterDamageBonus + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.WaterDamageBonus;
                IncomingDamage.ThunderDamageBonus = IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.AllElementalDamageBonus + IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.ThunderDamageBonus + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.ThunderDamageBonus;
                IncomingDamage.WindDamageBonus = IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.AllElementalDamageBonus + IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.WindDamageBonus + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.WindDamageBonus;
                IncomingDamage.FrostDamageBonus = IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.AllElementalDamageBonus + IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.FrostDamageBonus + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.FrostDamageBonus;

                //Set Status Chance
                IncomingDamage.StatusChance = IncomingDamage.Sender.GetComponent<EntityStats>().PlayerSheet.StatusChance + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.StatusChance;
            }

            //Set Damage Property Up For Player
            if (IncomingDamage.Sender.GetComponent<EntityHealth>().Identity.Equals(EntityTarget.Enemy))
            {
                //Physical
                if (IncomingDamage.DamageClass.Equals(DamageClassification.Physical))
                {
                    //Damage
                    IncomingDamage.BaseDamage = IncomingDamage.Sender.GetComponent<EntityStats>().EnemySheet.PhysicalDamage + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.PhysicalAtk;
                    IncomingDamage.CriticalChance = IncomingDamage.Sender.GetComponent<EntityStats>().EnemySheet.CriticalChance + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.PhysicalCriticalStrikeChance;
                    IncomingDamage.CriticalDamage = IncomingDamage.Sender.GetComponent<EntityStats>().EnemySheet.CriticalDamage + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.PhysicalCriticalStrikeDamage;
                }

                //Physical
                if (IncomingDamage.DamageClass.Equals(DamageClassification.Magical))
                {
                    //Damage
                    IncomingDamage.BaseDamage = IncomingDamage.Sender.GetComponent<EntityStats>().EnemySheet.MagicalDamage + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.MagicalAtk;
                    IncomingDamage.CriticalChance = IncomingDamage.Sender.GetComponent<EntityStats>().EnemySheet.CriticalChance + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.MagicalCriticalStrikeChance;
                    IncomingDamage.CriticalDamage = IncomingDamage.Sender.GetComponent<EntityStats>().EnemySheet.CriticalDamage + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.MagicalCriticalStrikeDamage;
                }

                //Set Damage Bonuses
                IncomingDamage.DreamDamageBonus = IncomingDamage.Sender.GetComponent<EntityStats>().EnemySheet.DreamDamage + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.DreamDamageBonus;
                IncomingDamage.NightmareDamageBonus = IncomingDamage.Sender.GetComponent<EntityStats>().EnemySheet.NightmareDamage + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.NightmareDamageBonus;

                //Set Elemental Damage Bonus
                IncomingDamage.AllElementalDamageBonus = IncomingDamage.Sender.GetComponent<EntityStats>().EnemySheet.ElementalDamage + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.AllElementalDamageBonus;

                //Set Status Chance
                IncomingDamage.StatusChance = IncomingDamage.Sender.GetComponent<EntityStats>().EnemySheet.StatusChance + IncomingDamage.Sender.GetComponent<EntityStats>().BuffSheet.StatusChance;
            }

            //Checks If Health Bars Exist Or Not
            if (OtherEntity.Health != null)
            {
                if (IncomingDamage.SpecialFlag != ParryFlag)
                {
                    if (!OtherEntity.isInvunerable)
                    {
                        //Armor Damage Rules
                        //Damage Armor Only If The Enemy Is Not Shielded Or Guarding
                        //2nd Priority Health
                        if (OtherEntity.isArmored && !OtherEntity.isShielded && !OtherEntity.isGuarding)
                        {
                            DamageTypes SuccessfulType = DamageTypes.CutDamage;

                            //Only Cut Damage Can Break Through Armor
                            if (IncomingDamage.DamageType.Equals(SuccessfulType))
                            {
                                //Calculate Damage Output
                                CalculateDamage(IncomingDamage, OtherEntity, HealthTypes.Armor);
                                return;
                            }
                            else
                            {
                                Debug.Log("The Current Instance Of Damage " + IncomingDamage.DamageType.ToString() + " Struck Was Not " + SuccessfulType.ToString() + " . Damage Resisted.");
                            }
                        }

                        //Shielded Damage Rules
                        //3rd Priority Health
                        if (OtherEntity.isShielded)
                        {
                            DamageTypes SuccessfulTypeA = DamageTypes.DreamDamage;
                            DamageTypes SuccessfulTypeB = DamageTypes.NightmareDamage;
                            DamageTypes SuccessfulTypeC = DamageTypes.ScarletFateDamage;

                            //Only Magic Damage of the OPPOSITE element Can Break Through the Shield
                            if (IncomingDamage.DamageType.Equals(SuccessfulTypeA) && OtherEntity.ShieldType.Equals(HealthTypes.NightmareMagic) && OtherEntity.Health[OtherEntity.NightmareHealthIndex].HealthValue > 0)
                            {
                                //Calculate Damage Output
                                CalculateDamage(IncomingDamage, OtherEntity, HealthTypes.NightmareMagic);
                                return;
                            }

                            //Only Magic Damage of the OPPOSITE element Can Break Through the Shield
                            if (IncomingDamage.DamageType.Equals(SuccessfulTypeB) && OtherEntity.ShieldType.Equals(HealthTypes.DreamMagic) && OtherEntity.Health[OtherEntity.DreamHealthIndex].HealthValue > 0)
                            {
                                //Calculate Damage Output
                                CalculateDamage(IncomingDamage, OtherEntity, HealthTypes.DreamMagic);
                                return;
                            }

                            //Only Magic Damage of the OPPOSITE element Can Break Through the Shield
                            if (IncomingDamage.DamageType.Equals(SuccessfulTypeC) && OtherEntity.ShieldType.Equals(HealthTypes.DreamMagic) && OtherEntity.Health[OtherEntity.DreamHealthIndex].HealthValue > 0 || IncomingDamage.DamageType.Equals(SuccessfulTypeC) && OtherEntity.ShieldType.Equals(HealthTypes.NightmareMagic) && OtherEntity.Health[OtherEntity.NightmareHealthIndex].HealthValue > 0)
                            {
                                //Calculate Damage Output
                                CalculateDamage(IncomingDamage, OtherEntity, HealthTypes.NightmareMagic);
                                CalculateDamage(IncomingDamage, OtherEntity, HealthTypes.DreamMagic);
                                return;
                            }
                        }

                        //Guard Damage Rules
                        //Flexible Priotrity Always Highest When Used Without A Shield
                        //If a shield is present the guard will not be dealt damage regardless of being in a guard state or not
                        if (OtherEntity.isGuarding)
                        {
                            //All Damage Types Can Harm Guards
                            //GuardBreak Flag can cause guards to be broken immdiately

                            if (!OtherEntity.isShielded)
                            {
                                //Calculate Damage Output
                                CalculateDamage(IncomingDamage, OtherEntity, HealthTypes.Guard);
                                return;
                            }
                            else
                            {
                                Debug.Log("This Entity IS Shielded. Therefore, the damage will be diverted to the shield.");
                            }
                        }

                        //Red Health Damage Rules
                        //Can Only Be Damaged When all other forms of protection are gone
                        if (!OtherEntity.isArmored && !OtherEntity.isShielded && !OtherEntity.isGuarding)
                        {
                            //Any Damage Will Hit

                            //Calculate Damage Output
                            CalculateDamage(IncomingDamage, OtherEntity, HealthTypes.Base);
                            return;
                        }
                    }
                }
                else
                {
                    Debug.Log("Target Is Invunerable. Unable To Damage.");
                }

                //Cuts Through Defenses Straight To Red Health Always No Matter What
                if (IncomingDamage.SpecialFlag == ParryFlag)
                {
                    CalculateDamage(IncomingDamage, OtherEntity, HealthTypes.Base);
                    return;
                }
            }
            else
            {
                Debug.Log("Entity Target Script Found. No Health Bar Has Been Assigned. Damage Will Not Be Sent. Error");
            }
        }

        //Critical And Status Chance Should Be Able To Be Beyond 100%
        private static void CalculateDamage(DamageProperty IncomingDamage, EntityHealth OtherEntity, HealthTypes HealthToTakeDamage)
        {
            //Clamp Damage From Being Negative
            //Damage Can Be 0
            IncomingDamage.CalculatedDamage = Mathf.Clamp(IncomingDamage.CalculatedDamage, 0, 99999999);

            //Elemental Damage Cannot Crit
            if (!IncomingDamage.DamageType.Equals(DamageTypes.ElementalDamage) || IncomingDamage.DamageType.Equals(DamageTypes.ElementalDamage) && IncomingDamage.Element.Equals(ElementalAffix.Thunder) || IncomingDamage.DamageType.Equals(DamageTypes.ElementalDamage) && IncomingDamage.Element.Equals(ElementalAffix.NightStrikes) || IncomingDamage.DamageType.Equals(DamageTypes.ElementalDamage) && IncomingDamage.Element.Equals(ElementalAffix.SkywardCry))
            {
                //Critical Chance Randomized Number
                int critChance = Random.Range(0, 100);

                //Roll Critical Chance But Reduce The Chance From The Critical Guard Stat For Both Enemies And The Player Accordingly
                if (OtherEntity.Identity.Equals(EntityTarget.Enemy))
                {
                    IncomingDamage.CriticalChance -= OtherEntity.Stats.EnemySheet.CriticalChanceReduction;
                }

                if (OtherEntity.Identity.Equals(EntityTarget.Player))
                {
                    IncomingDamage.CriticalChance -= OtherEntity.Stats.PlayerSheet.CriticalChanceReduction;
                }

                //Guarenteed Pass
                if (critChance >= 100)
                {
                    IncomingDamage.CalculatedDamage = Mathf.RoundToInt((IncomingDamage.BaseDamage * IncomingDamage.DamageScaleFactor) + ((IncomingDamage.BaseDamage + IncomingDamage.DamageScaleFactor) * IncomingDamage.CriticalDamage));

                    Debug.Log("Critical Chance Automatic Success. " + IncomingDamage.CriticalChance + "%");
                }
                else
                {
                    //Can Roll
                    if (critChance > 0)
                    {
                        //Pass
                        if (IncomingDamage.CriticalChance >= critChance)
                        {
                            IncomingDamage.CalculatedDamage = Mathf.RoundToInt((IncomingDamage.BaseDamage * IncomingDamage.DamageScaleFactor) + ((IncomingDamage.BaseDamage + IncomingDamage.DamageScaleFactor) * IncomingDamage.CriticalDamage));
                            Debug.Log("Critical Chance Success. " + IncomingDamage.CriticalChance + "%");
                        }
                        //Fail
                        else
                        {
                            IncomingDamage.CalculatedDamage = Mathf.RoundToInt((IncomingDamage.BaseDamage * IncomingDamage.DamageScaleFactor));
                            Debug.Log("Critical Chance Failed. " + IncomingDamage.CriticalChance + "%");
                        }
                    }
                    //Automatic Failure
                    else
                    {
                        IncomingDamage.CalculatedDamage = Mathf.RoundToInt((IncomingDamage.BaseDamage * IncomingDamage.DamageScaleFactor));
                        Debug.Log("Critical Chance Automatic Failure. " + IncomingDamage.CriticalChance + "%");
                    }
                }
            }

            //Elemental Damage Cannot Proc The Element Again
            //Rolled Status Chance
            if (!IncomingDamage.Element.Equals(ElementalAffix.Elementless) && !IncomingDamage.DamageType.Equals(DamageTypes.ElementalDamage))
            {
                //Status Chance Randomized Number
                int statusChance = Random.Range(0, 100);

                //Striking An Entity With An Element That they are weak too get a bonus of +25 if strong to it is reduced by -75
                if (OtherEntity.State.ElementalWeakness.Contains(IncomingDamage.Element))
                {
                    IncomingDamage.StatusChance += 25;
                }

                if (OtherEntity.State.ElementalStrengths.Contains(IncomingDamage.Element))
                {
                    IncomingDamage.StatusChance -= 50;
                }

                if (OtherEntity.Identity.Equals(EntityTarget.Enemy))
                {
                    //Reduce Chance Based On Specific Element
                    if (IncomingDamage.Element.Equals(ElementalAffix.Fire) || IncomingDamage.Element.Equals(ElementalAffix.ShadowFlame) || IncomingDamage.Element.Equals(ElementalAffix.HolyFire))
                    {
                        IncomingDamage.StatusChance -= OtherEntity.Stats.EnemySheet.FireDamageReduction;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.Water) || IncomingDamage.Element.Equals(ElementalAffix.AbyssalCurrent) || IncomingDamage.Element.Equals(ElementalAffix.CleansingTide))
                    {
                        IncomingDamage.StatusChance -= OtherEntity.Stats.EnemySheet.WaterDamageReduction;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.Wind) || IncomingDamage.Element.Equals(ElementalAffix.SinisterWinds) || IncomingDamage.Element.Equals(ElementalAffix.HeavenlyGale))
                    {
                        IncomingDamage.StatusChance -= OtherEntity.Stats.EnemySheet.WindDamageReduction;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.Frost) || IncomingDamage.Element.Equals(ElementalAffix.PhantomFreeze) || IncomingDamage.Element.Equals(ElementalAffix.Cryopathy))
                    {
                        IncomingDamage.StatusChance -= OtherEntity.Stats.EnemySheet.FrostDamageReduction;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.Thunder) || IncomingDamage.Element.Equals(ElementalAffix.NightStrikes) || IncomingDamage.Element.Equals(ElementalAffix.SkywardCry))
                    {
                        IncomingDamage.StatusChance -= OtherEntity.Stats.EnemySheet.ThunderDamageReduction;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.World))
                    {
                        IncomingDamage.StatusChance -= (OtherEntity.Stats.EnemySheet.FireDamageReduction - OtherEntity.Stats.EnemySheet.WaterDamageReduction - OtherEntity.Stats.EnemySheet.WindDamageReduction - OtherEntity.Stats.EnemySheet.FrostDamageReduction - OtherEntity.Stats.EnemySheet.ThunderDamageReduction);
                    }
                }

                if (OtherEntity.Identity.Equals(EntityTarget.Player))
                {
                    //Reduce Chance Based On Specific Element
                    if (IncomingDamage.Element.Equals(ElementalAffix.Fire) || IncomingDamage.Element.Equals(ElementalAffix.ShadowFlame) || IncomingDamage.Element.Equals(ElementalAffix.HolyFire))
                    {
                        IncomingDamage.StatusChance -= OtherEntity.Stats.PlayerSheet.FireChanceResistance;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.Water) || IncomingDamage.Element.Equals(ElementalAffix.AbyssalCurrent) || IncomingDamage.Element.Equals(ElementalAffix.CleansingTide))
                    {
                        IncomingDamage.StatusChance -= OtherEntity.Stats.PlayerSheet.WaterChanceResistance;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.Wind) || IncomingDamage.Element.Equals(ElementalAffix.SinisterWinds) || IncomingDamage.Element.Equals(ElementalAffix.HeavenlyGale))
                    {
                        IncomingDamage.StatusChance -= OtherEntity.Stats.PlayerSheet.WindChanceResistance;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.Frost) || IncomingDamage.Element.Equals(ElementalAffix.PhantomFreeze) || IncomingDamage.Element.Equals(ElementalAffix.Cryopathy))
                    {
                        IncomingDamage.StatusChance -= OtherEntity.Stats.PlayerSheet.FrostChanceResistance;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.Thunder) || IncomingDamage.Element.Equals(ElementalAffix.NightStrikes) || IncomingDamage.Element.Equals(ElementalAffix.SkywardCry))
                    {
                        IncomingDamage.StatusChance -= OtherEntity.Stats.PlayerSheet.ThunderChanceResistance;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.World))
                    {
                        IncomingDamage.StatusChance -= (OtherEntity.Stats.PlayerSheet.FireChanceResistance - OtherEntity.Stats.PlayerSheet.WaterChanceResistance - OtherEntity.Stats.PlayerSheet.WindChanceResistance - OtherEntity.Stats.PlayerSheet.FrostChanceResistance - OtherEntity.Stats.PlayerSheet.ThunderChanceResistance);
                    }
                }

                //Guarenteed Pass
                if (IncomingDamage.StatusChance >= 100)
                {
                    if (IncomingDamage.Element.Equals(ElementalAffix.Fire))
                    {
                        //Add Status Effect
                        OtherEntity.GetComponent<EntityState>().isFireAffected = true;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.Water))
                    {
                        //Add Status Effect
                        OtherEntity.GetComponent<EntityState>().isWaterAffected = true;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.Wind))
                    {
                        //Add Status Effect
                        OtherEntity.GetComponent<EntityState>().isWindAffected = true;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.Frost))
                    {
                        //Add Status Effect
                        OtherEntity.GetComponent<EntityState>().isFrostAffected = true;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.Thunder))
                    {
                        //Add Status Effect
                        OtherEntity.GetComponent<EntityState>().isThunderAffected = true;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.ShadowFlame))
                    {
                        //Add Status Effect
                        OtherEntity.GetComponent<EntityState>().isNightmareFireAffected = true;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.SinisterWinds))
                    {
                        //Add Status Effect
                        OtherEntity.GetComponent<EntityState>().isNightmareWindAffected = true;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.NightStrikes))
                    {
                        //Add Status Effect
                        OtherEntity.GetComponent<EntityState>().isNightmareThunderAffected = true;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.PhantomFreeze))
                    {
                        //Add Status Effect
                        OtherEntity.GetComponent<EntityState>().isNightmareFrostAffected = true;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.AbyssalCurrent))
                    {
                        //Add Status Effect
                        OtherEntity.GetComponent<EntityState>().isNightmareWaterAffected = true;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.HolyFire))
                    {
                        //Add Status Effect
                        OtherEntity.GetComponent<EntityState>().isDreamFireAffected = true;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.CleansingTide))
                    {
                        //Add Status Effect
                        OtherEntity.GetComponent<EntityState>().isDreamWaterAffected = true;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.HeavenlyGale))
                    {
                        //Add Status Effect
                        OtherEntity.GetComponent<EntityState>().isDreamWindAffected = true;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.Cryopathy))
                    {
                        //Add Status Effect
                        OtherEntity.GetComponent<EntityState>().isDreamFrostAffected = true;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.SkywardCry))
                    {
                        //Add Status Effect
                        OtherEntity.GetComponent<EntityState>().isDreamThunderAffected = true;
                    }

                    if (IncomingDamage.Element.Equals(ElementalAffix.World))
                    {
                        //Add Status Effect
                        OtherEntity.GetComponent<EntityState>().isWorldAffected = true;
                    }

                    Debug.Log("Elemental Status Chance AUTOMATIC Success. " + IncomingDamage.StatusChance + "%");
                }
                else
                {
                    if (IncomingDamage.StatusChance >= statusChance)
                    {
                        if (IncomingDamage.Element.Equals(ElementalAffix.Fire))
                        {
                            //Add Status Effect
                            OtherEntity.GetComponent<EntityState>().isFireAffected = true;
                        }

                        if (IncomingDamage.Element.Equals(ElementalAffix.Water))
                        {
                            //Add Status Effect
                            OtherEntity.GetComponent<EntityState>().isWaterAffected = true;
                        }

                        if (IncomingDamage.Element.Equals(ElementalAffix.Wind))
                        {
                            //Add Status Effect
                            OtherEntity.GetComponent<EntityState>().isWindAffected = true;
                        }

                        if (IncomingDamage.Element.Equals(ElementalAffix.Frost))
                        {
                            //Add Status Effect
                            OtherEntity.GetComponent<EntityState>().isFrostAffected = true;
                        }

                        if (IncomingDamage.Element.Equals(ElementalAffix.Thunder))
                        {
                            //Add Status Effect
                            OtherEntity.GetComponent<EntityState>().isThunderAffected = true;
                        }

                        if (IncomingDamage.Element.Equals(ElementalAffix.ShadowFlame))
                        {
                            //Add Status Effect
                            OtherEntity.GetComponent<EntityState>().isNightmareFireAffected = true;
                        }

                        if (IncomingDamage.Element.Equals(ElementalAffix.SinisterWinds))
                        {
                            //Add Status Effect
                            OtherEntity.GetComponent<EntityState>().isNightmareWindAffected = true;
                        }

                        if (IncomingDamage.Element.Equals(ElementalAffix.NightStrikes))
                        {
                            //Add Status Effect
                            OtherEntity.GetComponent<EntityState>().isNightmareThunderAffected = true;
                        }

                        if (IncomingDamage.Element.Equals(ElementalAffix.PhantomFreeze))
                        {
                            //Add Status Effect
                            OtherEntity.GetComponent<EntityState>().isNightmareFrostAffected = true;
                        }

                        if (IncomingDamage.Element.Equals(ElementalAffix.AbyssalCurrent))
                        {
                            //Add Status Effect
                            OtherEntity.GetComponent<EntityState>().isNightmareWaterAffected = true;
                        }

                        if (IncomingDamage.Element.Equals(ElementalAffix.HolyFire))
                        {
                            //Add Status Effect
                            OtherEntity.GetComponent<EntityState>().isDreamFireAffected = true;
                        }

                        if (IncomingDamage.Element.Equals(ElementalAffix.CleansingTide))
                        {
                            //Add Status Effect
                            OtherEntity.GetComponent<EntityState>().isDreamWaterAffected = true;
                        }

                        if (IncomingDamage.Element.Equals(ElementalAffix.HeavenlyGale))
                        {
                            //Add Status Effect
                            OtherEntity.GetComponent<EntityState>().isDreamWindAffected = true;
                        }

                        if (IncomingDamage.Element.Equals(ElementalAffix.Cryopathy))
                        {
                            //Add Status Effect
                            OtherEntity.GetComponent<EntityState>().isDreamFrostAffected = true;
                        }

                        if (IncomingDamage.Element.Equals(ElementalAffix.SkywardCry))
                        {
                            //Add Status Effect
                            OtherEntity.GetComponent<EntityState>().isDreamThunderAffected = true;
                        }

                        if (IncomingDamage.Element.Equals(ElementalAffix.World))
                        {
                            //Add Status Effect
                            OtherEntity.GetComponent<EntityState>().isWorldAffected = true;
                        }

                        Debug.Log("Elemental Status Chance To Inflict " + IncomingDamage.Element + " with a chance of " + IncomingDamage.StatusChance + "% was Successfull!");

                    }
                    else
                    {
                        Debug.Log("Elemental Status Chance To Inflict " + IncomingDamage.Element + " with a chance of " + IncomingDamage.StatusChance + "%" + ". The chance was below " + statusChance + ". Failed On Status Attempt.");
                    }
                }
            }

            //Additive Damage Bonuses
            int NeedleDamageBonus;
            int ScissorDamageBonus;
            int NestleDamageBonus;
            int SpinningRodDamageBonus;
            int DreamDamageBonus;
            int NightmareDamageBonus;
            int ParryDamageBonus;
            int ScarletDamageBonus;


            //Nestle Damage Bonuses
            if (IncomingDamage.DamageType.Equals(DamageTypes.NestleDamage))
            {
                NestleDamageBonus = Mathf.RoundToInt((IncomingDamage.BaseDamage + IncomingDamage.DamageScaleFactor) * IncomingDamage.NestleDamageBonus);
                IncomingDamage.CalculatedDamage += NestleDamageBonus;
            }

            //Spinning Rod Damage Bonuses
            if (IncomingDamage.DamageType.Equals(DamageTypes.SpinningRodDamage))
            {
                SpinningRodDamageBonus = Mathf.RoundToInt((IncomingDamage.BaseDamage + IncomingDamage.DamageScaleFactor) * IncomingDamage.SpinningRodDamageBonus);
                IncomingDamage.CalculatedDamage += SpinningRodDamageBonus;
            }

            //Needle Damage Bonuses
            if (IncomingDamage.DamageType.Equals(DamageTypes.StrikeDamage))
            {
                NeedleDamageBonus = Mathf.RoundToInt((IncomingDamage.BaseDamage + IncomingDamage.DamageScaleFactor) * IncomingDamage.NeedleDamageBonus);
                IncomingDamage.CalculatedDamage += NeedleDamageBonus;
            }

            //Scissor Damage Bonuses
            if (IncomingDamage.DamageType.Equals(DamageTypes.CutDamage))
            {
                ScissorDamageBonus = Mathf.RoundToInt((IncomingDamage.BaseDamage + IncomingDamage.DamageScaleFactor) * IncomingDamage.ScissorDamageBonus);
                IncomingDamage.CalculatedDamage += ScissorDamageBonus;
            }

            //Dream Damage Bonuses
            if (IncomingDamage.DamageType.Equals(DamageTypes.DreamDamage))
            {
                DreamDamageBonus = Mathf.RoundToInt((IncomingDamage.BaseDamage + IncomingDamage.DamageScaleFactor) * IncomingDamage.DreamDamageBonus);
                IncomingDamage.CalculatedDamage += DreamDamageBonus;
            }

            //Nightmare Damage Bonuses
            if (IncomingDamage.DamageType.Equals(DamageTypes.NightmareDamage))
            {
                NightmareDamageBonus = Mathf.RoundToInt((IncomingDamage.BaseDamage + IncomingDamage.DamageScaleFactor) * IncomingDamage.NightmareDamageBonus);
                IncomingDamage.CalculatedDamage += NightmareDamageBonus;
            }

            //Scarlet Fate Bonus
            if (IncomingDamage.DamageType.Equals(DamageTypes.ScarletFateDamage))
            {
                ScarletDamageBonus = Mathf.RoundToInt(OtherEntity.Health[OtherEntity.BaseHealthIndex].HealthValue * 0.15f);
                IncomingDamage.CalculatedDamage += ScarletDamageBonus;
            }

            //Parry Damage Bonuses
            if (IncomingDamage.SpecialFlag == ParryFlag)
            {
                ParryDamageBonus = Mathf.RoundToInt((IncomingDamage.BaseDamage + IncomingDamage.DamageScaleFactor) * IncomingDamage.ParryDamageBonus);
                IncomingDamage.CalculatedDamage += ParryDamageBonus;
            }

            if (!IncomingDamage.DamageType.Equals(DamageTypes.ScarletFateDamage))
            {
                //Special Case Ignore Armor Flag
                if (IncomingDamage.SpecialFlag != IgnoreArmorFlag)
                {
                    //Subtract With Damage Reductive Stats
                    if (IncomingDamage.DamageClass.Equals(DamageClassification.Physical))
                    {
                        //Damage Reduction Rules
                        if (OtherEntity.Equals(EntityTarget.Player))
                        {
                            IncomingDamage.CalculatedDamage -= OtherEntity.Stats.PlayerSheet.PhysicalDamageFlatNumberReduction;
                        }

                        //Damage Reduction Rules
                        if (OtherEntity.Equals(EntityTarget.Enemy))
                        {
                            int percentageDamageReductionValue = Mathf.RoundToInt(IncomingDamage.CalculatedDamage * OtherEntity.Stats.EnemySheet.GeneralDamageReductionPercentage);

                            IncomingDamage.CalculatedDamage -= percentageDamageReductionValue;

                            if (IncomingDamage.DamageType.Equals(DamageTypes.CutDamage))
                            {
                                int percentageCutDamageReductionValue = Mathf.RoundToInt(IncomingDamage.CalculatedDamage * OtherEntity.Stats.EnemySheet.CutDamageReduction);

                                IncomingDamage.CalculatedDamage -= percentageDamageReductionValue;
                            }

                            if (IncomingDamage.DamageType.Equals(DamageTypes.StrikeDamage))
                            {
                                int percentageStrikeDamageReductionValue = Mathf.RoundToInt(IncomingDamage.CalculatedDamage * OtherEntity.Stats.EnemySheet.StrikeDamageReduction);

                                IncomingDamage.CalculatedDamage -= percentageDamageReductionValue;
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("Physical Armor Ignored.");
                }

                //Special Case Ignore Shield Flag
                if (IncomingDamage.SpecialFlag != IgnoreShieldFlag)
                {
                    //Subtract With Damage Reductive Stats
                    if (IncomingDamage.DamageClass.Equals(DamageClassification.Magical))
                    {
                        if (OtherEntity.Equals(EntityTarget.Player))
                        {
                            int elementalDamageResistance = 0;

                            //Reduce Magical Damage Flat
                            IncomingDamage.CalculatedDamage -= OtherEntity.Stats.PlayerSheet.MagicalDamageFlatNumberReduction;
                            
                            //Reduce Elemental Damage By Percentage
                            if(IncomingDamage.DamageType.Equals(DamageTypes.ElementalDamage))
                            {
                                elementalDamageResistance = Mathf.RoundToInt(IncomingDamage.CalculatedDamage * OtherEntity.Stats.PlayerSheet.ElementalDamageResistance);
                                IncomingDamage.CalculatedDamage -= elementalDamageResistance; 
                            }

                            Debug.Log("Damage Mitigated: " + elementalDamageResistance + OtherEntity.Stats.PlayerSheet.MagicalDamageFlatNumberReduction);
                        }

                        if (OtherEntity.Equals(EntityTarget.Enemy))
                        {
                            int percentageDamageReductionValue = Mathf.RoundToInt(IncomingDamage.CalculatedDamage * OtherEntity.Stats.EnemySheet.GeneralDamageReductionPercentage);

                            IncomingDamage.CalculatedDamage -= percentageDamageReductionValue;

                            if (IncomingDamage.DamageType.Equals(DamageTypes.NightmareDamage))
                            {
                                int percentageNightmareDamageReductionValue = Mathf.RoundToInt(IncomingDamage.CalculatedDamage * OtherEntity.Stats.EnemySheet.NightmareDamageReduction);

                                IncomingDamage.CalculatedDamage -= percentageDamageReductionValue;
                                Debug.Log("Damage Mitigated: " + percentageNightmareDamageReductionValue + percentageDamageReductionValue + OtherEntity.Stats.EnemySheet.NightmareDamageReduction);
                            }

                            if (IncomingDamage.DamageType.Equals(DamageTypes.DreamDamage))
                            {
                                int percentageDreamDamageReductionValue = Mathf.RoundToInt(IncomingDamage.CalculatedDamage * OtherEntity.Stats.EnemySheet.DreamDamageReduction);

                                IncomingDamage.CalculatedDamage -= percentageDreamDamageReductionValue;
                                Debug.Log("Damage Mitigated: " + percentageDreamDamageReductionValue + percentageDamageReductionValue + OtherEntity.Stats.EnemySheet.DreamDamageReduction);
                            }

                            //Reduce Elemental Damage By Percentage
                            if (IncomingDamage.DamageType.Equals(DamageTypes.ElementalDamage))
                            {
                                int elementalDamageResistance = Mathf.RoundToInt(IncomingDamage.CalculatedDamage * OtherEntity.Stats.EnemySheet.AllElementalDamageReduction);
                                IncomingDamage.CalculatedDamage -= elementalDamageResistance;
                                Debug.Log("Damage Mitigated: " + percentageDamageReductionValue + elementalDamageResistance + OtherEntity.Stats.EnemySheet.AllElementalDamageReduction);
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("Magical Shield Ignored.");
                }
            }
            else
            {
                Debug.Log("Scarlet Fate Damage Avoided Damage Reductive Values.");
            }

            //Reduce Health

            //Look For Matching Health Element Then Damage it
            if (HealthToTakeDamage.Equals(HealthTypes.Base))
            {
                //Check If The Health Bar Exists 
                if (OtherEntity.BaseHealthIndex > -1)
                {
                    OtherEntity.Health[OtherEntity.BaseHealthIndex].HealthValue -= IncomingDamage.CalculatedDamage;
                    Debug.Log("Damaged " + OtherEntity.gameObject.name +  "  For : " + IncomingDamage.CalculatedDamage + " To The " + HealthToTakeDamage.ToString());
                }
                else
                {
                    Debug.Log("Health Bar Not Indexed. Prevented Error");
                }
                return;
            }

            if (HealthToTakeDamage.Equals(HealthTypes.Armor))
            {
                //Check If The Health Bar Exists 
                if (OtherEntity.ArmorHealthIndex > -1)
                {
                    if(IncomingDamage.Element.Equals(ElementalAffix.Thunder))
                    {
                        int BonusDamage = Mathf.RoundToInt(IncomingDamage.CalculatedDamage * 1.0f);
                        IncomingDamage.CalculatedDamage += BonusDamage;
                        Debug.Log("Thunder Granted 100% Bonus Damage: " + BonusDamage);
                    }
                    
                    OtherEntity.Health[OtherEntity.ArmorHealthIndex].HealthValue -= IncomingDamage.CalculatedDamage;
                    Debug.Log("Damaged Target For : " + IncomingDamage.CalculatedDamage + " To The " + HealthToTakeDamage.ToString());
                }
                else
                {
                    Debug.Log("Health Bar Not Indexed. Prevented Error");
                }

                return;
            }

            if (HealthToTakeDamage.Equals(HealthTypes.Guard))
            {
                //Check If The Health Bar Exists 
                if (OtherEntity.GuardHealthIndex > -1)
                {
                    OtherEntity.Health[OtherEntity.GuardHealthIndex].HealthValue -= IncomingDamage.CalculatedDamage;
                    Debug.Log("Damaged Target For : " + IncomingDamage.CalculatedDamage + " To The " + HealthToTakeDamage.ToString());
                }
                else
                {
                    Debug.Log("Health Bar Not Indexed. Prevented Error");
                }
                return;
            }

            if (HealthToTakeDamage.Equals(HealthTypes.DreamMagic))
            {
                //Check If The Health Bar Exists 
                if (OtherEntity.DreamHealthIndex > -1)
                {
                    OtherEntity.Health[OtherEntity.DreamHealthIndex].HealthValue -= IncomingDamage.CalculatedDamage;
                    Debug.Log("Damaged Target For : " + IncomingDamage.CalculatedDamage + " To The " + HealthToTakeDamage.ToString());
                }
                else
                {
                    Debug.Log("Health Bar Not Indexed. Prevented Error");
                }
                return;
            }

            if (HealthToTakeDamage.Equals(HealthTypes.NightmareMagic))
            {
                //Check If The Health Bar Exists 
                if (OtherEntity.NightmareHealthIndex > -1)
                {
                    OtherEntity.Health[OtherEntity.NightmareHealthIndex].HealthValue -= IncomingDamage.CalculatedDamage;
                    Debug.Log("Damaged Target For : " + IncomingDamage.CalculatedDamage + " To The " + HealthToTakeDamage.ToString());
                }
                else
                {
                    Debug.Log("Health Bar Not Indexed. Prevented Error");
                }
                return;
            }
        }
        
        public static void SendHealingEvent(HealingProperty IncomingHeal, EntityHealth OtherEntity)
        {
            //Check If The Entity Is Matching
            if (OtherEntity.Identity.Equals(IncomingHeal.Target) && !OtherEntity.isDead)
            {
                //Look For Matching Health Element Then Heal it
                if(IncomingHeal.Health.Equals(HealthTypes.Base))
                {
                    //Check If The Health Bar Exists 
                    if (OtherEntity.BaseHealthIndex > -1)
                    {
                        OtherEntity.Health[OtherEntity.BaseHealthIndex].HealthValue += IncomingHeal.HealingValue;
                        Debug.Log("Healed Target For : " + IncomingHeal.HealingValue + " To The " + IncomingHeal.Health.ToString());
                    }
                    else
                    {
                        Debug.Log("Health Bar Not Indexed. Prevented Error");
                    }
                    return;
                }

                //Only Enemies Receive Armor Healing 
                if (IncomingHeal.Health.Equals(HealthTypes.Armor) && OtherEntity.Identity.Equals(EntityTarget.Enemy))
                {
                    //Check If The Health Bar Exists 
                    if (OtherEntity.ArmorHealthIndex > -1)
                    {
                        OtherEntity.Health[OtherEntity.ArmorHealthIndex].HealthValue += IncomingHeal.HealingValue;
                        Debug.Log("Healed Target For : " + IncomingHeal.HealingValue + " To The " + IncomingHeal.Health.ToString());
                    }
                    else
                    {
                        Debug.Log("Health Bar Not Indexed. Prevented Error");
                    }

                    return;
                }

                if (IncomingHeal.Health.Equals(HealthTypes.Guard))
                {
                    //Check If The Health Bar Exists 
                    if (OtherEntity.GuardHealthIndex > -1)
                    {
                        OtherEntity.Health[OtherEntity.GuardHealthIndex].HealthValue += IncomingHeal.HealingValue;
                        Debug.Log("Healed Target For : " + IncomingHeal.HealingValue + " To The " + IncomingHeal.Health.ToString());
                    }
                    else
                    {
                        Debug.Log("Health Bar Not Indexed. Prevented Error");
                    }
                    return;
                }

                if (IncomingHeal.Health.Equals(HealthTypes.DreamMagic))
                {
                    //Check If The Health Bar Exists 
                    if (OtherEntity.DreamHealthIndex > -1)
                    {
                        OtherEntity.Health[OtherEntity.DreamHealthIndex].HealthValue += IncomingHeal.HealingValue;
                        Debug.Log("Healed Target For : " + IncomingHeal.HealingValue + " To The " + IncomingHeal.Health.ToString());
                    }
                    else
                    {
                        Debug.Log("Health Bar Not Indexed. Prevented Error");
                    }
                    return;
                }

                if (IncomingHeal.Health.Equals(HealthTypes.NightmareMagic))
                {
                    //Check If The Health Bar Exists 
                    if (OtherEntity.NightmareHealthIndex > -1)
                    {
                        OtherEntity.Health[OtherEntity.NightmareHealthIndex].HealthValue += IncomingHeal.HealingValue;
                        Debug.Log("Healed Target For : " + IncomingHeal.HealingValue + " To The " + IncomingHeal.Health.ToString());
                    }
                    else
                    {
                        Debug.Log("Health Bar Not Indexed. Prevented Error");
                    }
                    return;
                }
            }
            else
            {
                Debug.Log("Healing Attempt Prevented. The Healing Target Does Not Match The Healing Source.");
                return;
            }
        }
    }
}
