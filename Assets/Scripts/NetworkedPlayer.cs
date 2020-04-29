using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkedPlayer : NetworkBehaviour
{
    public Character character;
    public string PlayerName => character.PlayerName;
    public int TeamIndex { get; private set; }
    public Team Team => TeamManager.Instance.teams[TeamIndex];

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

        TeamIndex = TeamManager.Instance.JoinSmallestTeam(this);
    }
}
