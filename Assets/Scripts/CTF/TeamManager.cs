using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Team
{
    public int CurrentScore;
}

public class TeamManager : MonoBehaviour
{
    private static TeamManager instance;
    public static TeamManager Instance
    {
        get { if (!instance) instance = FindObjectOfType<TeamManager>(); return instance; }
    }

    [SerializeField]
    private List<Team> teams = new List<Team>();

    private int teamCount = 2;

    [Header("Colors")]
    public Color teamColor0 = Color.red;
    public Color teamColor1 = Color.cyan;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        for (int i = 0; i < teamCount; i++)
        {
            Team team = new Team();
            teams.Add(team);
        }
    }

    public int GetTeamScore(int index)
    {
        return teams[index].CurrentScore;
    }

    public Color GetTeamColor(int index)
    {
        if (index == 0)
        {
            return teamColor0;
        }

        else if (index == 1)
        {
            return teamColor1;
        }

        else return Color.grey;
    }
}
