using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkedPlayer : NetworkBehaviour
{
    public Character character;

    private void Start()
    {
        if (!hasAuthority)
        {
            character.enabled = false;

            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
