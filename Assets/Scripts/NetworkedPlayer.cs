using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkedPlayer : NetworkBehaviour
{
    public Character character;
    public string PlayerName => character.PlayerName;

    private void Start()
    {
        if (!hasAuthority)
        {
            character.enabled = false;

            GetComponent<Rigidbody>().isKinematic = true;
        }

        TeamManager.Instance.JoinSmallestTeam(this);
    }
}
