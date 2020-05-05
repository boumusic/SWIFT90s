using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkedPlayer : NetworkBehaviour
{
    [SyncVar]
    private int teamIndex = 0;
    [SyncVar(hook = nameof(UpdateName))]
    private string username = "Krusher98";

    public Character character;
    public NetworkAnimator animator;
    public Team Team => TeamManager.Instance.teams[TeamIndex];

    [Header("Inputs")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode attackKey = KeyCode.Mouse0;
    public KeyCode dodgeKey = KeyCode.Mouse1;
    public KeyCode tauntKey = KeyCode.E;
    private bool inputEnabled => TeamManager.Instance.InputEnabled;

    public int TeamIndex { get => teamIndex; set => teamIndex = value; }
    public string Username { get => username;}

    [HideInInspector] public Vector3 spawnPosition;

    private void Start()
    {
        character.Initialize();
        if (!hasAuthority)
        {
            character.enabled = false;
            character.UpdateTexture();

            GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            CmdUpdateName(FindObjectOfType<PlayerInfo>().username);

            spawnPosition = transform.position;

            UIManager.Instance.AssignPlayer(this);

            //character.animator.onAttackEndAnim += () => animator.SetTrigger("Attack");
            //character.animator.onDoubleJumpAnim += () => animator.SetTrigger("DoubleJump");
            //character.animator.onJumpAnim += () => animator.SetTrigger("Jump");
            //character.animator.onLandAnim += () => animator.SetTrigger("Land");
            //character.animator.onDeathAnim += () => animator.SetTrigger("Death");
            //character.animator.onDodgeAnim += () => animator.SetTrigger("Dodge");
        }

        TeamManager.Instance.JoinTeam(TeamIndex, this);
    }

    [Command]
    void CmdUpdateName(string name)
    {
        username = name;
    }

    public void UpdateName(string oldValue, string newValue)
    {
        character.UpdateTextName();
        UIManager.Instance.RefreshPortraits();
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

        if (Input.GetKeyDown(tauntKey))
        {
            character.Taunt();
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

        transform.position = TeamIndex == 0 ? NetworkManager.startPosition0.position : NetworkManager.startPosition1.position;

        spawnPosition = transform.position;
    }
}
