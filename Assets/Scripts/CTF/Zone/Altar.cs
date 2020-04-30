﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : LevelZone
{
    [Header("Altar")]
    public Flag flag;
    public ParticleSystem capturedFx;

    private bool isEnabled = true;
    private bool captured = false;

    private void Awake()
    {
        flag.Initialize(teamIndex);
    }

    public override void OnCharacterStay(Character character)
    {
        base.OnCharacterStay(character);
        if(!character.HasFlag && isEnabled && character.TeamIndex != teamIndex && !character.IsDead)
        {
            capturedFx.Play();
            UIManager.Instance.LogMessage(character.PlayerName + " Captured the flag of Team " + teamIndex + "!");
            character.CaptureFlag(this);
            UpdateFlag();
            captured = true;
            CTFManager.Instance.CapturedFlagOfTeam(teamIndex);
        }
    }

    public void ResetFlag()
    {
        captured = false;
        UpdateFlag();
    }

    public void Enable(bool enable)
    {
        isEnabled = enable;
        UpdateFlag();
    }

    public void UpdateFlag()
    {
        flag.gameObject.SetActive(!captured && isEnabled);
    }
}
