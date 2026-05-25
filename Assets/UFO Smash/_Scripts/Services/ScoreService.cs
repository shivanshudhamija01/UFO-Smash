using UnityEngine;

public class ScoreService : IScoreService
{
    private int score;
    public ScoreService()
    {
        score = 0;
    }
    public int GetScore()
    {
        return score;
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Score added: " + amount + ". Total Score: " + score);
    }
}
