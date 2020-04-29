using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pataya.QuikFeedback;

public class CharacterFeedbacks : MonoBehaviour
{
    public bool FindObjects = true;
    public QuikFeedback[] quikFeedbacks;

    private void Start()
    {
        if(FindObjects)
        {
            quikFeedbacks = GetComponentsInChildren<QuikFeedback>(true);
        }
    }

    public void Play(string name)
    {
        for (int i = 0; i < quikFeedbacks.Length; i++)
        {
            if(quikFeedbacks[i].feedbackName == name)
            {
                quikFeedbacks[i].Play();
            }
        }
    }
}
