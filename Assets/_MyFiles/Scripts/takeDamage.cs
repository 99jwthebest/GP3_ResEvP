using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class takeDamage : MonoBehaviour
{
    public static takeDamage instance;

    //public enum collisionType { head, body, arms}
    //public collisionType damageType;

    private void Awake()
    {
        instance = this;
    }

    public void enemyTakeDamage(float value)
    {
        ZombieAI.Instance.health -= value;
        if (ZombieAI.Instance.health <= 0 )
        {
            ZombieAI.Instance.EnemyDie();
        }
        /*try
        {
        }
        catch 
        {
            Debug.Log("zombie is not connected with controller!!!.....");
        }*/
    }
}
