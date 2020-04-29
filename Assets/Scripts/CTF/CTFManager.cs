using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTFManager : MonoBehaviour
{
    private static CTFManager instance;
    public static CTFManager Instance
    {
        get { if (!instance) instance = FindObjectOfType<CTFManager>(); return instance; }
    }

    private List<LevelZone> zones = new List<LevelZone>();

    [Header("Gameplay Rules")]
    public int minutes = 5;
    public int seconds = 0;
    public int goalPoints = 3;

    public float halfTimeDuration = 5f;
    private bool reachedHalfTime = false;

    private Timer timer;
    public Timer Timer => timer;

    private void Start()
    {
        StartTimer();
    }

    private void StartTimer()
    {
        timer = new Timer(minutes, seconds, TimerOver);
        timer.Start();
    }

    private void Update()
    {
        timer.Update();
    }

    private bool isDraw = false;

    private void TimerOver()
    {
        if (reachedHalfTime)
        {
            if (!isDraw)
            {
                GameOver();
            }
            else
            {
                // Trigger boolean d'overtime, le prochain flag est décisif
            }
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

        TeamManager.Instance.ToggleInputs(false);
    }

    private void StartHalfTime()
    {
        reachedHalfTime = true;

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
        yield return new WaitForSeconds(halfTimeDuration);

        //Enable les Inputs
        TeamManager.Instance.ToggleInputs(true);

        StartTimer();
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
