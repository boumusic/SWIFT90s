using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_SoundTest : MonoBehaviour
{
    AudioManager AM;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("aaaaaiou");
       AM = AudioManager.instance;

        AudioManager.instance.PlaySound(AudioManager.instance.AS_Announcer, AudioManager.instance.AC_Countdown_1);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log(" A Pressed");
            AM.PlaySoundRandomPitch(AM.AS_Fight, AM.AC_Hit);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            AM.PlaySoundFullRandom(AM.AS_Movement, AM.AC_FootStep);
        }

    }
}
