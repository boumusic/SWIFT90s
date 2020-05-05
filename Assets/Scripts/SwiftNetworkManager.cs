using UnityEngine;
using Mirror;
using System.Collections;

public class SwiftNetworkManager : NetworkManager
{
    public TeamManager teamManager;
    public CTFManager ctfManager;

    public override void Awake()
    {
        base.Awake();

        ctfManager.Awake();
        teamManager.Awake();

    }

    public override void Start()
    {

        switch (FindObjectOfType<PlayerInfo>().role)
        {
            case 0:
                StartClient();
                break;
            case 1:
                StartHost();
                break;
            case 2:
                StartServer();
                break;
            default:
                break;
        }
    }

    public override GameObject OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject player = base.OnServerAddPlayer(conn);

        player.GetComponent<NetworkedPlayer>().TeamIndex = (numPlayers % 2 == 0) ? 1 : 0;

        if (numPlayers == maxConnections)
        {
            Debug.Log("trying server start timer");
            CTFManager.Instance.ServerStartTimer();
        }
        CTFManager.Instance.ServerUpdatePlayerCount(numPlayers, maxConnections);

        return player;
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);

        CTFManager.Instance.ServerUpdatePlayerCount(numPlayers, maxConnections);
        UIManager.Instance.UpdatePlayerCounterUI(numPlayers, maxConnections);
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, NetworkIdentity player)
    {
        base.OnServerRemovePlayer(conn, player);

        CTFManager.Instance.ServerUpdatePlayerCount(numPlayers, maxConnections);
        UIManager.Instance.UpdatePlayerCounterUI(numPlayers, maxConnections);
    }
}