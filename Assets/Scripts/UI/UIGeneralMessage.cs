using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIGeneralMessage : MonoBehaviour
{
    public Animator animator;
    public TextMeshProUGUI textMessage;
    public float delayMessage = 2f;
    private Coroutine delay;

    public void Message(string message)
    {
        textMessage.text = message;
        animator.SetTrigger("In");
        if (delay != null) StopCoroutine(delay);
        delay = StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(delayMessage);
        animator.SetTrigger("Out");
    }
}
