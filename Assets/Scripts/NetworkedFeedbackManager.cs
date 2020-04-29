using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedFeedback
{
    public AudioClip clip;
    public GameObject particleSystem;
}

public class NetworkedFeedbackManager : MonoBehaviour
{
    private static NetworkedFeedbackManager instance;
    public static NetworkedFeedbackManager Instance
    {
        get { if (!instance) instance = FindObjectOfType<NetworkedFeedbackManager>(); return instance; }
    }
}
