using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : LevelZone
{
    public override void OnCharacterStay(Character character)
    {
        base.OnCharacterStay(character);
        if(character.HasFlag && character.TeamIndex == teamIndex)
        {
            TeamManager.Instance.Score(teamIndex);
            character.DropFlag();
            CTFManager.Instance.ScoredFlagOfTeam(1 - character.TeamIndex);
        }
    }
}
