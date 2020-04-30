using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerCounterUI : MonoBehaviour
{
    [Header("Components")]
    public TextMeshProUGUI text;
    
    public void UpdateUI(int currentPlayerCount, int maxPlayerCount)
    {
        text.text = currentPlayerCount + "/" + maxPlayerCount;
    }
}
