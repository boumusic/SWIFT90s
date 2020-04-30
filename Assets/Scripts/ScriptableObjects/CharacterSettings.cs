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
    public float downButtonFallSpeedMultiplier = 2f;

    [Header("Pass through Platform")]
    public int defaultLayer;
    public int passThroughLayer;
    public int passThroughLayerPlatform = 10;
    public float fallThroughDuration = 0.5f;

    [Header("Dodge")]
    public Propulsion dodge;
    public float dodgeCooldown = 1f;

    [Header("Cast")]
    public LayerMask groundMask;
    public LayerMask wallMask;

    [Header("Ground")]
    public float groundCastRadius = 1;
    public float castBoxWidth = 1;
    public float groundRaycastDown = 1;
    public float castGroundOrigin = 1;

    [Header("Wall")]
    public float castWallHeight = 0.7f;
    public float castWallLength = 1f;

    [Header("Ceiling")]
    public float castCeilingOrigin = 1;
    public float castCeilingLength = 1f;

    [Header("Wallslide")]    
    public float timeToReachMaxSlide = 0.5f;
    public float minWallSlideSpeed = 4f;
    public float maxWallSlideSpeed = 4f;
    public AnimationCurve slideCurve;

    [Header("WallJump")]
    public Propulsion wallJump;

    [Header("Attack")]
    public float attackDuration = 1f;
    public float attackDelay = 0.2f;
    public float attackCooldown = 0.5f;
    public float attackLength = 1f;
    public float attackOrigin = 1f;
    public float attackWidth = 1f;
    public float attackOffset = 1f;
    public Propulsion attackImpulse;
}
