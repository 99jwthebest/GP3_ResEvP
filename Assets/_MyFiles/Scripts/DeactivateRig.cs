using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class DeactivateRig : MonoBehaviour
{
    Rig pistolRig;

    // Start is called before the first frame update
    void Start()
    {
        pistolRig = GetComponent<Rig>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startRig()
    {
        pistolRig.weight = 1.0f;

        /*float noWeight = 0;
        float weight = 1;
        pistolRig.weight = Mathf.Lerp(noWeight, weight, .5f * Time.deltaTime);*/

    }

    public void stopRig()
    {
        pistolRig.weight = 0.0f;

        /*float noWeight = 0;
        float weight = 1;
        pistolRig.weight = Mathf.Lerp(weight, noWeight, .5f * Time.deltaTime);*/
    }
}
