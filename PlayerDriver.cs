using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageClass;
using StateClasses;
using InventoryClass;

public enum MovementType {Strafe, FreeMove, FPSMove}

//Check Souls Astray for best settings and then try to see if that fixes anything after reordering and cleaning up the script. If not do needed fixes. After that build UI!
public class PlayerDriver : MonoBehaviour
{
	[System.Serializable]
	public class PlayerPhysics
	{
		[Header("Player Locks")]
		public bool movementLock;
		public bool airMovementLock;
		public bool turnLock;

		[Header("Vectors")]
		public Vector3 velocity;//physics vector
		public Vector3 move;//wasd vector
		public Vector3 vel;//final vector
		public Vector3 dir;

		[Header("Player Scaling")]
		public float slopeLimit = 50;
		public float modelHeight = 1f;
		public float rayHeight = 1f;
		public float capsuleOriginPoint = -0.12f;
		public float capsuleOriginHeight = 2f;
		public Vector3 collisionOriginStart = new Vector3(0, 0.91f, 0);
		public Vector3 collisionOriginEnd = new Vector3(0, -1.14f, 0);
		public float collisionRadius = 0.5f;
		public float rayPointDistance = 1.0f;
		public float floorRayDistance = 2f;

		[Header("Gravity Options")]
		public float intialGravity = 80f;//gravity amount affecting all movement
		public float gravityMultiplier;
		public float gravityMultiplierMax;
		public float gravityIncrease;
		public bool groundDetectionToggle = true;

		[Header("Simple Movement Motor Functions")]
		public float turnSpeed = 15f; //turn smoothing
		public float directionLookSpeed = 5f; //speed the player turns the physical model to look at key direcitons
		public MovementType MoveType;
		public float baseMovementSpeed = 10f; //walking speed
		public float currentMovementSpeed;
		public Vector3 physicalSpeed;
		public float maxRigidBodySpeed = 100;
		public float airforce = 0.5f;

		[Header("Physics Drivers")]
		public Rigidbody rb;// your rigidbody
		public LayerMask excludePlayer;// masks that should not be collided with and detected as ground
		public Collider CapsuleCol;//Body Capsule collider

		[Header("Run")]
		public bool isRunning;
		public float runSpeed = 15f;

		[Header("Dodging")]
		public bool isSliding;
		public float MaxDodgeDuration = 0.5f;
		public float DodgeSpeed = 20f;

		[Header("Crouching")]
		public bool isCrouching = false;
		public LayerMask crouchLayers;
		public float crouchDistCheck = 3f;
		public float crouchMovementSpeed = 4f;

		[Header("Jump")]
		public int numberOfTotalJumpsMax = 2;
		public float currJumpTime = 0;
		public float setJumpTime = 1;
		public float jumpHeight;
		public float jumpDeadZoneTime = 0.3f;
		public float jumpforce = 6f;
		public float jumpdoubleforce = 3f;
		public float incrementJumpFallSpeed = 0.1f;

		[Header("Wall Jump Stats")]
		public float NotchKickHeight = 5f;
		public float NotchKickDistance = 8f;
		public float WallSlideSpeed = 2;
		public float WallSlideSpeedAcceleration = 2;
		public Vector3 Offset = new Vector3(0,0,-0.5f);
		public LayerMask wallKickLayer;

		[Header("Cardinal Cowl")]
		public int currentWingFlaps = 5;
		public int maximumWingFlaps = 5;
		public float wingFlapHeight = 5;
		public float wingForce = 25f;
		public float wingGravityMovementSpeed = 3f;
		public float wingGravitySlowFactor = 0.02f;
		public float wingGravitySlowCurrDuration = 1.5f;
		public float wingGravitySlowMaxDuration = 1.5f;
		public float wingDeadZoneTime;
		public float activeWingDeadZoneTime;

		[Header("Hovering")]
		public float maxSpoolStamina = 5;
		public float spoolAcceleration = 1.5f;
		public float maxSpinningSpoolSpeed = 3f;
		public float spinningSpoolSpeed = 1.5f;
		public float spinningSpoolForce = 1.5f;

		[Header("Swimming")]
		public bool isSwimming = false;
		public bool isSwimSprinting = false;
		public bool isUnderWater = false;
		public float maximumSwimForce = 3;
		public float maximumSprintSwimForce = 6;
		public float swimmingSprintSpeed = 5f;
		public float swimmingSpeed = 3f;
		public float swimmingSpeedForce = 25f;

		[Header("Waking Descent")]
		public float wakingDescentAccelerationMaximum;
		public float wakingDescentAccelerationRate;
		public float wakingDescentMovementSpeed;
		public float wakingDescentPushBackForce;
		public float wakingDescentDistCheck;
		public float wakingDescentdamageRadius;
		public LayerMask wakingDescentMask;

		[Header("Grappling")]
		public bool PreGrappling = false;
		public bool applyGrappleJumpForce = false;
		public bool linecastGrappleBlock = false;
		public bool onGrappleScreen = false;
		public Transform GrappleSwingObject;
		public Transform GrapplePointObject;
		public Transform lineOrigin;
		public LineRenderer grappleLine;
		public float maxGrappleSwingAngle = 75f;
		public float grapplePointDist = 50f;
		public float grappleSwingDist = 50f;
		public LayerMask grappleSwingLayer;
		public LayerMask grappleHoldLayer;
		public LayerMask grappleExclusionList;
		public float maxSwingSpeed = 100f;
		public float grappleReelSpeed = 10f;
		public float grappleLeapTime = 5f;
		public float grappleLeapHeight = 10f;
		public float grappleLeapForce = 20f;
		public float swingForce = 150f;
		public float grappleSpeed = 2f;
		public float grapplePointOffset = 2f;

		[Header("Player States")]
		public bool ApplyGravity = true;//is gravity on or off
		public bool DoubleJump = false;
		public bool Grounded = false;//touching the ground or not touching the ground
		public bool PhysicallyGrounded = false;
		public bool hovering = false;
		public bool dashing = false;
		public bool canDash = true;
		public bool descending = false;
		public bool readyToWallKick = false;
		public bool isWallKicking = false;
		public bool pullingToGrapplePoint;
		public bool pullingToSwingPoint;
		public bool swingMode;
		public bool holdMode;
		public bool inSwingRange = false;
		public bool inPointRange = false;
		public bool wingFlapToggle;
		public bool slowWingGravityMode;
		public bool isWallHanging = false;
	}

	[System.Serializable]
	public class CooldownEntity
	{
		public bool ready = true;
		public string AbillityName;
		public float currentCooldown;
		public float maximumCooldown;
	}

	
	public PlayerPhysics physicsProperties;
	public List<CooldownEntity> Cooldowns;
	public PlayerCamera MyCamera;
	public BuoyancyPhysics BuoyancyForce;

	private GroundChecker gChecker;
	private PlayerSettings p_Settings;
	private EntityStats Stats;
	private EntityState State;
	private Quaternion baseRotation;
	private Quaternion targetRotation;
	private float currSpoolStamina;
	public int numberOfTotalJumpsAvaliable;
	private float currDodgeDuration;
	private float wakingDescentAcceleration;
	

	void Awake()
    {
		InitializePlayer();
	}

	// Start is called before the first frame update
	void Start()
	{
		State = GetComponent<EntityState>();
		Stats = GetComponent<EntityStats>();
		p_Settings = GetComponent<PlayerSettings>();
		BuoyancyForce = GetComponent<BuoyancyPhysics>();
		gChecker = GetComponent<GroundChecker>();
		baseRotation = transform.rotation;
		activeDeadZoneTimer = physicsProperties.jumpDeadZoneTime;
	}

	void InitializePlayer()
    {
		//Don't Destory These On Load Of Other Scenes
		DontDestroyOnLoad(gameObject);
		DontDestroyOnLoad(MyCamera.Rotater);
		DontDestroyOnLoad(MyCamera.GeneralLook);
		DontDestroyOnLoad(MyCamera.GameCamera);

		//Grab Spawners
		foreach(PlayerSpawner spawner in FindObjectsOfType<PlayerSpawner>())
        {
			spawner.currentPlayer = gameObject;
		}
	}

	// Update is called once per frame
	void Update()
	{
		DirectionalTurning();
		CardinalCowl();
		CooldownManager();
		HookReeling();
	}

	RaycastHit testhit;

	private void FixedUpdate()
	{
		StartCoroutine(GetPhysicalSpeed());
		CompleteMovement();
		swingForceAndSpeedLimiter();;
		DodgeLogic();
		SlidingLogic();
		NotchKick();
		WakingDescent();
		Dodge();
		Sliding();
		RotaryHook();
		RotaryHookDisengage();		
	}

	private void LateUpdate()
	{
		GrappleLineRender();
	}

    #region Cooldowns
    public void CooldownManager()
	{
		foreach (CooldownEntity cooldown in Cooldowns)
		{
			cooldown.currentCooldown = Mathf.Clamp(cooldown.currentCooldown, 0, 999);

			if (!cooldown.ready)
			{
				cooldown.currentCooldown -= Time.deltaTime;
			}

			if (cooldown.currentCooldown <= 0 && !cooldown.ready)
			{
				cooldown.currentCooldown = 0;
				cooldown.ready = true;
			}
		}
	}
	public bool CooldownState(string cooldownName)
    {
		foreach(CooldownEntity Cooldown in Cooldowns)
        {
			if(Cooldown.AbillityName == cooldownName)
            {
				//print("Coodlown State of " + cooldownName + " returned Successfully.");
				return Cooldown.ready;
            }
        }

		Debug.LogError("ERROR: Cooldown name " + "( " + cooldownName + " )" + " was not found. The state of " + cooldownName +  " will be returned true at all times.");
		return true;
    }

	public void IntiateCooldown(string cooldownName)
	{
		foreach (CooldownEntity Cooldown in Cooldowns)
		{
			if (Cooldown.AbillityName == cooldownName)
			{
				Cooldown.ready = false;
				Cooldown.currentCooldown = Cooldown.maximumCooldown;
				print("Started " + Cooldown.AbillityName + " Cooldown.");
				return;
			}
		}

		Debug.LogError("ERROR: Cooldown name " + "( " + cooldownName + " )" + " was not found. No cooldown has begun. Nothing happened.");
		return;
	}
	public void ReduceCooldown(string cooldownName, int amount)
	{
		foreach (CooldownEntity Cooldown in Cooldowns)
		{
			if (Cooldown.AbillityName == cooldownName)
			{
				Cooldown.ready = false;
				Cooldown.currentCooldown -= amount;
				print("Reduced " + cooldownName + " by " + amount + " seconds.");
				return;
			}
		}

		Debug.LogError("ERROR: Cooldown name " + "( " + cooldownName + " )" + "was not found. Cooldown was not reduced.");
		return;
	}

	public void ResetCooldownName(string cooldownName)
	{
		//Check if any cooldowns equal a certain name all at once
		foreach(CooldownEntity Cooldown in Cooldowns)
        {
			//if the name is found 
			if(Cooldown.AbillityName == cooldownName)
            {
				Cooldown.ready = true;
				Cooldown.currentCooldown = Cooldown.maximumCooldown;
				print("Cooldown " + cooldownName + " was successfully reset.");
				return;
            }
        }

		Debug.LogError("ERROR: Cooldown name " + "( " + cooldownName + " )" + " was not found. The reset was unsuccessfull.");
		return;
	}
	public void ResetAllCooldowns()
	{
		foreach (CooldownEntity cooldown in Cooldowns)
		{
			cooldown.ready = true;
			cooldown.currentCooldown = 0;
		}

		if(Cooldowns.Count > 0)
        {
			print("All Cooldowns Reset. " + Cooldowns.Count + "cooldowns were reset.");
		}
		else
        {
			Debug.LogWarning("There are no cooldowns to reset.");
		}
	}
    #endregion

    void CompleteMovement()
	{
		//This Order of Gravity (); Move (); Jump (); FinalMove (); GroundChecking (); CollisionCheck (); cannot be changed or this will not work at all.
		Gravity();
		Move();
		Swim();
		Crawling();
		Running();
		Jump();
		GroundChecking();
		CollisionCheck();
		MoveCorrection();
	}
	void Swim()
    {
		if (!physicsProperties.movementLock && physicsProperties.isSwimming)
        {
			if (physicsProperties.Grounded)
            {
				BuoyancyForce.EndDive();
				physicsProperties.rb.velocity = Vector3.zero;
            }

			//Key Codes
			KeyCode SprintKey = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.DodgeIndex].key;
			KeyCode AscendKey = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.JumpIndex].key;
			KeyCode DiveKey = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.CrouchIndex].key;
			KeyCode WKey = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.WalkForwardIndex].key;
			KeyCode AKey = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.WalkLeftIndex].key;
			KeyCode SKey = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.WalkRightIndex].key;
			KeyCode DKey = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.WalkBackwardsIndex].key;

			//Detect Diving
			if (Input.GetKey(DiveKey))
			{
				physicsProperties.isUnderWater = true;
			}
			//Disable Underwater Detection
			if (!Input.GetKey(DiveKey) && BuoyancyForce.waterSurfaced)
			{
				physicsProperties.isUnderWater = false;
			}

			if(!BuoyancyForce.waterSurfaced)
            {
				physicsProperties.isUnderWater = true;
			}

			//Disable Previous Force Base On Key Down For New Key Direction
			if (Input.GetKeyUp(AscendKey) || Input.GetKeyUp(DiveKey) || Input.GetKeyDown(AscendKey) || Input.GetKeyDown(DiveKey) || Input.GetKeyDown(WKey) || Input.GetKeyDown(AKey) || Input.GetKeyDown(SKey) || Input.GetKeyDown(DKey))
			{
				physicsProperties.rb.velocity = Vector3.zero;
			}

			//Prevent Y Force but allow X and Z force
			if (physicsProperties.isUnderWater && !BuoyancyForce.isSinking && !BuoyancyForce.waterSurfaced)
            {
				physicsProperties.rb.velocity = new Vector3(physicsProperties.rb.velocity.x, 0, physicsProperties.rb.velocity.z);
			}

			//Input that combines X or Z to be only on the Z vector for movement forward for force application
			float input = Mathf.Clamp01(InputManager.Instance.PlayerInput.MovementVector.x + InputManager.Instance.PlayerInput.MovementVector.z);

			//Direction Vector for free moving force
			Vector3 Dir = physicsProperties.velocity;

			//Sprint
			if (Input.GetKey(SprintKey) && !Input.GetKeyDown(AscendKey) && !Input.GetKeyDown(DiveKey))
            {
				physicsProperties.isSwimSprinting = true;

				if (physicsProperties.rb.velocity.magnitude > physicsProperties.maximumSprintSwimForce)
				{
					physicsProperties.rb.velocity = (physicsProperties.rb.velocity.normalized * (physicsProperties.maximumSprintSwimForce));
				}

				//Force Mode Application that depends on the current movement type
				if (physicsProperties.MoveType.Equals(MovementType.Strafe))
				{
					//Apply Forces
					physicsProperties.rb.AddRelativeForce(physicsProperties.move * CalculatePlayerMovement(physicsProperties.swimmingSpeedForce), ForceMode.Acceleration);
					transform.position += Vector3.up * physicsProperties.move.y * CalculatePlayerMovement(physicsProperties.swimmingSpeed + 2) * Time.deltaTime;
				}

				if (physicsProperties.MoveType.Equals(MovementType.FPSMove))
				{
					//Apply Forces
					physicsProperties.rb.AddRelativeForce(physicsProperties.move * CalculatePlayerMovement(physicsProperties.swimmingSpeedForce), ForceMode.Acceleration);
				}

				if (physicsProperties.MoveType.Equals(MovementType.FreeMove))
				{
					//Apply Forces
					physicsProperties.rb.AddRelativeForce(Dir * CalculatePlayerMovement(physicsProperties.swimmingSpeedForce), ForceMode.Acceleration);
				}
			}
			else
            {
				physicsProperties.isSwimSprinting = false;

				//Force Mode Application that depends on the current movement type
				if (physicsProperties.rb.velocity.magnitude > physicsProperties.maximumSwimForce)
				{
					physicsProperties.rb.velocity = (physicsProperties.rb.velocity.normalized * (physicsProperties.maximumSwimForce));
				}

				if(physicsProperties.MoveType.Equals(MovementType.Strafe))
                {
					//Apply Forces
					physicsProperties.rb.AddRelativeForce(physicsProperties.move * CalculatePlayerMovement(physicsProperties.swimmingSpeedForce), ForceMode.Acceleration);
					transform.position += Vector3.up * physicsProperties.move.y * CalculatePlayerMovement(physicsProperties.swimmingSpeed + 2) * Time.deltaTime;
				}

				if (physicsProperties.MoveType.Equals(MovementType.FPSMove))
				{
					//Apply Forces
					physicsProperties.rb.AddRelativeForce(physicsProperties.move * CalculatePlayerMovement(physicsProperties.swimmingSpeedForce), ForceMode.Acceleration);
				}

				if (physicsProperties.MoveType.Equals(MovementType.FreeMove))
				{
					//Apply Forces
					physicsProperties.rb.AddRelativeForce(Dir * CalculatePlayerMovement(physicsProperties.swimmingSpeedForce), ForceMode.Acceleration);
				}
			}
		}
		else
        {
			//Reset States After Leaving the water once
			if(!physicsProperties.isSwimming && physicsProperties.isSwimSprinting)
            {
				physicsProperties.isSwimSprinting = false;
			}

			if(BuoyancyForce.isSubmerged)
            {
				BuoyancyForce.isSubmerged = false;
			}
		}
    }
	IEnumerator GetPhysicalSpeed()
    {
		Vector3 lastPosOfPhysicalSpeed = transform.position;
		yield return new WaitForEndOfFrame();
		physicsProperties.physicalSpeed = ((lastPosOfPhysicalSpeed - transform.position) / Time.deltaTime);
    }
	public bool canJump = true;
	private float activeDeadZoneTimer;
	private Transform GrappleObj;
	private bool disableNotchKick = false;
	private void Jump()
	{
		KeyCode key = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.JumpIndex].key;

		if (Stash.Instance.unlockList.unlockHyphinFeather)
        {
			physicsProperties.numberOfTotalJumpsMax = 2;
        }
		else
        {
			physicsProperties.numberOfTotalJumpsMax = 1;
		}

		if (!physicsProperties.movementLock)
		{
			if (physicsProperties.Grounded && physicsProperties.PhysicallyGrounded && !physicsProperties.holdMode && !physicsProperties.pullingToGrapplePoint || physicsProperties.isSwimming)
			{
				disableNotchKick = false;
				physicsProperties.jumpHeight = 0;
				canJump = true;
				numberOfTotalJumpsAvaliable = physicsProperties.numberOfTotalJumpsMax;
			}

			if (numberOfTotalJumpsAvaliable <= 0)
			{
				canJump = false;
			}

			//Grounded Jump
			if (numberOfTotalJumpsAvaliable == physicsProperties.numberOfTotalJumpsMax && activeDeadZoneTimer == physicsProperties.jumpDeadZoneTime && physicsProperties.PhysicallyGrounded && Input.GetKey(key) && !physicsProperties.dashing && !Physics.Raycast(transform.position, transform.up, out crouchRay, physicsProperties.crouchDistCheck, physicsProperties.crouchLayers) && !State.isStunned)
			{
				//crouch state is false
				physicsProperties.groundDetectionToggle = false;
				physicsProperties.isCrouching = false;
				physicsProperties.applyGrappleJumpForce = false;

				activeDeadZoneTimer = physicsProperties.jumpDeadZoneTime;

				physicsProperties.jumpHeight += physicsProperties.jumpforce + 1f;

				disableNotchKick = true;

				numberOfTotalJumpsAvaliable -= 1;
				print("Reduced Jump");
			}

			if(!physicsProperties.PhysicallyGrounded || !physicsProperties.Grounded)
            {
				physicsProperties.jumpHeight -= (physicsProperties.jumpHeight * Time.deltaTime) - physicsProperties.incrementJumpFallSpeed * Time.deltaTime;
			}

			//Note: PhysicallyGrounded Fixed Frames Causes Irregularities In Jump Triggering
			if (!physicsProperties.PhysicallyGrounded && !physicsProperties.isSwimming && !physicsProperties.holdMode && !physicsProperties.swingMode)
			{
				//Hyphin's Feather Double Jump
				if (activeDeadZoneTimer == physicsProperties.jumpDeadZoneTime && Input.GetKey(key) && !physicsProperties.applyGrappleJumpForce && Stash.Instance.unlockList.unlockHyphinFeather && !physicsProperties.readyToWallKick && !physicsProperties.swingMode && !physicsProperties.pullingToSwingPoint && canJump && !State.isStunned)
				{
					disableNotchKick = true;
					physicsProperties.applyGrappleJumpForce = false;

					physicsProperties.gravityMultiplier = 0;

					numberOfTotalJumpsAvaliable -= 1;
					print("Reduced Jump Double");

					physicsProperties.groundDetectionToggle = false;
					physicsProperties.rb.velocity = Vector3.zero;
					physicsProperties.jumpHeight += physicsProperties.jumpdoubleforce;
					physicsProperties.vel.y = physicsProperties.jumpdoubleforce;
					physicsProperties.velocity.y = physicsProperties.jumpdoubleforce;

					numberOfTotalJumpsAvaliable -= 1;
					physicsProperties.isWallKicking = false;
					physicsProperties.DoubleJump = true;
				}

				//cancel wall kicking
				if(physicsProperties.isWallKicking && Stash.Instance.unlockList.unlockHyphinFeather && !physicsProperties.readyToWallKick && Input.GetKey(key) && !physicsProperties.swingMode && !physicsProperties.pullingToSwingPoint && activeDeadZoneTimer == physicsProperties.jumpDeadZoneTime || State.isStunned)
                {
					disableNotchKick = true;
					physicsProperties.applyGrappleJumpForce = false;

					physicsProperties.gravityMultiplier = 0;

					numberOfTotalJumpsAvaliable -= 1;
					print("Reduced Jump Double");

					physicsProperties.groundDetectionToggle = false;
					physicsProperties.rb.velocity = Vector3.zero;
					physicsProperties.jumpHeight += physicsProperties.jumpdoubleforce;
					physicsProperties.vel.y = physicsProperties.jumpdoubleforce;
					physicsProperties.velocity.y = physicsProperties.jumpdoubleforce;

					numberOfTotalJumpsAvaliable -= 1;
					physicsProperties.isWallKicking = false;
					physicsProperties.DoubleJump = true;
				}
			}

			//Create a way to jump out of water
			if (!physicsProperties.groundDetectionToggle)
			{
				activeDeadZoneTimer -= Time.deltaTime;

				if (activeDeadZoneTimer <= 0)
				{
					physicsProperties.groundDetectionToggle = true;
					activeDeadZoneTimer = physicsProperties.jumpDeadZoneTime;
					disableNotchKick = false;

					if (physicsProperties.DoubleJump)
                    {
						physicsProperties.DoubleJump = false;
                    }
				}
			}

			physicsProperties.velocity.y += physicsProperties.jumpHeight;
		}
	}
	public void ResetGroundDetection()
    {
		physicsProperties.groundDetectionToggle = true;
		activeDeadZoneTimer = physicsProperties.jumpDeadZoneTime;
		disableNotchKick = false;
	}
	void MoveCorrection()
    {
		if(State.isStunned)
        {
			physicsProperties.velocity = new Vector3(0, 0, 0);
			physicsProperties.move = Vector3.zero;
			physicsProperties.vel = Vector3.zero;
			physicsProperties.dir = Vector3.zero;
			physicsProperties.jumpHeight = 0;
        }
    }
	void Move()
	{
		if (!physicsProperties.movementLock)
		{
			float flightAcceleration = 0;

			//Cap Acceleration Gain
			flightAcceleration = Mathf.Clamp(physicsProperties.currentMovementSpeed, 0, physicsProperties.maxSpinningSpoolSpeed);

			if (physicsProperties.hovering)
			{
				//Additively Gain Acceleration when using any of these keys
				flightAcceleration += (physicsProperties.spinningSpoolSpeed * Time.deltaTime) - ((physicsProperties.spinningSpoolSpeed * Time.deltaTime * Stats.DebuffSheet.MovementSpeedReduction) / 2);
			}
			if (physicsProperties.Grounded)
			{
				flightAcceleration = Mathf.LerpUnclamped(flightAcceleration, 0, Time.deltaTime);
			}

			//Camera Driven Direction
			Vector3 CamForward = Vector3.zero;
			Vector3 CamRight = Vector3.zero;
			Vector3 CamUp = Vector3.zero;

			if (physicsProperties.MoveType.Equals(MovementType.FreeMove))
			{
				//Get Camera Directions
				CamForward = MyCamera.Rotater.forward;

				CamRight = MyCamera.Rotater.right;
				CamUp = Vector3.up;
			}

			if(physicsProperties.MoveType.Equals(MovementType.Strafe) && MyCamera.LockOnTarget != null)
			{
				//Get Camera Directions
				CamForward = MyCamera.LockOnTarget.GetComponent<LockOnTargetHelper>().lockOnEmpty.transform.forward;
				CamRight = MyCamera.LockOnTarget.GetComponent<LockOnTargetHelper>().lockOnEmpty.transform.right;
				CamUp = Vector3.up;
			}

			Vector3 targetLocation = Vector3.zero;

			if (!physicsProperties.MoveType.Equals(MovementType.FPSMove))
			{
				//Create Location Via Movement Vector * Camera Forward
				targetLocation = InputManager.Instance.PlayerInput.MovementVector.z * CamForward;

				//Create Location Via Movement Vector * Camera Right
				targetLocation += InputManager.Instance.PlayerInput.MovementVector.x * CamRight;
				targetLocation += InputManager.Instance.PlayerInput.MovementVector.y * CamUp;

				if(physicsProperties.MoveType.Equals(MovementType.Strafe))
                {
					//Create Location Via Movement Vector 
					targetLocation = new Vector3(InputManager.Instance.PlayerInput.MovementVector.x * CamRight.x , 0, InputManager.Instance.PlayerInput.MovementVector.z * CamForward.z);
					targetLocation = MyCamera.LockOnTarget.GetComponent<LockOnTargetHelper>().lockOnEmpty.transform.TransformDirection(targetLocation);
				}

				physicsProperties.velocity += targetLocation;
			}

			if (!State.isStunned)
			{
				//Prevents Dual Key Inputs from being faster then one key input
				//physicsProperties.move = Vector3.ClampMagnitude(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")), 1f);
				physicsProperties.move = Vector3.ClampMagnitude(InputManager.Instance.PlayerInput.MovementVector, 1f);

				//Original Movement Method
				if (physicsProperties.MoveType.Equals(MovementType.FPSMove))
				{
					physicsProperties.velocity += physicsProperties.move;
				}

				//Air Control
				if (!physicsProperties.isSwimming && !physicsProperties.readyToWallKick && !physicsProperties.Grounded && !physicsProperties.airMovementLock && !physicsProperties.pullingToGrapplePoint && !physicsProperties.pullingToSwingPoint && !physicsProperties.hovering && !physicsProperties.dashing && !physicsProperties.swingMode && !physicsProperties.holdMode && !State.isStunned && !physicsProperties.isWallHanging)
				{
					//Sprint
					float input = Mathf.Clamp01(InputManager.Instance.PlayerInput.MovementVector.x + InputManager.Instance.PlayerInput.MovementVector.z);

					//Direction Vector
					Vector3 Dir = new Vector3(0, 0, input);

					if (!physicsProperties.slowWingGravityMode)
					{
						if (physicsProperties.MoveType.Equals(MovementType.FreeMove) || physicsProperties.MoveType.Equals(MovementType.FPSMove))
						{
							physicsProperties.currentMovementSpeed = ((flightAcceleration * Time.deltaTime) + CalculatePlayerMovement(physicsProperties.baseMovementSpeed));

							//Grapple Jump Force
							if (physicsProperties.applyGrappleJumpForce)
							{
								Vector3 dir = transform.TransformDirection(physicsProperties.rb.velocity);
								physicsProperties.vel = new Vector3((physicsProperties.velocity.x) + dir.x * physicsProperties.grappleLeapForce * Time.deltaTime, (physicsProperties.velocity.y) * physicsProperties.grappleLeapForce * Time.deltaTime, (physicsProperties.velocity.z) + dir.z * physicsProperties.grappleLeapForce * Time.deltaTime);
								physicsProperties.rb.AddForce(physicsProperties.vel, ForceMode.Impulse);
							}
							else
                            {
								physicsProperties.vel = new Vector3(physicsProperties.velocity.x * physicsProperties.currentMovementSpeed, 0, physicsProperties.velocity.z * physicsProperties.currentMovementSpeed);
							}
						}

						if (physicsProperties.MoveType.Equals(MovementType.Strafe))
						{
							physicsProperties.currentMovementSpeed = ((flightAcceleration * Time.deltaTime) + CalculatePlayerMovement(physicsProperties.baseMovementSpeed));

							physicsProperties.vel = new Vector3(physicsProperties.velocity.x * physicsProperties.currentMovementSpeed, 0, physicsProperties.velocity.z * physicsProperties.currentMovementSpeed);
						}
					}
					else
					{
						if (physicsProperties.MoveType.Equals(MovementType.FreeMove) || physicsProperties.MoveType.Equals(MovementType.FPSMove))
						{
							if (!physicsProperties.isRunning)
							{
								physicsProperties.currentMovementSpeed = ((flightAcceleration * Time.deltaTime) + CalculatePlayerMovement(physicsProperties.baseMovementSpeed));
							}
							else
                            {
								physicsProperties.currentMovementSpeed = ((flightAcceleration * Time.deltaTime) + CalculatePlayerMovement(physicsProperties.runSpeed + physicsProperties.baseMovementSpeed));
							}

							//Grapple Jump Force
							if (physicsProperties.applyGrappleJumpForce)
							{
								Vector3 dir = transform.TransformDirection(physicsProperties.rb.velocity);
								physicsProperties.vel = new Vector3((physicsProperties.velocity.x) + dir.x * physicsProperties.grappleLeapForce * Time.deltaTime, (physicsProperties.velocity.y) * physicsProperties.grappleLeapForce * Time.deltaTime, (physicsProperties.velocity.z) + dir.z * physicsProperties.grappleLeapForce * Time.deltaTime);
								physicsProperties.rb.AddRelativeForce(physicsProperties.vel);
							}
							else
							{
								if (!physicsProperties.isWallKicking)
								{
									physicsProperties.vel = new Vector3(physicsProperties.velocity.x * physicsProperties.currentMovementSpeed, physicsProperties.velocity.y, physicsProperties.velocity.z * physicsProperties.currentMovementSpeed);
								}
							}
						}

						if (physicsProperties.MoveType.Equals(MovementType.Strafe))
						{
							if (!physicsProperties.isRunning)
							{
								physicsProperties.currentMovementSpeed = ((flightAcceleration * Time.deltaTime) + CalculatePlayerMovement(physicsProperties.baseMovementSpeed));
							}
							else
							{
								physicsProperties.currentMovementSpeed = ((flightAcceleration * Time.deltaTime) + CalculatePlayerMovement(physicsProperties.runSpeed + physicsProperties.baseMovementSpeed));
							}

							if (!physicsProperties.isWallKicking)
							{
								physicsProperties.vel = new Vector3(physicsProperties.velocity.x, physicsProperties.velocity.y, (physicsProperties.velocity.x + physicsProperties.velocity.z) * physicsProperties.currentMovementSpeed);
							}
						}
					}

					if (physicsProperties.MoveType.Equals(MovementType.Strafe))
					{
						//Apply Forces
						//This causes you to always move forward while in a grapple jump off state
						physicsProperties.rb.AddRelativeForce(physicsProperties.vel * CalculatePlayerMovement(physicsProperties.airforce), ForceMode.Acceleration);
					}

					if (physicsProperties.MoveType.Equals(MovementType.FPSMove))
					{
						//Apply Forces
						//This causes you to always move forward while in a grapple jump off state
						if (!physicsProperties.isWallKicking)
						{
							physicsProperties.rb.AddRelativeForce(physicsProperties.move * CalculatePlayerMovement(physicsProperties.airforce), ForceMode.Acceleration);
						}
					}

					if (physicsProperties.MoveType.Equals(MovementType.FreeMove))
					{
						//Apply Forces
						if (!physicsProperties.isWallKicking)
						{
							physicsProperties.rb.AddRelativeForce(physicsProperties.move * CalculatePlayerMovement(physicsProperties.airforce), ForceMode.Acceleration);
						}
					}
				}

				//Wall Kick Movment
				if(!physicsProperties.isSwimming && physicsProperties.isWallKicking)
				{
					physicsProperties.currentMovementSpeed = CalculatePlayerMovement(physicsProperties.baseMovementSpeed);

					physicsProperties.vel = new Vector3(0, physicsProperties.velocity.y, 1 * physicsProperties.NotchKickDistance);
				}

				//Must Fix Strafe Lock On Direction
				//Walking
				if (gChecker.groundSlopeAngle < physicsProperties.slopeLimit && !physicsProperties.isSwimming && physicsProperties.Grounded && !physicsProperties.dashing && !State.isStunned && !physicsProperties.isCrouching && !physicsProperties.isRunning)
				{
					physicsProperties.currentMovementSpeed = CalculatePlayerMovement(physicsProperties.baseMovementSpeed);

					if (physicsProperties.MoveType.Equals(MovementType.FreeMove) || physicsProperties.MoveType.Equals(MovementType.FPSMove))
					{
						physicsProperties.vel = new Vector3(physicsProperties.velocity.x * physicsProperties.currentMovementSpeed, 0, physicsProperties.velocity.z * physicsProperties.currentMovementSpeed);
					}

					if (physicsProperties.MoveType.Equals(MovementType.Strafe))
					{
						physicsProperties.vel = new Vector3(physicsProperties.velocity.x * physicsProperties.currentMovementSpeed, 0, physicsProperties.velocity.z * physicsProperties.currentMovementSpeed);
					}
				}

				//Swimming
				if (physicsProperties.isSwimming && !State.isStunned)
				{
					if (!physicsProperties.isSwimSprinting)
					{
						physicsProperties.currentMovementSpeed = CalculatePlayerMovement(physicsProperties.swimmingSpeed);

						if (physicsProperties.MoveType.Equals(MovementType.FreeMove))
						{
							if(physicsProperties.isUnderWater)
                            {
								BuoyancyForce.enablePhysics = false;
								physicsProperties.vel = new Vector3(physicsProperties.velocity.x * physicsProperties.currentMovementSpeed, physicsProperties.velocity.y * physicsProperties.currentMovementSpeed, physicsProperties.velocity.z * physicsProperties.currentMovementSpeed);
							}
							else
                            {
								BuoyancyForce.enablePhysics = true;
								physicsProperties.vel = new Vector3(physicsProperties.velocity.x * physicsProperties.currentMovementSpeed, 0, physicsProperties.velocity.z * physicsProperties.currentMovementSpeed);
							}
						}

						if(physicsProperties.MoveType.Equals(MovementType.FPSMove))
                        {
							if (physicsProperties.isUnderWater)
							{
								BuoyancyForce.enablePhysics = false;
								physicsProperties.vel = new Vector3(physicsProperties.velocity.z * physicsProperties.currentMovementSpeed, physicsProperties.velocity.y * physicsProperties.currentMovementSpeed, physicsProperties.velocity.x * physicsProperties.currentMovementSpeed);
							}
							else
							{
								BuoyancyForce.enablePhysics = true;
								physicsProperties.vel = new Vector3(physicsProperties.velocity.x * physicsProperties.currentMovementSpeed, physicsProperties.velocity.y * physicsProperties.currentMovementSpeed, physicsProperties.velocity.z * physicsProperties.currentMovementSpeed);
							}
						}

						if (physicsProperties.MoveType.Equals(MovementType.Strafe))
						{
							if (physicsProperties.isUnderWater)
							{
								BuoyancyForce.enablePhysics = false;
								physicsProperties.vel = new Vector3(physicsProperties.velocity.x * physicsProperties.currentMovementSpeed, physicsProperties.velocity.y * physicsProperties.currentMovementSpeed, physicsProperties.velocity.z * physicsProperties.currentMovementSpeed);
							}
							else
							{
								BuoyancyForce.enablePhysics = true;
								physicsProperties.vel = new Vector3(physicsProperties.velocity.x * physicsProperties.currentMovementSpeed, physicsProperties.velocity.y * physicsProperties.currentMovementSpeed, physicsProperties.velocity.z * physicsProperties.currentMovementSpeed);
							}
						}
					}
					else
                    {
						physicsProperties.currentMovementSpeed = CalculatePlayerMovement(physicsProperties.swimmingSprintSpeed);

						if (physicsProperties.MoveType.Equals(MovementType.FreeMove) || physicsProperties.MoveType.Equals(MovementType.FPSMove))
						{
							if (physicsProperties.isUnderWater)
							{
								BuoyancyForce.enablePhysics = false;
								physicsProperties.vel = new Vector3(physicsProperties.velocity.x * physicsProperties.currentMovementSpeed, physicsProperties.velocity.y * physicsProperties.currentMovementSpeed, physicsProperties.velocity.z * physicsProperties.currentMovementSpeed);
							}
							else
							{
								BuoyancyForce.enablePhysics = true;
								physicsProperties.vel = new Vector3(physicsProperties.velocity.x * physicsProperties.currentMovementSpeed, physicsProperties.velocity.y * physicsProperties.currentMovementSpeed, physicsProperties.velocity.z * physicsProperties.currentMovementSpeed);
							}
						}

						if (physicsProperties.MoveType.Equals(MovementType.Strafe))
						{
							if (physicsProperties.isUnderWater)
							{
								BuoyancyForce.enablePhysics = false;
								physicsProperties.vel = new Vector3(physicsProperties.velocity.x * physicsProperties.currentMovementSpeed, physicsProperties.velocity.y * physicsProperties.currentMovementSpeed, physicsProperties.velocity.z * physicsProperties.currentMovementSpeed);
							}
							else
							{
								BuoyancyForce.enablePhysics = true;
								physicsProperties.vel = new Vector3(physicsProperties.velocity.x * physicsProperties.currentMovementSpeed, physicsProperties.velocity.y * physicsProperties.currentMovementSpeed, physicsProperties.velocity.z * physicsProperties.currentMovementSpeed);
							}
						}
					}
				}

				//Running
				if (!physicsProperties.isSwimming && !physicsProperties.dashing && !State.isStunned && !physicsProperties.isCrouching && physicsProperties.isRunning)
				{
					physicsProperties.currentMovementSpeed = CalculatePlayerMovement(physicsProperties.runSpeed);

					if (physicsProperties.MoveType.Equals(MovementType.FreeMove) || physicsProperties.MoveType.Equals(MovementType.FPSMove))
					{
						physicsProperties.vel = new Vector3(physicsProperties.velocity.x * physicsProperties.currentMovementSpeed, 0, physicsProperties.velocity.z * physicsProperties.currentMovementSpeed);
					}

					if (physicsProperties.MoveType.Equals(MovementType.Strafe))
					{
						physicsProperties.vel = new Vector3(physicsProperties.velocity.x * physicsProperties.currentMovementSpeed, 0, physicsProperties.velocity.z * physicsProperties.currentMovementSpeed);
					}
				}

				//Crouching
				if (!physicsProperties.isSwimming && physicsProperties.Grounded && physicsProperties.isCrouching && !physicsProperties.dashing && !State.isStunned)
				{
					physicsProperties.currentMovementSpeed = CalculatePlayerMovement(physicsProperties.crouchMovementSpeed);

					if (physicsProperties.MoveType.Equals(MovementType.FreeMove) || physicsProperties.MoveType.Equals(MovementType.FPSMove))
					{
						physicsProperties.vel = new Vector3(physicsProperties.velocity.x * physicsProperties.currentMovementSpeed, physicsProperties.velocity.y , physicsProperties.velocity.z * physicsProperties.currentMovementSpeed);
					}

					if (physicsProperties.MoveType.Equals(MovementType.Strafe))
					{
						physicsProperties.vel = new Vector3(physicsProperties.velocity.x * physicsProperties.currentMovementSpeed, physicsProperties.velocity.y, physicsProperties.velocity.z * physicsProperties.currentMovementSpeed);
					}
				}


				SpinningSpool();

				if (!physicsProperties.dashing)
				{
					if (physicsProperties.MoveType.Equals(MovementType.FreeMove))
					{
						//Set Direction
						if (physicsProperties.isWallKicking || physicsProperties.applyGrappleJumpForce)
						{
							physicsProperties.vel = transform.TransformDirection(physicsProperties.vel);
						}

						transform.TransformDirection(transform.forward);

						//Actively Moving
						transform.position += physicsProperties.vel * Time.deltaTime;
						physicsProperties.velocity = Vector3.zero;
					}

					if (physicsProperties.MoveType.Equals(MovementType.Strafe))
					{
						//Set Direction
						physicsProperties.vel = transform.TransformDirection(physicsProperties.vel);
						transform.TransformDirection(transform.forward);

						//Actively Moving
						transform.position += (physicsProperties.vel * Time.deltaTime);
						physicsProperties.velocity = Vector3.zero;
					}

					if (physicsProperties.MoveType.Equals(MovementType.FPSMove))
					{
						physicsProperties.vel = transform.TransformDirection(physicsProperties.vel);
						transform.TransformDirection(transform.forward);

						//Actively Moving
						transform.position += physicsProperties.vel * Time.deltaTime;
						physicsProperties.velocity = Vector3.zero;
					}
				}
			}
		}
	}
    private void Gravity()
	{
		if (physicsProperties.ApplyGravity && !physicsProperties.Grounded && !physicsProperties.swingMode && !physicsProperties.pullingToSwingPoint && !physicsProperties.pullingToGrapplePoint && !physicsProperties.slowWingGravityMode)
		{
			physicsProperties.gravityMultiplier += physicsProperties.gravityIncrease * Time.deltaTime;
			physicsProperties.gravityMultiplier = Mathf.Clamp(physicsProperties.gravityMultiplier, 0, physicsProperties.gravityMultiplierMax);
			physicsProperties.velocity.y -= (physicsProperties.intialGravity * Time.deltaTime + (physicsProperties.gravityMultiplier * Time.deltaTime));
			physicsProperties.vel = new Vector3(physicsProperties.velocity.x, physicsProperties.velocity.y, physicsProperties.velocity.z);
			transform.position += physicsProperties.vel * Time.deltaTime;
		}

		if (physicsProperties.Grounded || physicsProperties.swingMode || physicsProperties.pullingToSwingPoint || physicsProperties.pullingToGrapplePoint || physicsProperties.hovering || physicsProperties.slowWingGravityMode)
		{
			physicsProperties.gravityMultiplier = 0;
		}
	}
	private Vector3 rayPoint = Vector3.zero;
	RaycastHit tempHit;
	private void GroundChecking()
	{
		rayPoint = new Vector3(transform.position.x, transform.position.y + physicsProperties.rayHeight, transform.position.z);

		if (Physics.Raycast(rayPoint, Vector3.down, out tempHit, physicsProperties.floorRayDistance + physicsProperties.modelHeight, physicsProperties.excludePlayer) && physicsProperties.groundDetectionToggle && !physicsProperties.isSwimming)
		{
			physicsProperties.Grounded = true;
			physicsProperties.currJumpTime = 0;
			enableWallSlide = false;

			//Must set player to be parented to a uniform object that is (1,1,1) in order to prevent skew scaling
			if (tempHit.transform.GetComponentInParent<MovingPlatform>() != null && tempHit.transform.GetComponentInParent<MovingPlatform>().UniformParent != null)
            {
				transform.SetParent(tempHit.transform.GetComponentInParent<MovingPlatform>().UniformParent, true);
			} 
		}
		else
		{
			physicsProperties.Grounded = false;

			if (!physicsProperties.readyToWallKick)
			{
				transform.localScale = new Vector3(1, 1, 1);
				transform.SetParent(null);
			}
		}

		if (physicsProperties.Grounded && Vector3.Distance(transform.position, tempHit.point) <= 1.1)
		{
			Vector3 Goal = new Vector3(transform.position.x, tempHit.point.y + physicsProperties.modelHeight, transform.position.z);

			//Slides Player Off Of Slopes
			if(gChecker.groundSlopeAngle > physicsProperties.slopeLimit)
            {
				transform.position = Goal;
			}
		}
	}
	private void OnCollisionEnter(Collision collision)
	{
		physicsProperties.PhysicallyGrounded = true;
	}
	private void OnCollisionStay(Collision collision)
    {
		physicsProperties.PhysicallyGrounded = true;
    }
	private void OnCollisionExit(Collision collision)
	{
		physicsProperties.PhysicallyGrounded = false;
	}
	private void CollisionCheck()
	{
		Collider[] overlaps = new Collider[10];
		Collider myCollider = new Collider();

		int num = 0;

		if (physicsProperties.CapsuleCol != null)
		{
			num = Physics.OverlapCapsuleNonAlloc(transform.TransformPoint(physicsProperties.collisionOriginStart), transform.TransformPoint(physicsProperties.collisionOriginEnd), physicsProperties.collisionRadius, overlaps, physicsProperties.excludePlayer, QueryTriggerInteraction.UseGlobal);
			myCollider = physicsProperties.CapsuleCol;
		}

		for (int i = 0; i < num; i++)
		{
			Transform t = overlaps[i].transform;
			Vector3 dir;
			float dist;

			if (Physics.ComputePenetration(physicsProperties.CapsuleCol, transform.position, transform.rotation, overlaps[i], t.position, t.rotation, out dir, out dist))
			{
				Vector3 penetrationVector = dir * dist;
				Vector3 velocityProjected = Vector3.Project(physicsProperties.velocity + physicsProperties.vel + physicsProperties.move, -dir);
				transform.position = transform.position + penetrationVector;
				physicsProperties.vel -= velocityProjected;
			}

		}
	}
	Vector3 DashDir = Vector3.zero;
	bool DodgeReleased = true;
	void Dodge()
	{
		KeyCode key = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.DodgeIndex].key;

		if (!physicsProperties.descending && !physicsProperties.slowWingGravityMode && !physicsProperties.readyToWallKick && !physicsProperties.isCrouching && !physicsProperties.isSwimming && !physicsProperties.movementLock && Input.GetKey(key) && Cooldowns[0].ready && !physicsProperties.dashing && !physicsProperties.swingMode && !physicsProperties.holdMode && !physicsProperties.pullingToGrapplePoint && !physicsProperties.pullingToSwingPoint && !State.isStunned && !physicsProperties.isSwimming)
		{
			DashDir = new Vector3(physicsProperties.move.x, 0, physicsProperties.move.z);

			int TimeReduction = 2;

			//Lower Cooldown Duration Per Dodge While On Fire "SEE DODGING"
			foreach (DebuffEffect Debuff in State.Debuffs.DebuffEffects)
			{
				if (Debuff.DebuffName == "On Fire")
				{
					Debuff.CurrentDuration -= TimeReduction;
					print("Reduced The Duration Of The On-Fire Status by " + TimeReduction + " seconds.");
				}
			}


			currDodgeDuration = (physicsProperties.MaxDodgeDuration);
			disableNotchKick = false;
			physicsProperties.dashing = true;
			DodgeReleased = false;
		}

		if(!Input.GetKey(key) && !physicsProperties.dashing)
        {
			DodgeReleased = true;
		}
	}
	void DodgeLogic()
	{
		//Side Step
		if (physicsProperties.dashing && !physicsProperties.swingMode && !physicsProperties.holdMode && !State.isStunned)
		{
			physicsProperties.CapsuleCol.isTrigger = true;
			physicsProperties.dashing = true;
			physicsProperties.groundDetectionToggle = false;

			//Disable Gravity
			physicsProperties.ApplyGravity = false;
			physicsProperties.rb.velocity = Vector3.zero;
			physicsProperties.gravityMultiplier = 0;
			physicsProperties.rb.useGravity = false;

			currDodgeDuration -= Time.deltaTime * 1.5f;

			physicsProperties.currentMovementSpeed = (physicsProperties.DodgeSpeed);

			//Move Transform

			if(physicsProperties.MoveType.Equals(MovementType.FreeMove))
            {
				physicsProperties.vel = new Vector3(physicsProperties.velocity.x, physicsProperties.velocity.y, DashDir.z + physicsProperties.currentMovementSpeed * 1.2f);

				//Set Direction
				physicsProperties.vel = transform.TransformDirection(physicsProperties.vel);
				transform.TransformDirection(transform.forward);

				//Actively Moving
				//Stops Directional Upward Movement
				physicsProperties.vel.y = 0;
				transform.position += physicsProperties.vel  * Time.deltaTime;
				physicsProperties.velocity = Vector3.zero;
			}

			if (physicsProperties.MoveType.Equals(MovementType.Strafe) || physicsProperties.MoveType.Equals(MovementType.FPSMove))
			{
				//Dash Forward If No Keys Are Hit
				if (DashDir == Vector3.zero)
				{
					physicsProperties.vel = new Vector3(physicsProperties.velocity.x, physicsProperties.velocity.y, physicsProperties.velocity.z + physicsProperties.currentMovementSpeed * 1.2f);

					//Set Direction
					physicsProperties.vel = transform.TransformDirection(physicsProperties.vel);
					transform.TransformDirection(transform.forward);

					//Actively Moving
					transform.position += physicsProperties.vel * Time.deltaTime;
					physicsProperties.velocity = Vector3.zero;
				}
				//Dash In Our Direction Our Keys Are Being Pressed
				//FIX DODGE DIRECTION A/S/D ISSUE!!!!!
				else
				{
					physicsProperties.vel = new Vector3(DashDir.x * physicsProperties.currentMovementSpeed, physicsProperties.velocity.y, DashDir.z * physicsProperties.currentMovementSpeed * 1.2f);

					//Set Direction
					physicsProperties.vel = transform.TransformDirection(physicsProperties.vel);
					transform.TransformDirection(physicsProperties.move);

					//Actively Moving
					transform.position += physicsProperties.vel * Time.deltaTime;
					physicsProperties.velocity = Vector3.zero;
				}
			}
		}

		//Cancel Mid Dash
		if (State.isStunned)
		{
			physicsProperties.CapsuleCol.isTrigger = false;

			physicsProperties.dashing = false;
			physicsProperties.ApplyGravity = true;
			physicsProperties.rb.useGravity = true;

			DashDir = Vector3.zero;
		}

		if(physicsProperties.isSwimming)
        {
			physicsProperties.CapsuleCol.isTrigger = false;

			physicsProperties.dashing = false;
			physicsProperties.ApplyGravity = true;
			physicsProperties.rb.useGravity = true;
		}

		if (currDodgeDuration <= 0)
		{
			physicsProperties.CapsuleCol.isTrigger = false;

			IntiateCooldown("Dodge");

			//Set Run State To True
			physicsProperties.isRunning = true;
			physicsProperties.ApplyGravity = true;
			physicsProperties.rb.useGravity = true;

			DashDir = Vector3.zero;
			currDodgeDuration = physicsProperties.MaxDodgeDuration;
			physicsProperties.dashing = false;
		}
	}
	private bool SlideReleased = true;
	void Sliding()
    {
		KeyCode key = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.CrouchIndex].key;

		if (Input.GetKey(key) && !physicsProperties.isSwimming && physicsProperties.isRunning && SlideReleased && !physicsProperties.movementLock && physicsProperties.isCrouching && Cooldowns[0].ready && !physicsProperties.dashing && !physicsProperties.swingMode && !physicsProperties.holdMode && !physicsProperties.pullingToGrapplePoint && !physicsProperties.pullingToSwingPoint && !State.isStunned)
		{
			DashDir = new Vector3(physicsProperties.move.x, 0, physicsProperties.move.z);

			currDodgeDuration = physicsProperties.MaxDodgeDuration;
			physicsProperties.isSliding = true;
			SlideReleased = false;
		}

		if (!Input.GetKey(key) && !physicsProperties.isSliding)
		{
			SlideReleased = true;
		}

		if (physicsProperties.isCrouching && physicsProperties.isSwimming)
		{
			physicsProperties.CapsuleCol.isTrigger = false;

			IntiateCooldown("Dodge");

			//Cancel run/slide state
			physicsProperties.isRunning = false;
			physicsProperties.isSliding = false;

			physicsProperties.dashing = false;
			physicsProperties.ApplyGravity = true;
			physicsProperties.rb.useGravity = true;

			currDodgeDuration = physicsProperties.MaxDodgeDuration;
			physicsProperties.dashing = false;
			physicsProperties.isCrouching = false;
		}
	}
	void SlidingLogic()
	{
		//Sliding
		if (physicsProperties.isSliding && !physicsProperties.swingMode && !physicsProperties.holdMode && !State.isStunned)
		{
			//Crouch Height
			physicsProperties.CapsuleCol.GetComponent<CapsuleCollider>().center = new Vector3(0, -0.5f, 0);
			physicsProperties.CapsuleCol.GetComponent<CapsuleCollider>().radius = 0.5f;
			physicsProperties.CapsuleCol.GetComponent<CapsuleCollider>().height = 1;

			physicsProperties.CapsuleCol.isTrigger = true;
			physicsProperties.groundDetectionToggle = false;

			//Disable Gravity
			physicsProperties.ApplyGravity = false;
			physicsProperties.rb.velocity = Vector3.zero;
			physicsProperties.gravityMultiplier = 0;
			physicsProperties.rb.useGravity = false;

			currDodgeDuration -= Time.deltaTime * 1.5f;

			physicsProperties.currentMovementSpeed = CalculatePlayerMovement(physicsProperties.DodgeSpeed);

			//Move Transform

			//Dash Forward
			physicsProperties.vel = new Vector3(physicsProperties.velocity.x, physicsProperties.velocity.y, physicsProperties.velocity.z + physicsProperties.currentMovementSpeed * 1.2f);

			//Set Direction
			physicsProperties.vel = transform.TransformDirection(physicsProperties.vel);
			transform.TransformDirection(transform.forward);

			//Actively Moving
			transform.position += physicsProperties.vel * Time.deltaTime;
			physicsProperties.velocity = Vector3.zero;
		}

		//Cancel Mid Dash
		if (State.isStunned)
		{
			physicsProperties.CapsuleCol.isTrigger = false;

			//Cancel run/slide state
			physicsProperties.isRunning = false;
			physicsProperties.isSliding = false;

			physicsProperties.dashing = false;
			physicsProperties.ApplyGravity = true;
			physicsProperties.rb.useGravity = true;

			
		}

		if (currDodgeDuration <= 0)
		{
			physicsProperties.CapsuleCol.isTrigger = false;

			IntiateCooldown("Dodge");

			//Cancel run/slide state
			physicsProperties.isRunning = false;
			physicsProperties.isSliding = false;

			physicsProperties.dashing = false;
			physicsProperties.ApplyGravity = true;
			physicsProperties.rb.useGravity = true;

			currDodgeDuration = physicsProperties.MaxDodgeDuration;
			physicsProperties.dashing = false;
		}
	}
	void Running()
    {
		if(physicsProperties.isRunning && !p_Settings.gameplaySettings.Mode.Equals(CameraMode.TargetMode) && !State.isStunned)
        {
			physicsProperties.currentMovementSpeed = CalculatePlayerMovement(physicsProperties.runSpeed);

			//If no direction is being input then stop running
			if(InputManager.Instance.PlayerInput.MovementVector == Vector3.zero)
            {
				physicsProperties.isRunning = false;
            }

			if(physicsProperties.slowWingGravityMode)
            {
				physicsProperties.isRunning = false;
            }
        }

		if(physicsProperties.isRunning && !physicsProperties.wingFlapToggle && Stash.Instance.unlockList.unlockCardinalCowl || p_Settings.gameplaySettings.Mode.Equals(CameraMode.TargetMode) || State.isStunned)
        {
			physicsProperties.isRunning = false;
        }
    }
	public void SpinningSpool()
	{
		if (Stash.Instance.unlockList.unlockSpinningSpool)
		{
			//Limit Stamina
			currSpoolStamina = Mathf.Clamp(currSpoolStamina, 0, physicsProperties.maxSpoolStamina);

			//Key Input
			KeyCode key = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.JumpIndex].key;

			float flightAcceleration = 0;

			//Cap Acceleration Gain
			flightAcceleration = Mathf.Clamp(physicsProperties.currentMovementSpeed, 0, physicsProperties.maxSpinningSpoolSpeed);

			if (physicsProperties.hovering && physicsProperties.rb.useGravity && !State.isStunned)
			{
				//Additively Gain Acceleration when using any of these keys
				flightAcceleration += (physicsProperties.spinningSpoolSpeed * Time.deltaTime);
				physicsProperties.gravityMultiplier = 0;
				currSpoolStamina -= Time.deltaTime;

				physicsProperties.rb.velocity = Vector3.zero;
				physicsProperties.isWallKicking = false;
				physicsProperties.currentMovementSpeed += physicsProperties.spoolAcceleration * Time.deltaTime;

				//Accelerate Rigidbody
				if(physicsProperties.rb.velocity.y <= 10)
                {
					physicsProperties.rb.AddRelativeForce(InputManager.Instance.PlayerInput.Horizontal * physicsProperties.spinningSpoolForce * 5f, -1f, InputManager.Instance.PlayerInput.Vertical * physicsProperties.spinningSpoolForce / 2f);
					physicsProperties.vel = new Vector3(physicsProperties.velocity.x * physicsProperties.currentMovementSpeed, physicsProperties.velocity.y, physicsProperties.velocity.z * physicsProperties.currentMovementSpeed);
				}
			}
			if (physicsProperties.Grounded)
			{
				flightAcceleration = Mathf.LerpUnclamped(flightAcceleration, 0, Time.deltaTime);
			}

			//Burn Stamina + Hover Activation
			if (Stash.Instance.unlockList.unlockCardinalCowl && !physicsProperties.slowWingGravityMode && Input.GetKey(key) && numberOfTotalJumpsAvaliable <= 0 && physicsProperties.currentWingFlaps <= 0 && !physicsProperties.dashing && currSpoolStamina > 0 && !State.isStunned && activeDeadZoneTimer == physicsProperties.jumpDeadZoneTime || Input.GetKey(key) && numberOfTotalJumpsAvaliable <= 0 && !physicsProperties.dashing && currSpoolStamina > 0 && !State.isStunned && activeDeadZoneTimer == physicsProperties.jumpDeadZoneTime && !Stash.Instance.unlockList.unlockCardinalCowl)
			{
				physicsProperties.hovering = true;
				physicsProperties.applyGrappleJumpForce = false;
			}

			if (!Input.GetKey(key) || physicsProperties.dashing || currSpoolStamina <= 0 || State.isStunned || physicsProperties.readyToWallKick)
			{
				physicsProperties.hovering = false;
			}

			if (physicsProperties.Grounded)
			{
				currSpoolStamina = physicsProperties.maxSpoolStamina;
			}

			if (physicsProperties.hovering && physicsProperties.dashing)
			{
				physicsProperties.hovering = false;
			}
		}
	}
	RaycastHit swanHit;
	//Work On This Next
	void WakingDescent()
	{
		KeyCode key = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.CrouchIndex].key;

		if (Stash.Instance.unlockList.unlockWakingDescent)
		{
			if (!physicsProperties.dashing && !physicsProperties.slowWingGravityMode && !physicsProperties.readyToWallKick && !physicsProperties.isSwimming && Input.GetKey(key) && Cooldowns[1].ready && !physicsProperties.dashing && !physicsProperties.Grounded && !physicsProperties.swingMode && !physicsProperties.holdMode && !physicsProperties.pullingToGrapplePoint && !physicsProperties.pullingToSwingPoint && !State.isStunned && !Physics.Raycast(transform.position, Vector3.down, out swanHit, physicsProperties.wakingDescentDistCheck))
			{
				physicsProperties.descending = true;
				physicsProperties.airMovementLock = true;
				physicsProperties.applyGrappleJumpForce = false;
				IntiateCooldown("Waking Descent");
			}

			if (physicsProperties.descending)
			{
				wakingDescentAcceleration = Mathf.Clamp(wakingDescentAcceleration, 0, physicsProperties.wakingDescentAccelerationMaximum);
				wakingDescentAcceleration += Time.deltaTime * physicsProperties.wakingDescentAccelerationRate;
				transform.position += new Vector3(0, Vector3.down.y * (wakingDescentAcceleration + (physicsProperties.wakingDescentMovementSpeed * Time.deltaTime)), 0);

				KeyCode CancelKey = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.JumpIndex].key;
				
				//If Space is Input Then Cancel Force
				if(Input.GetKey(CancelKey))
                {
					physicsProperties.rb.velocity = new Vector3(0, 0, 0);
					wakingDescentAcceleration = 0;
					physicsProperties.airMovementLock = false;
					physicsProperties.dashing = false;
					physicsProperties.descending = false;
				}
			}

			if (physicsProperties.descending && Physics.Raycast(transform.position, Vector3.down, out swanHit, 2))
			{
				Collider[] hitColliders = Physics.OverlapSphere(transform.position, physicsProperties.wakingDescentdamageRadius, physicsProperties.wakingDescentMask);

				foreach (Collider Enemy in hitColliders)
				{
					if (Enemy.gameObject.GetComponent<EntityHealth>() != null && Enemy.gameObject.GetComponent<EntityHealth>().Identity.Equals(EntityTarget.Enemy) || physicsProperties.isSwimming)
					{
						DamageProperty WakingDescentDamage = new DamageProperty(gameObject, EntityTarget.Enemy, null, 0, 0, Stats.PlayerSheet.StatusChance,1.5f, DamageClassification.Magical, DamageTypes.DreamDamage, ElementalAffix.Elementless,0, 0);

						CallHealthEvent.SendDamageEvent(WakingDescentDamage, Enemy.GetComponent<EntityHealth>());

						float RandomKnockBackForce = Random.Range(0, 100f);
						float RandomTorque = Random.Range(-1, 1);
						float RandomDirectionX = Random.Range(-1, 1);
						float RandomDirectionY = Random.Range(-1, 1);
						float RandomDirectionZ = Random.Range(-1, 1);

						Vector3 RandomDir = new Vector3(RandomDirectionX, RandomDirectionY, RandomDirectionZ);

						//Apply Knockback
						Enemy.transform.GetComponent<Rigidbody>().AddForce(transform.up * (RandomKnockBackForce + (physicsProperties.wakingDescentPushBackForce * wakingDescentAcceleration)), ForceMode.Impulse);
						Enemy.transform.GetComponent<Rigidbody>().AddTorque(RandomDir * (RandomTorque * (physicsProperties.wakingDescentPushBackForce * wakingDescentAcceleration)), ForceMode.Impulse);
					}
				}

				//Fixed Position
				transform.position = swanHit.point;

				physicsProperties.rb.velocity = new Vector3(0, 0, 0);
				physicsProperties.jumpHeight = 0;
				physicsProperties.vel.y = 0;
				physicsProperties.velocity.y = 0;
				wakingDescentAcceleration = 0;
				physicsProperties.airMovementLock = false;
				physicsProperties.dashing = false;
				physicsProperties.descending = false;
			}
		}
	}
	RaycastHit hit;
	RaycastHit swingHookHit;
	SpringJoint springJoint;
	bool createSwingPhysics = false;
    void RotaryHook()
	{
		if (Stash.Instance.unlockList.unlockRotaryHook)
		{
			#region SwingHook
			//SWING HOOK//
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, physicsProperties.grappleSwingDist, physicsProperties.grappleSwingLayer);
			float nearestDistance = float.MaxValue;
			float distance;

			KeyCode key = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.GrappleIndex].key;

			if (!physicsProperties.swingMode && !physicsProperties.pullingToSwingPoint && !physicsProperties.pullingToGrapplePoint && !physicsProperties.holdMode)
			{
				foreach (var collider in hitColliders)
				{
					distance = Vector3.Distance(transform.position, collider.transform.position);

					if (distance < nearestDistance && collider.transform.GetComponent<GrappleSwing>() != null)
					{
						nearestDistance = distance;
						physicsProperties.GrappleSwingObject = collider.transform;
					}
				}
			}

			if (hitColliders == null && !physicsProperties.swingMode)
			{
				physicsProperties.GrappleSwingObject = null;
			}

			if (physicsProperties.GrappleSwingObject != null)
			{
				Vector3 screenPoint = PlayerSettings.Instance.gameplaySettings.WorldCamera.WorldToViewportPoint(physicsProperties.GrappleSwingObject.position);
				physicsProperties.onGrappleScreen = screenPoint.z > 0f && screenPoint.x > 0f && screenPoint.x < 1f && screenPoint.y > 0f && screenPoint.y < 1f;

				//physicsProperties.onGrappleScreen = screenPoint.z > 0f && screenPoint.x > 0.3f && screenPoint.x < 0.7f && screenPoint.y > 0.3f && screenPoint.y < 0.7f;
			}

			float swingDist = physicsProperties.grappleSwingDist;

			if(physicsProperties.GrappleSwingObject != null)
            {
				physicsProperties.linecastGrappleBlock = Physics.Linecast(transform.position, physicsProperties.GrappleSwingObject.position, out swingHookHit, physicsProperties.grappleExclusionList);
			}
			else
            {
				physicsProperties.linecastGrappleBlock = true;
            }

			if(physicsProperties.GrappleSwingObject != null && !GetComponent<PlayerInteractor>().grapplePoint && Vector3.Distance(transform.position, physicsProperties.GrappleSwingObject.position) <= swingDist && !physicsProperties.linecastGrappleBlock && physicsProperties.onGrappleScreen && physicsProperties.GrappleSwingObject != null)
            {
				GetComponent<PlayerInteractor>().grappleSwing = true;
			}
			else
            {
				GetComponent<PlayerInteractor>().grappleSwing = false;
				UIManager.Instance.displayGrappleSwing = true;
			}
			
			//Requires line of sight to grapple and hold for Grapple Points //Disabled onGrappleScreen boolean
			if (!physicsProperties.descending && !createSwingPhysics && physicsProperties.onGrappleScreen && !physicsProperties.linecastGrappleBlock && !physicsProperties.inPointRange && !MyCamera.CancelHook && !physicsProperties.isSwimming && CooldownState("Grapple Hook") && !physicsProperties.dashing && !State.isStunned && !physicsProperties.swingMode && !physicsProperties.pullingToSwingPoint && !physicsProperties.pullingToGrapplePoint && !physicsProperties.holdMode && Input.GetKey(key) && Vector3.Distance(transform.position, physicsProperties.GrappleSwingObject.position) <= swingDist)
			{
				UIManager.Instance.displayGrappleActions = true;
				physicsProperties.slowWingGravityMode = false;
				physicsProperties.jumpHeight = 0;
				physicsProperties.pullingToSwingPoint = true;
				physicsProperties.isWallKicking = false;
				numberOfTotalJumpsAvaliable = 1;
				currSpoolStamina = physicsProperties.maxSpoolStamina;
				physicsProperties.GrappleSwingObject.GetComponent<GrappleSwing>().AttachPlayer(gameObject);
				GrappleObj = physicsProperties.GrappleSwingObject;
				PlayerSettings.Instance.gameplaySettings.Mode = CameraMode.GrappleMode;
				transform.eulerAngles = new Vector3(transform.eulerAngles.x, MyCamera.Rotater.eulerAngles.y, 0);
				IntiateCooldown("Grapple Hook");
			}

			//Move To Point Based on Grapple Speed
			if (physicsProperties.pullingToSwingPoint)
			{
				transform.position = Vector3.LerpUnclamped(transform.position, physicsProperties.GrappleSwingObject.position, physicsProperties.grappleSpeed * Time.deltaTime);
				physicsProperties.ApplyGravity = false;
				physicsProperties.rb.useGravity = false;
				physicsProperties.rb.velocity = Vector3.zero;
				Quaternion q = Quaternion.FromToRotation(transform.forward, transform.up) * Quaternion.LookRotation(physicsProperties.GrappleSwingObject.position - transform.position, transform.up);
				Vector3 angles = new Vector3(q.eulerAngles.x, transform.eulerAngles.y, 0);
				transform.rotation = Quaternion.Euler(angles);
				//print(Vector3.Distance(transform.position, physicsProperties.physicsProperties.GrappleSwingObject.position) + " Goal = " + physicsProperties.physicsProperties.GrappleSwingObject.GetComponent<GrappleSwing>().HookRange + 3f);
			}

			//Calculate If we are in range
			if (physicsProperties.GrappleSwingObject != null && Vector3.Distance(transform.position, physicsProperties.GrappleSwingObject.position) <= swingDist)
			{
				physicsProperties.inSwingRange = true;
			}
			else
			{
				physicsProperties.inSwingRange = false;
			}

			//Now lets see if we get close enough to our grapple position to know when we are too close and we can disengage with a offest margin
			if (physicsProperties.pullingToSwingPoint && physicsProperties.GrappleSwingObject != null && Vector3.Distance(transform.position, physicsProperties.GrappleSwingObject.position) <= physicsProperties.GrappleSwingObject.GetComponent<GrappleSwing>().HookRange)
			{
				//physicsProperties.rb.velocity = physicsProperties.rb.velocity.normalized * physicsProperties.physicsProperties.maxSwingSpeed;

				//Reset Jumps For some extra mobillity fun!
				numberOfTotalJumpsAvaliable = 1;
				physicsProperties.rb.useGravity = false;
				physicsProperties.ApplyGravity = false;
				physicsProperties.swingMode = true;
				physicsProperties.pullingToSwingPoint = false;
			}

			//Actively Hang
			if (physicsProperties.swingMode)
			{
				//Replenish Fuel For Flight
				currSpoolStamina = physicsProperties.maxSpoolStamina;
				physicsProperties.movementLock = true;
				physicsProperties.rb.useGravity = true;
				physicsProperties.velocity = Vector3.zero;
				physicsProperties.vel = Vector3.zero;
				physicsProperties.gravityMultiplier = 0;
			}

			//Create Spring Joint
			if (physicsProperties.swingMode && !createSwingPhysics)
			{
				springJoint = gameObject.AddComponent<SpringJoint>();
				springJoint.autoConfigureConnectedAnchor = false;
				springJoint.connectedAnchor = physicsProperties.GrappleSwingObject.position;

				//float distancefromPoint = Vector3.Distance(transform.position, physicsProperties.GrappleObject.position);

				springJoint.maxDistance = physicsProperties.GrappleSwingObject.GetComponent<GrappleSwing>().HookRange;
				springJoint.minDistance = physicsProperties.GrappleSwingObject.GetComponent<GrappleSwing>().HookRange;

				springJoint.spring = 5f; //Bounciness
				springJoint.damper = 3f; //Slows spring bounciness
				springJoint.massScale = 20f; //Weights that exceed this causes the hooked object to drop to the floor.
				print("CREATED SPRING JOINT");
				createSwingPhysics = true;
			}
			#endregion
			#region HookShot
			//HOOK SHOT

			if (!GetComponent<PlayerInteractor>().grappleSwing && Physics.Raycast(PlayerSettings.Instance.gameplaySettings.WorldCamera.transform.position, PlayerSettings.Instance.gameplaySettings.WorldCamera.transform.forward, out hit, physicsProperties.grapplePointDist, physicsProperties.grappleHoldLayer))
			{
				GetComponent<PlayerInteractor>().grapplePoint = true;
				GetComponent<PlayerInteractor>().grappleSwing = false;
			}
			else
			{
				GetComponent<PlayerInteractor>().grapplePoint = false;
				UIManager.Instance.displayGrapplePoint = true;
			}

			//Requires line of sight to grapple and hold for Grapple Points
			if (!physicsProperties.descending && CooldownState("Grapple Hook") && Physics.Raycast(PlayerSettings.Instance.gameplaySettings.WorldCamera.transform.position, PlayerSettings.Instance.gameplaySettings.WorldCamera.transform.forward, out hit, physicsProperties.grapplePointDist, physicsProperties.grappleHoldLayer) && Physics.Linecast(transform.position, hit.transform.position))
			{
				physicsProperties.inPointRange = true;
				physicsProperties.slowWingGravityMode = false;
				physicsProperties.GrapplePointObject = hit.transform;
				numberOfTotalJumpsAvaliable = 1; 

				if (!State.isStunned && !physicsProperties.swingMode && !physicsProperties.pullingToSwingPoint && !physicsProperties.pullingToGrapplePoint && !physicsProperties.holdMode && Input.GetKeyDown(key))
				{
					//Add Object To We Hit To Be Stored For Grappling too
					physicsProperties.GrapplePointObject = hit.transform;

					if (physicsProperties.GrapplePointObject.GetComponent<GrapplePoint>() == null)
					{
						print("Grapple Point Attempt Failed. No Grapple Point Script Found");
						return;
					}

					GrappleObj = physicsProperties.GrapplePointObject;
					physicsProperties.pullingToGrapplePoint = true;
					physicsProperties.rb.useGravity = false;
				}
			}
			else
			{
				physicsProperties.inPointRange = false;
			}


			Vector3 EnemyGoalPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 3.5f);

			//Move To Point Based on Grapple Speed
			if (physicsProperties.GrapplePointObject != null && physicsProperties.pullingToGrapplePoint)
			{
				UIManager.Instance.displayGrapplePointActions = true;
				IntiateCooldown("Grapple Hook");

				//Move Normally To A Point
				transform.position = Vector3.Lerp(transform.position, physicsProperties.GrapplePointObject.GetComponent<GrapplePoint>().HangPosition.position, 2.5f * physicsProperties.grappleSpeed * Time.deltaTime);
				physicsProperties.rb.velocity = Vector3.LerpUnclamped(physicsProperties.rb.velocity.normalized, Vector3.zero, Time.deltaTime);
			}

			//Pulling Self Code

			//Now lets see if we get close enough to our grapple position to know when we are too close and we can disengage with a offest margin
			if (physicsProperties.GrapplePointObject != null && physicsProperties.pullingToGrapplePoint && Vector3.Distance(transform.position, physicsProperties.GrapplePointObject.GetComponent<GrapplePoint>().HangPosition.position) <= 3f)
			{
				//Reset Jumps For some extra mobillity fun!
				numberOfTotalJumpsAvaliable = 1;

				PlayerSettings.Instance.gameplaySettings.Mode = CameraMode.GrappleMode;

				physicsProperties.pullingToGrapplePoint = false;
				physicsProperties.holdMode = true;
			}

			//Actively Hold
			if (physicsProperties.holdMode)
			{
				print("READY TO DISENGAGE");

				//Snap Position
				transform.position = physicsProperties.GrapplePointObject.GetComponent<GrapplePoint>().HangPosition.position;

				transform.SetParent(null);
				physicsProperties.movementLock = true;
				physicsProperties.ApplyGravity = false;
				physicsProperties.rb.useGravity = false;
				physicsProperties.rb.velocity = Vector3.zero;
				physicsProperties.velocity = Vector3.zero;
				physicsProperties.vel = Vector3.zero;
				physicsProperties.gravityMultiplier = 0;
			}
			#endregion
		}
	}
	float currGrappleLeapTime = 0;
	void RotaryHookDisengage()
	{
		if (Stash.Instance.unlockList.unlockRotaryHook)
		{
			KeyCode key = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.JumpIndex].key;

			//Jump Disengage Force
			if (physicsProperties.applyGrappleJumpForce)
			{
				currGrappleLeapTime = Mathf.Clamp(currGrappleLeapTime, 0, physicsProperties.grappleLeapTime);
				currGrappleLeapTime -= Time.deltaTime;
				physicsProperties.groundDetectionToggle = true;

				if (physicsProperties.Grounded || physicsProperties.isSwimming || physicsProperties.readyToWallKick)
				{
					physicsProperties.applyGrappleJumpForce = false;
					physicsProperties.rb.velocity = Vector3.zero;
				}

				if (currGrappleLeapTime <= 0)
				{
					physicsProperties.applyGrappleJumpForce = false;
				}
			}

			//Hook Range Jump Inflluence
			float HookRangeJumpBonus = 0;

			if (physicsProperties.GrappleSwingObject != null)
            {
				//Limit Hook Jump to 10
				HookRangeJumpBonus = Mathf.Clamp(HookRangeJumpBonus, 0, 10);
				HookRangeJumpBonus = physicsProperties.GrappleSwingObject.GetComponent<GrappleSwing>().HookRange;
			}

			//Disengage Swing Hook
			if (physicsProperties.swingMode && Input.GetKey(key))
			{
				physicsProperties.currentWingFlaps = physicsProperties.maximumWingFlaps;
				MyCamera.CurrZoom = 0;
				print("DISENGAGED");

				//Percentage float for multiplying the force to be stronger or less when releasing the jump depending on angle
				float jumppercentage = (transform.position.y / physicsProperties.GrappleSwingObject.position.y);
				physicsProperties.jumpHeight += (HookRangeJumpBonus/2 + physicsProperties.grappleLeapHeight) * jumppercentage;
				physicsProperties.vel.y = (HookRangeJumpBonus/2 + physicsProperties.grappleLeapHeight) * jumppercentage;
				physicsProperties.velocity.y = (HookRangeJumpBonus/2 + physicsProperties.grappleLeapHeight) * jumppercentage;

				physicsProperties.GrappleSwingObject.GetComponent<GrappleSwing>().DetachPlayer();
				currSpoolStamina = physicsProperties.maxSpoolStamina;
				numberOfTotalJumpsAvaliable = 1;

				//Add some Jump force
				physicsProperties.jumpHeight += physicsProperties.jumpforce / 2f;

				//Releasing all constraints
				physicsProperties.applyGrappleJumpForce = true;
				physicsProperties.movementLock = false;
				physicsProperties.ApplyGravity = true;
				createSwingPhysics = false;
				Destroy(springJoint);
				physicsProperties.swingMode = false;
				transform.rotation = baseRotation;
				PlayerSettings.Instance.gameplaySettings.Mode = CameraMode.UnTargetedMode;
				UIManager.Instance.clearAllDisplayActions = true;
			}

			//Forced Disengage
			if (physicsProperties.swingMode && State.isStunned)
			{
				print("DISENGAGED");
				physicsProperties.currentWingFlaps = physicsProperties.maximumWingFlaps;
				MyCamera.CurrZoom = 0;
				physicsProperties.GrappleSwingObject.GetComponent<GrappleSwing>().DetachPlayer();
				currSpoolStamina = physicsProperties.maxSpoolStamina;
				numberOfTotalJumpsAvaliable = 1;

				//Add some Jump force
				physicsProperties.jumpHeight += physicsProperties.jumpforce / 2f;

				//Releasing all constraints
				physicsProperties.movementLock = false;
				physicsProperties.ApplyGravity = true;
				createSwingPhysics = false;
				Destroy(springJoint);
				physicsProperties.swingMode = false;
				transform.rotation = baseRotation;
				PlayerSettings.Instance.gameplaySettings.Mode = CameraMode.UnTargetedMode;
			}

			//Disengage Hook
			if (physicsProperties.holdMode && Input.GetKeyDown(key))
			{
				physicsProperties.currentWingFlaps = physicsProperties.maximumWingFlaps;
				MyCamera.CurrZoom = 0;
				currSpoolStamina = physicsProperties.maxSpoolStamina;
				numberOfTotalJumpsAvaliable = 1;

				print("DISENGAGED");

				//Releasing all constraints
				transform.SetParent(null);
				physicsProperties.applyGrappleJumpForce = true;
				physicsProperties.GrapplePointObject = null;
				physicsProperties.movementLock = false;
				physicsProperties.ApplyGravity = true;
				physicsProperties.rb.useGravity = true;
				physicsProperties.rb.velocity = Vector3.zero;
				physicsProperties.velocity = Vector3.zero;
				physicsProperties.vel = Vector3.zero;
				physicsProperties.gravityMultiplier = 0;
				physicsProperties.holdMode = false;
				transform.rotation = baseRotation;
				PlayerSettings.Instance.gameplaySettings.Mode = CameraMode.UnTargetedMode;
				UIManager.Instance.clearAllDisplayActions = true;
			}

			//Forced Disengage
			if (physicsProperties.holdMode && State.isStunned)
			{
				physicsProperties.currentWingFlaps = physicsProperties.maximumWingFlaps;
				MyCamera.CurrZoom = 0;
				currSpoolStamina = physicsProperties.maxSpoolStamina;
				numberOfTotalJumpsAvaliable = 1;

				print("DISENGAGED");

				//Releasing all constraints
				transform.SetParent(null);
				physicsProperties.GrapplePointObject = null;
				physicsProperties.movementLock = false;
				physicsProperties.ApplyGravity = true;
				physicsProperties.rb.useGravity = true;
				physicsProperties.rb.velocity = Vector3.zero;
				physicsProperties.velocity = Vector3.zero;
				physicsProperties.vel = Vector3.zero;
				physicsProperties.gravityMultiplier = 0;
				physicsProperties.holdMode = false;
				transform.rotation = baseRotation;
				PlayerSettings.Instance.gameplaySettings.Mode = CameraMode.UnTargetedMode;
				UIManager.Instance.clearAllDisplayActions = true;
			}

			//Cancel For Enemies dying whilst holding on
			if (physicsProperties.holdMode && physicsProperties.GrapplePointObject == null && physicsProperties.GrappleSwingObject == null)
			{
				physicsProperties.currentWingFlaps = physicsProperties.maximumWingFlaps;
				MyCamera.CurrZoom = 0;
				currSpoolStamina = physicsProperties.maxSpoolStamina;
				numberOfTotalJumpsAvaliable = 1;

				print("DISENGAGED");

				//Releasing all constraints
				transform.SetParent(null);
				physicsProperties.GrapplePointObject = null;
				physicsProperties.movementLock = false;
				physicsProperties.ApplyGravity = true;
				physicsProperties.rb.useGravity = true;
				physicsProperties.rb.velocity = Vector3.zero;
				physicsProperties.velocity = Vector3.zero;
				physicsProperties.vel = Vector3.zero;
				physicsProperties.gravityMultiplier = 0;
				physicsProperties.holdMode = false;
				transform.rotation = baseRotation;
				PlayerSettings.Instance.gameplaySettings.Mode = CameraMode.UnTargetedMode;
				UIManager.Instance.clearAllDisplayActions = true;
			}

			//If Grapple Point Disengage
			if (physicsProperties.pullingToGrapplePoint && physicsProperties.GrapplePointObject == null)
			{
				physicsProperties.currentWingFlaps = physicsProperties.maximumWingFlaps;
				MyCamera.CurrZoom = 0;
				currSpoolStamina = physicsProperties.maxSpoolStamina;
				numberOfTotalJumpsAvaliable = 1;

				print("DISENGAGED");

				//Releasing all constraints
				transform.SetParent(null);
				physicsProperties.GrapplePointObject = null;
				physicsProperties.movementLock = false;
				physicsProperties.ApplyGravity = true;
				physicsProperties.rb.useGravity = true;
				physicsProperties.holdMode = false;
				transform.rotation = baseRotation;
				PlayerSettings.Instance.gameplaySettings.Mode = CameraMode.UnTargetedMode;
				UIManager.Instance.clearAllDisplayActions = true;
			}

			if (physicsProperties.swingMode && physicsProperties.isSwimming)
			{
				physicsProperties.currentWingFlaps = physicsProperties.maximumWingFlaps;
				MyCamera.CurrZoom = 0;
				currSpoolStamina = physicsProperties.maxSpoolStamina;
				numberOfTotalJumpsAvaliable = 1;

				print("DISENGAGED");

				//Releasing all constraints
				transform.SetParent(null);
				physicsProperties.GrapplePointObject = null;
				physicsProperties.movementLock = false;
				physicsProperties.ApplyGravity = true;
				physicsProperties.rb.useGravity = true;
				physicsProperties.rb.velocity = Vector3.zero;
				physicsProperties.velocity = Vector3.zero;
				physicsProperties.vel = Vector3.zero;
				physicsProperties.gravityMultiplier = 0;
				physicsProperties.holdMode = false;
				transform.rotation = baseRotation;
				PlayerSettings.Instance.gameplaySettings.Mode = CameraMode.UnTargetedMode;
				UIManager.Instance.clearAllDisplayActions = true;
			}
		}
	}
	void GrappleLineRender()
	{
		if (Stash.Instance.unlockList.unlockRotaryHook)
		{
			if (physicsProperties.swingMode || physicsProperties.pullingToSwingPoint || physicsProperties.pullingToGrapplePoint || physicsProperties.holdMode)
			{
				physicsProperties.grappleLine.enabled = true;
			}
			if (!physicsProperties.swingMode && !physicsProperties.pullingToSwingPoint && !physicsProperties.pullingToGrapplePoint || physicsProperties.holdMode)
			{
				physicsProperties.grappleLine.enabled = false;
			}

			if (physicsProperties.swingMode && physicsProperties.GrappleSwingObject != null || physicsProperties.pullingToSwingPoint && physicsProperties.GrappleSwingObject != null)
			{
				physicsProperties.grappleLine.SetPosition(1, physicsProperties.GrappleSwingObject.position);
				physicsProperties.grappleLine.SetPosition(0, physicsProperties.lineOrigin.position);
			}

			if (physicsProperties.pullingToGrapplePoint && physicsProperties.GrapplePointObject != null)
			{
				physicsProperties.grappleLine.SetPosition(1, physicsProperties.GrapplePointObject.position);
				physicsProperties.grappleLine.SetPosition(0, physicsProperties.lineOrigin.position);
			}
		}
	}
	Vector3 playerQ = Vector3.zero;
	Vector3 swingQ = Vector3.zero;
	void swingForceAndSpeedLimiter()
	{
		//Actively Hang Graviton Movement
		if (Stash.Instance.unlockList.unlockRotaryHook && physicsProperties.swingMode)
		{
			//Fix Rotation Scewing issue. Rotating the character changes the angle to the goal for some reason.
			Vector3 playerQ = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
			Vector3 swingQ = new Vector3(physicsProperties.GrappleSwingObject.eulerAngles.x, transform.eulerAngles.y, 0);

			KeyCode key = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.AimDownSightsIndex].key;

			if (Input.GetKey(key))
			{
				physicsProperties.rb.drag = 2f;
				physicsProperties.rb.angularDrag = 5f;
				UIManager.Instance.displayAdvancedGrappleActions = true;
			}

			if (!Input.GetKey(key))
			{
				physicsProperties.rb.drag = 0.1f;
				physicsProperties.rb.angularDrag = 0.05f;
				UIManager.Instance.displayAdvancedGrappleActions = false;
			}

			if(!Input.GetKeyUp(key))
            {
				UIManager.Instance.displayGrappleActions = true;
			}

			//Accelerate Swing
			if (!Input.GetKey(key) && physicsProperties.GrappleSwingObject != null && Quaternion.Angle(Quaternion.Euler(playerQ), Quaternion.Euler(swingQ)) < physicsProperties.maxGrappleSwingAngle)
			{
				physicsProperties.rb.AddRelativeForce(0, 0, (InputManager.Instance.PlayerInput.Vertical * CalculatePlayerMovement(physicsProperties.swingForce)));
			}
			else
			{
				if (!Input.GetKey(key) || Quaternion.Angle(Quaternion.Euler(playerQ), Quaternion.Euler(swingQ)) > physicsProperties.maxGrappleSwingAngle)
				{
					//Applies force to swing back in the opposite direction
					physicsProperties.rb.AddForce(Vector3.down * 100f, ForceMode.Acceleration);
				}
			}

			if (physicsProperties.rb.velocity.magnitude > physicsProperties.maxSwingSpeed)
			{
				physicsProperties.rb.velocity = (physicsProperties.rb.velocity.normalized * (physicsProperties.maxSwingSpeed));
			}
		}
		else
		{
			if (physicsProperties.rb.velocity.magnitude > physicsProperties.maxRigidBodySpeed)
			{
				physicsProperties.rb.velocity = (physicsProperties.rb.velocity.normalized * (physicsProperties.maxRigidBodySpeed));
			}
		}
	}
	void HookReeling()
    {
		if (Stash.Instance.unlockList.unlockRotaryHook)
		{
			KeyCode key = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.AimDownSightsIndex].key;

			if (Input.GetKey(key) && physicsProperties.swingMode && springJoint != null && !State.isStunned)
			{
				springJoint.minDistance = Mathf.Clamp(springJoint.minDistance, 1, physicsProperties.GrappleSwingObject.GetComponent<GrappleSwing>().HookRange);
				springJoint.maxDistance = Mathf.Clamp(springJoint.maxDistance, 1, physicsProperties.GrappleSwingObject.GetComponent<GrappleSwing>().HookRange);

				float reel = (InputManager.Instance.PlayerInput.Vertical * physicsProperties.grappleReelSpeed) * Time.deltaTime;

				springJoint.minDistance -= reel;
				springJoint.maxDistance -= reel;
			}
		}
	}
    //Wall Jump
    RaycastHit kickRay;
	private Vector3 pos;
	public bool enableWallSlide;
	void NotchKick()
	{
		//Actively Hang Graviton Movement
		if (Stash.Instance.unlockList.unlockNotchKick)
		{
			//Key Input
			KeyCode key = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.JumpIndex].key;
			KeyCode key1 = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.DodgeIndex].key;
			KeyCode key3 = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.GuardIndex].key;

			KeyCode keyW = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.WalkForwardIndex].key;
			KeyCode keyA = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.WalkLeftIndex].key;
			KeyCode keyS = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.WalkRightIndex].key;
			KeyCode keyD = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.WalkBackwardsIndex].key;

			if (!physicsProperties.Grounded && Physics.Raycast(transform.position, transform.forward, out kickRay, 1f, physicsProperties.wallKickLayer) || Input.GetKey(keyW) && !physicsProperties.Grounded && Physics.Raycast(transform.position, transform.forward, out kickRay, 1f, physicsProperties.wallKickLayer) || Input.GetKey(keyA) && !physicsProperties.Grounded && Physics.Raycast(transform.position, transform.forward, out kickRay, 1f, physicsProperties.wallKickLayer) || Input.GetKey(keyS) && !physicsProperties.Grounded && Physics.Raycast(transform.position, transform.forward, out kickRay, 1f, physicsProperties.wallKickLayer) || Input.GetKey(keyD) && !physicsProperties.Grounded && Physics.Raycast(transform.position, transform.forward, out kickRay, 1f, physicsProperties.wallKickLayer))
			{
				enableWallSlide = true;
			}

			if(physicsProperties.Grounded || physicsProperties.Grounded && Physics.Raycast(transform.position, transform.forward, out kickRay, 1f, physicsProperties.wallKickLayer))
            {
				enableWallSlide = false;

				if(physicsProperties.readyToWallKick)
                {
					physicsProperties.rb.useGravity = true;
					physicsProperties.ApplyGravity = true;
					physicsProperties.isWallKicking = false;
					physicsProperties.isWallHanging = false;
				}
			}

			//Check For Wall And Attach
			if (!State.isStunned && enableWallSlide && Vector3.Angle(kickRay.normal, transform.forward) >= 150 && !PlayerSettings.Instance.gameplaySettings.Mode.Equals(CameraMode.FPSMode) && !physicsProperties.isSwimming && !physicsProperties.Grounded && Physics.Raycast(transform.position, transform.forward, out kickRay, 1f, physicsProperties.wallKickLayer) && !physicsProperties.hovering && !physicsProperties.holdMode && !physicsProperties.swingMode && !physicsProperties.descending)
			{
				//Reset Jumps
				numberOfTotalJumpsAvaliable = 1;

				//Face Wall
				transform.rotation = Quaternion.LookRotation(-kickRay.normal);

				//Set Wall Slide Boolean
				physicsProperties.readyToWallKick = true;

				//Remove Speed On Wall
				physicsProperties.rb.velocity = Vector3.zero;
				physicsProperties.vel = new Vector3(0, physicsProperties.vel.y, 0);

				//Cancel Running
				physicsProperties.isRunning = false;

				//Turn Off All Gravity
				physicsProperties.vel.y = 0;
				physicsProperties.velocity.y = 0;
				physicsProperties.rb.useGravity = false;
				physicsProperties.gravityMultiplier = 0;
				physicsProperties.ApplyGravity = false;

				//Hasten Wall Slide
				if (Input.GetKey(key1) && !physicsProperties.Grounded)
				{
					physicsProperties.WallSlideSpeedAcceleration = Mathf.Clamp(physicsProperties.WallSlideSpeedAcceleration, 0, 50f);
					physicsProperties.currentMovementSpeed = CalculatePlayerMovement(physicsProperties.WallSlideSpeedAcceleration += Time.deltaTime + (physicsProperties.WallSlideSpeed));
				}
				else
				{
					physicsProperties.currentMovementSpeed = CalculatePlayerMovement(physicsProperties.WallSlideSpeed);
					physicsProperties.WallSlideSpeedAcceleration = 0;
				}

				if(Input.GetKey(key1) && physicsProperties.Grounded)
                {
					physicsProperties.currentMovementSpeed = CalculatePlayerMovement(physicsProperties.WallSlideSpeed);
					physicsProperties.WallSlideSpeedAcceleration = 0;
				}
			}
			else
			{
				if (!physicsProperties.isSwimming && physicsProperties.readyToWallKick && !Input.GetKey(key3))
				{
					physicsProperties.WallSlideSpeedAcceleration = physicsProperties.WallSlideSpeed;
					physicsProperties.readyToWallKick = false;
					physicsProperties.rb.useGravity = true;
					physicsProperties.ApplyGravity = true;
					physicsProperties.turnLock = false;
				}
			}

			//Wall Jump
			if (physicsProperties.readyToWallKick && !physicsProperties.Grounded)
			{
				if(kickRay.transform != null && Vector3.Angle(kickRay.normal, transform.forward) <= 180 && Vector3.Angle(kickRay.normal, transform.forward) > 150)
                {
					//physicsProperties.ApplyGravity = true;
					//physicsProperties.readyToWallKick = false;
					//physicsProperties.isWallHanging = false;
					//physicsProperties.turnLock = false;
				}

				//Set Position And Fall if you are not latched and pressing the guard key to Wall Hang
				if (!Input.GetKey(key3))
				{
					physicsProperties.isWallHanging = false;
					physicsProperties.turnLock = false;

					//Wall Offset Slide
					pos = new Vector3(transform.position.x, kickRay.point.y + (-physicsProperties.currentMovementSpeed * Time.deltaTime), transform.position.z);
					transform.position = pos;
				}
				else
				{
					physicsProperties.turnLock = true;

					//Must set player to be parented to a uniform object that is (1,1,1) in order to prevent skew scaling
					if (Physics.Raycast(transform.position, transform.forward, out kickRay, 1f, physicsProperties.wallKickLayer))
					{
						if (kickRay.transform.GetComponentInParent<MovingPlatform>() != null && kickRay.transform.GetComponentInParent<MovingPlatform>().UniformParent != null)
						{
							transform.SetParent(kickRay.transform.GetComponentInParent<MovingPlatform>().UniformParent);
						}

						physicsProperties.isWallHanging = true;
					}
					else
					{
						//Flags
						physicsProperties.isWallHanging = false;
					}
				}

				//Wait For Input
				if (Input.GetKey(key) && activeDeadZoneTimer == physicsProperties.jumpDeadZoneTime)
				{
					print("Wall Jumped");
					//Set Wall Kicking True
					physicsProperties.turnLock = false;
					physicsProperties.ApplyGravity = true;
					physicsProperties.isWallKicking = true;
					physicsProperties.rb.useGravity = true;
					enableWallSlide = false;

					//Face Opposite Of Wall
					transform.rotation = Quaternion.LookRotation(kickRay.normal);

					//Set Gravity To 0
					physicsProperties.gravityMultiplier = 0;
					physicsProperties.groundDetectionToggle = false;

					//Set Jump Height Off Of Wall
					physicsProperties.jumpHeight += physicsProperties.NotchKickHeight;
					physicsProperties.vel.y = physicsProperties.NotchKickHeight;
					physicsProperties.velocity.y = physicsProperties.NotchKickHeight;

					//This Requires Key Movement To Move Forward!
				}
			}

			//Detects Ceiling
			if(Physics.Raycast(transform.position,transform.up, 1f, physicsProperties.crouchLayers) && physicsProperties.isWallKicking)
            {
				physicsProperties.isWallKicking = false;
				physicsProperties.isWallHanging = false;
			}

			if (physicsProperties.readyToWallKick && physicsProperties.Grounded)
			{
				physicsProperties.readyToWallKick = false;
				physicsProperties.rb.useGravity = true;
				physicsProperties.ApplyGravity = true;
				physicsProperties.isWallKicking = false;
				physicsProperties.isWallHanging = false;
			}

			if (physicsProperties.Grounded || physicsProperties.isSwimming)
			{
				physicsProperties.isWallKicking = false;
				physicsProperties.isWallHanging = false;
			}
		}
	}
	//Wing Flap Work On This Second!
	void CardinalCowl()
	{
		if (Stash.Instance.unlockList.unlockCardinalCowl)
		{
			//Key Input
			KeyCode key = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.JumpIndex].key;
			KeyCode keyCancel = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.GuardIndex].key;

			//Active Dead Zone Time Clamp
			physicsProperties.activeWingDeadZoneTime = Mathf.Clamp(physicsProperties.activeWingDeadZoneTime, 0, physicsProperties.jumpDeadZoneTime);

			//Flap Wing
			if (Input.GetKeyDown(key) && !physicsProperties.DoubleJump && !physicsProperties.isSwimming && !physicsProperties.isWallKicking && !physicsProperties.holdMode && !physicsProperties.swingMode && !physicsProperties.pullingToGrapplePoint && !physicsProperties.pullingToSwingPoint && !Input.GetKey(keyCancel) && physicsProperties.currentWingFlaps > 0 && numberOfTotalJumpsAvaliable <= 0 && physicsProperties.wingFlapToggle && !State.isStunned)
			{
				print("Flap");
				physicsProperties.currentWingFlaps -= 1;
				physicsProperties.wingFlapToggle = false;
				physicsProperties.slowWingGravityMode = true;
				physicsProperties.applyGrappleJumpForce = false;
			}

			//Lighten Gravity Timer
			if (physicsProperties.slowWingGravityMode)
			{
				physicsProperties.rb.velocity = Vector3.zero;
				physicsProperties.wingGravitySlowCurrDuration -= Time.deltaTime;

				//Sprint
				float input = Mathf.Clamp01(InputManager.Instance.PlayerInput.MovementVector.x + InputManager.Instance.PlayerInput.MovementVector.z);

				//Direction Vector
				Vector3 Dir = new Vector3(0, 0, input);

				if (physicsProperties.MoveType.Equals(MovementType.Strafe))
				{
					//Apply Forces
					physicsProperties.rb.AddRelativeForce(physicsProperties.vel * CalculatePlayerMovement(physicsProperties.wingForce), ForceMode.Impulse);
				}

				if (physicsProperties.MoveType.Equals(MovementType.FPSMove))
				{
					//Apply Forces
					physicsProperties.rb.AddRelativeForce(physicsProperties.move * CalculatePlayerMovement(physicsProperties.wingForce), ForceMode.Impulse);
				}

				if (physicsProperties.MoveType.Equals(MovementType.FreeMove))
				{
					//Apply Forces
					physicsProperties.rb.AddRelativeForce(Dir * CalculatePlayerMovement(physicsProperties.wingForce), ForceMode.Impulse);
				}

				//Gravity Reduction
				physicsProperties.ApplyGravity = false;
				physicsProperties.rb.useGravity = false;

				if (physicsProperties.wingGravitySlowCurrDuration <= 0)
				{
					physicsProperties.ApplyGravity = true;
					physicsProperties.rb.useGravity = true;
					physicsProperties.slowWingGravityMode = false;
					physicsProperties.wingGravitySlowCurrDuration = physicsProperties.wingGravitySlowMaxDuration;
				}

				if (physicsProperties.slowWingGravityMode && physicsProperties.isWallKicking)
				{
					physicsProperties.ApplyGravity = true;
					physicsProperties.rb.useGravity = true;
					physicsProperties.slowWingGravityMode = false;
					physicsProperties.wingGravitySlowCurrDuration = physicsProperties.wingGravitySlowMaxDuration;
				}

				if (physicsProperties.descending)
				{
					physicsProperties.ApplyGravity = true;
					physicsProperties.rb.useGravity = true;
					physicsProperties.slowWingGravityMode = false;
					physicsProperties.wingGravitySlowCurrDuration = physicsProperties.wingGravitySlowMaxDuration;
				}
			}

			//Dead Zone Timer
			if (!physicsProperties.wingFlapToggle)
			{
				physicsProperties.activeWingDeadZoneTime -= Time.deltaTime;

				physicsProperties.wingGravitySlowCurrDuration = physicsProperties.wingGravitySlowMaxDuration;
				physicsProperties.jumpHeight += physicsProperties.wingFlapHeight;
				physicsProperties.vel.y += physicsProperties.wingFlapHeight;
				physicsProperties.velocity.y += physicsProperties.wingFlapHeight;

				if (physicsProperties.activeWingDeadZoneTime <= 0)
				{
					physicsProperties.wingFlapToggle = true;
					physicsProperties.activeWingDeadZoneTime = physicsProperties.wingDeadZoneTime;
				}
			}

			//Reset And Canceled
			if (physicsProperties.Grounded || Input.GetKey(keyCancel) || physicsProperties.isSwimming || physicsProperties.isWallKicking || physicsProperties.slowWingGravityMode && physicsProperties.pullingToGrapplePoint || physicsProperties.slowWingGravityMode && physicsProperties.pullingToSwingPoint)
			{
				physicsProperties.currentWingFlaps = physicsProperties.maximumWingFlaps;
				physicsProperties.activeWingDeadZoneTime = 0;
				physicsProperties.wingGravitySlowCurrDuration = physicsProperties.wingGravitySlowMaxDuration;
				physicsProperties.activeWingDeadZoneTime = physicsProperties.wingDeadZoneTime;
			}
		}
	} //Curtain?????
	private RaycastHit crouchRay;
	private bool toggledCrouch = false;
	void Crawling()
    {
		//Get Crouch Key Input
		if (InputManager.Instance != null)
		{
			KeyCode Crouch = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.CrouchIndex].key;

			//When on the ground, not dashing and not stunned
			if (!physicsProperties.isSwimming && physicsProperties.Grounded && !physicsProperties.dashing && !State.isStunned)
			{
				//If Key is pressed while previous conditions are met
				if (p_Settings.gameplaySettings.CrouchType.Equals(CrouchMode.Hold))
				{
					if (Input.GetKey(Crouch))
					{
						//crouch state is true
						physicsProperties.isCrouching = true;
					}
					else
					{
						//releasing crouch while still under an object will not allow you to stop crouching regardless of input
						if (physicsProperties.isCrouching && !Physics.Raycast(transform.position, transform.up, out crouchRay, physicsProperties.crouchDistCheck, physicsProperties.crouchLayers) || !physicsProperties.Grounded || physicsProperties.isSwimming)
						{
							//crouch state is false
							physicsProperties.isCrouching = false;
						}
					}
				}

				if (p_Settings.gameplaySettings.CrouchType.Equals(CrouchMode.Toggle))
				{
					if (Input.GetKey(Crouch) && !toggledCrouch)
					{
						//crouch state is true
						physicsProperties.isCrouching = !physicsProperties.isCrouching;
						toggledCrouch = true;
					}

					if (!Input.GetKey(Crouch))
					{
						toggledCrouch = false;
					}

					if (!physicsProperties.Grounded || physicsProperties.isSwimming)
					{
						//crouch state is false
						physicsProperties.isCrouching = false;
					}

					if (Input.GetKeyDown(Crouch) && physicsProperties.isCrouching && !Physics.Raycast(transform.position, transform.up, out crouchRay, physicsProperties.crouchDistCheck, physicsProperties.crouchLayers))
					{
						physicsProperties.isCrouching = true;
					}
				}
			}
		}

		//Crouch State reduces collider size to go under objects
		if(physicsProperties.isCrouching)
        {
			physicsProperties.currentMovementSpeed = CalculatePlayerMovement(physicsProperties.crouchMovementSpeed);

			physicsProperties.CapsuleCol.GetComponent<CapsuleCollider>().center = new Vector3(0, -0.5f, 0);
			physicsProperties.CapsuleCol.GetComponent<CapsuleCollider>().radius = 0.5f;
			physicsProperties.CapsuleCol.GetComponent<CapsuleCollider>().height = 1;
		}
		//Returns it to normal
		else
        {
			physicsProperties.CapsuleCol.GetComponent<CapsuleCollider>().center = new Vector3(0, 0, 0);
			physicsProperties.CapsuleCol.GetComponent<CapsuleCollider>().radius = 0.5f;
			physicsProperties.CapsuleCol.GetComponent<CapsuleCollider>().height = 2;
		}
	}
	public float CalculatePlayerMovement(float baseSpeed)
    {
		float speedCalc = baseSpeed + (((baseSpeed * Stats.BuffSheet.MovementSpeedBonus) + (baseSpeed * Stats.PlayerSheet.MovementSpeedBonus)) * Stats.DebuffSheet.MovementSpeedReduction);
		return speedCalc;
    }
	//Work On This Third
	void DirectionalTurning()
    {
		//Align Player to floor
		//transform.rotation = Quaternion.AngleAxis(gChecker.groundSlopeAngle, transform.right);

		if (physicsProperties.MoveType.Equals(MovementType.FPSMove) && !physicsProperties.turnLock && !physicsProperties.swingMode && !physicsProperties.pullingToGrapplePoint && !physicsProperties.pullingToSwingPoint && !physicsProperties.holdMode && !physicsProperties.swingMode && !State.isStunned)
        {
			//Rotate Player
			transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * PlayerSettings.Instance.gameplaySettings.sensitivity * Time.deltaTime);
		}
		
		if (physicsProperties.MoveType.Equals(MovementType.FreeMove) && !physicsProperties.turnLock && !State.isStunned)
		{
			//Input Vector
			Vector3 InputDirection = new Vector3(InputManager.Instance.PlayerInput.Horizontal, InputManager.Instance.PlayerInput.Vertical, 0);

			//Directional Input [Straight]
			KeyCode key0 = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.WalkForwardIndex].key;
			KeyCode key1 = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.WalkBackwardsIndex].key;
			KeyCode key2 = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.WalkLeftIndex].key;
			KeyCode key3 = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.WalkRightIndex].key;

			//Angles
			if (MyCamera != null)
			{
				Vector3 lookrotforward = new Vector3(MyCamera.transform.forward.x, MyCamera.transform.forward.y, MyCamera.transform.forward.z);
				Vector3 lookrotcardinalforwardright = new Vector3((MyCamera.transform.forward.x + MyCamera.transform.right.x) / 2f, (MyCamera.transform.forward.y + MyCamera.transform.right.y) / 2f, (MyCamera.transform.forward.z + MyCamera.transform.right.z) / 2f);
				Vector3 lookrotcardinalforwardleft = new Vector3((-MyCamera.transform.forward.x + MyCamera.transform.right.x) / 2f, (-MyCamera.transform.forward.y + -MyCamera.transform.right.y) / 2f, (-MyCamera.transform.forward.z + MyCamera.transform.right.z) / 2f);
				Vector3 lookrotsides = new Vector3(MyCamera.transform.right.x, MyCamera.transform.right.y, MyCamera.transform.right.z);
				Vector3 lookrotcardinalbackright = new Vector3((-MyCamera.transform.forward.x + MyCamera.transform.right.x) / 2f, (-MyCamera.transform.forward.y + MyCamera.transform.right.y) / 2f, (-MyCamera.transform.forward.z + MyCamera.transform.right.z) / 2f);
				Vector3 lookrotcardinalbackleft = new Vector3((MyCamera.transform.forward.x + MyCamera.transform.right.x) / 2f, (MyCamera.transform.forward.y + MyCamera.transform.right.y) / 2f, (MyCamera.transform.forward.z + MyCamera.transform.right.z) / 2f);


				//Not Grappling
				if (!physicsProperties.swingMode)
				{
					//Forward
					if (Input.GetKey(key0) && !Input.GetKey(key2) && !Input.GetKey(key3) && !physicsProperties.isWallKicking)
					{
						//Set Direction To Be Camera Driven
						Quaternion Angle = Quaternion.Euler(0, 0, 0);
						lookrotforward.y = 0;
						transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookrotforward), physicsProperties.directionLookSpeed * Time.deltaTime);
					}

					//Back
					if (Input.GetKey(key1) && !Input.GetKey(key2) && !Input.GetKey(key3) && !physicsProperties.isWallKicking)
					{
						//Set Direction To Be Camera Driven
						Quaternion Angle = Quaternion.Euler(new Vector3(0, 180, 0));
						lookrotforward.y = 0;
						transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-lookrotforward), physicsProperties.directionLookSpeed * Time.deltaTime);
					}

					//Left
					if (Input.GetKey(key2) && !Input.GetKey(key0) && !Input.GetKey(key1) && !physicsProperties.isWallKicking)
					{
						//Set Direction To Be Camera Driven
						Quaternion Angle = Quaternion.Euler(new Vector3(0, -90, 0));
						lookrotsides.y = 0;
						transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-lookrotsides), physicsProperties.directionLookSpeed * Time.deltaTime);
					}

					//Right
					if (Input.GetKey(key3) && !Input.GetKey(key0) && !Input.GetKey(key1) && !physicsProperties.isWallKicking)
					{
						//Set Direction To Be Camera Driven
						Quaternion Angle = Quaternion.Euler(new Vector3(0, 90, 0));
						lookrotsides.y = 0;
						transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookrotsides), physicsProperties.directionLookSpeed * Time.deltaTime);
					}

					//Cardinal Inputs [Diagnoal]
					//Fix Cardinal issue
					//Forward Right
					if (Input.GetKey(key0) && Input.GetKey(key3) && !physicsProperties.isWallKicking)
					{
						//Set Direction To Be Camera Driven
						Quaternion Angle = Quaternion.Euler(new Vector3(0, 35, 0));
						Quaternion FirstAngle = Quaternion.LookRotation(new Vector3(MyCamera.Rotater.right.x, MyCamera.Rotater.right.y, MyCamera.Rotater.right.z));
						Quaternion SecondAngle = Quaternion.LookRotation(new Vector3(MyCamera.Rotater.forward.x, MyCamera.Rotater.forward.y, MyCamera.Rotater.forward.z));

						FirstAngle.x = 0;
						SecondAngle.x = 0;
						FirstAngle.z = 0;
						SecondAngle.z = 0;

						transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Slerp(FirstAngle, SecondAngle, 0.5f), physicsProperties.directionLookSpeed * Time.deltaTime);
					}

					//Forward Left
					if (Input.GetKey(key0) && Input.GetKey(key2) && !physicsProperties.isWallKicking)
					{
						//Set Direction To Be Camera Driven
						Quaternion Angle = Quaternion.Euler(new Vector3(0, -35, 0));
						Quaternion FirstAngle = Quaternion.LookRotation(new Vector3(-MyCamera.Rotater.right.x, -MyCamera.Rotater.right.y, -MyCamera.Rotater.right.z));
						Quaternion SecondAngle = Quaternion.LookRotation(new Vector3(MyCamera.Rotater.forward.x, MyCamera.Rotater.forward.y, MyCamera.Rotater.forward.z));

						FirstAngle.x = 0;
						SecondAngle.x = 0;
						FirstAngle.z = 0;
						SecondAngle.z = 0;

						transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Slerp(FirstAngle, SecondAngle, 0.5f), physicsProperties.directionLookSpeed * Time.deltaTime);
					}

					//Backward Right
					if (Input.GetKey(key1) && Input.GetKey(key3) && !physicsProperties.isWallKicking)
					{
						//Set Direction To Be Camera Driven
						Quaternion Angle = Quaternion.Euler(new Vector3(0, 135, 0));
						Quaternion FirstAngle = Quaternion.LookRotation(new Vector3(MyCamera.Rotater.right.x, MyCamera.Rotater.right.y, MyCamera.Rotater.right.z));
						Quaternion SecondAngle = Quaternion.LookRotation(new Vector3(-MyCamera.Rotater.forward.x, -MyCamera.Rotater.forward.y, -MyCamera.Rotater.forward.z));

						FirstAngle.x = 0;
						SecondAngle.x = 0;
						FirstAngle.z = 0;
						SecondAngle.z = 0;

						transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Slerp(FirstAngle, SecondAngle, 0.5f), physicsProperties.directionLookSpeed * Time.deltaTime);
					}

					//Backward Left
					if (Input.GetKey(key1) && Input.GetKey(key2) && !physicsProperties.isWallKicking)
					{
						//Set Direction To Be Camera Driven
						Quaternion Angle = Quaternion.Euler(new Vector3(0, -135, 0));
						Quaternion FirstAngle = Quaternion.LookRotation(new Vector3(-MyCamera.Rotater.right.x, -MyCamera.Rotater.right.y, -MyCamera.Rotater.right.z));
						Quaternion SecondAngle = Quaternion.LookRotation(new Vector3(-MyCamera.Rotater.forward.x, -MyCamera.Rotater.forward.y, -MyCamera.Rotater.forward.z));

						FirstAngle.x = 0;
						SecondAngle.x = 0;
						FirstAngle.z = 0;
						SecondAngle.z = 0;

						transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Slerp(FirstAngle, SecondAngle, 0.5f), physicsProperties.directionLookSpeed * Time.deltaTime);
					}
				}
			}
		}


		if(physicsProperties.swingMode)
        {
			currGrappleLeapTime = physicsProperties.grappleLeapTime;

			Quaternion q = Quaternion.FromToRotation(transform.up, transform.forward) * Quaternion.LookRotation(physicsProperties.GrappleSwingObject.position - transform.position, transform.up);
			Vector3 angles = new Vector3(q.eulerAngles.x, transform.eulerAngles.y, 0);
			transform.rotation = Quaternion.Euler(angles);

			KeyCode key = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.AimDownSightsIndex].key;

			if (Input.GetKey(key) && physicsProperties.physicalSpeed.magnitude < 2)
			{
				Vector3 Goal = new Vector3(transform.eulerAngles.x, MyCamera.Rotater.eulerAngles.y, 0);
				Quaternion Rot = Quaternion.Euler(Goal);
				transform.rotation = Rot;
			}
		}

		//Orients Player To Strafe Around Target
		if (physicsProperties.MoveType.Equals(MovementType.Strafe))
        {
			if (MyCamera.LockOnTarget != null)
			{
				Vector3 lookDir = -transform.position - -MyCamera.LockOnTarget.transform.position;
				lookDir.y = 0;

				Quaternion q = Quaternion.LookRotation(lookDir);

				if (Quaternion.Angle(q, baseRotation) <= 180)
				{
					targetRotation = q;
				}

				transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, physicsProperties.directionLookSpeed * (Time.deltaTime / Time.timeScale));
			}
        }
    }
	public void SetMoveType(MovementType Type)
    {
		physicsProperties.MoveType = Type;
    }

	#region Depercated
	/*void HyperThreadingLogic()
	{
		//Key Input
		KeyCode key = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.HyperThreadIndex].key;

		if (Input.GetKeyDown(key) && !physicsProperties.swingMode && !physicsProperties.holdMode && !physicsProperties.pullingToGrapplePoint && !physicsProperties.pullingToSwingPoint)
		{
			HyperThreadingToggle(physicsProperties.isHyperThreading);
		}

		HyperThreading();
	}
	//Slow Time And Increase Your Speed
	void HyperThreadingToggle(bool toggle)
	{
		if (toggle)
		{
			physicsProperties.isHyperThreading = !physicsProperties.isHyperThreading;
		}
		else
		{
			physicsProperties.isHyperThreading = !physicsProperties.isHyperThreading;
		}
	}
	void HyperThreading()
	{
		if (physicsProperties.isHyperThreading && Stats.PlayerSheet.CurrentTension < Stats.PlayerSheet.TensionLimit)
		{
			Time.timeScale = physicsProperties.slowdownFactor;
			Time.fixedDeltaTime = Time.timeScale * 0.02f;

			//Increase Tension
			//Stats.PlayerSheet.CurrentTension += Time.deltaTime * physicsProperties.HyperThreadTensionConsumption;

			//Add Speed Buff Effect
			StateSetter.SendBuff(physicsProperties.HyperThreadSpeedBoost, State, Stats);
		}
		else
		{
			//1.5f represents the duration to smooth out slow to realtime speed
			Time.timeScale += (1f / 0.5f) * Time.unscaledDeltaTime;
			Time.timeScale = Mathf.Clamp(Time.timeScale, 0, 1f);
			Time.fixedDeltaTime = 0.02f;
		}

		//Disengage HyperThread Mode For Grapple Action
		if (physicsProperties.isHyperThreading && physicsProperties.swingMode || physicsProperties.isHyperThreading && physicsProperties.holdMode || physicsProperties.isHyperThreading && physicsProperties.pullingToGrapplePoint || physicsProperties.isHyperThreading && physicsProperties.pullingToSwingPoint)
		{
			physicsProperties.isHyperThreading = false;
		}

		if (physicsProperties.isHyperThreading && Stats.PlayerSheet.CurrentTension >= Stats.PlayerSheet.TensionLimit)
		{
			physicsProperties.isHyperThreading = false;
		}
	}*/
#endregion
}