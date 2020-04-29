using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOver : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;
    public TextMeshProUGUI text;

    public void DisplayGameOver(bool victory)
    {
        string txt = victory ? "Victory" : "Defeat";
        animator.SetTrigger(txt);
        text.text = txt;
    }
}
