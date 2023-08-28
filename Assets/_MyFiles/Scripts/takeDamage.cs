using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class takeDamage : MonoBehaviour
{
    public enum collisionType { head, body, arms}
    public collisionType damageType;

    public void HIT(float value)
    {
        try
        {
            ZombieAI.Instance.health -= value;
            if (ZombieAI.Instance.health <= 0 )
            {
                ZombieAI.Instance.EnemyDie();
            }
        }
        catch 
        {
            Debug.Log("zombie is not connected with controller!!!.....");
        }
    }
}
