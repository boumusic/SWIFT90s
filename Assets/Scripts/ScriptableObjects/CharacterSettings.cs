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

    [Header("Pass through Platform")]
    public int defaultLayer;
    public int passThroughLayer;
    public float fallThroughDuration = 0.5f;

    [Header("Cast")]
    public LayerMask groundMask;
    [Header("Ground")]
    public float groundCastRadius = 1;
    public float castBoxWidth = 1;
    public float groundRaycastDown = 1;
    public float castGroundOrigin = 1;
    [Header("Wall")]
    public float castWallLength = 1f;
    [Header("Ceiling")]
    public float castCeilingOrigin = 1;
    public float castCeilingLength = 1f;

    [Header("Wallslide")]
    public float timeToReachMaxSlide = 0.5f;
    public float minWallSlideSpeed = 4f;
    public float maxWallSlideSpeed = 4f;
    public AnimationCurve slideCurve;
}
