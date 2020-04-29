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

    public void StartCountdown(int count, Action onEnd = null)
    {
        this.count = count;
        this.onEnd = onEnd;
        StartCoroutine(CountingDown());
    }

    private IEnumerator CountingDown()
    {
        for (int i = count; i > 0; i--)
        {
            CountdownInstance inst = GetInstance();
            inst.gameObject.SetActive(true);
            inst.In(i);
            yield return new WaitForSeconds(1f);
        }
        onEnd?.Invoke();
    }

    private CountdownInstance GetInstance()
    {
        for (int i = 0; i < pool.Length; i++)
        {
            if(!pool[i].gameObject.activeInHierarchy)
            {
                return pool[i];
            }
        }

        return null;
    }
}
