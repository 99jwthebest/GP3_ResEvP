using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class DeactivateChestRig : MonoBehaviour
{
    Rig chestRig;

    // Start is called before the first frame update
    void Start()
    {
        chestRig = GetComponent<Rig>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void startRig()
    {
        chestRig.weight = 1.0f;

        /*float noWeight = 0;
        float weight = 1;
        pistolRig.weight = Mathf.Lerp(noWeight, weight, .5f * Time.deltaTime);*/

    }

    public void stopRig()
    {
        chestRig.weight = 0.0f;

        /*float noWeight = 0;
        float weight = 1;
        pistolRig.weight = Mathf.Lerp(weight, noWeight, .5f * Time.deltaTime);*/
    }
}
