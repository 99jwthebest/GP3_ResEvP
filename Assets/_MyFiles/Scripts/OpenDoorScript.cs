using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorScript : MonoBehaviour
{
    public GameObject openDoorText;
    public GameObject animOpenDoor;
    public GameObject ThisTrigger;
    public AudioSource DoorOpenSound;
    public bool Action = false;

    private void Start()
    {
        openDoorText.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.transform.tag == "Player")
        {
            openDoorText.SetActive(true);
            Action = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        openDoorText.SetActive(false);
        Action = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(Action == true)
            {
                openDoorText.SetActive(false);
                animOpenDoor.GetComponent<Animator>().Play("OpenDoorAnim");
                ThisTrigger.SetActive(false);
                DoorOpenSound.Play();
                Action = false;
            }
        }
    }
}
