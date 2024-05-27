using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemInformation : MonoBehaviour, IPointerClickHandler
{
    /*
     _slotItemCode�� �ʱ�ȭ ���ָ�
    �̹��� Ŭ���� �ش� code�� �´� �������� ������
    �κ��丮 ��� ������ ������ ǥ������.

        * ItemSlot�� ���� �����ϴ� ItemSlot�� ����ϴ� ������Ʈ���� ������������!
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
