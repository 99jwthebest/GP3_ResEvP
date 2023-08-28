using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem instance;

    int currentScore;
    int highScore;

    int zombiesKilled;

    public TextMeshProUGUI currentScoreText;
    //public TextMeshProUGUI highScoreText;

    public TextMeshProUGUI zombiesKilledText;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        currentScore = 0;
        currentScoreText.text = "Score: " + currentScore;

        zombiesKilled = 0;
        zombiesKilledText.text = "Zombies Killed: " + zombiesKilled;
    }

    public void AddScore()
    {
        currentScore += 200;
        currentScoreText.text = "Score: " + currentScore;

        zombiesKilled += 1;
        zombiesKilledText.text = "Zombies Killed: " + zombiesKilled;

        CountdownTimer.Instance.AddTime();
    }
}
