using UnityEngine;

public class PickupController : MonoBehaviour
{
    public PlayerController pController;
    public Rigidbody rb;
    public BoxCollider gunCollide;
    public Transform player, gunContainer, MainCamera;

    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;

    public bool equipped;
    public static bool slotFull;

    private void Start()
    {
        // Setup
        if(!equipped)
        {
            pController.gunEquipped = false;
            rb.isKinematic = false;
            gunCollide.isTrigger = false;
            rb.useGravity = false;
        }
        if (equipped)
        {
            pController.gunEquipped = true;
            rb.isKinematic = true;
            gunCollide.isTrigger = true;
            slotFull = true;
        }
    }

    private void Update()
    {
        // Check if player is in range and "E" is pressed
        Vector3 distanceToPlayer = player.position - transform.position;
        if(!equipped && distanceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E) && !slotFull)
        {
            PickUp();
        }

        // Drop if equipped and "Q" is pressed
        if(equipped && Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }
    }

    private void PickUp()
    {
        equipped = true;
        slotFull = true;

        // Make weapon a child of the camera and move it to default position
        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        //transform.localScale = Vector3.one;
        /*
        */
        // Make Rigidbody kinematic and BoxCollider a trigger
        rb.isKinematic = true;
        gunCollide.isTrigger = true;

        // Enable Script
        pController.gunEquipped = true;
    }

    private void Drop()
    {

        equipped = false;
        slotFull = false;
        rb.useGravity = true;

        // Set parent to null
        transform.SetParent(null);

        // Make Rigidbody kinematic and BoxCollider a trigger
        rb.isKinematic = false;
        gunCollide.isTrigger = false;

        // Gun carries momentum of player
        rb.velocity = pController.GetComponent<CharacterController>().velocity;

        // AddForce
        rb.AddForce(MainCamera.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(MainCamera.up * dropUpwardForce, ForceMode.Impulse);
        // Add Random rotation
        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10); 


        // Enable Script
        pController.gunEquipped = false;
    }
}
