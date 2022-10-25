using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInputs _input;

    [Header("Movement")]
    private float moveSpeed;
    private bool holdingItem;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float RotationSpeed = 1.0f;
    [SerializeField] private float groundDrag;
    [SerializeField] private float airDrag;
    [SerializeField] private float stopDistanceCollider;
    [SerializeField] private LayerMask layerMaskStop;
    [SerializeField] private float enemyStopDistanceCollider;
    [SerializeField] private LayerMask enemyLayerMaskStop;
    [SerializeField] private GroundCheck groundCheck;

    [Header("Jumping")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    private bool readyToJump;

    [Header("Crouching")]
    [SerializeField] private float crouchSpeedMovement;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchYScale;
    private float startYScale;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    private bool grounded;

    [Header("Slope Handling")]
    [SerializeField] private float maxSlopeAngle;
    [SerializeField] private float onSlopeMultiplyer;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Gravity")]
    [SerializeField] private LayerMask gravityDirectionMask;
    private Vector3 gravityChangeDirection;

    [Space]
    [SerializeField] private Transform orientation;
    [SerializeField] private MovementState state;
    [SerializeField] private GameObject playerIdle;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    [SerializeField] private GameObject CinemachineCameraTarget;
    [Tooltip("How far in degrees can you move the camera up")]
    [SerializeField] private float TopClamp = 90.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    [SerializeField] private float BottomClamp = -90.0f;

    [Header("Animation")]
    [SerializeField] private HandsAnimation handRAnimation;
    [SerializeField] private HandsAnimation handLAnimation;
    [SerializeField] private WeaponAnimation HandLWeaponAnimation;
    private GameObject handL;

    [Space]
    [SerializeField] private float smooth;


    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;


    private PlayerInput _playerInput;
    private PlayerActions _playerActions;
    private float _rotationVelocity;
    private const float _threshold = 0.01f;
    private float _cinemachineTargetPitch;
    private DirectionProps directionProps;
    private bool rotate = false;
    private Vector3 rotateDirection;
    private Quaternion target;
    private bool crouched = false;
    private float startTopClamp;


    [SerializeField] private AudioSource WalkingAudio;
    [SerializeField] private AudioSource RunningAudio;
    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
        }
    }
    private QuestManager questManager;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    private void Start()
    {
        GravityController.onGravityChange += PlayerRotationOn;

        _playerInput = GetComponent<PlayerInput>();
        _input = GetComponent<PlayerInputs>();
        _playerActions = GetComponent<PlayerActions>();
        questManager = FindObjectOfType<QuestManager>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        startTopClamp = TopClamp;
        readyToJump = true;
        handL = handLAnimation?.gameObject;

        startYScale = playerIdle.transform.localScale.y;
    }
    private void OnDestroy()
    {
        GravityController.onGravityChange -= PlayerRotationOn;
    }

    private void Update()
    {
        // ground check
        grounded = groundCheck.GetGrounded();

        MyInput();
        SpeedControl();
        StateHandler();
        
        if (rotate)
        {
            rb.useGravity = false;
            rb.velocity = gravityChangeDirection * 2.5f;
            if (Physics.Raycast(transform.position, gravityChangeDirection.normalized, playerHeight + 2f, gravityDirectionMask))
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                
                target = Quaternion.Euler(rotateDirection.x, rotateDirection.y, rotateDirection.z);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);

                if (Quaternion.Angle(transform.rotation, target) <= 5f)
                {
                    transform.eulerAngles = new Vector3(rotateDirection.x, rotateDirection.y, rotateDirection.z);
                    rb.isKinematic = false;
                    rb.velocity = Vector3.zero;
                    rotate = false;
                }
            }
        }
        // handle drag
        else if (grounded)
            rb.drag = groundDrag;
        else
        {
            rb.drag = airDrag;
            rb.AddForce(-transform.up * gravity * Time.deltaTime, ForceMode.Force);
        }
    }
    private void LateUpdate()
    {
        CameraRotation();
    }
    private void CameraRotation()
    {
        // if there is an input
        if (_input.look.sqrMagnitude >= _threshold)
        {
            //Don't multiply mouse input by Time.deltaTime
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;//up down
            _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;       //left right

            // clamp our pitch rotation
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Update Cinemachine camera target pitch
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

            if(!rotate)
                // rotate the player left and right
                transform.Rotate(Vector3.up * _rotationVelocity);
        }
    }

    private void FixedUpdate()
    {
        if(!rotate)
            MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = _input.move.x;
        verticalInput = _input.move.y;

        if (_input.crouch)
        {
            _playerActions.QuestAction(KeyCode.LeftControl);
            playerIdle.transform.localScale = new Vector3(playerIdle.transform.localScale.x, crouchYScale, playerIdle.transform.localScale.z);
            //GetComponent<PlayerActions>().keyCode = KeyCode.LeftControl;
        }

        // stop crouch
        if (!_input.crouch)
        {
            playerIdle.transform.localScale = new Vector3(playerIdle.transform.localScale.x, startYScale, playerIdle.transform.localScale.z);
            crouched = false;
        }

    }

    private void StateHandler()
    {
        // Mode - Crouching
        if (_input.crouch)
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeedMovement;
        }

        // Mode - Sprinting
        else if (_input.sprint)
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
            // RunningAudio.GetComponent<AudioSource>().Play();
            // WalkingAudio.GetComponent<AudioSource>().Pause();
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
            // WalkingAudio.GetComponent<AudioSource>().Play();
            // RunningAudio.GetComponent<AudioSource>().Pause();
        }
        // Mode - Air
        else
        {
            //groundCheck.SetGroundCheck(false,groundCheckRadius);
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(_input.mousePosition);
        RaycastHit hit;

        if ((Physics.Raycast(ray, out hit, stopDistanceCollider, layerMaskStop) && holdingItem)||(Physics.Raycast(ray, out hit, enemyStopDistanceCollider, enemyLayerMaskStop)))
        {
            Debug.DrawLine(ray.origin, hit.point);
            if (verticalInput == 1)
            {
                moveDirection = (transform.right * horizontalInput).normalized;
            }
            else
            {
                moveDirection = (transform.forward * verticalInput + transform.right * horizontalInput).normalized;
            }
        }
        else
            // calculate movement direction
            moveDirection = (transform.forward * verticalInput + transform.right * horizontalInput).normalized;

        // when to jump   
        if (_input.jump && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //on crouch
        if (_input.crouch && !crouched)
        {
            rb.AddForce(-transform.up * crouchSpeed * Time.fixedDeltaTime, ForceMode.Impulse);
            crouched = true;
        }
        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * onSlopeMultiplyer * Time.fixedDeltaTime, ForceMode.Force);
            if (rb.velocity.y > 0)
                rb.AddForce(-transform.up * 70f, ForceMode.Force);
        }
        // on ground
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * Time.fixedDeltaTime, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier * Time.fixedDeltaTime, ForceMode.Force);

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();

        //Hands Animation
        HandAnimation();
    }

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed * Time.fixedDeltaTime;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel;

            if (directionProps != null)
            {
                switch (directionProps._axis)
                {
                    case AXIS.X:
                        flatVel = new Vector3(0f, rb.velocity.y, rb.velocity.z);

                        // limit velocity if needed
                        if (flatVel.magnitude > moveSpeed)
                        {
                            Vector3 limitedVel = flatVel.normalized * moveSpeed * Time.fixedDeltaTime;
                            rb.velocity = new Vector3(rb.velocity.x, limitedVel.y, limitedVel.z);
                        }
                        break;
                    case AXIS.Y:
                        flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

                        // limit velocity if needed
                        if (flatVel.magnitude > moveSpeed)
                        {
                            Vector3 limitedVel = flatVel.normalized * moveSpeed * Time.fixedDeltaTime;
                            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
                        }
                        break;
                    case AXIS.Z:
                        flatVel = new Vector3(rb.velocity.x, rb.velocity.y, 0f);

                        // limit velocity if needed
                        if (flatVel.magnitude > moveSpeed)
                        {
                            Vector3 limitedVel = flatVel.normalized * moveSpeed * Time.fixedDeltaTime;
                            rb.velocity = new Vector3(limitedVel.x, limitedVel.y, rb.velocity.z);
                        }
                        break;
                }
            }
        }
    }
    private void Jump()
    {
        _playerActions.QuestAction(KeyCode.Space);
        if (!questManager.GetIfKeyIsActiveFirstTime(KeyCode.Space)) { return; }
        exitingSlope = true;

        // reset y velocity
        rb.AddForce(transform.up * jumpForce * Time.fixedDeltaTime, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, -transform.up, out slopeHit, playerHeight + 0.6f, whatIsGround))
        {
            float angle = Vector3.Angle(transform.up, slopeHit.normal);

            return angle < maxSlopeAngle && angle > 10f;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    public void PlayerRotationOn()
    {
        directionProps = FindObjectOfType<GravityController>().directionProps;

        var signNum = directionProps._sign == SIGN.POSITIVE ? 1 : -1;


        switch (directionProps._axis)
        {
            case AXIS.X:
                rotateDirection = new Vector3(0, 0, 90 * signNum);
                break;
            case AXIS.Y:
                if (signNum == -1)
                {
                    rotateDirection = Vector3.zero;
                }
                else if (signNum == 1)
                {
                    rotateDirection = new Vector3(0, 0, 180);
                }
                break;
            case AXIS.Z:
                rotateDirection = new Vector3(90 * signNum, 0, 0);
                break;
        }
        
        rotate = true;
    }

    public void SetGravityDirection(Vector3 direction)
    {
        gravityChangeDirection = direction;
    }

    public void SetHolding(bool holding)//set if player holding item or not.
    {
        holdingItem = holding;
        if (holding)
            TopClamp = 20f;
        else
            TopClamp = startTopClamp;
    }


    #region Hands Animation
    private void HandAnimation()
    {
        if (moveDirection != Vector3.zero)
        {
            handRAnimation.WalkingAnimation(true);
            if (handL.TryGetComponent(out HandsAnimation handsAnimation))
            {
                handsAnimation.WalkingAnimation(true);
            }
            else if (handL.TryGetComponent(out WeaponAnimation weaponAnimation))
            {
                weaponAnimation.WalkingAnimation(true);
            }
        }
        else
        {
            handRAnimation.WalkingAnimation(false);

            if (handL.TryGetComponent(out HandsAnimation handsAnimation))
            {
                handsAnimation.WalkingAnimation(false);
            }
            else if (handL.TryGetComponent(out WeaponAnimation weaponAnimation))
            {
                weaponAnimation.WalkingAnimation(false);
            }
        }
    }
    public void DisableAnimation()
    {
        handRAnimation.WalkingAnimation(false);

        if (handL.TryGetComponent(out HandsAnimation handsAnimation))
        {
            handsAnimation.WalkingAnimation(false);
        }
        if (handL.TryGetComponent(out WeaponAnimation weaponAnimation))
        {
            weaponAnimation.WalkingAnimation(false);
        }
    }
    public void SetLeftHand()
    {
        handL = handLAnimation.gameObject;
    }
    public void SetLHandWeapon()
    {
        handL = HandLWeaponAnimation.gameObject;
    }
    #endregion
}
