using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateEnemies : MonoBehaviour
{
    public GameObject zombie;
    public int xPos;
    public int zPos;
    public int enemyCount;

    void Start()
    {
        StartCoroutine(EnemyDrop());
    }

    IEnumerator EnemyDrop()
    {
        while (enemyCount < 10)
        {
            xPos = Random.Range(-20, 20);
            zPos = Random.Range(10, 40);
            Vector3 randomPos = new Vector3(xPos, 2, zPos);

            Instantiate(zombie, randomPos, Quaternion.identity);

            yield return new WaitForSeconds(.5f);
            enemyCount++;
        }
    }

}
