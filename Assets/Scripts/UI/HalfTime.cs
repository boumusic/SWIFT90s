using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfTime : MonoBehaviour
{
    public Animator animator;

    public void In()
    {
        animator.SetTrigger("In");
    }

    public void Out()
    {
        animator.SetTrigger("Out");
    }
}
