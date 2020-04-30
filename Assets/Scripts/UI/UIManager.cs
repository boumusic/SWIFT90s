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

    [Header("Half Time")]
    public HalfTime halfTime;
    public float halfTimeMessageDuration = 3;

    [Header("Pause")]
    public GameObject pause;
    private bool isPaused = false;
    public bool IsPaused => isPaused;

    [Header("UI FlagZone")]
    public GameObject uiFlagZonePrefab;
    private List<LevelZone> flagZones = new List<LevelZone>();
    
    [Header("Game over")]
    public GameOver gameOver;

    [Header("Portraits")]
    public float portraitOffset = 20f;
    public float portraitHeight = 20f;
    public float portraitSpacing = 300f;
    public GameObject portraitPrefab;
    public Transform portraitParent;
    private List<PlayerPortrait> portraits = new List<PlayerPortrait>();
    
    private void Start()
    {
       //AssignPlayer(FindObjectOfType<NetworkedPlayer>());
    }

    public void AssignPlayer(NetworkedPlayer p)
    {
        player = p;
    }

    private void Update()
    {
        if (player == null) return;

        PositionKillFeeds();

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

    public void DisplayHalftimeMessage()
    {
        halfTime.In();
        StartCoroutine(HalfTimeDuration());
    }

    public void DisplayEndgameScreen()
    {
        bool won = Player.Team.HasWon;
        gameOver.DisplayGameOver(won);

        AudioManager AM = AudioManager.instance;

        if(won)
        {
            AM.PlaySoundRandomInList(AM.AS_Announcer, AM.AC_Victory);
        }

        else
        {
            AM.PlaySoundRandomInList(AM.AS_Announcer, AM.AC_Defeat);
        }
    }

    private IEnumerator HalfTimeDuration()
    {
        yield return new WaitForSeconds(halfTimeMessageDuration);
        halfTime.Out();
    }

    public void RefreshPortraits()
    {
        Debug.Log("Refresh");
        for (int i = 0; i < portraits.Count; i++)
        {
            Destroy(portraits[i].gameObject);
        }

        portraits.Clear();

        TeamManager tm = TeamManager.Instance;
        int index = 0;
        for (int t = 0; t < tm.teams.Count; t++)
        {
            for (int p = 0; p < tm.teams[t].players.Count; p++)
            {
                GameObject newPortrait = Instantiate(portraitPrefab, portraitParent);
                PlayerPortrait portrait = newPortrait.GetComponent<PlayerPortrait>();
                portraits.Add(portrait);
                Vector3 position = new Vector3(portraitOffset + index * 300 * (t == 0 ? 1 : -1), portraitHeight, 0);
                portrait.rect.anchorMin = new Vector2(t, 0);
                portrait.rect.anchorMax = new Vector2(t, 0);
                portrait.rect.anchoredPosition = position;
                portrait.UpdateVisuals(tm.teams[t].players[p].character);
                index++;
            }
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
