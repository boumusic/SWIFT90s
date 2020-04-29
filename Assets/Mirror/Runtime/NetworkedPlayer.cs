using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkedPlayer : NetworkBehaviour
{
    [SyncVar]
    public int teamIndex;

    public Character character;
    public string PlayerName => character.PlayerName;
    public Team Team => TeamManager.Instance.teams[teamIndex];

    private void Start()
    {
        character.Initialize(this);
        if (!hasAuthority)
        {
            character.enabled = false;

            GetComponent<Rigidbody>().isKinematic = true;
        }

        else
        {
            UIManager.Instance.AssignPlayer(this);
        }

        TeamManager.Instance.JoinTeam(teamIndex, this);
    }
}
