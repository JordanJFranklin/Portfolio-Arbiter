using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSettings;
using InputKeys;
using DialogueClass;

public class PlayerCamera : MonoBehaviour
{
    [Header("Basic Settings")]
    public LayerMask Wall;
    private float YHeight;
    public float PlatformingMinHeight;
    public float PlatformingMaxHeight;
    public float XAngle;
    public float MinYAngle = -180;
    public float MaxYAngle = 180;
    public float CurrZoom = 0;
    public float ZoomMax = 5;
    public float ZoomMin = -5;
    public float ZoomSpeed = 2f;
    public float PlatformingCameraSpeed = 1.5f;
    public float PlatformingRotaterSpeed = 2f;
    public float PlatformingDirectionSmoothing = 2f;
    public float PlatformingLookSmoothing = 2f;

    [Header("Grapple Camera")]
    public Vector3 GrapplePos;
    public float maxGrappleDist = 2.5f;
    public Transform GrapplePoint;

    [Header("Lock On Settings")]
    public bool keyPressed = false;
    public bool isLockedOn;
    public GameObject GameCamera;
    public GameObject LockOnTarget;
    public float LockOnTimerAutoAdjust;
    public float LockOnCameraSpeed;
    public float LockOnRotationSpeed;
    public float LockOnDistance;
    public float LockOnDistanceCancelLimit;

    [Header("Dialogue Settings")]
    public Transform GeneralLook;
    public CameraDialogueData CameraData;

    [Header("Position Offset Settings")]
    public Vector3 PlatformingPos;
    public Vector3 PlatformingRotaterPos;
    public Vector3 LockOnCameraPos;
    public Vector3 LockOnAdjustmentPos;
    public Vector3 CameraFPSPos;

    Vector3 heightVector;
    float pivotHeight;
    public Transform Player;
    public Transform Rotater;
    private WorldTimeScaler WorldTimer;
    private float timeAdjust = 0;
    public bool AdjustCamera = false;
    public bool CancelHook = false;
    private bool forceCameraReset = false;
    private bool cancelLockOn = false;
    

    // Start is called before the first frame update
    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        timeAdjust = LockOnTimerAutoAdjust;
        WorldTimer = GetComponent<WorldTimeScaler>();
        baseRotation = transform.rotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CameraModes();
    }

    //Camera Motor
    private void CameraModes()
    {
        if (PlayerSettings.Instance.gameplaySettings.Mode.Equals(CameraMode.UnTargetedMode))
        {
            UnTargetedMode();
            PreTargetMode();
        }

        if (PlayerSettings.Instance.gameplaySettings.Mode.Equals(CameraMode.TargetMode))
        {
            TargetMode();
            TargetedAutoAdjust();
        }
        else
        {
            UIManager.Instance.displayLockCameraActions = false;
        }

        if(PlayerSettings.Instance.gameplaySettings.Mode.Equals(CameraMode.GrappleMode))
        {
            GrappleCamera();
        }

        if(cancelLockOn)
        {
            CancelLockOn();
        }

        if (PlayerSettings.Instance.gameplaySettings.Mode.Equals(CameraMode.FPSMode))
        {
            FirstPersonMode();
        }

        if(PlayerSettings.Instance.gameplaySettings.Mode.Equals(CameraMode.DialogueMode))
        {
            DialougeCamera();
        }
    }

    private void GrappleCamera()
    {
        if (Player.gameObject.GetComponent<PlayerDriver>().physicsProperties.GrappleSwingObject && !UIManager.Instance.isPaused || Player.gameObject.GetComponent<PlayerDriver>().physicsProperties.GrapplePointObject && !UIManager.Instance.isPaused)
        {
            if (Player.gameObject.GetComponent<PlayerDriver>().physicsProperties.GrappleSwingObject != null && Player.gameObject.GetComponent<PlayerDriver>().physicsProperties.swingMode || Player.gameObject.GetComponent<PlayerDriver>().physicsProperties.GrappleSwingObject != null && Player.gameObject.GetComponent<PlayerDriver>().physicsProperties.pullingToSwingPoint)
            {
                GrapplePoint = Player.gameObject.GetComponent<PlayerDriver>().physicsProperties.GrappleSwingObject;
            }

            if(Player.gameObject.GetComponent<PlayerDriver>().physicsProperties.GrapplePointObject != null && Player.gameObject.GetComponent<PlayerDriver>().physicsProperties.holdMode || Player.gameObject.GetComponent<PlayerDriver>().physicsProperties.GrappleSwingObject != null && Player.gameObject.GetComponent<PlayerDriver>().physicsProperties.pullingToGrapplePoint)
            {
                GrapplePoint = Player.gameObject.GetComponent<PlayerDriver>().physicsProperties.GrapplePointObject;
            }

            forceCameraReset = false;

            //Player Movement Mode
            Player.GetComponent<PlayerDriver>().SetMoveType(MovementType.FPSMove);

            //Parenting
            Rotater.SetParent(null);
            transform.SetParent(Rotater);
            GeneralLook.SetParent(transform);
            GeneralLook.position = transform.position;

            float dist = -DistanceCalculation(Player, GrapplePoint.transform) * 0.2f;

            if(dist < -maxGrappleDist)
            {
                dist = -maxGrappleDist;
            }

            //Vectors
            Vector3 DistanceOffset = new Vector3(0, 0, dist);
            Vector3 CollisionDistanceOffset = new Vector3(0, 0, -DistanceCalculation(Player, Rotater) * 0.4f);

            //Rotater Camera Position
            Rotater.position = (Player.position + GrapplePoint.position) / 2f;

            //Mouse Movement Axis
            if (PlayerSettings.Instance.gameplaySettings.InvertY)
            {
                YHeight += -(Input.GetAxis("Mouse Y") * PlayerSettings.Instance.gameplaySettings.sensitivity) * Time.deltaTime;
            }
            else
            {
                YHeight += (Input.GetAxis("Mouse Y") * PlayerSettings.Instance.gameplaySettings.sensitivity) * Time.deltaTime;
            }

            if (PlayerSettings.Instance.gameplaySettings.InvertX)
            {
                //Rotate The Orbit Point
                //Rotater.Rotate(Vector3.up, -(Input.GetAxis("Mouse X") * PlayerSettings.Instance.gameplaySettings.sensitivity) * Time.deltaTime);
                XAngle += -(Input.GetAxis("Mouse X") * PlayerSettings.Instance.gameplaySettings.sensitivity) * Time.deltaTime;
            }
            else
            {
                //Rotate The Orbit Point
                //Rotater.Rotate(Vector3.up, (Input.GetAxis("Mouse X") * PlayerSettings.Instance.gameplaySettings.sensitivity) * Time.deltaTime);
                XAngle += (Input.GetAxis("Mouse X") * PlayerSettings.Instance.gameplaySettings.sensitivity) * Time.deltaTime;
            }

            //Clamp Up/Down Angle
            YHeight = Mathf.Clamp(YHeight, MinYAngle, MaxYAngle);

            //Rotate The Orbit Point
            Quaternion angle = Quaternion.Euler(new Vector3(YHeight, XAngle, Player.eulerAngles.z));
            Rotater.rotation = Quaternion.Lerp(Rotater.rotation, angle, (2 * (Time.deltaTime / Time.timeScale)) * LockOnRotationSpeed);

            //Create Camera Position
            //DistanceOffset + LockOnCameraPos
            transform.localPosition = Vector3.Lerp(transform.localPosition, DistanceOffset + LockOnCameraPos, (2 * (Time.deltaTime / Time.timeScale)) * LockOnCameraSpeed);

            //Create Camera Rotation
            transform.rotation = Quaternion.LookRotation(Rotater.position - transform.position);

            //Create Camera Collision Correction
            if (Physics.Linecast(transform.position, Rotater.position, out hit, Wall))
            {
                print("Obstruction To Camera");

                if (hit.collider.CompareTag("Wall"))
                {
                    //Corrective Position
                    CollisionDistanceOffset.y = hit.point.y;
                    Vector3 LockOnCameraPosAdjust = LockOnCameraPos;
                    LockOnCameraPosAdjust.y = hit.point.y;
                    GameCamera.transform.position = Vector3.Lerp(GameCamera.transform.position, hit.point + transform.TransformDirection(CollisionDistanceOffset) + transform.TransformDirection(LockOnCameraPos), (2 * (Time.deltaTime / Time.timeScale)) * LockOnCameraSpeed);
                    //cancelLockOn = true;
                }
            }
            else
            {
                //Move Camera To Default Position
                GameCamera.transform.localPosition = Vector3.Lerp(GameCamera.transform.localPosition, transform.localPosition, (2 * (Time.deltaTime / Time.timeScale)) * LockOnCameraSpeed);
            }
        }
    }

    //Create Dialogue Camera 
    //Look Into Camera Transitions experimental
    public void AdvanceDialougeCamera(CameraDialoguePositions CameraShot)
    {
        if (CameraData.DialoguePoints != null)
        {
            CameraData.DialoguePoints.Clear();
            CameraData.DialoguePoints.Add(CameraShot);
            Player.GetComponent<PlayerDriver>().physicsProperties.movementLock = true;
            CameraData.DialoguePoints[0] = CameraShot;
            PlayerSettings.Instance.gameplaySettings.Mode = CameraMode.DialogueMode;
        }
        else
        {
            Debug.LogError("Dialogue Camera Shot is empty.");
        }
    }

    public void EndDialogueCamera()
    {
        Player.GetComponent<PlayerDriver>().physicsProperties.movementLock = false;
        isLockedOn = false;
        GameCamera.GetComponent<Animation>().enabled = false;
        CameraData.DialoguePoints.Clear();
        PlayerSettings.Instance.gameplaySettings.Mode = CameraMode.UnTargetedMode;
        print("Returning Camera To Normal. Conversation Ended");
    }

    private Quaternion baseRotation;
    private Quaternion targetRotation;
    private void DialougeCamera()
    {
        //Look Point Parenting
        GeneralLook.SetParent(null);

        GameCamera.transform.rotation = transform.rotation;
        GameCamera.transform.position = transform.position;

        if (CameraData.DialoguePoints.Count > 0 && CameraData.DialoguePoints[0] != null)
        {
            if(CameraData.DialoguePoints[0].LocalizedParent != null && CameraData.DialoguePoints[0].PositionType.Equals(DialogueCameraPositionType.Local))
            {
                Rotater.SetParent(CameraData.DialoguePoints[0].LocalizedParent);
                transform.SetParent(Rotater);
            }
            else
            {
                Rotater.SetParent(Player);
                transform.SetParent(null);
            }
            
            //Animation
            if(GameCamera.GetComponent<Animation>() != null)
            {
                GameCamera.GetComponent<Animation>().enabled = true;

                if (CameraData.DialoguePoints[0].Animation != null)
                {
                    CameraData.DialoguePoints[0].Animation.legacy = true;
                    GameCamera.GetComponent<Animation>().AddClip(CameraData.DialoguePoints[0].Animation, "Camera Animation");
                    GameCamera.GetComponent<Animation>().Play("Camera Animation");
                }
            }
            
            //FOV Handling
            if (CameraData.DialoguePoints[0].FOVType.Equals(DialogueCameraFOVType.Default))
            {
                GameCamera.GetComponent<Camera>().fieldOfView = PlayerSettings.Instance.gameplaySettings.FieldOfView;
            }

            if (CameraData.DialoguePoints[0].FOVType.Equals(DialogueCameraFOVType.Static))
            {
                GameCamera.GetComponent<Camera>().fieldOfView = CameraData.DialoguePoints[0].CameraFOV;
            }

            if (CameraData.DialoguePoints[0].FOVType.Equals(DialogueCameraFOVType.Dynamic))
            {
                GameCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(GameCamera.GetComponent<Camera>().fieldOfView, CameraData.DialoguePoints[0].CameraFOV, Time.deltaTime * CameraData.DialoguePoints[0].CameraFOVZoomSpeed);
            }

            //Face Player To Speaker Or Direction
            //Player Look
            if (CameraData.DialoguePoints[0].Speakers[0] != null && CameraData.DialoguePoints[0].Speakers.Count == 1)
            {
                Vector3 lookDir = -Player.position - -CameraData.DialoguePoints[0].Speakers[0].transform.position;
                lookDir.y = 0;

                Quaternion q = Quaternion.LookRotation(lookDir);

                if (Quaternion.Angle(q, baseRotation) <= 180)
                {
                    targetRotation = q;
                }

                Player.rotation = Quaternion.Slerp(Player.rotation, targetRotation, 2 * Time.deltaTime);
            }

            if (CameraData.DialoguePoints[0].Speakers != null && CameraData.DialoguePoints[0].Speakers.Count > 1)
            {
                Vector3 lookDir = -Player.position - -GeneralLook.transform.position;
                lookDir.y = 0;

                Quaternion q = Quaternion.LookRotation(lookDir);

                if (Quaternion.Angle(q, baseRotation) <= 180)
                {
                    targetRotation = q;
                }

                Player.rotation = Quaternion.Slerp(Player.rotation, targetRotation, 2 * Time.deltaTime);
            }

            //Set Camera Position
            #region Position
            if (CameraData.DialoguePoints[0].MovementType.Equals(DialogueCameraMovementType.Instant))
            {
                if (CameraData.DialoguePoints[0].PositionType.Equals(DialogueCameraPositionType.World))
                {
                    transform.position = CameraData.DialoguePoints[0].Position;
                    transform.SetParent(null);
                }

                if (CameraData.DialoguePoints[0].PositionType.Equals(DialogueCameraPositionType.Local) && CameraData.DialoguePoints[0].LocalizedParent != null)
                {
                    transform.SetParent(CameraData.DialoguePoints[0].LocalizedParent);
                    transform.position = CameraData.DialoguePoints[0].LocalizedParent.position + CameraData.DialoguePoints[0].Position;
                }
            }

            if (CameraData.DialoguePoints[0].MovementType.Equals(DialogueCameraMovementType.Interpolated))
            {
                if (CameraData.DialoguePoints[0].PositionType.Equals(DialogueCameraPositionType.World))
                {
                    transform.position = Vector3.Lerp(transform.position, CameraData.DialoguePoints[0].Position, Time.deltaTime * CameraData.DialoguePoints[0].PosLerpSpeed);
                    transform.SetParent(null);
                }

                if (CameraData.DialoguePoints[0].PositionType.Equals(DialogueCameraPositionType.Local) && CameraData.DialoguePoints[0].LocalizedParent != null)
                {
                    transform.position = Vector3.Lerp(transform.position, CameraData.DialoguePoints[0].LocalizedParent.position + CameraData.DialoguePoints[0].Position, Time.deltaTime * CameraData.DialoguePoints[0].PosLerpSpeed);
                    transform.SetParent(CameraData.DialoguePoints[0].LocalizedParent);
                }
            }
            #endregion
            //Set Camera Rotation
            #region Rotation
            if (CameraData.DialoguePoints[0].RotationStyle.Equals(DialogueCameraRotationStyle.LookToSpeakers))
            {
                if (CameraData.DialoguePoints[0].RotationType.Equals(DialogueCameraRotationType.Instant))
                {
                    //If There is only one speaker look at that one person
                    if (CameraData.DialoguePoints[0].Speakers[0] != null && CameraData.DialoguePoints[0].Speakers.Count == 1)
                    {
                        transform.LookAt(CameraData.DialoguePoints[0].Speakers[0].transform);
                    }

                    //if There is more then one speaker to look at then get the midpoint of all the speakers and look at the center of them all instead
                    if (CameraData.DialoguePoints[0].Speakers != null && CameraData.DialoguePoints[0].Speakers.Count > 1)
                    {
                        Vector3 MidPoint = Vector3.zero;

                        foreach (GameObject Speaker in CameraData.DialoguePoints[0].Speakers)
                        {
                            MidPoint += Speaker.transform.position;
                        }

                        transform.LookAt(GeneralLook);
                    }
                }

                if (CameraData.DialoguePoints[0].RotationType.Equals(DialogueCameraRotationType.Interpolated))
                {
                    //If There is only one speaker look at that one person
                    if (CameraData.DialoguePoints[0].Speakers[0] != null && CameraData.DialoguePoints[0].Speakers.Count == 1)
                    {
                        //Goal Angle To Reset Too
                        Quaternion angle = Quaternion.LookRotation(CameraData.DialoguePoints[0].Speakers[0].transform.position - transform.position);
                        transform.rotation = Quaternion.Slerp(transform.rotation, angle, Time.deltaTime * CameraData.DialoguePoints[0].RotLerpSpeed);
                    }

                    //if There is more then one speaker to look at then get the midpoint of all the speakers and look at the center of them all instead
                    if (CameraData.DialoguePoints[0].Speakers != null && CameraData.DialoguePoints[0].Speakers.Count > 1)
                    {
                        Vector3 MidPoint = Vector3.zero;

                        foreach (GameObject Speaker in CameraData.DialoguePoints[0].Speakers)
                        {
                            MidPoint += Speaker.transform.position;
                        }

                        GeneralLook.position = (Player.position + MidPoint) / 2f;

                        Quaternion angle = Quaternion.LookRotation(GeneralLook.position - transform.position);
                        transform.rotation = Quaternion.Slerp(transform.rotation, angle, Time.deltaTime * CameraData.DialoguePoints[0].RotLerpSpeed);
                    }
                }
            }

            if (CameraData.DialoguePoints[0].RotationStyle.Equals(DialogueCameraRotationStyle.Given))
            {
                if (CameraData.DialoguePoints[0].RotationType.Equals(DialogueCameraRotationType.Instant))
                {
                    transform.rotation = CameraData.DialoguePoints[0].GivenLookDirection;
                }

                if (CameraData.DialoguePoints[0].RotationType.Equals(DialogueCameraRotationType.Interpolated))
                {
                    //Look In Direction
                    transform.rotation = Quaternion.Slerp(transform.rotation, CameraData.DialoguePoints[0].GivenLookDirection, Time.deltaTime * CameraData.DialoguePoints[0].RotLerpSpeed);
                }
            }
            #endregion
        }
    }

    //Will be completely viable to fight melee and ranged in but more important for ranged weapons/skills/attacks. Easier to aim beam slashes, etc.
    //Bows, Telescopes, Look Around, etc
    private void FirstPersonMode()
    {
        XAngle = 0;

        if (!UIManager.Instance.isPaused)
        {
            //Player Movement Mode
            Player.GetComponent<PlayerDriver>().SetMoveType(MovementType.FPSMove);

            //Parenting
            Rotater.SetParent(Player);
            transform.SetParent(Rotater);

            //Rotater Position
            Rotater.position = Player.position + Rotater.TransformDirection(CameraFPSPos);

            //Mouse Movement Axis
            YHeight += -Input.GetAxis("Mouse Y") * PlayerSettings.Instance.gameplaySettings.sensitivity * Time.deltaTime;

            //Clamp Up/Down Angle
            YHeight = Mathf.Clamp(YHeight, MinYAngle, MaxYAngle);

            //Rotater Rotation
            Quaternion angle = Quaternion.Euler(new Vector3(YHeight, Player.eulerAngles.y, Player.eulerAngles.z));
            Rotater.rotation = Quaternion.Lerp(Rotater.rotation, angle, (2 * (Time.deltaTime / Time.timeScale)) * 10);

            //Rotating Player In DirectionalTurning() See In Player Driver Script

            //Camera Position
            transform.position = Rotater.position;
            transform.rotation = Rotater.rotation;

            //Set GameCamera Position
            GameCamera.transform.localPosition = transform.localPosition;
            GameCamera.transform.rotation = Rotater.rotation;

            if(Input.mouseScrollDelta.y < 0)
            {
                CurrZoom = 0;
                PlayerSettings.Instance.gameplaySettings.Mode = CameraMode.UnTargetedMode;
            }
        }
    }

    //LightWeight Distance Calculation Method
    private float DistanceCalculation(Transform origin , Transform target)
    {
        float dist = (origin.position - target.position).magnitude;

        return dist;
    }

    private bool stickyLockon;
    //Grab Targets
    private void PreTargetMode()
    {
        float nearestDistance = float.MaxValue;
        float distance;

        cancelLockOn = false;

        if (!UIManager.Instance.isPaused)
        {
            //Grab A List Of Targets
            Collider[] Targets = Physics.OverlapSphere(Player.position, LockOnDistance, LayerMask.GetMask("Enemy"));

            //If List Is Populated
            if (!isLockedOn)
            {
                foreach (var collider in Targets)
                {
                    //This allows For targeting to be exclusively on enemies
                    if (collider.GetComponent<EntityHealth>() != null && collider.GetComponent<EntityHealth>().Identity.Equals(EntityTarget.Enemy))
                    {
                        EntityHealth enemy = collider.GetComponent<EntityHealth>();

                        int i = enemy.GrabHealthBar();

                        if (enemy.Health[i].HealthValue > 0)
                        {
                            if (DistanceCalculation(Player, enemy.transform) < nearestDistance)
                            {
                                nearestDistance = DistanceCalculation(Player, enemy.transform);
                                LockOnTarget = collider.gameObject;
                            }
                        }
                    }
                }

                //Key Input
                KeyCode key = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.TargetEnemyIndex].key;

                //Key Input Locker
                if (Input.GetKeyDown(key) && !UIManager.Instance.isPaused && !keyPressed)
                {
                    //Switch Camera Mode To Targeted Entity
                    if (LockOnTarget.GetComponent<LockOnTargetHelper>() != null)
                    {
                        UIManager.Instance.displayLockCameraActions = true;
                        LockOnTarget.GetComponent<LockOnTargetHelper>().SetLockOn(Player.gameObject);
                        isLockedOn = true;
                        keyPressed = true;
                        PlayerSettings.Instance.gameplaySettings.Mode = CameraMode.TargetMode;
                        print("Locked Camera");
                    }
                }
            }
        }
    }

    //Auto Adjust
    private void TargetedAutoAdjust()
    {
        //Key Input
        KeyCode key = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.ResetCameraIndex].key;

        //Reset Camera
        if (Input.GetKey(key) && !UIManager.Instance.isPaused)
        {
            forceCameraReset = true;
            AdjustCamera = true;
            timeAdjust = 0;
        }

        //Lock On Auto Adjustment Timer 
        if (Input.GetAxis("Mouse X") != 0 && !forceCameraReset && !UIManager.Instance.isPaused)
        {
            timeAdjust = LockOnTimerAutoAdjust;
            AdjustCamera = false;
        }
        else
        {
            if (timeAdjust > 0 && !forceCameraReset)
            {
                timeAdjust -= Time.deltaTime;
            }
        }

        if (timeAdjust <= 0)
        {
            AdjustCamera = true;
        }
        else
        {
            AdjustCamera = false;
        }

        if (AdjustCamera && !UIManager.Instance.isPaused)
        {
            //Goal Angle To Reset Too
            Quaternion angle = Quaternion.Euler(new Vector3(Player.eulerAngles.x + LockOnAdjustmentPos.x, Player.eulerAngles.y + LockOnAdjustmentPos.y, Player.eulerAngles.z + LockOnAdjustmentPos.z));
            Rotater.rotation = Quaternion.Slerp(Rotater.rotation, angle, Time.deltaTime * 2f);

            //Get Absolute Value Between The Goal angle and consider it complete if it is atleast 90% of the way there
            if (Mathf.Abs(Quaternion.Dot(Rotater.rotation, angle)) > 0.95f)
            {
                //Set Angle And Height To Reset Values
                XAngle = LockOnAdjustmentPos.y;
                YHeight = LockOnAdjustmentPos.x;

                forceCameraReset = false;
                AdjustCamera = false;
            }
        }
    }
    RaycastHit hit;
    //Z Targeting
    private void TargetMode()
    {
        if (LockOnTarget != null && !UIManager.Instance.isPaused)
        {
            forceCameraReset = false;

            //Key Input
            KeyCode key = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.TargetEnemyIndex].key;

            //Player Movement Mode
            Player.GetComponent<PlayerDriver>().SetMoveType(MovementType.Strafe);

            //Parenting
            Rotater.SetParent(null);
            transform.SetParent(Rotater);
            GeneralLook.SetParent(transform);
            GeneralLook.position = transform.position;

            //Vectors
            Vector3 DistanceOffset = new Vector3(0, 0, -DistanceCalculation(Player, LockOnTarget.transform) * 0.25f);
            Vector3 CollisionDistanceOffset = new Vector3(0, 0, -DistanceCalculation(Player, Rotater) * 0.75f);

            //Rotater Camera Position
            Rotater.position = (Player.position + LockOnTarget.transform.position) / 2f;

            //Mouse Movement Axis
            YHeight += Input.GetAxis("Mouse Y") * PlayerSettings.Instance.gameplaySettings.sensitivity * Time.deltaTime;
            XAngle += Input.GetAxis("Mouse X") * PlayerSettings.Instance.gameplaySettings.sensitivity * Time.deltaTime;


            //Clamp Up/Down Angle
            YHeight = Mathf.Clamp(YHeight, MinYAngle, MaxYAngle);

            //Rotate The Orbit Point
            if (!AdjustCamera)
            {
                Quaternion angle = Quaternion.Euler(new Vector3(YHeight, Player.eulerAngles.y + XAngle, Player.eulerAngles.z));
                Rotater.rotation = Quaternion.Lerp(Rotater.rotation, angle, (2 * (Time.deltaTime / Time.timeScale)) * LockOnRotationSpeed);
            }

            //Create Camera Position
            transform.localPosition = Vector3.Lerp(transform.localPosition, DistanceOffset + LockOnCameraPos, (2 * (Time.deltaTime / Time.timeScale)) * LockOnCameraSpeed);

            //Create Camera Rotation
            transform.LookAt(Rotater);

            //Create Camera Collision Correction
            if (Physics.Linecast(transform.position, Rotater.position, out hit, Wall))
            {
                print("Obstruction To Camera");

                if (hit.collider.CompareTag("Wall"))
                {
                    //Corrective Position
                    CollisionDistanceOffset.y = hit.point.y;
                    Vector3 LockOnCameraPosAdjust = LockOnCameraPos;
                    LockOnCameraPosAdjust.y = hit.point.y;
                    GameCamera.transform.position = Vector3.Lerp(GameCamera.transform.position, hit.point + transform.TransformDirection(CollisionDistanceOffset) + transform.TransformDirection(LockOnCameraPos), (2 * (Time.deltaTime / Time.timeScale)) * LockOnCameraSpeed);
                    //cancelLockOn = true;
                }
            }
            else
            {
                //Move Camera To Default Position
                GameCamera.transform.localPosition = Vector3.Lerp(GameCamera.transform.localPosition, transform.localPosition, (2 * (Time.deltaTime / Time.timeScale)) * LockOnCameraSpeed);
            }

            //Cancel Lock Methods
            //Reset Toggle Cancellbillity
            if (!Input.GetKey(key))
            {
                keyPressed = false;
            }

            //Distance Is Too Far
            if (DistanceCalculation(Player, LockOnTarget.transform) > LockOnDistanceCancelLimit)
            {
                cancelLockOn = true;
                PlayerSettings.Instance.gameplaySettings.Mode = CameraMode.UnTargetedMode;
            }

            //Cancel Lock If Enemy Dies
            if (LockOnTarget.GetComponent<EntityHealth>() != null && LockOnTarget.GetComponent<EntityHealth>().Health[LockOnTarget.GetComponent<EntityHealth>().GrabHealthBar()].HealthValue <= 0)
            {
                cancelLockOn = true;
                PlayerSettings.Instance.gameplaySettings.Mode = CameraMode.UnTargetedMode;
            }

            //On Release Of Key
            if (!Input.GetKey(key) && PlayerSettings.Instance.gameplaySettings.ZTargetType.Equals(ZTargetMode.Hold))
            {
                cancelLockOn = true;
                PlayerSettings.Instance.gameplaySettings.Mode = CameraMode.UnTargetedMode;
            }

            //On Another Key Stroke
            if (!keyPressed && Input.GetKeyDown(key) && PlayerSettings.Instance.gameplaySettings.ZTargetType.Equals(ZTargetMode.Toggle))
            {
                cancelLockOn = true;
                print("UnToggle Lockon Mode");
            }
        }
        else
        {
            cancelLockOn = true;
        }
    }
    //Cancel Lock
    void CancelLockOn()
    {
        isLockedOn = false;
        PlayerSettings.Instance.gameplaySettings.Mode = CameraMode.UnTargetedMode;
    }
    //Normal Camera

    private void UnTargetedMode()
    {
        if (!UIManager.Instance.isPaused && InputManager.Instance != null)
        {
            XAngle = 0;

            //Key Input
            KeyCode grapplekey = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.GrappleIndex].key;
            KeyCode cancelkey = InputManager.Instance.PlayerInput.Inputs[InputManager.Instance.PlayerInput.KeyIndex.AimDownSightsIndex].key;

            if (!CancelHook && Input.GetKey(grapplekey) && Input.GetKey(cancelkey))
            {
                CancelHook = true;
            }

            if (CancelHook && !Input.GetKey(grapplekey) && !Input.GetKey(cancelkey))
            {
                CancelHook = false;
            }

            if (!Input.GetKey(grapplekey) && !CancelHook || Player.gameObject.GetComponent<PlayerDriver>().physicsProperties.isSwimming || CancelHook || Input.GetKey(grapplekey) && !CancelHook)
            {
                //Player Movement Mode
                Player.GetComponent<PlayerDriver>().SetMoveType(MovementType.FreeMove);

                //Parenting
                Rotater.SetParent(null);
                transform.SetParent(Rotater);
                GeneralLook.SetParent(transform);
                GeneralLook.position = transform.position;

                float ZoomCap = (ZoomMax + -PlatformingPos.z);
                //Clamp Zoom
                CurrZoom = Mathf.Clamp(CurrZoom, ZoomMin, ZoomMax);

                //Vectors
                Vector3 ZoomOffset = new Vector3(0, 0, CurrZoom);
                Vector3 GoalPos = PlatformingPos + ZoomOffset;
                Vector3 GoalRotPos = Player.position;

                //Create Zoom Feature
   

                //Increase Zoom
                if (Input.mouseScrollDelta.y > 0 && CurrZoom < ZoomMax)
                {
                    CurrZoom += 1.5f * ZoomSpeed;
                }

                //Lower Zoom
                if (Input.mouseScrollDelta.y < 0 && CurrZoom > ZoomMin)
                {
                    CurrZoom -= 1.5f * ZoomSpeed;
                }

                //Mouse Movement Axis
                if(PlayerSettings.Instance.gameplaySettings.InvertY)
                {
                    YHeight += -(Input.GetAxis("Mouse Y") * PlayerSettings.Instance.gameplaySettings.sensitivity) * Time.deltaTime;
                }
                else
                {
                    YHeight += (Input.GetAxis("Mouse Y") * PlayerSettings.Instance.gameplaySettings.sensitivity) * Time.deltaTime;
                }

                //Clamp Up/Down Angle
                YHeight = Mathf.Clamp(YHeight, MinYAngle, MaxYAngle);

                //Create Camera Collision Correction
                if (Physics.Linecast(Rotater.position, transform.position, out hit, Wall))
                {
                    //print("Obstruction To Camera");

                    if (hit.collider.CompareTag("Wall"))
                    {
                        //Corrective Position
                        GameCamera.transform.position = Vector3.Lerp(GameCamera.transform.position, hit.point, (2 * (Time.deltaTime / Time.timeScale)) * PlatformingCameraSpeed);
                    }
                }
                else
                {
                    //Move Camera To Default Position
                    GameCamera.transform.position = transform.position;
                }

                //Keep Camera Brain In the right place at all times
                //transform.localPosition = Vector3.Lerp(transform.localPosition, GoalPos, (2 * (Time.deltaTime / Time.timeScale)) * PlatformingCameraSpeed);
                transform.localPosition = GoalPos;
                Rotater.eulerAngles = new Vector3(YHeight, Rotater.eulerAngles.y, 0);

                if (PlayerSettings.Instance.gameplaySettings.InvertX)
                {
                    //Rotate The Orbit Point
                    Rotater.Rotate(Vector3.up, -(Input.GetAxis("Mouse X") * PlayerSettings.Instance.gameplaySettings.sensitivity) * Time.deltaTime);
                }
                else
                {
                    //Rotate The Orbit Point
                    Rotater.Rotate(Vector3.up, Input.GetAxis("Mouse X") * PlayerSettings.Instance.gameplaySettings.sensitivity * Time.deltaTime);
                }

                Rotater.position = Vector3.Lerp(Rotater.position, GoalRotPos, (2 * (Time.deltaTime / Time.timeScale)) * PlatformingRotaterSpeed);

                //Rotate
                //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Rotater.position - transform.position), Time.deltaTime * 2f);
                transform.rotation = Quaternion.LookRotation(Rotater.position - transform.position);
                

                //Curr Zoom Turns Into Fps Mode At Max Zoom
                if (CurrZoom >= ZoomMax)
                {
                    PlayerSettings.Instance.gameplaySettings.Mode = CameraMode.FPSMode;
                }
            }

            if (!Player.gameObject.GetComponent<PlayerDriver>().physicsProperties.linecastGrappleBlock && !Player.gameObject.GetComponent<PlayerDriver>().physicsProperties.isSwimming && !CancelHook && Input.GetKey(grapplekey) && Player.gameObject.GetComponent<PlayerDriver>().physicsProperties.GrappleSwingObject != null && !UIManager.Instance.isPaused && GrapplePoint != null && DistanceCalculation(Player, GrapplePoint) < Player.gameObject.GetComponent<PlayerDriver>().physicsProperties.grappleSwingDist)
            {
                Player.gameObject.GetComponent<PlayerDriver>().physicsProperties.PreGrappling = true;

                if (Input.GetKeyDown(grapplekey))
                {
                    UIManager.Instance.displayPreGrappleActions = true;
                }

                if (Player.gameObject.GetComponent<PlayerDriver>().physicsProperties.GrappleSwingObject != null)
                {
                    GrapplePoint = Player.gameObject.GetComponent<PlayerDriver>().physicsProperties.GrappleSwingObject;
                }

                forceCameraReset = false;

                //Player Movement Mode
                Player.GetComponent<PlayerDriver>().SetMoveType(MovementType.FreeMove);

                //Parenting
                Rotater.SetParent(null);
                transform.SetParent(Rotater);
                GeneralLook.SetParent(transform);
                GeneralLook.position = transform.position;

                //Vectors
                Vector3 DistanceOffset = new Vector3(0, 0, -DistanceCalculation(Player, GrapplePoint.transform) * 0.5f);
                Vector3 CollisionDistanceOffset = new Vector3(0, 0, -DistanceCalculation(Player, Rotater) * 0.75f);

                //Rotater Camera Position
                Rotater.position = (Player.position + GrapplePoint.position) / 2f;

                //Clamp Up/Down Angle
                YHeight = Mathf.Clamp(YHeight, MinYAngle, MaxYAngle);

                //Mouse Movement Axis
                if (PlayerSettings.Instance.gameplaySettings.InvertY)
                {
                    YHeight -= Input.GetAxis("Mouse Y") * PlayerSettings.Instance.gameplaySettings.sensitivity * Time.deltaTime;
                }
                else
                {
                    YHeight += -Input.GetAxis("Mouse Y") * PlayerSettings.Instance.gameplaySettings.sensitivity * Time.deltaTime;
                }

                //Clamp Up/Down Angle
                YHeight = Mathf.Clamp(YHeight, MinYAngle, MaxYAngle);

                //Rotate The Orbit Point
                Quaternion angle = Quaternion.Euler(new Vector3(YHeight, Rotater.eulerAngles.y + XAngle, Player.eulerAngles.z));
                Rotater.rotation = Quaternion.Lerp(Rotater.rotation, angle, (2 * (Time.deltaTime / Time.timeScale)) * LockOnRotationSpeed);

                //Create Camera Position
                transform.localPosition = Vector3.Lerp(transform.localPosition, DistanceOffset + LockOnCameraPos, Time.deltaTime * LockOnCameraSpeed);

                //Create Camera Rotation
                transform.LookAt(Player.gameObject.GetComponent<PlayerDriver>().physicsProperties.GrappleSwingObject);

                //Create Camera Collision Correction
                if (Physics.Linecast(transform.position, GrapplePoint.position, out hit, Wall))
                {
                    print("Obstruction To Camera");

                    if (hit.collider.CompareTag("Wall"))
                    {
                        //Corrective Position
                        CollisionDistanceOffset.y = hit.point.y;
                        Vector3 LockOnCameraPosAdjust = LockOnCameraPos;
                        LockOnCameraPosAdjust.y = hit.point.y;
                        GameCamera.transform.position = Vector3.Lerp(GameCamera.transform.position, hit.point + transform.TransformDirection(CollisionDistanceOffset) + transform.TransformDirection(LockOnCameraPos), (2 * (Time.deltaTime / Time.timeScale)) * LockOnCameraSpeed);
                    }
                }
                else
                {
                    //Move Camera To Default Position
                    GameCamera.transform.localPosition = Vector3.Lerp(GameCamera.transform.localPosition, transform.localPosition, (2 * (Time.deltaTime / Time.timeScale)) * LockOnCameraSpeed);
                }
            }
            else
            {
                Player.gameObject.GetComponent<PlayerDriver>().physicsProperties.PreGrappling = false;
            }
        }

    }
}