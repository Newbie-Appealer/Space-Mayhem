using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftSlot : MonoBehaviour , IPointerEnterHandler 
{
    [SerializeField]
    private int _idx;
    [SerializeField]
    private Sprite _slotSprite;

    // 프로퍼티
    public int idx { get => _idx; set { _idx = value; } }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CraftUiManager.intance.F_OnOffDtailPanel( _idx, true);
    }

}
