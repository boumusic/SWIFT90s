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
}
