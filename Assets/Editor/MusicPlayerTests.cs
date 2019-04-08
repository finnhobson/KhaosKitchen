using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class MusicPlayerTests
{

    [Test] 
    public void IsRoundPausedNotNull()
    {
        MusicPlayer music = new MusicPlayer();
        Assert.NotNull(music.IsRoundPaused);
    }

    [Test]
    public void IsRoundPausedStartUp()
    {
        MusicPlayer music = new MusicPlayer();
        Assert.IsFalse(music.IsRoundPaused);
    }

    [Test]
    public void IsMusicPausedNotNull()
    {
        MusicPlayer music = new MusicPlayer();
        Assert.NotNull(music.IsMusicPaused);
    }

    [Test]
    public void IsMusicPausedStartUp()
    {
        MusicPlayer music = new MusicPlayer();
        Assert.IsFalse(music.IsMusicPaused);
    }
}
