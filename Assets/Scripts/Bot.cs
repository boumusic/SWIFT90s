using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    public float attackDelay = 2f;
    public Character character;

    private void Start()
    {
        StartCoroutine(Attacking());
    }

    private IEnumerator Attacking()
    {
        yield return new WaitForSeconds(attackDelay);
        character.StartAttack();
        StartCoroutine(Attacking());
    }

}
