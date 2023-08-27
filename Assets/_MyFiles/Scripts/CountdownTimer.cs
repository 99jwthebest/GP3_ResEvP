using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public static CountdownTimer Instance;

    float currentTime = 0f;
    [SerializeField]
    private float startingTime = 5f;
    [SerializeField]
    private float addTimeFromZombie = 10f;

    [SerializeField]
    private TextMeshProUGUI countdownText;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentTime = startingTime;
    }

    void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        countdownText.text = currentTime.ToString("F2");

        if(currentTime <= 0)
        {
            currentTime = 0;
        }
    }

    public void AddTime()
    {
        currentTime += addTimeFromZombie;
        countdownText.text = currentTime.ToString("F2");
    }
}
