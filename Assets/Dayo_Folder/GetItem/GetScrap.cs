using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetScrap : MonoBehaviour
{
    [Header("=== Scrap ===")]
    [SerializeField] private TextMeshProUGUI _getScrapName;
    [SerializeField] private Image _getScrapImage;

    public void F_GetScrapUIUpdate(Sprite v_spr, string v_name)
    {
        _getScrapImage.sprite = v_spr;
        _getScrapName.text = v_name;
    }
}
