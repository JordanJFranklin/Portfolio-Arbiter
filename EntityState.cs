using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HealthClasses;
using StateClasses;
using StatClasses;
using DamageClass;

public class EntityState : MonoBehaviour
{
    public List<DebuffTypes> Immunities;
    public List<ElementalAffix> ElementalWeakness;
    public List<ElementalAffix> ElementalStrengths;
    public DebuffElements Debuffs;
    public BuffElements Buffs;
    public List<TickHandler> TickHolders;

    [Header("Status Condtions")]
    public bool isStunned;
    public bool isGuardBroken;
    public bool isTattered;
    public bool isGuardCracked;

    [Header("Elemental Neutral Condtions")]
    public bool isFireAffected;
    public bool isThunderAffected;
    public bool isFrostAffected;
    public bool isWaterAffected;
    public bool isWindAffected;
    [Header("Elemental Dream Condtions")]
    public bool isDreamFireAffected;
    public bool isDreamThunderAffected;
    public bool isDreamFrostAffected;
    public bool isDreamWaterAffected;
    public bool isDreamWindAffected;
    [Header("Elemental Nightmare Condtions")]
    public bool isNightmareFireAffected;
    public bool isNightmareThunderAffected;
    public bool isNightmareFrostAffected;
    public bool isNightmareWaterAffected;
    public bool isNightmareWindAffected;
    [Header("Special Element Condtions")]
    public bool isWorldAffected;

    private static float elementalFireRate = 0.5f;
    public LayerMask WallAOE = 1 << 0 | 1 << 13;
    public LayerMask PlayerElement = 1 << 9;
    public LayerMask EnemyElement = 1 << 12;
    private float elementalWindLiftForce = 1f;
    private float elementalWindSuctionForce = 30f;
    private static float elementalWindRate = 0.2f;
    private static float elementalWindRadius = 10f;
    private static float fireDuration = 10;
    private static float waterDuration = 10;
    private static float windDuration = 7;
    private static float lightningStunDuration = 2.5f;
    private static float lightningRadius = 5;
    private static float frostDuration = 7;
    private static float frostSlowPercentage = 0.25f;
    private static float frostAttackSpeedSlowPercentage = 0.25f;
    private static float frostBonusDamagePercentage = 0.25f;
    public DamageTester Test;

    private EntityStats Stats;
    private EntityHealth Health;
    public PlayerDriver Player;
    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        Stats = GetComponent<EntityStats>();
        Health = GetComponent<EntityHealth>();
        Player = GetComponent<PlayerDriver>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        CountDown();
        TickHandler();
        OnFire();
        Windy();
        Thunderbolt();
        Freezing();
        Drenched();
        EnemyWeightClass();


        //Fix Duplication Issue
        if (Input.GetKeyDown(KeyCode.F1) && Test != null)
        {
            CallHealthEvent.SendDamageEvent(Test.DamageSource, Health);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        //Physically Touching An Enemy While On Fire Has A Chance For Non Fire Immune Enemies To Become On Fire
        if (isFireAffected && collision.gameObject.GetComponent<EntityState>() != null && collision.gameObject.GetComponent<EntityHealth>() != null && collision.gameObject.GetComponent<EntityStats>() != null)
        {
            if (GetComponent<EntityHealth>().Identity.Equals(EntityTarget.Player))
            {
                DamageProperty OnFire = new DamageProperty(gameObject, EntityTarget.Enemy, "", 0, 0, Stats.PlayerSheet.StatusChance, 0, DamageClassification.Magical, DamageTypes.ElementalDamage, ElementalAffix.Fire, 0, 0);
                CallHealthEvent.SendDamageEvent(OnFire, collision.gameObject.GetComponent<EntityHealth>());
            }

            if (GetComponent<EntityHealth>().Identity.Equals(EntityTarget.Enemy))
            {
                DamageProperty OnFire = new DamageProperty(gameObject, EntityTarget.Player, "", 0, 0, Stats.PlayerSheet.StatusChance, 0, DamageClassification.Magical, DamageTypes.ElementalDamage, ElementalAffix.Fire, 0, 0);
                CallHealthEvent.SendDamageEvent(OnFire, collision.gameObject.GetComponent<EntityHealth>());
            }
        }

        //Catch Fire On Enviroment
        if(collision.gameObject.GetComponent<ElementalEntity>() != null)
        {
            switch(collision.gameObject.GetComponent<ElementalEntity>().Element)
            {
                case ElementalAffix.Fire:
                    DebuffEffect Fire = new DebuffEffect(gameObject, "On Fire", DebuffTriggerTypes.Passive, DebuffTypes.ElementalEffect, ElementalAffix.Fire, fireDuration, fireDuration);

                    StateSetter.SendDebuff(Fire, GetComponent<EntityState>(), Stats);
                    break;
                default:
                    Debug.Log("Element Type " + collision.gameObject.GetComponent<ElementalEntity>().Element + " cannot be passed.");
                    break;
            }
        }
    }

    private void EnemyWeightClass()
    {
        if(Health.Identity.Equals(EntityTarget.Enemy) && rb != null)
        {
            switch (Stats.EnemySheet.Size)
            {
                case EnemySize.Small:
                    rb.constraints = RigidbodyConstraints.None;
                    break;

                case EnemySize.Normal:
                    rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                    break;

                case EnemySize.Large:
                    rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                    break;
            }
        }
    }

    #region Elements
    private float nextOnFireTick = 0;
    void OnFire()
    {
        //SEE DODGE ON PLAYERDRIVER For Dodging Reduces Duration Of Burn
        //SEE ON COLLISON ENTER For Physical Touch Of Entities Passing Fire
        //SEE ON COLLISION ENTER For Non Fire Immune Enemies That Touch A On Fire Object In The Enviroment Guarentees On Fire Status

        if (isFireAffected)
        {
            DebuffEffect Fire = new DebuffEffect(gameObject, "On Fire", DebuffTriggerTypes.Passive, DebuffTypes.ElementalEffect, ElementalAffix.Fire, fireDuration, fireDuration);

            StateSetter.SendDebuff(Fire, GetComponent<EntityState>(), Stats);

            //Cleanses Fire
            if (isWaterAffected || isDreamWaterAffected || isNightmareWaterAffected)
            {
                StateSetter.RemoveDebuff(Fire,GetComponent<EntityState>(), Stats);
                nextOnFireTick = 0;
            }

            //Extend/Refresh Fire Duration
            //Hitting With Another Successfull Instance Of The Same Element Refreshes The Duration too.
            if (isWindAffected || isDreamWindAffected || isNightmareWindAffected)
            {
                StateSetter.SendDebuff(Fire, GetComponent<EntityState>(), Stats);
            }

            //Burning Effect. Elemental Damage Instances Cannot Proc The Same Effect Over Again
            if(GetComponent<EntityHealth>().Identity.Equals(EntityTarget.Player))
            {
                DamageProperty OnFire = new DamageProperty(gameObject, EntityTarget.Enemy, "", 0, Stats.PlayerSheet.PhysicalAtk + Stats.PlayerSheet.MagicalAtk, Stats.PlayerSheet.StatusChance, 0.1f, DamageClassification.Magical, DamageTypes.ElementalDamage, ElementalAffix.Fire, 0, 0);

                if(Time.time > nextOnFireTick)
                {
                    //FireRate
                    nextOnFireTick = Time.time + elementalFireRate;

                    CallHealthEvent.SendDamageEvent(OnFire, Health);
                }
            }

            if (GetComponent<EntityHealth>().Identity.Equals(EntityTarget.Enemy))
            {
                DamageProperty OnFire = new DamageProperty(gameObject, EntityTarget.Player, "", 0, Stats.EnemySheet.PhysicalDamage + Stats.EnemySheet.MagicalDamage, Stats.PlayerSheet.StatusChance, 0.1f, DamageClassification.Magical, DamageTypes.ElementalDamage, ElementalAffix.Fire, 0, 0);

                if (Time.time > nextOnFireTick)
                {
                    //FireRate
                    nextOnFireTick = Time.time + elementalFireRate;

                    CallHealthEvent.SendDamageEvent(OnFire, Health);
                }
            }
        }
    }
    private float nextOnWindTick = 0;
    void Windy()
    {
        //Must Add Other Effects To This Besides Damage
        //Stagger, Lifted, Guard Prevention
        
        if (isWindAffected)
        {
            DebuffEffect Wind = new DebuffEffect(gameObject, "Winded", DebuffTriggerTypes.Passive, DebuffTypes.ElementalEffect, ElementalAffix.Wind, windDuration, windDuration);

            StateSetter.SendDebuff(Wind, GetComponent<EntityState>(), Stats);

            //Wind Effect. Elemental Damage Instances Cannot Proc The Same Effect Over Again
            if (GetComponent<EntityHealth>().Identity.Equals(EntityTarget.Player))
            {
                //Vaccum Effect
                Collider[] VaccumForce = Physics.OverlapSphere(transform.position, elementalWindRadius, PlayerElement);

                foreach (Collider Other in VaccumForce)
                {
                    if (Other.gameObject.GetComponent<Rigidbody>() != null)
                    {
                        Vector3 dir = transform.position - Other.transform.position;

                        //Apply Vaccum
                        Other.gameObject.GetComponent<Rigidbody>().AddExplosionForce(-elementalWindSuctionForce, transform.position, elementalWindRadius, 0f, ForceMode.Acceleration);
                    }
                }

                DamageProperty Windy = new DamageProperty(gameObject, EntityTarget.Player, "", 0, Stats.PlayerSheet.MagicalAtk + Stats.PlayerSheet.PhysicalAtk, Stats.PlayerSheet.StatusChance, 0.05f, DamageClassification.Magical, DamageTypes.ElementalDamage, ElementalAffix.Wind, 0, 0);

                if (Time.time > nextOnWindTick)
                {
                    //FireRate
                    nextOnWindTick = Time.time + elementalWindRate;

                    //Damage To This Entity
                    CallHealthEvent.SendDamageEvent(Windy, Health);

                    //AoE Damage
                    foreach(Collider Enemy in VaccumForce)
                    {
                        //Damage To Other Entities In Vaccum
                        CallHealthEvent.SendDamageEvent(Windy, Enemy.gameObject.GetComponent<EntityHealth>());

                        //if enemy is guarding has a chance to stagger them slowing their attack speed
                        if (Stats.EnemySheet.isGuarding)
                        {
                            StaggerEffect Stagger = new StaggerEffect(StaggerType.LightStagger, Stats.PlayerSheet.StaggerResistance);

                            StateSetter.StaggerChance(false, Stagger, Stats);
                        }
                    }
                }
            }

            if (GetComponent<EntityHealth>().Identity.Equals(EntityTarget.Enemy))
            {
                //Vaccum Effect
                Collider[] VaccumForce = Physics.OverlapSphere(transform.position, elementalWindRadius, EnemyElement);

                //Small Enemies Get Lifted
                if (Stats.EnemySheet.Size.Equals(EnemySize.Small))
                {
                    transform.position += (Vector3.up * elementalWindLiftForce) * Time.deltaTime;
                    rb.useGravity = false;
                }

                //Medium Enemies Vaccum Effect
                foreach (Collider Other in VaccumForce)
                {
                    if (Other.gameObject.GetComponent<Rigidbody>() != null)
                    {
                        Vector3 dir = transform.position - Other.transform.position;

                        //Apply Vaccum
                        if (Other.gameObject.GetComponent<EntityStats>().EnemySheet.Size.Equals(EnemySize.Small) || Other.gameObject.GetComponent<EntityStats>().EnemySheet.Size.Equals(EnemySize.Normal))
                        {
                            Other.gameObject.GetComponent<Rigidbody>().AddExplosionForce(-elementalWindSuctionForce, transform.position, elementalWindRadius, 0f, ForceMode.Acceleration);
                        }
                    }
                }

                DamageProperty Windy = new DamageProperty(gameObject, EntityTarget.Enemy, "", 0, Stats.EnemySheet.PhysicalDamage + Stats.EnemySheet.MagicalDamage, Stats.PlayerSheet.StatusChance, 0.05f, DamageClassification.Magical, DamageTypes.ElementalDamage, ElementalAffix.Wind, 0, 0);

                if (Time.time > nextOnWindTick)
                {
                    //FireRate
                    nextOnWindTick = Time.time + elementalWindRate;

                    CallHealthEvent.SendDamageEvent(Windy, Health);

                    foreach (Collider Enemy in VaccumForce)
                    {
                        //Deals Damage To Enemy Health Bars
                        CallHealthEvent.SendDamageEvent(Windy, Enemy.gameObject.GetComponent<EntityHealth>());

                        //if enemy is guarding has a chance to stagger them slowing their attack speed
                        if(Stats.EnemySheet.isGuarding)
                        {
                            StaggerEffect Stagger = new StaggerEffect(StaggerType.LightStagger, Stats.EnemySheet.StaggerResistance);
                            
                            StateSetter.StaggerChance(false, Stagger, Stats);

                            //Stagger Will Cause The Animation Speed For An Attack To Come Out Slower For 1.5 seconds.
                        }
                    }
                }
            }
        }
    }
    public void Thunderbolt()
    {
        if (isThunderAffected)
        {
            //Stuns On Impact
            DebuffEffect Thunder = new DebuffEffect(gameObject, "Thunderbolt", DebuffTriggerTypes.Passive, DebuffTypes.ElementalEffect, ElementalAffix.Thunder, lightningStunDuration, lightningStunDuration);
            DebuffEffect Stun = new DebuffEffect(gameObject, "Thunderbolt Stun", DebuffTriggerTypes.Passive, DebuffTypes.Stun, lightningStunDuration, lightningStunDuration);

            StateSetter.SendDebuff(Thunder, GetComponent<EntityState>(), Stats);
            StateSetter.SendDebuff(Stun, GetComponent<EntityState>(), Stats);

            //Unlike All Other Elemental Effects Lightning Can Crit
            if (GetComponent<EntityHealth>().Identity.Equals(EntityTarget.Player))
            {
                DamageProperty ThunderBolt = new DamageProperty(gameObject, EntityTarget.Player, "", 0, Stats.PlayerSheet.PhysicalAtk + Stats.PlayerSheet.MagicalAtk, Stats.PlayerSheet.StatusChance, 1f, DamageClassification.Magical, DamageTypes.ElementalDamage, ElementalAffix.Thunder, Stats.PlayerSheet.MagicalCriticalStrikeChance + Stats.PlayerSheet.PhysicalCriticalStrikeChance, Stats.PlayerSheet.MagicalCriticalStrikeDamage + Stats.PlayerSheet.PhysicalCriticalStrikeDamage);
                CallHealthEvent.SendDamageEvent(ThunderBolt, Health);

                //Lighting Blast
                Collider[] LightningAOE = Physics.OverlapSphere(transform.position, lightningRadius, PlayerElement);

                foreach (Collider Other in LightningAOE)
                {
                    if (Other.gameObject.GetComponent<Rigidbody>() != null && Physics.Linecast(transform.position, Other.transform.position, WallAOE))
                    {
                        DamageProperty ThunderBoltAOE = new DamageProperty(gameObject, EntityTarget.Player, "", 0, Stats.PlayerSheet.PhysicalAtk + Stats.PlayerSheet.MagicalAtk, Stats.PlayerSheet.StatusChance, 0.5f, DamageClassification.Magical, DamageTypes.ElementalDamage, ElementalAffix.Thunder, Stats.PlayerSheet.MagicalCriticalStrikeChance + Stats.PlayerSheet.PhysicalCriticalStrikeChance, (Stats.PlayerSheet.MagicalCriticalStrikeDamage + Stats.PlayerSheet.PhysicalCriticalStrikeDamage));
                        CallHealthEvent.SendDamageEvent(ThunderBoltAOE, Health);
                    }
                }
            }

            //Unlike All Other Elemental Effects Lightning Can Crit
            if (GetComponent<EntityHealth>().Identity.Equals(EntityTarget.Enemy))
            {
                DamageProperty ThunderBolt = new DamageProperty(gameObject, EntityTarget.Enemy, "", 0, Stats.EnemySheet.PhysicalDamage + Stats.EnemySheet.MagicalDamage, Stats.EnemySheet.StatusChance, 1f, DamageClassification.Magical, DamageTypes.ElementalDamage, ElementalAffix.Thunder, Stats.EnemySheet.CriticalChance, Stats.EnemySheet.CriticalDamage);
                CallHealthEvent.SendDamageEvent(ThunderBolt, Health);

                //Lighting Blast
                Collider[] LightningAOE = Physics.OverlapSphere(transform.position, lightningRadius, PlayerElement);

                foreach (Collider Other in LightningAOE)
                {
                    if (Other.gameObject.GetComponent<Rigidbody>() != null && Physics.Linecast(transform.position, Other.transform.position, WallAOE))
                    {
                        DamageProperty ThunderBoltAOE = new DamageProperty(gameObject, EntityTarget.Enemy, "", 0, Stats.EnemySheet.PhysicalDamage + Stats.EnemySheet.MagicalDamage, Stats.EnemySheet.StatusChance, 0.5f, DamageClassification.Magical, DamageTypes.ElementalDamage, ElementalAffix.Thunder, Stats.EnemySheet.CriticalChance, Stats.EnemySheet.CriticalDamage);
                        CallHealthEvent.SendDamageEvent(ThunderBoltAOE, Health);
                    }
                }
            }

            isThunderAffected = false;
        }
    }

    public void Freezing()
    {
        if (isFrostAffected)
        {
            //Apply Element
            //Apply Slow
            //Apply Attack Speed
            //Apply Damage Amp To Target
            
            DebuffEffect Frost = new DebuffEffect(gameObject, "Chilled", DebuffTriggerTypes.Passive, DebuffTypes.ElementalEffect, ElementalAffix.Frost, frostDuration, frostDuration);
            DebuffEffect MoveSlow = new DebuffEffect(gameObject, "Chilled Movespeed Slow", DebuffTriggerTypes.Passive, DebuffTypes.Slowed, ElementalAffix.Elementless, frostDuration, frostDuration, frostSlowPercentage);
            DebuffEffect AttackSpeedSlow = new DebuffEffect(gameObject, "Chilled Attack Speed Slow", DebuffTriggerTypes.Passive, DebuffTypes.AttackSpeedReduction, ElementalAffix.Elementless, frostDuration, frostDuration, frostAttackSpeedSlowPercentage);
            DebuffEffect DamageAmp = new DebuffEffect(gameObject, "Chilled Damage Amplification", DebuffTriggerTypes.Passive, DebuffTypes.DamageAmplification, ElementalAffix.Elementless, frostDuration, frostDuration, frostBonusDamagePercentage);

            StateSetter.SendDebuff(Frost, GetComponent<EntityState>(), Stats);
            StateSetter.SendDebuff(MoveSlow, GetComponent<EntityState>(), Stats);
            StateSetter.SendDebuff(AttackSpeedSlow, GetComponent<EntityState>(), Stats);
            StateSetter.SendDebuff(DamageAmp, GetComponent<EntityState>(), Stats);

            isFrostAffected = false;
        }
    }
    
    void Drenched()
    {
        //Amplify The Next Element With New Effects
        if (isWaterAffected)
        {
            //Double All Effect Strength And Durations Which is Built Into Each Specific Effect
            DebuffEffect Wet = new DebuffEffect(gameObject, "Drenched", DebuffTriggerTypes.Passive, DebuffTypes.ElementalEffect, ElementalAffix.Water, waterDuration, waterDuration);

            StateSetter.SendDebuff(Wet, GetComponent<EntityState>(), Stats);

            isWaterAffected = false;
        }
    }
    #endregion
    void CountDown()
    {
        //Buffs
        foreach (BuffEffect Buff in Buffs.BuffEffects)
        {
            //Count down time
            Buff.CurrentDuration -= Time.deltaTime;

            //Remove Buff
            if (Buff.CurrentDuration <= 0)
            {
                StateSetter.RemoveBuff(Buff, GetComponent<EntityState>(), GetComponent<EntityStats>(), Buffs.BuffEffects.IndexOf(Buff));
            }
        }

        //Debuffs
        foreach (DebuffEffect Debuff in Debuffs.DebuffEffects)
        {
            //Count down time
            Debuff.CurrentDuration -= Time.deltaTime;

            //Remove Debuff
            if (Debuff.CurrentDuration <= 0)
            {
                StateSetter.RemoveDebuff(Debuff, GetComponent<EntityState>(), GetComponent<EntityStats>());
            }
        }
    }
    void TickHandler()
    {
        if (TickHolders.Count > 0)
        {
            foreach (TickHandler TickTimer in TickHolders)
            {
                if (Time.time > TickTimer.tickCounter)
                {
                    TickTimer.tickCounter = Time.time + TickTimer.tickRate;
                    TickTimer.pingTick = true;
                }
            }
        }
    }
}