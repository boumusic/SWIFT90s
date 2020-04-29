using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownInstance : MonoBehaviour
{
    [Header("Components")]
    public TextMeshProUGUI text;

    public void In(int count)
    {
        text.text = count.ToString();
        StartCoroutine(Deactivate());
    }

    private IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
