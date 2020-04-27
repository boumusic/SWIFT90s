using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleDebug : MonoBehaviour
{
    public bool enable = false;
    [Range(0,3)] public float timeScale = 1f;

    private void Update()
    {
        if(enable)
        {
            Time.timeScale = timeScale;
        }
    }
}
