using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float aimRotationSpeed = 10f;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform barrelTransform;
    [SerializeField]
    private Transform bulletParent;
    [SerializeField]
    private float bulletHitMissDistance = 30f;

    [SerializeField]
    private float animationSmoothTime = 0.1f;
    [SerializeField]
    private float animationPlayTransition = 0.15f;
    [SerializeField]
    private Transform aimTarget;
    [SerializeField]
    private float aimDistance = 1f;
    [SerializeField]
    private Vector3 addingForAim;

    ///Stuff below is stuff I added without the video
    public bool characterRotateWithCam = false;
    private float turnSpeedMultiplier;
    public float turnSpeed = 10f;
    private Vector3 targetDirection;
    private Quaternion freeRotation;
    private Vector2 input;

    //Stuff above is stuff I added without the video

    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraTransform;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction shootAction;

    private Animator animator;
    int jumpAnimation;
    int recoilAnimation;

    int moveXAnimationParameterID;
    int moveZAnimationParameterID;

    Vector2 currentAnimationBlendVector;
    Vector2 animationVelocity;

    [SerializeField] RecoilShake recoilShake;


    private void Awake()  //awake happens before onEnable and then after that it's start
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;
        // Cache a reference to all of the input actions to avoid them with strings constantly.
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        shootAction = playerInput.actions["Shoot"];
        // Lock the cursor to the middle of the screen.
        Cursor.lockState = CursorLockMode.Locked;
        // Animations
        animator = GetComponent<Animator>();
        jumpAnimation = Animator.StringToHash("PistolJump");
        recoilAnimation = Animator.StringToHash("PistolShootRecoil");
        moveXAnimationParameterID = Animator.StringToHash("MoveX");
        moveZAnimationParameterID = Animator.StringToHash("MoveZ");

    }

    private void OnEnable()
    {
        shootAction.performed += _ => ShootGun();
    }

    private void OnDisable()
    {
        shootAction.performed -= _ => ShootGun();
    }

    /// <summary>
    /// Spawn a bullet and shoot in the direction of the gun barrel. If the raycast hits the environment,
    /// the bullet travels towards to point of contact, else it will not have a target and be destroyed
    /// at a certain distance away from the player.
    /// </summary>
    private void ShootGun()
    {
        RaycastHit hit;
        GameObject bullet = GameObject.Instantiate(bulletPrefab, barrelTransform.position, Quaternion.identity, bulletParent);
        BulletController bulletController = bullet.GetComponent<BulletController>();
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity))
        {
            bulletController.target = hit.point;
            bulletController.hit = true;

            if (hit.collider.CompareTag("Enemy"))
            {
                ZombieAI.Instance.EnemyDie();
            }
        }
        else
        {
            bulletController.target = cameraTransform.position + cameraTransform.forward * bulletHitMissDistance;
            bulletController.hit = false;
        }
        animator.CrossFade(recoilAnimation, animationPlayTransition);
        //CinemachineShake.Instance.ShakeCamera(1f, .25f);  ****this is for melee, Maybe
        recoilShake.ScreenShake();
    }

    void Update()
    {
        aimTarget.position = cameraTransform.position + cameraTransform.forward * aimDistance;

        aimTarget.localPosition += addingForAim;

        groundedPlayer = controller.isGrounded;
        // If the player is on the ground, there is no need to apply a downwards force.
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        input = moveAction.ReadValue<Vector2>();
        currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, input, ref animationVelocity, animationSmoothTime);
        Vector3 move = new Vector3(currentAnimationBlendVector.x, 0, currentAnimationBlendVector.y);
        // Take into account the camera direction when moving the player.
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized; //Maybe make this work for when you want to aim but not work when not aiming
        move.y = 0f;

        controller.Move(move * Time.deltaTime * playerSpeed);
        // Blend Strafe Animation.
        animator.SetFloat(moveXAnimationParameterID, currentAnimationBlendVector.x);
        animator.SetFloat(moveZAnimationParameterID, currentAnimationBlendVector.y);

        // Changes the height position of the player..
        if (jumpAction.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            animator.CrossFade(jumpAnimation, animationPlayTransition);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // For when Aiming and walking. Not when running. Just like RE:4 Remake, it strafes.
        // The strafe is more loose when walking but when aiming it's more tight.
        // in other words, Leon rotates faster along with the camera when aiming but when walking, he rotates slower.
        // When runnning, I want to be able to use the "else" statement in the UpdateTargetDirection()
        // Rotate towards camera direction.
        /*float targetAngle = cameraTransform.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, aimRotationSpeed * Time.deltaTime);
        */


        UpdateTargetDirection();
        if (input != Vector2.zero && targetDirection.magnitude > 0.1f)
        {
            Vector3 lookDirection = targetDirection.normalized;
            freeRotation = Quaternion.LookRotation(lookDirection, transform.up);
            var diferenceRotation = freeRotation.eulerAngles.y - transform.eulerAngles.y;
            var eulerY = transform.eulerAngles.y;

            if (diferenceRotation < 0 || diferenceRotation > 0) eulerY = freeRotation.eulerAngles.y;
            var euler = new Vector3(0, eulerY, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), turnSpeed * turnSpeedMultiplier * Time.deltaTime);
        }
        
    }


    public virtual void UpdateTargetDirection()
    {
        if (!characterRotateWithCam)
        {
            turnSpeedMultiplier = 0.2f;
            var forward = transform.TransformDirection(Vector3.forward);
            forward.y = 0;

            //get the right-facing direction of the referenceTransform
            var right = transform.TransformDirection(Vector3.right);
            targetDirection = input.x * right + Mathf.Abs(input.y) * forward;
        }
        else
        {
            turnSpeedMultiplier = 1f;
            var forward = cameraTransform.TransformDirection(Vector3.forward);
            forward.y = 0;

            //get the right-facing direction of the referenceTransform
            var right = cameraTransform.TransformDirection(Vector3.right);

            // determine the direction the player will face based on input and the referenceTransform's right and forward directions
            targetDirection = input.x * right + input.y * forward;
        }
    }
}
