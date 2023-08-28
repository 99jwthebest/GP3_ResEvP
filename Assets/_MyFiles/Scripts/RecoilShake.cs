using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RecoilShake : MonoBehaviour
{
    [SerializeField] CinemachineImpulseSource screenShake;

    public void ScreenShake()
    {
        screenShake.GenerateImpulse();
    }
}
