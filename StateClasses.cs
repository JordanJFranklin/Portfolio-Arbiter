using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HealthClasses;
using DamageClass;

//Passive will always apply the effect until the duration ends
//Tick Rate applies damage every certain amount of time passed
public enum DebuffTriggerTypes {Passive, TickRate}
public enum DebuffTypes {Null, Stun, Slowed, Tattered, GuardBreak, GuardCrack, ElementalEffect, AttackSpeedReduction, DamageAmplification}
//Passive will always apply the effect until the duration ends
//OnNextAttack will prime any of your next attacks with bonus damage and then end the buff after being used
//Iterative follows a numerical attack threshold to perform a special effect. Attacks are counted and something special happens when the threshold is reached. Upon reaching it. The numerical count is reset to repeat until the duration ends.
//Stack causing the effect to increase incrementally on each application. Rather then only reseting a duration it also increments the buff to be stronger and subtracts the previous effect off and replaces it with the new stronger one which has a cap.
public enum BuffTriggerMode {Passive, OnNextAttack, Iterative, Stack}
public enum BuffTypes {Null, PhysicalAtk, MagicalAtk, NeedleBonus, ScissorBonus, SpinningRodBonus, NestelBonus,DreamBonus, NightmareBonus, ParryBonus, MovementSpeedBonus, MagicCriticalChance, PhysicalCriticalChance, MagicCriticalDamage, PhysicalCriticalDamage, GuardBonus, PhysicalArmorBonus, MagicalArmorBonus, BonusHealth, AttackSpeed, AllElemental, FireBonus, FrostBonus,ThunderBonus, WaterBonus, WindBonus, ElementalDamageResist, FireChance, FrostChance, ThunderChance, WaterChance, WindChance, SummonBonus, StatusChance, CriticalChanceReduction, ShieldDamageResist, StaggerResistance}

public enum StaggerType {LightStagger, MediumStagger, HeavyStagger}
namespace StateClasses
{
    [System.Serializable]
    public class StaggerEffect
    {
        [Header("Stagger Elements")]
        public StaggerType Stagger;
        public int StaggerResistance;
        public float KnockbackStaggerForce;
        public float KnockUpStaggerForce;
        public float KnockSideStaggerForce;

        public StaggerEffect(StaggerType stagger, int staggerResist, float knockbackstaggerforce, float knockbackstaggerup, float knockbackstaggerside)
        {
            this.Stagger = stagger;
            this.StaggerResistance = staggerResist;
            this.KnockbackStaggerForce = knockbackstaggerforce;
            this.KnockUpStaggerForce = knockbackstaggerup;
            this.KnockSideStaggerForce = knockbackstaggerside;
        }

        public StaggerEffect(StaggerType stagger, int staggerResist)
        {
            this.Stagger = stagger;
            this.StaggerResistance = staggerResist;
        }
    }
    
    [System.Serializable]
    public class DebuffEffect
    {
        [Header("Sender Reference")]
        public GameObject Sender;

        [Header("Basic Settings")]
        public string DebuffName;
        public DebuffTriggerTypes TriggerMode;
        public DebuffTypes DebuffType;
        public ElementalAffix Element;
        public float CurrentDuration;
        public float MaximumDuration;
        public float FactorValue;

        [Header("Tick Rate Settings")]
        public float TickRate;

        //All
        public DebuffEffect(GameObject sender, string name, DebuffTriggerTypes triggermode, DebuffTypes debuff, ElementalAffix element, float currduration, float maximumduration, float factorvalue, float tickrate)
        {
            this.Sender = sender;
            this.DebuffName = name;
            this.TriggerMode = triggermode;
            this.DebuffType = debuff;
            this.Element = element;
            this.CurrentDuration = currduration;
            this.MaximumDuration = maximumduration;
            this.FactorValue = factorvalue;
            this.TickRate = tickrate;
        }

        //No Tick Rate
        public DebuffEffect(GameObject sender, string name, DebuffTriggerTypes triggermode, DebuffTypes debuff, ElementalAffix element, float currduration, float maximumduration, float factorvalue)
        {
            this.Sender = sender;
            this.DebuffName = name;
            this.TriggerMode = triggermode;
            this.DebuffType = debuff;
            this.Element = element;
            this.CurrentDuration = currduration;
            this.MaximumDuration = maximumduration;
            this.FactorValue = factorvalue;
        }

        //No Factoring
        public DebuffEffect(GameObject sender, string name, DebuffTriggerTypes triggermode, DebuffTypes debuff, ElementalAffix element, float currduration, float maximumduration)
        {
            this.Sender = sender;
            this.DebuffName = name;
            this.TriggerMode = triggermode;
            this.DebuffType = debuff;
            this.Element = element;
            this.CurrentDuration = currduration;
            this.MaximumDuration = maximumduration;
        }

        //No Element
        public DebuffEffect(GameObject sender, string name, DebuffTriggerTypes triggermode, DebuffTypes debuff, float currduration, float maximumduration)
        {
            this.Sender = sender;
            this.DebuffName = name;
            this.TriggerMode = triggermode;
            this.DebuffType = debuff;
            this.CurrentDuration = currduration;
            this.MaximumDuration = maximumduration;
        }
    }


    [System.Serializable]
    public class TickHandler
    {
        [Header("Ticker")]
        public string tickName;
        public bool pingTick;
        public float tickCounter;
        public float tickRate;
    }

    [System.Serializable]
    public class DebuffElements
    {
        public List<DebuffEffect> DebuffEffects;
    }
    [System.Serializable]
    public class BuffEffect
    {
        [Header("Sender Reference")]
        public GameObject Sender;

        [Header("Basic Settings")]
        public string BuffName;
        public BuffTriggerMode TriggerMode;
        public BuffTypes Buff;
        public float CurrentDuration;
        public float MaximumDuration;
        public float FactorValue;

        [Header("Stack Settings")]
        public int CurrentStack;
        public int MaxStacks;
        public int StackValue = 1;
        public float StackIncrement;

        [Header("Iterative Settings")]
        public bool IterativeTrigger = false;
        public int CurrentAttackCounter;
        public int AttackThreshold;
    }


    [System.Serializable]
    public class BuffElements
    {
        public List<BuffEffect> BuffEffects;
    }

    public static class StateSetter
    {
        public static void SendBuff(BuffEffect NewBuff, EntityState Entity, EntityStats Stats)
        {
            //Set Buff Duration
            NewBuff.CurrentDuration = NewBuff.MaximumDuration;

            bool matchFound = false;

            //Check If The Buff Exists Or Not And The List Is Not Empty
            if (Entity.Buffs.BuffEffects.Count > 0)
            {
                //Loop Through List To Check For A Match
                for (int i = 0; i < Entity.Buffs.BuffEffects.Count; i++)
                {
                    //Check For A Match
                    if (NewBuff.BuffName == Entity.Buffs.BuffEffects[i].BuffName)
                    {
                        matchFound = true;

                        //Reset Duration
                        Entity.Buffs.BuffEffects[i].CurrentDuration = Entity.Buffs.BuffEffects[i].MaximumDuration;

                        //Trigger Handler
                        BuffTriggerHandler(NewBuff, Entity, i, Stats);
                        break;
                    }
                }

                if (!matchFound)
                {
                    //Add The Buff In
                    BuffTriggerHandler(NewBuff, Entity, -1, Stats);
                }
            }
            else
            {
                //Add The Buff In
                BuffTriggerHandler(NewBuff, Entity, -1, Stats);
            }
        }
        private static void BuffTriggerHandler(BuffEffect NewBuff, EntityState Entity, int i, EntityStats Stats)
        {
            //Note: i is the index passed

            //Trigger Conditions
            switch (Entity.Buffs.BuffEffects[i].TriggerMode)
            {
                case BuffTriggerMode.Passive:
                    {
                        //Apply Buff With No Additional Logic
                        AddBuff(NewBuff, Entity, Stats, i);
                    }
                    break;

                case BuffTriggerMode.OnNextAttack:
                    {
                        //Apply On Next Attack However, The Logic Of This Being Consumed On A Successfull Attack Will Trigger This Buff To Disappear prematurely
                        AddBuff(NewBuff, Entity, Stats, i);
                    }
                    break;

                case BuffTriggerMode.Iterative:
                    {
                        //Count Attack
                        NewBuff.CurrentAttackCounter = 1 + Entity.Buffs.BuffEffects[i].CurrentAttackCounter;

                        //Sets A Flag To True For Other Scripts To React To
                        if (NewBuff.CurrentAttackCounter >= NewBuff.AttackThreshold)
                        {
                            NewBuff.IterativeTrigger = true;
                        }

                        AddBuff(NewBuff, Entity, Stats, i);
                    }
                    break;

                case BuffTriggerMode.Stack:
                    {
                        //Increment Stack
                        int lastStack = Entity.Buffs.BuffEffects[i].CurrentStack + NewBuff.StackValue;
                        //Clamp Stacks
                        NewBuff.CurrentStack = Mathf.Clamp(NewBuff.CurrentStack, 0, NewBuff.MaxStacks);
                        //Set New Stack Count For New Buff
                        NewBuff.CurrentStack = lastStack;
                        //Add New Buffs
                        AddBuff(NewBuff, Entity, Stats, i);
                    }

                    break;
            }
        }
        public static void TriggerIterativeBuff(EntityState Entity, int i)
        {
            //i refers to the index of the buff

            //Reset Iteration On The Buff Once Effect Is Triggered 
            Entity.Buffs.BuffEffects[i].IterativeTrigger = false;
            Entity.Buffs.BuffEffects[i].CurrentAttackCounter = 0;
        }
        private static void AddBuff(BuffEffect NewBuff, EntityState Entity, EntityStats Stats, int i)
        {
            //Add Calculation To Stat Sheet So it can affect the entity
            //Depending On The Buff The Value Will Be Calculated And will be added to different places in different ways

            switch (NewBuff.Buff)
            {
                case BuffTypes.Null:
                    {
                        Debug.Log("Buff Has Null Empty Typing. Nothing Will Happen.");
                        return;
                    }
                    break;

                case BuffTypes.BonusHealth:
                    {
                        Debug.Log("Added Health Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.Health -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.Health += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.Health -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.Health += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.Health += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.Health += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.NightmareBonus:
                    {
                        Debug.Log("Added Nightmare Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.NightmareDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.NightmareDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.NightmareDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.NightmareDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.NightmareDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.NightmareDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.DreamBonus:
                    {
                        Debug.Log("Added Dream Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.DreamDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.DreamDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.DreamDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.DreamDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.DreamDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.DreamDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.GuardBonus:
                    {
                        Debug.Log("Added Guard Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.GuardIntegrity -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.GuardIntegrity += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.GuardIntegrity -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.GuardIntegrity += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.GuardIntegrity += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.GuardIntegrity += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.MagicalArmorBonus:
                    {
                        Debug.Log("Added Magical Armor Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.MagicalDamageFlatNumberReduction -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.MagicalDamageFlatNumberReduction += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.MagicalDamageFlatNumberReduction -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.MagicalDamageFlatNumberReduction += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.MagicalDamageFlatNumberReduction += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.MagicalDamageFlatNumberReduction += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;
                case BuffTypes.MagicalAtk:
                    {
                        Debug.Log("Added Magical Atk Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.MagicalAtk -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.MagicalAtk += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.MagicalAtk -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.MagicalAtk += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.MagicalAtk += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.MagicalAtk += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.MagicCriticalChance:
                    {
                        Debug.Log("Added Magic Critical Chance Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.MagicalCriticalStrikeChance -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.MagicalCriticalStrikeChance += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.MagicalCriticalStrikeChance -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.MagicalCriticalStrikeChance += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.MagicalCriticalStrikeChance += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.MagicalCriticalStrikeChance += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;
                case BuffTypes.MovementSpeedBonus:
                    {
                        Debug.Log("Added Movement Speed Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.MovementSpeedBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.MovementSpeedBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.MovementSpeedBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.MovementSpeedBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.MovementSpeedBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.MovementSpeedBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.NeedleBonus:
                    {
                        Debug.Log("Added Needle Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.NeedleDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.NeedleDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.NeedleDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.NeedleDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.NeedleDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.NeedleDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;
                case BuffTypes.ParryBonus:
                    {
                        Debug.Log("Added Parry Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.ParryDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.ParryDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.ParryDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.ParryDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.ParryDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.ParryDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.PhysicalArmorBonus:
                    {
                        Debug.Log("Added Physical Armor Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.PhysicalDamageFlatNumberReduction -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.PhysicalDamageFlatNumberReduction += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.PhysicalDamageFlatNumberReduction -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.PhysicalDamageFlatNumberReduction += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.PhysicalDamageFlatNumberReduction += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.PhysicalDamageFlatNumberReduction += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;
                case BuffTypes.PhysicalAtk:
                    {
                        Debug.Log("Added Physical Atk Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.PhysicalAtk -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.PhysicalAtk += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.PhysicalAtk -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.PhysicalAtk += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.PhysicalAtk += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.PhysicalAtk += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.PhysicalCriticalChance:
                    {
                        Debug.Log("Added Physical Critical Chance Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.PhysicalCriticalStrikeChance -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.PhysicalCriticalStrikeChance += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.PhysicalCriticalStrikeChance -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.PhysicalCriticalStrikeChance += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.PhysicalCriticalStrikeChance += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.PhysicalCriticalStrikeChance += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;
                case BuffTypes.PhysicalCriticalDamage:
                    {
                        Debug.Log("Added Physical Critical Damage Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.PhysicalCriticalStrikeDamage -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.PhysicalCriticalStrikeDamage += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.PhysicalCriticalStrikeDamage -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.PhysicalCriticalStrikeDamage += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.PhysicalCriticalStrikeDamage += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.PhysicalCriticalStrikeDamage += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.ScissorBonus:
                    {
                        Debug.Log("Added Scissor Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.ScissorDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.ScissorDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.ScissorDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.ScissorDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.ScissorDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.ScissorDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.SpinningRodBonus:
                    {
                        Debug.Log("Added SpinningRod Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.SpinningRodDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.SpinningRodDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.SpinningRodDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.SpinningRodDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.SpinningRodDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.SpinningRodDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;
                case BuffTypes.AttackSpeed:
                    {
                        Debug.Log("Added Attack Speed Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.AttackSpeed -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.AttackSpeed += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.AttackSpeed -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.AttackSpeed += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.AttackSpeed += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.AttackSpeed += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.NestelBonus:
                    {
                        Debug.Log("Added Nestel Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.NestelDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.NestelDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.NestelDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.NestelDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.NestelDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.NestelDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.AllElemental:
                    {
                        Debug.Log("Added All Elemental Damage Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.AllElementalDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.AllElementalDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.AllElementalDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.AllElementalDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.AllElementalDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.AllElementalDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.FireBonus:
                    {
                        Debug.Log("Added Fire Damage Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.FireDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.FireDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.FireDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.FireDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.FireDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.FireDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.WaterBonus:
                    {
                        Debug.Log("Added Water Damage Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.WaterDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.WaterDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.WaterDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.WaterDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.WaterDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.WaterDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.FrostBonus:
                    {
                        Debug.Log("Added Frost Damage Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.FrostDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.FrostDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.FrostDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.FrostDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.FrostDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.FrostDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.WindBonus:
                    {
                        Debug.Log("Added Wind Damage Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.WindDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.WindDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.WindDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.WindDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.WindDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.WindDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.ThunderBonus:
                    {
                        Debug.Log("Added Thunder Damage Bonus " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.ThunderDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.ThunderDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.ThunderDamageBonus -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.ThunderDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.ThunderDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.ThunderDamageBonus += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.ElementalDamageResist:
                    {
                        Debug.Log("Added Elemental Damage Resistance " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.ElementalDamageResistance -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.ElementalDamageResistance += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.ElementalDamageResistance -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.ElementalDamageResistance += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.ElementalDamageResistance += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.ElementalDamageResistance += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.FireChance:
                    {
                        Debug.Log("Added Fire Chance Resistance Shred " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.FireChanceResistance -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.FireChanceResistance += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.FireChanceResistance -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.FireChanceResistance += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.FireChanceResistance += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.FireChanceResistance += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.WaterChance:
                    {
                        Debug.Log("Added Water Chance Resistance Shred " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.WaterChanceResistance -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.WaterChanceResistance += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.WaterChanceResistance -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.WaterChanceResistance += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.WaterChanceResistance += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.WaterChanceResistance += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.ThunderChance:
                    {
                        Debug.Log("Added Thunder Chance Resistance Shred " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.ThunderChanceResistance -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.ThunderChanceResistance += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.ThunderChanceResistance -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.ThunderChanceResistance += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.ThunderChanceResistance += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.ThunderChanceResistance += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.FrostChance:
                    {
                        Debug.Log("Added Frost Chance Resistance Shred " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.FrostChanceResistance -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.FrostChanceResistance += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.FrostChanceResistance -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.FrostChanceResistance += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.FrostChanceResistance += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.FrostChanceResistance += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.WindChance:
                    {
                        Debug.Log("Added Wind Chance Resistance Shred " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.WindChanceResistance -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.WindChanceResistance += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.WindChanceResistance -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.WindChanceResistance += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.WindChanceResistance += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.WindChanceResistance += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.StatusChance:
                    {
                        Debug.Log("Added Status Chance " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.StatusChance -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.StatusChance += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.StatusChance -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.StatusChance += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.StatusChance += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.StatusChance += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.SummonBonus:
                    {
                        Debug.Log("Added Summons " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.SummoningPoints -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.SummoningPoints += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.SummoningPoints -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.SummoningPoints += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.SummoningPoints += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.SummoningPoints += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.CriticalChanceReduction:
                    {
                        Debug.Log("Added Critical Chance Reduction Shred " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.CriticalChanceReduction -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.CriticalChanceReduction += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.CriticalChanceReduction -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.CriticalChanceReduction += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.CriticalChanceReduction += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.CriticalChanceReduction += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.ShieldDamageResist:
                    {
                        Debug.Log("Added Shield Damage Resistance " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.ShieldDamageAbsorption -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.ShieldDamageAbsorption += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.ShieldDamageAbsorption -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.ShieldDamageAbsorption += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.ShieldDamageAbsorption += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.ShieldDamageAbsorption += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;

                case BuffTypes.StaggerResistance:
                    {
                        Debug.Log("Added Stagger Resistance " + Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack)));

                        //If Index of the same buff name is found then subtract the old and add the new version
                        if (i != -1)
                        {
                            //Passively Add It One Time
                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Passive) || Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.StaggerResistance -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue);
                                Stats.BuffSheet.StaggerResistance += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            if (Entity.Buffs.BuffEffects[i].TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.StaggerResistance -= Mathf.RoundToInt(Entity.Buffs.BuffEffects[i].FactorValue + (Entity.Buffs.BuffEffects[i].StackIncrement * Entity.Buffs.BuffEffects[i].CurrentStack));
                                Stats.BuffSheet.StaggerResistance += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                        //If Index is a unique name in the list then just add. No need to subtract anything
                        else
                        {
                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || NewBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                            {
                                Stats.BuffSheet.StaggerResistance += Mathf.RoundToInt(NewBuff.FactorValue);
                                break;
                            }

                            //Add New Buff Values
                            if (NewBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                            {
                                Stats.BuffSheet.StaggerResistance += Mathf.RoundToInt(NewBuff.FactorValue + (NewBuff.StackIncrement * NewBuff.CurrentStack));
                                break;
                            }
                        }
                    }
                    break;
            }

            //Add Buff To List
            Entity.Buffs.BuffEffects.Add(NewBuff);
        }
        public static void RemoveBuff(BuffEffect ExpiredBuff, EntityState Entity, EntityStats Stats, int i)
        {
            switch (ExpiredBuff.Buff)
            {
                case BuffTypes.Null:
                    {
                        Debug.Log("Buff Has Null Empty Typing. Nothing Will Happen.");
                        return;
                    }
                    break;

                case BuffTypes.BonusHealth:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.Health -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.Health -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Health Bonus " + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.NightmareBonus:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.NightmareDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.NightmareDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Nightmare Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.DreamBonus:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.DreamDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.DreamDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Dream Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.GuardBonus:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.GuardIntegrity -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.GuardIntegrity -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Guard Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.MagicalArmorBonus:
                    {
                        ///Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.MagicalDamageFlatNumberReduction -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.MagicalDamageFlatNumberReduction -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Magical Armor Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;
                case BuffTypes.MagicalAtk:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.MagicalAtk -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.MagicalAtk -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Magical Attack Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.MagicCriticalChance:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.MagicalCriticalStrikeChance -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.MagicalCriticalStrikeChance -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Magical Critical Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;
                case BuffTypes.MovementSpeedBonus:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.MovementSpeedBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.MovementSpeedBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Movement Speed Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.NeedleBonus:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.NeedleDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.NeedleDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Needle Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;
                case BuffTypes.ParryBonus:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.ParryDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.ParryDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Parry Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.PhysicalArmorBonus:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.PhysicalDamageFlatNumberReduction -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.PhysicalDamageFlatNumberReduction -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Physical Armor Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;
                case BuffTypes.PhysicalAtk:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.PhysicalAtk -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.PhysicalAtk -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Physical Atk Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.PhysicalCriticalChance:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.PhysicalCriticalStrikeChance -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.PhysicalCriticalStrikeChance -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Physical Critical Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;
                case BuffTypes.PhysicalCriticalDamage:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.PhysicalCriticalStrikeDamage -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.PhysicalCriticalStrikeDamage -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Physical Critical Damage Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.ScissorBonus:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.ScissorDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.ScissorDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Scissor Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.SpinningRodBonus:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.SpinningRodDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.SpinningRodDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Spun Bow Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.AttackSpeed:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.AttackSpeed -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.AttackSpeed -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Attack Speed Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.NestelBonus:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.NestelDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.NestelDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Nestel Damage Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.AllElemental:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.AllElementalDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.AllElementalDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed All Elemental Damage Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.FireBonus:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.FireDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.FireDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Fire Damage Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.WaterBonus:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.WaterDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.WaterDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Water Damage Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.FrostBonus:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.FrostDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.FrostDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Frost Damage Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.WindBonus:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.WindDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.WindDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Wind Damage Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.ThunderBonus:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.ThunderDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.ThunderDamageBonus -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Thunder Damage Bonus" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.ElementalDamageResist:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.ElementalDamageResistance -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.ElementalDamageResistance -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Elemental Damage Resistance " + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.FireChance:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.FireChanceResistance -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.FireChanceResistance -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Fire Chance Resistance " + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.WaterChance:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.WaterChanceResistance -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.WaterChanceResistance -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Water Chance Resistance " + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.ThunderChance:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.ThunderChanceResistance -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.ThunderChanceResistance -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Thunder Chance Resistance " + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.FrostChance:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.FrostChanceResistance -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.FrostChanceResistance -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Frost Chance Resistance " + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.WindChance:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.WindChanceResistance -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.WindChanceResistance -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Wind Chance Resistance " + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.StatusChance:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.StatusChance -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.StatusChance -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Status Chance" + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.SummonBonus:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.SummoningPoints -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.SummoningPoints -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Summon Bonus " + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.CriticalChanceReduction:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.CriticalChanceReduction -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.CriticalChanceReduction -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Critical Chance Reduction " + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.ShieldDamageResist:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.ShieldDamageAbsorption -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.ShieldDamageAbsorption -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Shield Damage Absorption " + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;

                case BuffTypes.StaggerResistance:
                    {
                        //Passively Add It One Time
                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Passive) || ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.OnNextAttack))
                        {
                            Stats.BuffSheet.StaggerResistance -= Mathf.RoundToInt(ExpiredBuff.FactorValue);
                            break;
                        }

                        if (ExpiredBuff.TriggerMode.Equals(BuffTriggerMode.Stack))
                        {
                            Stats.BuffSheet.StaggerResistance -= Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack));
                            break;
                        }

                        Debug.Log("Removed Stagger Resistance " + Mathf.RoundToInt(ExpiredBuff.FactorValue + (ExpiredBuff.StackIncrement * ExpiredBuff.CurrentStack)));
                    }
                    break;
            }

            //Add Buff To List
            Entity.Buffs.BuffEffects.Remove(ExpiredBuff);
        }
        public static void SendDebuff(DebuffEffect NewDebuff, EntityState Entity, EntityStats Stats)
        {
            bool matchFound = false;

            //Check If The DeBuff Exists Or Not And The List Is Not Empty
            //Debuff Immunity
            if (Entity.Immunities.Contains(NewDebuff.DebuffType))
            {
                Debug.Log("Target Is Immune To This Debuff");
                return;
            }

            //Loop Through List To Check For A Match
            foreach (DebuffEffect Debuff in Entity.Debuffs.DebuffEffects)
            {
                //Check For A Match
                if (NewDebuff.DebuffName == Debuff.DebuffName)
                {
                    //Reset Duration
                    Debuff.CurrentDuration = Debuff.MaximumDuration;

                    //Trigger Handler
                    DebuffTriggerHandler(NewDebuff, Entity, Entity.Debuffs.DebuffEffects.IndexOf(Debuff), Stats);
                    Debug.Log("Matching Debuff Found " + NewDebuff.DebuffName);
                    return;
                }
            }

            if (!matchFound)
            {
                //Add The DeBuff In
                DebuffTriggerHandler(NewDebuff, Entity, -1, Stats);
                Debug.Log("Added Unique Debuff " + NewDebuff.DebuffName);
            }
        }
        private static void DebuffTriggerHandler(DebuffEffect NewDebuff, EntityState Entity, int i, EntityStats Stats)
        {
            //Note: i is the index passed

            //Trigger Conditions
            //Debuff Types
            switch (NewDebuff.TriggerMode)
            {
                case DebuffTriggerTypes.Passive:
                    {
                        //Apply DeBuff With No Additional Logic
                        AddDebuff(NewDebuff, Entity, Stats);
                        Debug.Log("Added Passive");
                    }
                    break;

                case DebuffTriggerTypes.TickRate:
                    {
                        //Tick Rate Class
                        TickHandler NewTick = new TickHandler();

                        NewTick.tickName = NewDebuff.DebuffName;
                        NewTick.tickRate = NewDebuff.TickRate;

                        Entity.TickHolders.Add(NewTick);

                        //Apply Debuff
                        AddDebuff(NewDebuff, Entity, Stats);
                        Debug.Log("Added Tick Rate");
                    }
                    break;
            }
        }
        private static void AddDebuff(DebuffEffect NewDebuff, EntityState Entity, EntityStats Stats)
        {
            
            switch (NewDebuff.DebuffType)
            {
                case DebuffTypes.Null:
                    {
                        Debug.Log("Nothing Can Be Added. The Debuff Type Is Null");
                        return;
                    }
                    break;

                case DebuffTypes.Stun:
                    {
                        Entity.isStunned = true;
                        Debug.Log("Applied Stun");
                        Entity.Debuffs.DebuffEffects.Add(NewDebuff);
                        Debug.Log("Added " + NewDebuff.DebuffName + " To The Entity " + "(" + Entity.name + ") " + NewDebuff.TriggerMode.ToString());
                    }
                    break;

                case DebuffTypes.GuardBreak:
                    {
                        Entity.isGuardBroken = true;
                        Debug.Log("Applied Guard Broken");
                        Entity.Debuffs.DebuffEffects.Add(NewDebuff);
                        Debug.Log("Added " + NewDebuff.DebuffName + " To The Entity " + "(" + Entity.name + ") " + NewDebuff.TriggerMode.ToString());
                    }
                    break;

                case DebuffTypes.Tattered:
                    {
                        Entity.isTattered = true;
                        Debug.Log("Applied Tattered (Skill Paralysis)");
                        Entity.Debuffs.DebuffEffects.Add(NewDebuff);
                        Debug.Log("Added " + NewDebuff.DebuffName + " To The Entity " + "(" + Entity.name + ") " + NewDebuff.TriggerMode.ToString());
                    }
                    break;
                case DebuffTypes.Slowed:
                    {
                        Stats.DebuffSheet.MovementSpeedReduction += NewDebuff.FactorValue;
                        Debug.Log("Applied A Slow Of " + NewDebuff.FactorValue + "%");
                        Entity.Debuffs.DebuffEffects.Add(NewDebuff);
                        Debug.Log("Added " + NewDebuff.DebuffName + " To The Entity " + "(" + Entity.name + ") " + NewDebuff.TriggerMode.ToString());
                    }
                    break;
                case DebuffTypes.GuardCrack:
                    {
                        Entity.isGuardCracked = true;
                        Debug.Log("Applied Guard Crack");
                        Entity.Debuffs.DebuffEffects.Add(NewDebuff);
                        Debug.Log("Added " + NewDebuff.DebuffName + " To The Entity " + "(" + Entity.name + ") " + NewDebuff.TriggerMode.ToString());
                    }
                    break;
                case DebuffTypes.ElementalEffect:
                    {
                        switch (NewDebuff.Element)
                        {
                            case ElementalAffix.Fire:
                                Entity.isFireAffected = true;
                                Entity.Debuffs.DebuffEffects.Add(NewDebuff);
                                Debug.Log("Added " + NewDebuff.DebuffName + " To The Entity " + "(" + Entity.name + ") " + NewDebuff.TriggerMode.ToString());
                                break;
                            case ElementalAffix.Water:
                                Entity.isWaterAffected = true;
                                Entity.Debuffs.DebuffEffects.Add(NewDebuff);
                                Debug.Log("Added " + NewDebuff.DebuffName + " To The Entity " + "(" + Entity.name + ") " + NewDebuff.TriggerMode.ToString());
                                
                                break;
                            case ElementalAffix.Wind:
                                Entity.isWindAffected = true;
                                Entity.Debuffs.DebuffEffects.Add(NewDebuff);
                                Debug.Log("Added " + NewDebuff.DebuffName + " To The Entity " + "(" + Entity.name + ") " + NewDebuff.TriggerMode.ToString());
                                break;
                            case ElementalAffix.Thunder:
                                Entity.Debuffs.DebuffEffects.Add(NewDebuff);
                                Debug.Log("Added " + NewDebuff.DebuffName + " To The Entity " + "(" + Entity.name + ") " + NewDebuff.TriggerMode.ToString());
                                break;
                            case ElementalAffix.Frost:
                                Entity.isFrostAffected = true;
                                Entity.Debuffs.DebuffEffects.Add(NewDebuff);
                                Debug.Log("Added " + NewDebuff.DebuffName + " To The Entity " + "(" + Entity.name + ") " + NewDebuff.TriggerMode.ToString());
                                break;
                            case ElementalAffix.HolyFire:
                                Entity.isDreamFireAffected = true;
                                Entity.Debuffs.DebuffEffects.Add(NewDebuff);
                                Debug.Log("Added " + NewDebuff.DebuffName + " To The Entity " + "(" + Entity.name + ") " + NewDebuff.TriggerMode.ToString());
                                break;
                            case ElementalAffix.HeavenlyGale:
                                Entity.isDreamWaterAffected = true;
                                Entity.Debuffs.DebuffEffects.Add(NewDebuff);
                                Debug.Log("Added " + NewDebuff.DebuffName + " To The Entity " + "(" + Entity.name + ") " + NewDebuff.TriggerMode.ToString());
                                break;
                            case ElementalAffix.SkywardCry:
                                Entity.isDreamWindAffected = true;
                                Entity.Debuffs.DebuffEffects.Add(NewDebuff);
                                Debug.Log("Added " + NewDebuff.DebuffName + " To The Entity " + "(" + Entity.name + ") " + NewDebuff.TriggerMode.ToString());
                                break;
                            case ElementalAffix.Cryopathy:
                                Entity.isDreamThunderAffected = true;
                                Entity.Debuffs.DebuffEffects.Add(NewDebuff);
                                Debug.Log("Added " + NewDebuff.DebuffName + " To The Entity " + "(" + Entity.name + ") " + NewDebuff.TriggerMode.ToString());
                                break;
                            case ElementalAffix.CleansingTide:
                                Entity.isDreamFrostAffected = true;
                                Entity.Debuffs.DebuffEffects.Add(NewDebuff);
                                Debug.Log("Added " + NewDebuff.DebuffName + " To The Entity " + "(" + Entity.name + ") " + NewDebuff.TriggerMode.ToString());
                                break;

                            case ElementalAffix.ShadowFlame:
                                Entity.isNightmareFireAffected = true;
                                Entity.Debuffs.DebuffEffects.Add(NewDebuff);
                                Debug.Log("Added " + NewDebuff.DebuffName + " To The Entity " + "(" + Entity.name + ") " + NewDebuff.TriggerMode.ToString());
                                break;
                            case ElementalAffix.AbyssalCurrent:
                                Entity.isNightmareWaterAffected = true;
                                Entity.Debuffs.DebuffEffects.Add(NewDebuff);
                                Debug.Log("Added " + NewDebuff.DebuffName + " To The Entity " + "(" + Entity.name + ") " + NewDebuff.TriggerMode.ToString());
                                break;
                            case ElementalAffix.SinisterWinds:
                                Entity.isNightmareWindAffected = true;
                                Entity.Debuffs.DebuffEffects.Add(NewDebuff);
                                Debug.Log("Added " + NewDebuff.DebuffName + " To The Entity " + "(" + Entity.name + ") " + NewDebuff.TriggerMode.ToString());
                                break;
                            case ElementalAffix.NightStrikes:
                                Entity.isNightmareThunderAffected = true;
                                Entity.Debuffs.DebuffEffects.Add(NewDebuff);
                                Debug.Log("Added " + NewDebuff.DebuffName + " To The Entity " + "(" + Entity.name + ") " + NewDebuff.TriggerMode.ToString());
                                break;
                            case ElementalAffix.PhantomFreeze:
                                Entity.isNightmareFrostAffected = true;
                                Entity.Debuffs.DebuffEffects.Add(NewDebuff);
                                Debug.Log("Added " + NewDebuff.DebuffName + " To The Entity " + "(" + Entity.name + ") " + NewDebuff.TriggerMode.ToString());
                                break;

                            case ElementalAffix.World:
                                Entity.isWorldAffected = true;
                                break;
                        }
                    }
                    break;

                case DebuffTypes.AttackSpeedReduction:
                    Stats.DebuffSheet.AttackSpeedReduction += NewDebuff.FactorValue;
                    Entity.Debuffs.DebuffEffects.Add(NewDebuff);
                    Debug.Log("Added " + NewDebuff.DebuffName + " To The Entity " + "(" + Entity.name + ") " + NewDebuff.TriggerMode.ToString());
                    break;

                case DebuffTypes.DamageAmplification:
                    Stats.DebuffSheet.DamageAmplification += NewDebuff.FactorValue;
                    Entity.Debuffs.DebuffEffects.Add(NewDebuff);
                    Debug.Log("Added " + NewDebuff.DebuffName + " To The Entity " + "(" + Entity.name + ") " + NewDebuff.TriggerMode.ToString());
                    break;
            }

        }
        public static void RemoveDebuff(DebuffEffect ExpiredDebuff, EntityState Entity, EntityStats Stats)
        {
            bool match = false;

            //If Its A Tick Rate Then the ticker must be deleted too
            if (ExpiredDebuff.TriggerMode.Equals(DebuffTriggerTypes.TickRate))
            {
                //Loop to look for debuff name that matches the tick name
                for(int i = 0; i < Entity.TickHolders.Count; i ++)
                {
                    if(ExpiredDebuff.DebuffName == Entity.TickHolders[i].tickName)
                    {
                        Entity.TickHolders.Remove(Entity.TickHolders[i]);
                        Debug.Log("Ticker Deleted");
                        break;
                    }
                }

                Entity.Debuffs.DebuffEffects.Remove(ExpiredDebuff);
                Debug.Log("Removed Ticking Debuff");
                return;
            }
            //Handle Removing Debuff Normally
            else
            {
                //Debuff Types
                switch (ExpiredDebuff.DebuffType)
                {
                    case DebuffTypes.Null:
                        {
                            Debug.Log("Nothing Can Be Added. The Debuff Type Is Null");
                            return;
                        }
                        break;

                    case DebuffTypes.Stun:
                        {
                            for (int i = 0; i < Entity.Debuffs.DebuffEffects.Count; i++)
                            {
                                //If a copy is found we stop and dont switch stun off
                                if (Entity.Debuffs.DebuffEffects[i].DebuffType.Equals(DebuffTypes.Stun) && Entity.Debuffs.DebuffEffects[i].DebuffName != Entity.Debuffs.DebuffEffects[i].DebuffName)
                                {
                                    match = true;
                                    Debug.Log("Additional Stun Found. Not Removing Stun.");
                                    return;
                                }
                            }

                            if (!match)
                            {
                                Entity.isStunned = false;
                                Debug.Log("Stun Cleared. This was the last stun.");
                            }

                            Entity.Debuffs.DebuffEffects.Remove(ExpiredDebuff);
                        }
                        break;

                    case DebuffTypes.GuardBreak:
                        {
                            Entity.Debuffs.DebuffEffects.Remove(ExpiredDebuff);

                            if (Entity.Debuffs.DebuffEffects.Count > 0)
                            {
                                for (int i = 0; i < Entity.Debuffs.DebuffEffects.Count; i++)
                                {
                                    //If a copy is found we stop and dont switch stun off
                                    if (Entity.Debuffs.DebuffEffects[i].DebuffType.Equals(DebuffTypes.GuardBreak))
                                    {
                                        match = true;
                                        Debug.Log("Additional Stun Found. Not Removing Stun.");
                                        return;
                                    }
                                }

                                if (!match)
                                {
                                    Entity.isGuardBroken = false;
                                    Debug.Log("Stun Cleared. This was the last stun.");
                                }
                            }
                        }
                        break;

                    case DebuffTypes.GuardCrack:
                        {
                            Entity.Debuffs.DebuffEffects.Remove(ExpiredDebuff);

                            if (Entity.Debuffs.DebuffEffects.Count > 0)
                            {
                                for (int i = 0; i < Entity.Debuffs.DebuffEffects.Count; i++)
                                {
                                    //If a copy is found we stop and dont switch stun off
                                    if (Entity.Debuffs.DebuffEffects[i].DebuffType.Equals(DebuffTypes.GuardCrack))
                                    {
                                        match = true;
                                        Debug.Log("Additional Guard Crack Found. Not Removing Stun.");
                                        return;
                                    }
                                }

                                if (!match)
                                {
                                    Entity.isGuardCracked = false;
                                    Debug.Log("Guard Crack Cleared. This was the last guard crack.");
                                }
                            }
                        }
                        break;

                    case DebuffTypes.Tattered:
                        {
                            Entity.Debuffs.DebuffEffects.Remove(ExpiredDebuff);

                            if (Entity.Debuffs.DebuffEffects.Count > 0)
                            {
                                for (int i = 0; i < Entity.Debuffs.DebuffEffects.Count; i++)
                                {
                                    //If a copy is found we stop and dont switch stun off
                                    if (Entity.Debuffs.DebuffEffects[i].DebuffType.Equals(DebuffTypes.Tattered))
                                    {
                                        match = true;
                                        Debug.Log("Additional Stun Found. Not Removing Stun.");
                                        return;
                                    }
                                }

                                if (!match)
                                {
                                    Entity.isTattered = false;
                                    Debug.Log("Stun Cleared. This was the last stun.");
                                }
                            }
                        }
                        break;
                    case DebuffTypes.Slowed:
                        {
                            Stats.DebuffSheet.MovementSpeedReduction -= ExpiredDebuff.FactorValue;
                            Debug.Log("Reduced Movement Speed Penalty by " + ExpiredDebuff.FactorValue + "%");

                            Entity.Debuffs.DebuffEffects.Remove(ExpiredDebuff);
                        }
                        break;

                    case DebuffTypes.ElementalEffect:
                        {
                            switch (ExpiredDebuff.Element)
                            {
                                case ElementalAffix.Fire:
                                    Entity.isFireAffected = false;
                                    break;
                                case ElementalAffix.Water:
                                    Entity.isWaterAffected = false;
                                    break;
                                case ElementalAffix.Wind:
                                    Entity.isWindAffected = false;
                                    Entity.rb.useGravity = true;

                                    if (Entity.Player != null)
                                    {
                                        Entity.Player.physicsProperties.ApplyGravity = true;
                                    }
                                    break;
                                case ElementalAffix.Thunder:
                                    Entity.isThunderAffected = false;
                                    break;
                                case ElementalAffix.Frost:
                                    Entity.isFrostAffected = false;
                                    break;

                                case ElementalAffix.HolyFire:
                                    Entity.isDreamFireAffected = false;
                                    break;
                                case ElementalAffix.HeavenlyGale:
                                    Entity.isDreamWaterAffected = false;
                                    break;
                                case ElementalAffix.SkywardCry:
                                    Entity.isDreamWindAffected = false;
                                    break;
                                case ElementalAffix.Cryopathy:
                                    Entity.isDreamThunderAffected = false;
                                    break;
                                case ElementalAffix.CleansingTide:
                                    Entity.isDreamFrostAffected = false;
                                    break;

                                case ElementalAffix.ShadowFlame:
                                    Entity.isNightmareFireAffected = false;
                                    break;
                                case ElementalAffix.AbyssalCurrent:
                                    Entity.isNightmareWaterAffected = false;
                                    break;
                                case ElementalAffix.SinisterWinds:
                                    Entity.isNightmareWindAffected = false;
                                    break;
                                case ElementalAffix.NightStrikes:
                                    Entity.isNightmareThunderAffected = false;
                                    break;
                                case ElementalAffix.PhantomFreeze:
                                    Entity.isNightmareFrostAffected = false;
                                    break;
                                case ElementalAffix.World:
                                    Entity.isWorldAffected = false;
                                    break;
                            }

                            Entity.Debuffs.DebuffEffects.Remove(ExpiredDebuff);
                            Debug.Log("Removed Elemental Effect");
                        }
                        break;

                    case DebuffTypes.AttackSpeedReduction:
                        Stats.DebuffSheet.AttackSpeedReduction -= ExpiredDebuff.FactorValue;
                        Entity.Debuffs.DebuffEffects.Remove(ExpiredDebuff);
                        break;

                    case DebuffTypes.DamageAmplification:
                        Stats.DebuffSheet.DamageAmplification -= ExpiredDebuff.FactorValue;
                        Entity.Debuffs.DebuffEffects.Remove(ExpiredDebuff);
                        break;
                }
            }
        }
        public static void StaggerChance(bool isGuarenteed, StaggerEffect Stagger, EntityStats Stats)
        {
            if(isGuarenteed)
            {
                Debug.Log("Stagger Attempt Successfull");
                Stats.EnemySheet.isStaggered = true;
            }
            else
            {
                if(Stagger.StaggerResistance > 0)
                {
                    int staggerRoll = Random.Range(0, 100);

                    if (Stagger.StaggerResistance >= staggerRoll)
                    {
                        Debug.Log("Stagger Attempt Successfull");
                        Stats.EnemySheet.isStaggered = true;
                    }
                    else
                    {
                        Debug.Log("Stagger Attempt Failed.");
                    }
                }
                else
                {
                    Debug.Log("Stagger Attempt Failed.");
                }
                
            }
        }
    }
}
