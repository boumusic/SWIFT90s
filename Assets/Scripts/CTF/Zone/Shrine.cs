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
            character.Score();
            character.DropFlag();
            TeamManager.Instance.Score(teamIndex);
            CTFManager.Instance.ScoredFlagOfTeam(1 - character.TeamIndex);
        }
    }
}
