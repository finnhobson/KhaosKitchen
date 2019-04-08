using NUnit.Framework;

public class PauseGameTests
{
    [Test]
    public void PlayerPauseNotNull_Test()
    {
        var player = new Player();
        Assert.NotNull(player.IsGamePaused);
    }

    [Test]
    public void PlayerPausedInitialFalse_Test()
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
}
