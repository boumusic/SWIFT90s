using UnityEngine;
using Mirror;

public class SwiftNetworkManager : NetworkManager
{
    public override GameObject OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject player = base.OnServerAddPlayer(conn);

        player.GetComponent<NetworkedPlayer>().teamIndex = (conn.connectionId % 2 == 0) ? 0 : 1;

        return player;
    }
}