using UnityEngine;
using Mirror;

public class SwiftNetworkManager : NetworkManager
{
    public override GameObject OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject player = base.OnServerAddPlayer(conn);

        player.GetComponent<NetworkedPlayer>().teamIndex = (conn.connectionId % 2 == 0) ? 0 : 1;

        if (numPlayers == maxConnections)
        {
            CTFManager.Instance.RpcStartTimer();
        }
        CTFManager.Instance.RpcUpdatePlayerCount(numPlayers, maxConnections);

        Debug.Log("ree");
        return player;
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);

        CTFManager.Instance.RpcUpdatePlayerCount(numPlayers, maxConnections);
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, NetworkIdentity player)
    {
        base.OnServerRemovePlayer(conn, player);

        CTFManager.Instance.RpcUpdatePlayerCount(numPlayers, maxConnections);
    }
}