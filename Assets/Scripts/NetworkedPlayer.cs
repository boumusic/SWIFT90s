using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkedPlayer : NetworkBehaviour
{
    [SyncVar]
    public int teamIndex;

    public Character character;
    public NetworkAnimator animator;
    public string PlayerName => character.PlayerName;
    public Team Team => TeamManager.Instance.teams[teamIndex];

    [Header("Inputs")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode attackKey = KeyCode.Mouse0;
    public KeyCode dodgeKey = KeyCode.Mouse1;
    private bool inputEnabled => TeamManager.Instance.InputEnabled;

    [HideInInspector] public Vector3 spawnPosition;

    private void Start()
    {
        character.Initialize(this);
        if (!hasAuthority)
        {
            character.enabled = false;
            character.UpdateTexture();

            GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            spawnPosition = transform.position;

            UIManager.Instance.AssignPlayer(this);

            //character.animator.onAttackEndAnim += () => animator.SetTrigger("Attack");
            character.animator.onDoubleJumpAnim += () => animator.SetTrigger("DoubleJump");
            character.animator.onJumpAnim += () => animator.SetTrigger("Jump");
            //character.animator.onLandAnim += () => animator.SetTrigger("Land");
            character.animator.onDeathAnim += () => animator.SetTrigger("Death");
            character.animator.onDodgeAnim += () => animator.SetTrigger("Dodge");
        }

        TeamManager.Instance.JoinTeam(teamIndex, this);
    }

    private void Update()
    {
        if (!hasAuthority) return;

        Inputs();
    }
    

    private void Inputs()
    {
        if (!inputEnabled)
        {
            character.InputHorizontal(0);
            character.InputVertical(0);
            return;
        }

        if (Input.GetKeyDown(jumpKey))
        {
            character.ReceiveJumpInput();
        }

        if (Input.GetKeyDown(attackKey))
        {
            character.StartAttack();
        }

        if (Input.GetKeyDown(dodgeKey))
        {
            character.Dodge();
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        character.InputHorizontal(horizontal);
        character.InputVertical(vertical);
    }

    private void OnDestroy()
    {
        Team.Leave(this);
    }

    [Command]
    public void CmdKillPlayer(NetworkIdentity killerID, NetworkIdentity victimID)
    {
        Debug.Log(victimID);
        RpcKillPlayer(killerID, victimID);
    }

    [ClientRpc]
    public void RpcKillPlayer(NetworkIdentity killerID, NetworkIdentity victimID)
    {
        character.Kill(victimID.gameObject.GetComponent<Character>());
    }

    [ClientRpc]
    public void RpcRespawn()
    {
        character.DropFlag();

        transform.position = spawnPosition;

    }

    [ClientRpc]
    public void RpcSwitchSides()
    {
        character.DropFlag();

        if (!hasAuthority) return;

        transform.position = teamIndex == 0 ? NetworkManager.startPosition0.position : NetworkManager.startPosition1.position;

        spawnPosition = transform.position;
    }
}
