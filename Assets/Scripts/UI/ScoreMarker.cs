using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreMarker : MonoBehaviour
{
    [Header("Components")]
    public Image image;

    [Header("Settings")]
    public float nonScoredAlpha = 0.5f;
    private int teamIndex;

    public void Initialize(int index)
    {
        teamIndex = index;
        image.color = TeamManager.Instance.GetTeamColor(index);
    }

    public void Refresh(bool scored)
    {
        float a = 1f;
        if (!scored) a = nonScoredAlpha;
        image.color = new Color(image.color.r, image.color.g, image.color.b, a);
    }
}
