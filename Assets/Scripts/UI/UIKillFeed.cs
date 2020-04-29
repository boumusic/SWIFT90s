using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIKillFeed : MonoBehaviour
{
    public float lifeTime = 2f;
    public float smoothnessPos = 0.5f;
    public Animator animator;
    public TextMeshProUGUI textKiller;
    public TextMeshProUGUI textKilled;
    public RectTransform rect;

    private Vector3 targetPos;
    private Vector3 currentVelPos;

    private void OnEnable()
    {
        StartCoroutine(LifeTime());
    }

    private void Update()
    {
        rect.anchoredPosition = Vector3.SmoothDamp(rect.anchoredPosition, targetPos, ref currentVelPos, smoothnessPos);
    }

    public void Init(Character killer, Character killed)
    {
        textKiller.text = killer.PlayerName;
        textKiller.color = killer.TeamColor;

        textKilled.text = killed.PlayerName;
        textKilled.color = killed.TeamColor;        
    }

    public void Init(string killerName, int killerTeam, string killedName, int killedTeam)
    {
        textKiller.text = killerName;
        textKiller.color = TeamManager.Instance.GetTeamColor(killerTeam);

        textKilled.text = killedName;
        textKilled.color = TeamManager.Instance.GetTeamColor(killedTeam);
    }

    public void UpdatePosition(Vector3 pos)
    {
        targetPos = new Vector3(rect.anchoredPosition.x, pos.y, 0);
    }

    private IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(lifeTime);
        animator.SetTrigger("Out");
    }

    public void Die()
    {
        UIManager.Instance.UnregisterKillFeed(this);
    }
}
