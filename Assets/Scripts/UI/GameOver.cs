using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOver : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;
    public TextMeshProUGUI text;

    public void DisplayGameOver(Outcome outcome)
    {
        string txt = outcome.ToString();
        animator.SetTrigger(txt);
        text.text = txt;
    }
}
