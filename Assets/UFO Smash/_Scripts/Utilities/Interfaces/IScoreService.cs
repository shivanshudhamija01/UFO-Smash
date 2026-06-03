public interface IScoreService
{
    int GetScore();

    void AddScore(int amount);
    int GetHighScore();
}