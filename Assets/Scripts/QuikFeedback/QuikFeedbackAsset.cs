using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pataya.QuikFeedback;

namespace Pataya.QuikFeedback
{
    [CreateAssetMenu(fileName = "New QuikFeedback Asset", menuName = "QuikFeedback")]
    public class QuikFeedbackAsset : ScriptableObject
    {
        public float generalDelay = 0f;

        public ShakeFeedback shakeFeedback;
        public ZoomFeedback zoomFeedback;
        public PostProcessFeedback postProcessFeedback;
        public ParticleFeedback particleFeedback;
        public AnimationFeedback animationFeedback;
        public MaterialFeedback materialFeedback;
        public BounceFeedback bounceFeedback;
        public FreezeFrameFeedback freezeFrameFeedback;
    }
}

