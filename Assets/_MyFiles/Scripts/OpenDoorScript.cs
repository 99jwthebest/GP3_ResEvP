using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OpenDoorScript : MonoBehaviour
{
    public GameObject openDoorText;
    public TextMeshProUGUI priceOfDoorText;

    public GameObject animOpenDoor;
    public GameObject ThisTrigger;
    public AudioSource DoorOpenSound;
    public bool Action = false;
    public int priceOfDoor;

    private void Start()
    {
        openDoorText.SetActive(false);
        priceOfDoorText.text = "Press E to Open Door [Cost:" + priceOfDoor + "]";
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
                if(ScoreSystem.instance.TotalMoney > priceOfDoor)
                {
                    openDoorText.SetActive(false);
                    ScoreSystem.instance.spendMoney(priceOfDoor);
                    animOpenDoor.GetComponent<Animator>().Play("OpenDoorAnim");
                    ThisTrigger.SetActive(false);
                    DoorOpenSound.Play();
                    Action = false;
                }
                else
                {
                    Debug.Log("Don't have enough money!!!!!");
                }
            }
        }
    }
}
