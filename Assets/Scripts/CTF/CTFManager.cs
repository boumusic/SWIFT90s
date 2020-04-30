using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CTFManager : NetworkBehaviour
{
    private static CTFManager instance;
    public static CTFManager Instance
    {
        get { return instance; }
    }

    public void Awake()
    {
        instance = this;
    }

    private List<LevelZone> zones = new List<LevelZone>();

    [Header("Gameplay Rules")]
    public int minutes = 5;
    public int seconds = 0;
    public int goalPoints = 3;

    public float respawnTimer;

    public float halfTimeDuration = 5f;
    private bool reachedHalfTime = false;

    private Timer timer;
    public Timer Timer => timer;

    private void OnEnable()
    {
        timer = null;
    }

    public void ServerStartTimer()
    {
        Debug.Log("server is calling rpcstarttimer");
        RpcStartTimer();
    }

    [ClientRpc]
    public void RpcStartTimer()
    {
        Debug.Log("start");
        Countdown.Instance.StartCountdown(3, ()=>
        {
            timer = new Timer(minutes, seconds, TimerOver);
            timer.Start();

            TeamManager.Instance.ToggleInputs(true);

        }, "FIGHT");

    }

    public void ServerUpdatePlayerCount(int playerCount, int maxPlayerCount)
    {
        RpcUpdatePlayerCount(playerCount, maxPlayerCount);
    }

    [ClientRpc]
    public void RpcUpdatePlayerCount(int playerCount, int maxPlayerCount)
    {
        UIManager.Instance.UpdatePlayerCounterUI(playerCount, maxPlayerCount);
    }

    private void Update()
    {
        timer?.Update();
    }

    private bool isDraw => TeamManager.Instance.IsDraw;

    private void TimerOver()
    {
        if (reachedHalfTime)
        {
            GameOver();
        }

        else
        {
            StartHalfTime();
        }

    }

    public void TeamWins(Team team)
    {
        GameOver();
    }

    private void GameOver()
    {
        //Game over
        //Spawn l'écran de victoire/défaite
        UIManager.Instance.DisplayEndgameScreen();
        timer.Stop();
        TeamManager.Instance.ToggleInputs(false);
    }

    private void StartHalfTime()
    {
        AudioManager AM = AudioManager.instance;
        reachedHalfTime = true;
        AM.PlaySound(AM.AS_Feedback, AM.AC_RefereeWhistle);
        AM.PlaySoundRandomInList(AM.AS_Announcer, AM.AC_HalfTime);

        //Spawn l'écran de halftime
        UIManager.Instance.DisplayHalftimeMessage();

        //Disable les Inputs
        TeamManager.Instance.ToggleInputs(false);

        //Reset les flags
        //Inverser les spawns
        //Reset les positions des joueurs à leurs nouveaux spawns
        StartCoroutine(HalfTime());
    }

    private IEnumerator HalfTime()
    {
        if (isServer)
        {
            RpcSwitchLevelZonesSides();
            foreach (var player in FindObjectsOfType<NetworkedPlayer>())
            {
                player.RpcSwitchSides();
            }
        }
        
        yield return new WaitForSeconds(halfTimeDuration);


        Countdown.Instance.StartCountdown(3, () =>
        {
            timer = new Timer(minutes, seconds, TimerOver);
            timer.Start();

            TeamManager.Instance.ToggleInputs(true);

        }, "FIGHT");

    }

    [ClientRpc]
    public void RpcSwitchLevelZonesSides()
    {
        List<Shrine> shrines = new List<Shrine>();
        List<Vector3> shrinePositions = new List<Vector3>();

        List<Altar> altars = new List<Altar>();
        List<Vector3> altarPositions = new List<Vector3>();

        foreach (var shrine in FindObjectsOfType<Shrine>())
        {
            shrines.Add(shrine);
            shrinePositions.Add(shrine.transform.position);
        }
        foreach (var altar in FindObjectsOfType<Altar>())
        {
            altars.Add(altar);
            altarPositions.Add(altar.transform.position);
        }
        altars.OrderBy((x) => x.teamIndex);

        shrines[0].transform.position = shrinePositions[1];
        shrines[1].transform.position = shrinePositions[0];

        altars[0].transform.position = altarPositions[2];
        altars[1].transform.position = altarPositions[3];
        altars[2].transform.position = altarPositions[0];
        altars[3].transform.position = altarPositions[1];
    }

    private void InvertAllZones()
    {
        for (int i = 0; i < zones.Count; i++)
        {
            zones[i].teamIndex = 1 - zones[i].teamIndex;
        }
    }

    public void RegisterZone(LevelZone zone)
    {
        zones.Add(zone);
    }

    public void UnregisterZone(LevelZone zone)
    {
        zones.Remove(zone);
    }

    public void CapturedFlagOfTeam(int teamIndex)
    {
        for (int i = 0; i < zones.Count; i++)
        {
            if (zones[i] is Altar && zones[i].teamIndex == teamIndex)
            {
                (zones[i] as Altar).Enable(false);
            }
        }
    }

    public void ScoredFlagOfTeam(int teamIndex)
    {
        for (int i = 0; i < zones.Count; i++)
        {
            if (zones[i] is Altar && zones[i].teamIndex == teamIndex)
            {
                (zones[i] as Altar).ResetFlag();
                (zones[i] as Altar).Enable(true);
            }
        }

    }
}
