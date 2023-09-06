using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public static CountdownTimer Instance;

    float currentTime = 0;
   
    [SerializeField]
    private float startingTime = 30f;
    [SerializeField]
    private float addTimeFromZombie = 10f;

    [SerializeField]
    private TextMeshProUGUI minutesText;
    [SerializeField]
    private TextMeshProUGUI secondsText;
    [SerializeField]
    private TextMeshProUGUI millisecondsText;

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

        // Calculate minutes, seconds, and milliseconds
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        int milliseconds = Mathf.FloorToInt((currentTime * 100) % 100);

        minutesText.text = minutes.ToString("00");
        secondsText.text = seconds.ToString(":00");
        millisecondsText.text = "." + milliseconds.ToString("00");

    }

    public void AddTime()
    {
        currentTime += addTimeFromZombie;
    }
}
