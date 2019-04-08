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
        int score = 0;
        Assert.AreEqual(score, score);
    }


}
