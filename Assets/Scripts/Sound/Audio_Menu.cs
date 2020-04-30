using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Menu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartMusic()
    {
        AudioManager.instance.StartCoroutine(AudioManager.instance.FadeIn(AudioManager.instance.AS_Music, AudioManager.instance.AC_MenuTheme, 2f,0.7f));


    }

    public void LogoSound()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.AS_Feedback, AudioManager.instance.AC_JoinTheGame);
    }

    public void FadeSound()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.AS_Feedback, AudioManager.instance.AC_DodgeWhileAttacked);
    }

    public void Whoosh()
    {
        AudioManager.instance.PlaySoundFullRandom(AudioManager.instance.AS_Feedback, AudioManager.instance.AC_Dodge);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
