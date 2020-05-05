using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    public float attackDelay = 2f;
    public Character character;
    private float horiz;

    private void Start()
    {
        horiz = 1;
        StartCoroutine(Attacking());
    }

    private void Update()
    {
        character.InputHorizontal(horiz);
    }

    private IEnumerator Attacking()
    {
        yield return new WaitForSeconds(attackDelay);
        horiz *= -1f;
        StartCoroutine(Attacking());
    }

}
