using NUnit.Framework;

public class GameStateTests
{
    [Test]
    public void IsGameStartedNotNull_Test()
    {
        GameController gameController = new GameController();
        Assert.NotNull(gameController.isGameStarted);
    }

    [Test]
    public void IsGameStartedInitialStartUp_Test()
    {
        GameController gameController = new GameController();
        Assert.IsFalse(gameController.isGameStarted);
    }

    [Test]
    public void RoundTimerInitialisation_Test()
    {
        GameSettings.RoundTime = 90;
        GameController gameController = new GameController();
        gameController.StartRoundTimer();
        Assert.AreEqual(0, gameController.roundStartTime);
    }


    [Test]
    public void PlayerPauseNotNull_Test()
    {
        var player = new Player();
        Assert.NotNull(player.IsGamePaused);
    }

    [Test]
    public void PlayerPausedInitialStartUp_Test()
    {
        var player = new Player();
        Assert.IsFalse(player.IsGamePaused);
    }

    [Test]
    public void PlayerPauseGameState_Test()
    {
        var player = new Player();
        player.PausePlayer();
        Assert.AreEqual(true, player.IsGamePaused);
    }

    [Test]
    public void IsGameOverNotNull_Test()
    {
        GameController gameController = new GameController();
        Assert.NotNull(gameController.IsGameOver);
    }

    [Test]
    public void IsGameOverInitialSetUp_Test()
    {
        GameController gameController = new GameController();
        Assert.IsFalse(gameController.IsGameOver);
    }

    [Test]
    public void IsGameOver_Test()
    {
        GameController gameController = new GameController();
        gameController.GameOver();
        Assert.AreEqual(true, gameController.IsGameOver);
    }

}
