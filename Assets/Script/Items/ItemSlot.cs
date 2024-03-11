using System.Threading.Tasks.Sources;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image _itemImage;
    public TextMeshProUGUI _itemStack;
    public int _slotIndex;

    public void F_UpdateSlost(int v_code,int v_stack)
    {
        _itemImage.sprite = ResourceManager.Instance.F_GetInventorySprite(v_code);
        _itemStack.text = v_stack.ToString();
    }

    public void F_EmptySlot()
    {
        _itemImage.sprite = ResourceManager.Instance.emptySlotSprite;
        _itemStack.text = "";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("drag Start " + _slotIndex);

    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("drag on");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("drag end " + _slotIndex);
    }
}
