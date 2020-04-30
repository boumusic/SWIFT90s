using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Timer
{
    public float minutes;
    public float seconds;
    private Action action;

    private float timeLeft = 0f;
    private bool isFinished = false;
    private bool isStarted = false;

    public float TimeLeft { get => timeLeft; set => timeLeft = value; }

    public Timer(int minutes, int seconds, Action action = null)
    {
        this.minutes = minutes;
        this.seconds = seconds;
        this.action = action;
        timeLeft = minutes * 60 + seconds;
    }

    public void Start()
    {
        isStarted = true;
    }

    public void Stop()
    {
        isStarted = false;
    }

    public void Update()
    {
        if(isStarted)
        {
            if (timeLeft <= 0)
            {
                if (!isFinished)
                {
                    action?.Invoke();
                    isFinished = true;
                    timeLeft = 0f;
                    isStarted = false;
                }
            }

            else
            {
                timeLeft -= Time.deltaTime;
            }
        }        
    }

    public string GetTimeLeftString()
    {
        string left = "";
        int mins = (int)(timeLeft / 60);
        if (mins < 10) left += "0";
        left += mins.ToString();
        left += ":";
        int secs = (int)(timeLeft - (mins * 60));
        if (secs < 10) left += "0";
        left += secs.ToString();
        return left;
    }
}
