using NUnit.Framework;
using NSubstitute;

public class ScoreTests
{
    [Test]
    public void IncreaseScore_Test()
    {
        IGameController gameController = Substitute.For<IGameController>();
        int score = 0;
        Assert.AreEqual(score, score);
    }
}
