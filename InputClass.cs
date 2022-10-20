using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyActions{WalkForward,WalkBackwards,WalkRight,WalkLeft,Dodge,Jump,SkillA,SkillB,SkillC,SkillD,Crouch,UseHealthPotion,UseTensionPotion,TargetEnemy,Grapple,HyperThreading, LightAttack, HeavyAttack, SwitchArrowType, AimDownSights, SwitchWeapon, Guard, Interact, EscapeMenu, UI_Confirm, UI_Cancel, UI_Equip,ResetCamera}

namespace InputKeys
{
    [System.Serializable]
    public class InputKey
    {
        public string ActionName;
        public KeyActions BoundAction;
        public KeyCode key;
    }

    [System.Serializable]
    public class InputIndex
    {
        public int WalkForwardIndex = -1;
        public int WalkBackwardsIndex = -1;
        public int WalkLeftIndex = -1;
        public int WalkRightIndex = -1;
        public int DodgeIndex = -1;
        public int JumpIndex = -1;
        public int SkillAIndex = -1;
        public int SkillBIndex = -1;
        public int SkillCIndex = -1;
        public int SkillDIndex = -1;
        public int CrouchIndex = -1;
        public int UseHealthPotionIndex = -1;
        public int UseTensionPotionIndex = -1;
        public int TargetEnemyIndex = -1;
        public int GrappleIndex = -1;
        public int HyperThreadIndex = -1;
        public int LightAttackIndex = -1;
        public int HeavyAttackIndex = -1;
        public int AimDownSightsIndex = -1;
        public int SwitchArrowIndex = -1;
        public int SwitchWeaponIndex = -1;
        public int GuardIndex = -1;
        public int InteractIndex = -1;
        public int EscapeMenuIndex = -1;
        public int UI_ConfirmIndex = -1;
        public int UI_CancelIndex = -1;
        public int UI_EquipIndex = -1;
        public int ResetCameraIndex = -1;
    }

    [System.Serializable]
    public class InputScheme
    {
        public InputIndex KeyIndex;
        public List<InputKey> DefaultInputs = new List<InputKey>();
        public List<InputKey> Inputs = new List<InputKey>();

        [Header("Input Vectors")]
        public Vector3 MovementVector;
        public float Elevation;
        public float Horizontal;
        public float Vertical;
        public float acceleration;
        public float deacceleration;
    }
}