﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Shrine : LevelZone
{
    public ParticleSystem scoredFx;

    public override void OnCharacterStay(Character character)
    {
        base.OnCharacterStay(character);
        if(character.HasFlag && character.TeamIndex == teamIndex)
        {
            scoredFx.Play();
            character.Score();
            character.DropFlag();
            TeamManager.Instance.Score(teamIndex);
            CTFManager.Instance.ScoredFlagOfTeam(1 - character.TeamIndex);
            UIManager.Instance.LogMessage(character.PlayerName + " Scored for Team " + teamIndex + "!");
        }
    }

    public override void Awake()
    {
        base.Awake();

        NetworkStartPosition startPos = GetComponentInChildren<NetworkStartPosition>();

        if (!startPos) return;
        startPos.Register(teamIndex);
    }
}
