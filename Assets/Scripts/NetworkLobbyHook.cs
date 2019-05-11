using UnityEngine;
using UnityEngine.Networking;

public class NetworkLobbyHook : MonoBehaviour
{
    public void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        OldLobbyPlayer lp = lobbyPlayer.GetComponent<OldLobbyPlayer>();
        Player player = gamePlayer.GetComponent<Player>();

        player.PlayerUserName = lp.UserName;
        player.PlayerColour = lp.UserColour;
    }
}
