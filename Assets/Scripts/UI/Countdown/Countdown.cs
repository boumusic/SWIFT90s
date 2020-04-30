using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Countdown : MonoBehaviour
{
    private static Countdown instance;
    public static Countdown Instance
    {
        get
        {
            if (!instance) instance = FindObjectOfType<Countdown>();
            return instance;
        }
    }

    private int count;
    private Action onEnd;
    [SerializeField] private CountdownInstance[] pool;

    private void Awake()
    {
        //StartCountdown(5, Stop);
    }

    private void Stop()
    {
        Debug.Log("Stop");
    }

    private string endMessage = "";

    public void StartCountdown(int count, Action onEnd = null, string end = "")
    {
        this.count = count;
        this.onEnd = onEnd;
        endMessage = end;
        StartCoroutine(CountingDown());
    }

    private IEnumerator CountingDown()
    {
        for (int i = count; i >= 0; i--)
        {
            if (i == 3) AudioManager.instance.PlaySound(AudioManager.instance.AS_Announcer, AudioManager.instance.AC_Countdown_3);
            else if (i == 2) AudioManager.instance.PlaySound(AudioManager.instance.AS_Announcer, AudioManager.instance.AC_Countdown_2);
            else if (i == 1) AudioManager.instance.PlaySound(AudioManager.instance.AS_Announcer, AudioManager.instance.AC_Countdown_1);
            else if (i == 0) AudioManager.instance.PlaySound(AudioManager.instance.AS_Announcer, AudioManager.instance.AC_Countdown_Fight);

            CountdownInstance inst = GetInstance();
            inst.gameObject.SetActive(true);

            if (i == 0 && endMessage != "")
                inst.In(i, endMessage);
            else
                inst.In(i);
            yield return new WaitForSeconds(1f);
        }
        onEnd?.Invoke();
    }

    private CountdownInstance GetInstance()
    {
        for (int i = 0; i < pool.Length; i++)
        {
            if (!pool[i].gameObject.activeInHierarchy)
            {
                return pool[i];
            }
        }

        return null;
    }
}
