using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Team
{
    public int index = 0;
    public List<NetworkedPlayer> players = new List<NetworkedPlayer>();
    public int PlayerCount => players.Count;
    public Color Color { get => color; }
    public int Score { get => score; set => score = value; }

    public bool HasWon { get; private set; }
    private int score;
    private Color color;

    public void EarnPoint(int point = 1)
    {
        score += point;
        if (score >= CTFManager.Instance.goalPoints)
        {
            HasWon = true;
            CTFManager.Instance.TeamWins(this);
        }
    }

    public Team(int index, Color color)
    {
        this.index = index;
        this.color = color;
    }

    public void Join(NetworkedPlayer player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
            UIManager.Instance.RefreshPortraits();
            //Debug.Log(player.PlayerName + " joined team " + index);
        }
    }

    public void Leave(NetworkedPlayer player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
            UIManager.Instance.RefreshPortraits();
            Debug.Log(player.PlayerName + " left team " + index);
        }
    }
}

public class TeamManager : MonoBehaviour
{
    private static TeamManager instance;
    public static TeamManager Instance
    {
        get
        {
            if (!instance) instance = FindObjectOfType<TeamManager>();
            return instance;
        }
    }

    public static int TeamCount = 2;
    public List<Team> teams = new List<Team>();
    public List<Color> colors = new List<Color>();

    public bool InputEnabled { get; private set; }

    public int GetIndex(NetworkedPlayer player)
    {
        for (int i = 0; i < teams.Count; i++)
        {
            if (teams[i].players.Contains(player))
            {
                return i;
            }
        }

        return 0;
    }

    private void Awake()
    {
        InitializeTeams();
    }

    private void InitializeTeams()
    {
        teams.Clear();
        for (int i = 0; i < TeamCount; i++)
        {
            teams.Add(new Team(i, colors[i]));
        }
    }

    public int JoinSmallestTeam(NetworkedPlayer player)
    {
        int index = SmallestTeamIndex();
        teams[index]?.Join(player);
        return index;
    }

    public void JoinTeam(int i, NetworkedPlayer player)
    {
        teams[i].Join(player);
    }

    public int GetTeamScore(int i)
    {
        if (teams.Count > i)
            return teams[i].Score;
        return 0;
    }

    public void ToggleInputs(bool on)
    {
        InputEnabled = on;
    }

    public void Score(int i)
    {
        teams[i].EarnPoint();
    }

    public Color GetTeamColor(int i)
    {
        if (teams.Count <= i)
        {
            InitializeTeams();
        }

        if (teams.Count > 0)
        {
            if (i < teams.Count && i >= 0)
                return teams[i].Color;
        }

        return Color.white;
    }

    public Color GetOppositeTeamColor(int i)
    {
        for (int t = 0; t < teams.Count; t++)
        {
            if (t != i)
            {
                return teams[t].Color;
            }
        }
        return Color.white;
    }

    private int SmallestTeamIndex()
    {
        int smallest = 0;
        for (int i = 0; i < teams.Count; i++)
        {
            if (teams[i].PlayerCount < teams[smallest].PlayerCount)
            {
                smallest = i;
            }
        }

        return smallest;
    }
}
