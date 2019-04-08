using NUnit.Framework;

public class PauseGameTests
{
    [Test]
    public void PlayerPauseGameState_Test()
    {
        var player = new Player();
        player.PausePlayer();
        Assert.AreEqual(true, player.IsGamePaused);
    }
}
