using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftSlot : MonoBehaviour , IPointerEnterHandler 
{
    [SerializeField]
    private int _typeNum;
    [SerializeField]
    private int _detialNum;
    [SerializeField]
    private Image _slotIcon;

    // 프로퍼티
    public int typeNum { get => _typeNum; set { _typeNum = value; } }
    public int detialNum { get => _detialNum; set { _detialNum = value; } }

    public void F_SetImageIcon( Sprite v_sp)
    {
        this._slotIcon.sprite = v_sp;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        BuildMaster.Instance.housingUiManager.F_OnOffDtailPanel( _typeNum , _detialNum);
    }

}
