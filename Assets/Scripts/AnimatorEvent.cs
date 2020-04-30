using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEvent : MonoBehaviour
{
    private AudioManager AM = AudioManager.instance;
    public void FootStep()
    {       
        AM.PlaySoundFullRandom(AM.AS_Movement, AM.AC_FootStep);
    }

    public void Jump()
    {
        AM.PlaySoundFullRandom(AM.AS_Movement, AM.AC_Jump);
    }

    public void DJump()
    {
        AM.PlaySoundFullRandom(AM.AS_Movement, AM.AC_DJump);
    }

    public void Landing()
    {
        AM.PlaySoundFullRandom(AM.AS_Movement, AM.AC_Landing);
    }

    public void Dodge()
    {
        AM.PlaySoundFullRandom(AM.AS_Movement, AM.AC_Dodge);
    }

    public void Attack()
    {
        AM.PlaySoundFullRandom(AM.AS_Fight, AM.AC_Attack);
    }

    public void WallJump()
    {
        AudioManager.instance.PlaySoundFullRandom(AudioManager.instance.AS_Movement, AudioManager.instance.AC_WallJump);
    }
}
