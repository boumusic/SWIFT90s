using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("AudioSources")]
    public AudioSource AS_Announcer, AS_Movement, AS_Fight, AS_Feedback, AS_Music;

    [Header("Audioclips - Full Random")]
    public AudioClip[] AC_FootStep, AC_Jump, AC_DJump, AC_Dodge, AC_WallJump, AC_Attack, AC_Landing;

    [Header("Audioclips - Random pitch")]
    public AudioClip AC_Kill, AC_Death, AC_FlagTookAlly, AC_FlagTookEnemy;

    [Header("Audioclips - No Alteration")]
    public AudioClip AC_DodgeWhileAttacked, AC_Hit, AC_FlagAura;
    public AudioClip AC_ScoreAlly_01, AC_ScoreAlly_02, AC_ScoreAlly_03, AC_ScoreEnemy_01, AC_ScoreEnemy_02, AC_ScoreEnemy_03;

    [Header("Audioclips - Announcer")]
    public AudioClip AC_Countdown_3, AC_Countdown_2, AC_Countdown_1, AC_Countdown_Go, AC_Countdown_Fight;

    public AudioClip[] AC_Defeat, AC_Victory, AC_Draw, AC_SuddenDeath;

    [Header("Audioclips - Music")]
    public AudioClip AC_MenuTheme, AC_BattleTheme, AC_EndTheme;


    public void PlaySound(AudioSource audiosource, AudioClip audioclip)
    {
        audiosource.clip = audioclip;
        audiosource.Play();
    }

    public void PlaySoundRandomPitch(AudioSource audiosource, AudioClip audioclip)
    {
        audiosource.clip = audioclip;
        audiosource.pitch = Random.Range(0.9f, 1.1f);
        audiosource.Play();

    }

    public void PlaySoundRandomInList(AudioSource audiosource, AudioClip[] audioClips)
    {
        audiosource.clip = audioClips[Random.Range(0, audioClips.Length)];
        audiosource.Play();
    }

    public void PlaySoundFullRandom(AudioSource audiosource, AudioClip[] audioClips)
    {
        audiosource.clip = audioClips[Random.Range(0, audioClips.Length)];
        audiosource.pitch = Random.Range(0.9f, 1.1f);
        audiosource.Play();
        
    }

    public void FadeSoundOut(AudioClip audioClip)
    {

    }

    public void FadeSoundIn(AudioClip audioClip)
    {

    }

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
   
    void Update()
    {
        
    }
}
