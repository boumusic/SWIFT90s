using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public bool debugLogMessages = false;

    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (!instance) instance = FindObjectOfType<UIManager>();
            return instance;
        }
    }

    private NetworkedPlayer player;
    public NetworkedPlayer Player { get { if (!player) player = FindObjectOfType<NetworkedPlayer>(); return player; } }

    public NetworkedPlayer NetworkedPlayer => Player.GetComponentInParent<NetworkedPlayer>();

    private Character character => player == null ? null : player.character;

    [Header("Components")]
    public Canvas canvas;
    public UIGeneralMessage generalMessage;
    //public UIScoreboard scoreboard;

    [Header("Kill Feed")]
    public GameObject prefabKillFeed;
    public Transform killFeedParent;
    public float killFeedSpacing = 100f;
    public float killFeedOffset = 50f;

    private List<UIKillFeed> killFeeds = new List<UIKillFeed>();

    //[Header("Jauges")]
    //public UIJauge wallJauge;
    //public UIJauge dashJauge;

    [Header("Pause")]
    public GameObject pause;
    private bool isPaused = false;
    public bool IsPaused => isPaused;

    [Header("Flag Status")]
    public GameObject flagStatus;

    [Header("UI FlagZone")]
    public GameObject uiFlagZonePrefab;
    //private List<Zone> flagZones = new List<Zone>();

    [Header("Debug")]
    public TextMeshProUGUI sensText;
    public TextMeshProUGUI flowText;

    private void Start()
    {
       //AssignPlayer(FindObjectOfType<NetworkedPlayer>());
    }

    public void AssignPlayer(NetworkedPlayer p)
    {
        player = p;
        if (player)
        {
            //dashJauge.Init(ref character.OnStartDash, ref character.OnDashReady);
            //wallJauge.Init(ref character.OnStartWallclimb, ref character.OnWallclimbReady);


            /*
            //arg
            foreach (var item in FindObjectsOfType<UI360>())
            {
                item.AssignPlayer();
            }
            */
        }
    }

    private void Update()
    {
        if (player == null) return;

        PositionKillFeeds();
        UpdateCooldowns();

        //scoreboard.gameObject.SetActive(player.Tab);
        //sensText.text = "sensitivity : " + player.sensitivity.ToString("F3") + "/1";
        //flowText.text = "Flow : " + player.Character.CurrentFlow.ToString("F2") + "/200";
    }

    public void LogMessage(string message)
    {
        message = message.ToUpper();
        if (debugLogMessages) Debug.Log(message);
        generalMessage.Message(message);
    }

    public void UpdateCooldowns()
    {
        if (player)
        {
            //dashJauge.UpdateJauge(player.Character.DashCooldownProgress);
            //wallJauge.UpdateJauge(player.Character.WallClimbCharge);
        }
    }

    /*
    public void RegisterFlagZone(Zone zone)
    {
        flagZones.Add(zone);
        UIFlag uiFlag = NewPing(zone);
        uiFlag.Init(zone.teamIndex, zone.type);
    }

    private UIFlag NewPing(Zone zone)
    {
        GameObject newUI = Instantiate(uiFlagZonePrefab, canvas.transform);
        UIFlag uiFlag = newUI.GetComponent<UIFlag>();
        uiFlag.GetComponent<UI360>().FeedTarget(zone.gameObject);
        return uiFlag;
    }

    public void RegisterFlagZones(List<Zone> zones)
    {
        for (int i = 0; i < zones.Count; i++)
        {
            RegisterFlagZone(zones[i]);
        }
    }
    */

    public void TogglePause()
    {
        isPaused = !isPaused;
        pause.SetActive(isPaused);
        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }
    

    #region KillFeed

    public void DisplayKillFeed(Character killer, Character killed)
    {
        DisplayKillFeed(killer.PlayerName, killer.TeamIndex, killed.PlayerName, killed.TeamIndex);
    }

    public void DisplayKillFeed(string killerName, int killerTeam, string killedName, int killedTeam)
    {
        GameObject newKillFeed = Instantiate(prefabKillFeed, killFeedParent);
        newKillFeed.transform.parent = killFeedParent;
        newKillFeed.transform.localPosition = new Vector3(newKillFeed.transform.localPosition.x, -800f, 0f);
        newKillFeed.gameObject.SetActive(true);
        UIKillFeed kf = newKillFeed.GetComponent<UIKillFeed>();
        kf.Init(killerName, killerTeam, killedName, killedTeam);
        killFeeds.Add(kf);
    }

    private void PositionKillFeeds()
    {
        for (int i = 0; i < killFeeds.Count; i++)
        {
            killFeeds[i].UpdatePosition(new Vector3(0f, -killFeedSpacing * i - killFeedOffset));
        }
    }

    public void UnregisterKillFeed(UIKillFeed kf)
    {
        killFeeds.Remove(kf);
        Destroy(kf.gameObject);
    }

    #endregion
}
