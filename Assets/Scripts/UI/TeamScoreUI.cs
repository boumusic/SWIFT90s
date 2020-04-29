using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamScoreUI : MonoBehaviour
{
    [Header("Components")]
    public RectTransform rect;
    public GameObject scoreMarkerPrefab;
    private List<ScoreMarker> markers = new List<ScoreMarker>();

    [Header("Settings")]
    public int teamIndex = 0;
    public float spacing = 80f;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        int count = CTFManager.Instance.goalPoints;
        for (int i = 0; i < count; i++)
        {
            GameObject newMarker = Instantiate(scoreMarkerPrefab, transform);
            newMarker.transform.localPosition = new Vector3(i * spacing, 0, 0);
            ScoreMarker marker = newMarker.GetComponent<ScoreMarker>();
            marker.Initialize(teamIndex);
            markers.Add(marker);
        }
    }

    private void Update()
    {
        Refresh();
    }

    public void Refresh()
    {
        int score = TeamManager.Instance.GetTeamScore(teamIndex);
        for (int i = 0; i < markers.Count; i++)
        {
            if(i < score)
            {
                markers[i].Refresh(true);
            }
            else
            {
                markers[i].Refresh(false);
            }
        }
    }
}
