using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    public TextMeshProUGUI text;

    private void Update()
    {
        text.text = CTFManager.Instance.Timer?.GetTimeLeftString();
    }
}
