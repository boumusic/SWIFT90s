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

    private Timer timer;
    public Timer Timer => timer;

    private void Start()
    {
        timer = new Timer(minutes, seconds, TimerOver);
        timer.Start();
    }

    private void Update()
    {
        timer.Update();
    }

    private void TimerOver()
    {
        Debug.Log("Time is up !");
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
            if(zones[i] is Altar && zones[i].teamIndex == teamIndex)
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
