using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerPortrait : MonoBehaviour
{
    [Header("Components")]
    public Image image;
    public RectTransform rect;
    public TextMeshProUGUI text;
    public Sprite[] sprites;

    public void UpdateVisuals(Character chara)
    {
        image.sprite = sprites[chara.TeamIndex];
        text.text = chara.PlayerName;
    }
}
