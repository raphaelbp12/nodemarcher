using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TMPro.TMP_Text scoreLabel;
    public TMPro.TMP_Text multiplierLabel;
    public TMPro.TMP_Text highScoreLabel;
    public TMPro.TMP_Text lastScoreLabel;
    public PlayerScore playerScore { get; private set; }
    public int highestScore { get; private set; } = 0;
    public int lastScore { get; private set; } = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerScore == null)
        {
            scoreLabel.text = "";
            multiplierLabel.text = "";
        }
        else
        {
            scoreLabel.text = playerScore.GetCurrentScore();
            if (playerScore.streakMultiplier > 1)
            {
                multiplierLabel.text = "x" + playerScore.streakMultiplier;
            }
            else
            {
                multiplierLabel.text = "";
            }
        }
        highScoreLabel.text = "HIGH: " + highestScore.ToString();
        lastScoreLabel.text = "LAST: " + lastScore.ToString();

    }

    public void SetPlayerScore(PlayerScore playerScore)
    {
        this.playerScore = playerScore;
    }

    public void SaveScore()
    {
        lastScore = playerScore.score;
        if (playerScore.score > highestScore)
        {
            highestScore = playerScore.score;
        }
    }
}
