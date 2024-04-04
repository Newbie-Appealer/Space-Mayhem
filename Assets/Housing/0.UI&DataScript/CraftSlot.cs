using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftSlot : MonoBehaviour , IPointerEnterHandler 
{
    [SerializeField]
    private int _idx;
    [SerializeField]
    private Image _slotIcon;

    // 프로퍼티
    public int idx { get => _idx; set { _idx = value; } }

    public void F_SetImageIcon( Sprite v_sp)
    {
        this._slotIcon.sprite = v_sp;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        BuildMaster.Instance.housingUiManager.F_OnOffDtailPanel( _idx, true);
    }

}
