using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    public float health = 100;
    public static ZombieAI Instance;

    public GameObject Target;
    public float speed = 1.5f;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        transform.LookAt(Target.gameObject.transform);
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public void EnemyDie()
    {
        ScoreSystem.instance.AddScore();
        Destroy(gameObject);
    }
}
