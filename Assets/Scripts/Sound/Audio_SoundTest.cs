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

/*
        AM.PlaySoundFullRandom(AM.AS_Movement, AM.AC_FootStep);

        AM.PlaySoundFullRandom(AM.AS_Movement, AM.AC_Jump);

        AM.PlaySoundFullRandom(AM.AS_Movement, AM.AC_DJump);

        AM.PlaySoundFullRandom(AM.AS_Movement, AM.AC_Landing);

        AM.PlaySoundFullRandom(AM.AS_Movement, AM.AC_Dodge);

        AM.PlaySound(AM.AS_Fight, AM.AC_DodgeWhileAttacked);

        AM.PlaySoundFullRandom(AM.AS_Fight, AM.AC_Attack);

        AM.PlaySound(AM.AS_Fight, AM.AC_Hit);

        AM.PlaySoundRandomPitch(AM.AS_Feedback, AM.AC_Kill);

        AM.PlaySoundRandomPitch(AM.AS_Feedback, AM.AC_Death);

        AM.PlaySoundFullRandom(AM.AS_Movement, AM.AC_WallJump);

        AM.PlaySoundRandomPitch(AM.AS_Feedback, AM.AC_FlagTookAlly);

        AM.PlaySoundRandomPitch(AM.AS_Feedback, AM.AC_FlagTookEnemy);

        AM.PlaySoundLoop(AM.AC_FlagAura);
        //suivi de
        AM.FadeOut(AM.AS_Loop, 1f);

        AM.PlaySound(AM.AS_Feedback, AM.AC_ScoreAlly_01);

        AM.PlaySound(AM.AS_Feedback, AM.AC_ScoreAlly_02);

        AM.PlaySound(AM.AS_Feedback, AM.AC_ScoreAlly_03);
        //suivi de
        AM.MuteAudioSource(AM.AS_Loop, true);
        AM.MuteAudioSource(AM.AS_Movement, true);
        AM.MuteAudioSource(AM.AS_Fight, true);
        AM.FadeOut(AM.AS_Music, 0.5f);


        AM.PlaySound(AM.AS_Feedback, AM.AC_ScoreEnemy_01);

        AM.PlaySound(AM.AS_Feedback, AM.AC_ScoreEnemy_02);

        AM.PlaySound(AM.AS_Feedback, AM.AC_ScoreEnemy_03);

        AM.PlaySound(AM.AS_Announcer, AM.AC_Countdown_1);

        AM.PlaySound(AM.AS_Announcer, AM.AC_Countdown_2);

        AM.PlaySound(AM.AS_Announcer, AM.AC_Countdown_3);

        AM.PlaySound(AM.AS_Announcer, AM.AC_Countdown_Go);

        AM.PlaySound(AM.AS_Announcer, AM.AC_Countdown_Fight);

        AM.PlaySoundRandomInList(AM.AS_Announcer, AM.AC_Defeat);

        AM.PlaySoundRandomInList(AM.AS_Announcer, AM.AC_Victory);

        AM.PlaySoundRandomInList(AM.AS_Announcer, AM.AC_Draw);

        AM.PlaySoundRandomInList(AM.AS_Announcer, AM.AC_SuddenDeath);

        AM.FadeIn(AM.AS_Music, AM.AC_MenuTheme, 2f, 1f);
        //suivi de pendant le décompte
        AM.FadeOut(AM.AS_Music, 2f);

        AM.PlayMusic(AM.AC_BattleTheme);
        //seulement après le décompte

        AM.FadeIn(AM.AS_Music, AM.AC_EndTheme, 1.5f, 1f);
        //lorsque le joueur sort de l'écran de victoire
        AM.FadeOut(AM.AS_Music, 0.75f);

        

        AM.PlaySoundRandomInList(AM.AS_Announcer, AM.AC_ButtonPress);

        AM.AS_Fight.Stop();

    */
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
            AM.StartCoroutine(AM.FadeIn(AM.AS_Music, AM.AC_EndTheme, 3f, 0.7f));
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("I pressed");
          //  AM.FadeOut(AM.AS_Music, 2f);
            AM.StartCoroutine(AM.FadeOut(AM.AS_Music, 2f));
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            AM.PlayMusic(AM.AC_BattleTheme, 0.8f);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            AM.PlaySoundRandomPitch(AM.AS_Announcer, AM.AC_Countdown_1);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            AM.PlaySoundFullRandom(AM.AS_Movement, AM.AC_FootStep);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            AM.PlaySoundFullRandom(AM.AS_Movement, AM.AC_Jump);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            AM.PlaySound(AM.AS_Feedback, AM.AC_FlagTookAlly);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            
            AM.PlaySoundRandomPitch(AM.AS_Feedback, AM.AC_Death);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            AM.PlaySound(AM.AS_Feedback, AM.AC_RefereeWhistle);

            AM.PlaySoundRandomInList(AM.AS_Announcer, AM.AC_HalfTime);

        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            AM.PlaySound(AM.AS_Announcer, AM.AS_Announcer.clip);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            AM.PlaySound(AM.AS_Movement, AM.AS_Movement.clip);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            AM.PlaySound(AM.AS_Fight, AM.AS_Fight.clip);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            AM.PlaySound(AM.AS_Feedback, AM.AS_Feedback.clip);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            AM.PlaySound(AM.AS_Music, AM.AS_Music.clip);
        }

    }
}
