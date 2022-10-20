using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MemorySystem
{
    [System.Serializable]
    public class Memory
    {
        public string SkillName;
        public float SkillDamageScaling;
        public int TensionCost;
        public GameObject SkillObject;

        [Header("Upgrades")]
        public int ThreadCountLevel;
        public int ThreadCountLevelCap = 100;
    }
}
