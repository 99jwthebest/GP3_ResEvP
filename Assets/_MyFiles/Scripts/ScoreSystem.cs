using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem instance;

    int currentScore;
    int highScore;

    int zombiesKilled;

    [SerializeField]
    private int startingMoney;

    private int totalMoney;
    public int TotalMoney
    {
       get { return totalMoney; }
       private set { totalMoney = value; }
    }

    public TextMeshProUGUI currentScoreText;
    //public TextMeshProUGUI highScoreText;

    public TextMeshProUGUI zombiesKilledText;

    public TextMeshProUGUI moneyText;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        TotalMoney = startingMoney;
        moneyText.text = "Money: " + TotalMoney;

        currentScore = 0;
        currentScoreText.text = "Score: " + currentScore;

        zombiesKilled = 0;
        zombiesKilledText.text = "Zombies Killed: " + zombiesKilled;
    }

    public void AddScore(int sFromZomb)
    {
        currentScore += sFromZomb;
        currentScoreText.text = "Score: " + currentScore;

        TotalMoney += sFromZomb;
        moneyText.text = "Money: " + TotalMoney;

        zombiesKilled += 1;
        zombiesKilledText.text = "Zombies Killed: " + zombiesKilled;
    }

    public void spendMoney(int moneySpend)
    {
        TotalMoney -= moneySpend;
        moneyText.text = "Money: " + TotalMoney;
    }
}
