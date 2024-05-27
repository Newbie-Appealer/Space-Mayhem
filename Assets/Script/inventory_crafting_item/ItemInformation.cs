using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemInformation : MonoBehaviour, IPointerClickHandler
{
    /*
     _slotItemCode만 초기화 해주면
    이미지 클릭시 해당 code에 맞는 아이템의 정보를
    인벤토리 상댄 아이템 툴팁에 표시해줌.

        * ItemSlot과 따로 동작하니 ItemSlot을 사용하는 오브젝트에는 부착하지말것!
     */
    

    public int _slotItemCode = -1;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.onDrag)
            return;

        if (_slotItemCode == -1)
            return;

        if(eventData.button == PointerEventData.InputButton.Left)
        {
            UIManager.Instance.F_UpdateItemInformation(_slotItemCode);
            SoundManager.Instance.F_PlaySFX(SFXClip.CLICK3);
        }    
    }
}
