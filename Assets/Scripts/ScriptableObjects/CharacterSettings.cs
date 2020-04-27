using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSettings", menuName = "Gameplay/CharacterSettings", order = 50)]
public class CharacterSettings : ScriptableObject
{
    [Header("Run")]
    public float runSpeed = 5f;
    public float groundedAcceleration = 0.5f;
    [Range(1f, 2f)] public float groundedDeceleration = 1.1f;

    [Header("Jump")]
    public int jumpCount = 2;
    public float jumpStrength = 5f;
    public AnimationCurve jumpCurve;
    public float jumpDuration = 0.7f;
    public float aerialAcceleration = 0.5f;
    [Range(1f, 2f)] public float aerialDeceleration = 1.1f;

    [Header("Fall")]
    public float timeToReachMaxFall = 4f;
    public float maxFallSpeed = 20;
    public AnimationCurve fallCurve;

    [Header("Cast")]
    public LayerMask groundMask;
    public float groundCastRadius = 1;
    public float castBoxWidth = 1;
    public float groundRaycastDown = 1;
    public float groundRaycastUp = 1;
}
