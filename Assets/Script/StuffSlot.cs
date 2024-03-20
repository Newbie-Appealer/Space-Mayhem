using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StuffSlot : MonoBehaviour
{
    [Header("Temp Data")]
    [SerializeField] private int _itemCode;
    [SerializeField] private int _needCount;
    [SerializeField] string _text;
    public int itemCode => _itemCode;

    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _itemCounter;

    public void F_InitData(int v_itemCode, int v_needCount)
    {
        _itemCode = v_itemCode;
        _needCount = v_needCount;
        _itemImage.sprite = ResourceManager.Instance.F_GetInventorySprite(v_itemCode);
    }

    public void F_UpdateCounter(int v_currentCount)
    {
        _itemCounter.text = v_currentCount + " / " + _needCount;
    }
}
