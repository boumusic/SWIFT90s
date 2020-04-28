using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pataya.QuikFeedback
{
    public class GamepadRumbler : MonoBehaviour
    {
        public RumbleFeedback r;
        private bool isPlaying = false;
        private float progress = 0f;


        public void Rumble(RumbleFeedback _r)
        {
            bool canPlay = true;
            if (isPlaying)
            {
                if (_r.priority < r.priority)
                {
                    canPlay = false;
                }
            }

            if (canPlay)
            {
                //Gamepad.current?.ResetHaptics();
                r = _r;
                isPlaying = true;
                progress = 0f;
            }
        }

        private void Update()
        {
            if (isPlaying)
            {
                if (progress <= 1)
                {
                    progress += Time.deltaTime * (1 / r.duration);

                    float left = r.leftCurve.Evaluate(progress) * r.leftSpeed;
                    float right = r.rightCurve.Evaluate(progress) * r.rightSpeed;
                    //Gamepad.current?.SetMotorSpeeds(left, right);
                }

                else
                {
                    EndRumble();
                }
            }
        }

        private void OnApplicationQuit()
        {
            EndRumble();
        }

        private void EndRumble()
        {
            isPlaying = false;
            //Gamepad.current?.ResetHaptics();
        }
    }
}
