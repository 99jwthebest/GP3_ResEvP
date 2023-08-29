using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public List<Wave> waves = new List<Wave>();
    [System.Serializable]
    public class Wave
    {
        public string name;
        public Transform zombie;
        public int count;
        public float rate;
    }

    //public Wave[] waves;
}
