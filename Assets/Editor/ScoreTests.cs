using NUnit.Framework;

public class ScoreTests
{
    [Test]
    public void ScoreNotNull_Test()
    {
        GameController gameController = new GameController();
        Assert.NotNull(gameController.score);
    }

    [Test]
    public void IncreaseScore_Test()
    {
        GameController gameController = new GameController();
        gameController.IncreaseScore();
        var gameScore = gameController.score;
        int score = 0;
        Assert.AreEqual(score, gameScore);
    }

    [Test]
    public void ScoreStreakNotNull_Test()
    {
        Player player = new Player();
        Assert.NotNull(player.ScoreStreak);
    }

    [Test]
    public void IncreaseScoreStreak_Test() 
    {
        Player player = new Player();
        player.IncreaseScoreStreak();
        Assert.AreEqual(1, player.ScoreStreak);
    }

    [Test]
    public void ScoreStreakMaxExceed_Test()
    {
        Player player = new Player();
        player.IncreaseScoreStreak();
        player.IncreaseScoreStreak();
        player.IncreaseScoreStreak();
        Assert.AreEqual(3, player.ScoreStreak);
    }


}
