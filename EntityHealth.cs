using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HealthClasses;
using StateClasses;
using StatClasses;
using DamageClass;

[RequireComponent(typeof(EntityStats))]
[RequireComponent(typeof(EntityState))]

public class EntityHealth : MonoBehaviour
{
    //Determines The Type Of Entity The Object Is
    public EntityTarget Identity;
    //List Of All Health Bars Of The Entity
    public List<HealthProperty> Health;

    //Booleans Determine Health States 
    [Header("Health Flags")]
    public bool isInvunerable;
    public bool isOutOfCombat;
    public bool isDead;
    public bool isGuarding;
    public bool isArmored;
    public bool isShielded;
    public HealthTypes ShieldType;

    //Ints That Contains each Health Index
    [Header("Health Indexes")]
    public int BaseHealthIndex = -1;
    public int ArmorHealthIndex = -1;
    public int GuardHealthIndex = -1;
    public int DreamHealthIndex = -1;
    public int NightmareHealthIndex = -1;

    //Special Health Parameters
    [Header("Health Properties")]
    public float GuardRegenTimer;
    public float GuardRegenDelay = 10;
    public float GuardRegenerationThreshold = 5;
    public float ArmorRegenTimer;
    public float ArmorRegenerationThreshold = 5;

    //Required Components
    public EntityStats Stats;
    public EntityState State;
    public Flasks UseFlask;

    private void Awake()
    {
        Stats = GetComponent<EntityStats>();
        State = GetComponent<EntityState>();

        //Get Indexes for each found health bar
        IndexFlags();
    }


    private void Start()
    {
        //Set Current Health Bars 
        SetHealth();
    }

    // Update is called once per frame
    void Update()
    {
        HealthFlags();
        ClampHealth();
        RegainGuard();
        ArmorOutOfCombat();
        Die();
    }

    private GameObject chosenSpawnPoint;
    private void Die()
    {
        if(Health[BaseHealthIndex].HealthValue <= 0 && Identity.Equals(EntityTarget.Player))
        {
            isDead = true;

            //Get A List Of All Game Objects with the tag "Respawn"
            GameObject[] SpawnPoints;

            SpawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

            float closestdistance = Mathf.Infinity;

            foreach(GameObject respawn in SpawnPoints)
            {
                float dist = Vector3.Distance(gameObject.transform.position, respawn.transform.position);

                if (dist < closestdistance)
                {
                    closestdistance = dist;
                    chosenSpawnPoint = respawn;
                }
            }

            //Call SpawnPoint Function
            chosenSpawnPoint.GetComponent<PlayerSpawner>().SpawnPlayer();

            Destroy(gameObject);
            print("Player Died");
        }
    }

    public void Execute()
    {
        Health[BaseHealthIndex].HealthValue = 0;
    }

    public void IndexFlags()
    {
        //Loop To Grab Each Found Health Bar
        foreach(HealthProperty bar in Health)
        {
            //Health Bar Indexers For Each Type
            //Get The Indexes Once At Spawn Of The Object And Store Them Until The Object is Deleted for each health type
            if(Health[Health.IndexOf(bar)].HealthType.Equals(HealthTypes.Base))
            {
                BaseHealthIndex = Health.IndexOf(Health[Health.IndexOf(bar)]);
            }

            if (Health[Health.IndexOf(bar)].HealthType.Equals(HealthTypes.Armor))
            {
                ArmorHealthIndex = Health.IndexOf(Health[Health.IndexOf(bar)]);
            }

            if (Health[Health.IndexOf(bar)].HealthType.Equals(HealthTypes.Guard))
            {
                GuardHealthIndex = Health.IndexOf(Health[Health.IndexOf(bar)]);
            }

            if (Health[Health.IndexOf(bar)].HealthType.Equals(HealthTypes.DreamMagic))
            {
                DreamHealthIndex = Health.IndexOf(Health[Health.IndexOf(bar)]);
            }

            if (Health[Health.IndexOf(bar)].HealthType.Equals(HealthTypes.NightmareMagic))
            {
                NightmareHealthIndex = Health.IndexOf(Health[Health.IndexOf(bar)]);
            }
        }
    }

    private void HealthFlags()
    {
        //Armor Flags
        if (Health[ArmorHealthIndex].HealthValue > 0)
        {
            isArmored = true;
        }
        else
        {
            isArmored = false;
        }

        //Shield Flags
        if (Health[DreamHealthIndex].HealthValue > 0 || Health[NightmareHealthIndex].HealthValue > 0)
        {
            isShielded = true;
        }
        if (Health[DreamHealthIndex].HealthValue <= 0 && Health[NightmareHealthIndex].HealthValue <= 0)
        {
            isShielded = false;
        }

        //Base Flags
        if (Health[BaseHealthIndex].HealthValue < 0)
        {
            isDead = true;
        }
        else
        {
            isDead = false;
        }

        //Guard Flag Is Turned On Depending On Player Input Or AI Actually deciding to guard
    }

    public void SetHealth()
    {
        //Set Health Values To Maximum Value For Player
        if (Identity.Equals(EntityTarget.Player))
        {
            Health[BaseHealthIndex].HealthValue = Stats.PlayerSheet.Health;
            Health[GuardHealthIndex].HealthValue = Stats.PlayerSheet.GuardIntegrity;
            Health[BaseHealthIndex].HealthCap = Stats.PlayerSheet.Health;
            Health[GuardHealthIndex].HealthCap = Stats.PlayerSheet.GuardIntegrity;
        }

        //Set Health Values To Maximum Value For Enemy
        if (Identity.Equals(EntityTarget.Enemy))
        {
            Health[BaseHealthIndex].HealthValue = Stats.EnemySheet.HealthMax;
            Health[GuardHealthIndex].HealthValue = Stats.EnemySheet.GuardMax;
            Health[ArmorHealthIndex].HealthValue = Stats.EnemySheet.ArmorMax;
            Health[NightmareHealthIndex].HealthValue = Stats.EnemySheet.ShieldMax;
            Health[DreamHealthIndex].HealthValue = Stats.EnemySheet.ShieldMax;

            Health[BaseHealthIndex].HealthCap = Stats.EnemySheet.HealthMax;
            Health[GuardHealthIndex].HealthCap = Stats.EnemySheet.GuardMax;
            Health[ArmorHealthIndex].HealthCap = Stats.EnemySheet.ArmorMax;
            Health[NightmareHealthIndex].HealthCap = Stats.EnemySheet.ShieldMax;
            Health[DreamHealthIndex].HealthCap = Stats.EnemySheet.ShieldMax;
        }
    }

    public int GrabHealthBar()
    {
        //Grabs Health Of An Entity based on priority in this specific order
        //Guard Health Does Not Matter Here
        //Red Health Prio 1
        //Armor Prio 2
        //Shield is Prio 3 or 2

        if (Health[BaseHealthIndex].HealthValue > -1)
        {
            return BaseHealthIndex;
        }

        if (Health[ArmorHealthIndex].HealthValue > -1)
        {
            return ArmorHealthIndex;
        }

        if (Health[DreamHealthIndex].HealthValue > -1)
        {
            return DreamHealthIndex;
        }

        if (Health[NightmareHealthIndex].HealthValue > -1)
        {
            return NightmareHealthIndex;
        }

        return -1;
    }

    private void ClampHealth()
    {
        //Clamp Health Each Frame To Keep The Health Bar Less Than or Equal to Maximum and no lower then 0.
        if (Identity.Equals(EntityTarget.Player))
        {
            Health[BaseHealthIndex].HealthValue = Mathf.Clamp(Health[BaseHealthIndex].HealthValue, 0, Health[BaseHealthIndex].HealthCap);
            Health[GuardHealthIndex].HealthValue = Mathf.Clamp(Health[GuardHealthIndex].HealthValue, 0, Health[GuardHealthIndex].HealthCap);
            Health[DreamHealthIndex].HealthValue = Mathf.Clamp(Health[DreamHealthIndex].HealthValue, 0, Health[DreamHealthIndex].HealthCap);
            Health[NightmareHealthIndex].HealthValue = Mathf.Clamp(Health[NightmareHealthIndex].HealthValue, 0, Health[NightmareHealthIndex].HealthCap);
        }

        if (Identity.Equals(EntityTarget.Enemy))
        {
            Health[BaseHealthIndex].HealthValue = Mathf.Clamp(Health[BaseHealthIndex].HealthValue, 0, Health[BaseHealthIndex].HealthCap);
            Health[ArmorHealthIndex].HealthValue = Mathf.Clamp(Health[ArmorHealthIndex].HealthValue, 0, Health[ArmorHealthIndex].HealthCap);
            Health[GuardHealthIndex].HealthValue = Mathf.Clamp(Health[GuardHealthIndex].HealthValue, 0, Health[GuardHealthIndex].HealthCap);
            Health[DreamHealthIndex].HealthValue = Mathf.Clamp(Health[DreamHealthIndex].HealthValue, 0, Health[DreamHealthIndex].HealthCap);
            Health[NightmareHealthIndex].HealthValue = Mathf.Clamp(Health[NightmareHealthIndex].HealthValue, 0, Health[NightmareHealthIndex].HealthCap);
        }
    }

    void RegainGuard()
    {
        //Creating Healing Class Instance To Health Player Guard
        HealingProperty NatrualGuardPlayerRegeneration = new HealingProperty(Identity, HealthTypes.Guard, 1 + Stats.PlayerSheet.GuardRegeneration);

        //Heal health bar every tick when your guard isn't full and not guard broken
        if (Health[GuardHealthIndex].HealthValue < Stats.PlayerSheet.GuardIntegrity && Time.time > GuardRegenTimer && Identity.Equals(EntityTarget.Player) && !State.isGuardBroken)
        {
            GuardRegenTimer = Time.time + GuardRegenDelay;
            CallHealthEvent.SendHealingEvent(NatrualGuardPlayerRegeneration, GetComponent<EntityHealth>());
        }

        //Creating Healing Class Instance To Health Enemy Guard
        HealingProperty NatrualGuardEnemyRegeneration = new HealingProperty(Identity, HealthTypes.Guard, 1 + Stats.EnemySheet.GuardRegen);

        //Heal health bar every tick when your guard isn't full and not guard broken
        if (Health[GuardHealthIndex].HealthValue < Stats.EnemySheet.GuardMax && Time.time > GuardRegenTimer && Identity.Equals(EntityTarget.Enemy) && !State.isGuardBroken)
        {
            GuardRegenTimer = Time.time + (GuardRegenDelay - Stats.EnemySheet.GuardRecoverDelay);
            CallHealthEvent.SendHealingEvent(NatrualGuardEnemyRegeneration, GetComponent<EntityHealth>());
        }
    }

    void ArmorOutOfCombat()
    {
        //Healing Instance For Enemy
        HealingProperty NatrualArmorRegeneration = new HealingProperty(Identity, HealthTypes.Armor, Stats.EnemySheet.ArmorMax);

        //Heal Enemy Instantly If Out OF Combat To Their Armor Only
        if (isOutOfCombat && Identity.Equals(EntityTarget.Enemy) && Health[ArmorHealthIndex].HealthValue < Stats.EnemySheet.ArmorMax)
        {
            CallHealthEvent.SendHealingEvent(NatrualArmorRegeneration, GetComponent<EntityHealth>());
        }
    }
}
