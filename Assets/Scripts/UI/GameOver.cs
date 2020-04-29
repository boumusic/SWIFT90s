using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;

    public void DisplayGameOver(bool victory)
    {
        animator.SetTrigger("In");
    }
}
