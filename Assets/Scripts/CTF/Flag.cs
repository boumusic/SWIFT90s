using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    [Header("Components")]
    public Renderer rend;
    public Texture2D[] textures;

    public int TeamIndex { get; private set; }

    public void Initialize(int teamIndex)
    {
        this.TeamIndex = teamIndex;
        Color col = TeamManager.Instance.GetTeamColor(TeamIndex);
        rend.material.SetTexture("_MainTex", textures[teamIndex]);
    }
}
