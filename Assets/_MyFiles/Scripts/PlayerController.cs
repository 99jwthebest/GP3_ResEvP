using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float playerWalkSpeed = 2.0f;
    [SerializeField]
    private float playerAimWalkSpeed = 2.0f;
    [SerializeField]
    private float playerSprintSpeed = 7.0f;
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
    private InputAction sprintAction;

    private Animator animator;
    int jumpAnimation;
    int recoilAnimation;

    int moveXAnimationParameterID;
    int moveZAnimationParameterID;

    Vector2 currentAnimationBlendVector;
    Vector2 animationVelocity;
    Vector3 move;

    [SerializeField] RecoilShake recoilShake;

    // Gun System
    public float bulletDamageAmount;
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots; // range;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    // bools
    bool shooting, readyToShoot, reloading;

    // Reference
    /* public Camera fpsCam;
     public Transform attackPoint;
     public RaycastHit rayHit;
     public LayerMask whatIsEnemy;

    //Graphics
    public GameObject muzzleFlash, bulletHoleGraphic;
    */
    public TextMeshProUGUI ammoText;
    public bool gunEquipped;

    // End of Gun System
    [SerializeField]
    private int scoreFromZombiePart;
    [SerializeField]
    private int playerHealth = 100;
    [SerializeField]
    private int currentPHealth;
    public Slider healthSlider;


    private void Awake()  //awake happens before onEnable and then after that it's start
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;
        // Cache a reference to all of the input actions to avoid them with strings constantly.
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        shootAction = playerInput.actions["Shoot"];
        sprintAction = playerInput.actions["Sprint"];
        // Lock the cursor to the middle of the screen.
        Cursor.lockState = CursorLockMode.Locked;
        // Animations
        animator = GetComponent<Animator>();
        jumpAnimation = Animator.StringToHash("PistolJump");
        recoilAnimation = Animator.StringToHash("PistolShootRecoil");
        moveXAnimationParameterID = Animator.StringToHash("MoveX");
        moveZAnimationParameterID = Animator.StringToHash("MoveZ");
        currentPHealth = playerHealth;
        SetMaxHealth(playerHealth);

    }

    private void Start()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    // ************** Might Delete code below if not useful
    /*private void OnEnable()
    {
        if(!allowButtonHold)
        {
            shootAction.performed += _ => ShootGun();
        }
    }

    private void OnDisable()
    {
        if (!allowButtonHold)
        {
            shootAction.performed -= _ => ShootGun();
        }
    }*/

    /// <summary>
    /// Spawn a bullet and shoot in the direction of the gun barrel. If the raycast hits the environment,
    /// the bullet travels towards to point of contact, else it will not have a target and be destroyed
    /// at a certain distance away from the player.
    /// </summary>
    private void ShootGun()
    {
        readyToShoot = false;

        /*
        // Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        // Calculate Direction with Spread
        Vector3 direction = cameraTransform.forward + new Vector3(x, y, 0);
        // Replace cameraTransform.forward if you want to use Spread. I'm not sure that it'll work.
        */

        // Muzzle Flash
        //Instantiate(muzzleFlash, barrelTransform.position, Quaternion.identity);

        RaycastHit hit;
        GameObject bullet = GameObject.Instantiate(bulletPrefab, barrelTransform.position, Quaternion.identity, bulletParent);
        BulletController bulletController = bullet.GetComponent<BulletController>();
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity))
        {
            bulletController.target = hit.point;
            bulletController.hit = true;

            //checkHIT(hit);
            ZombieAI zombieAI = hit.collider.transform.root.gameObject.GetComponent<ZombieAI>();
           
            if (hit.collider.CompareTag("EnemyHead"))
            {
                Debug.Log("hit HEAD!!");
                zombieAI.enemyTakeDamage(bulletDamageAmount);
                ScoreSystem.instance.AddScore(scoreFromZombiePart);
                if(zombieAI.health <= 0)
                {
                    CountdownTimer.Instance.AddTime();
                }
            }
            if (hit.collider.CompareTag("EnemyBody"))
            {
                Debug.Log("hit BODY!!");
                zombieAI.enemyTakeDamage(bulletDamageAmount / 2);
                ScoreSystem.instance.AddScore(scoreFromZombiePart / 4);
            }
            if (hit.collider.CompareTag("EnemyArms"))
            {
                Debug.Log("hit ARMS!!");
                zombieAI.enemyTakeDamage(bulletDamageAmount / 4);
                ScoreSystem.instance.AddScore(scoreFromZombiePart / 10);
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
        
        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if(bulletsShot > 0 && bulletsLeft > 0)
            Invoke("ShootGun", timeBetweenShots);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            playerTakeDamage(10);
        }

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
        move = new Vector3(currentAnimationBlendVector.x, 0, currentAnimationBlendVector.y);
        // Take into account the camera direction when moving the player.
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized; //Maybe make this work for when you want to aim but not work when not aiming
        move.y = 0f;

        //controller.Move(move * Time.deltaTime * playerWalkSpeed);
        if(sprintAction.IsPressed() && !characterRotateWithCam) 
        {
            controller.Move(move * Time.deltaTime * playerSprintSpeed);

        }
        else
        {
            controller.Move(move * Time.deltaTime * playerWalkSpeed);

        }


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
        
        
        UpdateTargetDirection();
        

        if(allowButtonHold)
        {
            shooting = shootAction.IsPressed();
        }
        else
        {
            shooting = shootAction.triggered;
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }

        // Shoot
        if(gunEquipped && readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            ShootGun();
        }

        // SetText
        ammoText.SetText(bulletsLeft + " / " + magazineSize);

    }


    public void UpdateTargetDirection()
    {
        if (characterRotateWithCam)
        {
            float targetAngle = cameraTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, aimRotationSpeed * Time.deltaTime);
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

            // Ladder
            float avoidFloorDistance = .1f;
            float ladderGrabDistance = .4f;
            Physics.Raycast(transform.position + Vector3.up * avoidFloorDistance, targetDirection, out RaycastHit raycastHit, ladderGrabDistance);
           // Debug.Log(raycastHit.transform);
            // Ladder

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
    }

    public void startSprint()
    {
        controller.Move(move * Time.deltaTime * playerSprintSpeed);

    }

    public void endSprint()
    {
        controller.Move(move * Time.deltaTime * playerWalkSpeed);

    }

    public void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    public void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }

    public void SetMaxHealth(int health)
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;

    }

    public void SetHealth(int health)
    {
        healthSlider.value = health;
    }

    public void playerTakeDamage(int amountOfDam)
    {
        currentPHealth -= amountOfDam;

        SetHealth(currentPHealth);
    }

    /*private void checkHIT(RaycastHit hit)
    {
        takeDamageScript = hit.transform.GetComponent<takeDamage>();
        switch (takeDamageScript.damageType)
        {
            case takeDamage.collisionType.head : takeDamageScript.HIT(damageAmount);
                break;
            case takeDamage.collisionType.body : takeDamageScript.HIT(damageAmount / 2);
                break;
            case takeDamage.collisionType.arms : takeDamageScript.HIT(damageAmount / 4);
                break;
        }
    }*/
}
