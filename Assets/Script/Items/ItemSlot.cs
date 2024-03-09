using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public Image _itemImage;
    public TextMeshProUGUI _itemStack;

    private void Start()
    {
        Debug.Log(_itemImage.sprite.name);
    }
    public void UpdateSlost(int v_code,int v_stack)
    {
        _itemImage.sprite = ResourceManager.Instance.F_GetInventorySprite(v_code);
        _itemStack.text = v_stack.ToString();
    }
    public void OnDrop(PointerEventData eventData) //마우스 클릭을 놓을 때 실행
    {
        Debug.Log("OnDrop");
        // GameObject dropped 변수를 선언하고, 이는 이벤트 데이터로부터 가져온 드래그된 오브젝트를 나타냅니다.
        GameObject dropped = eventData.pointerDrag;

        // dropped 오브젝트에서 ItemDrag 컴포넌트를 가져와 itemDrag 변수에 할당합니다.
        ItemDrag itemDrag = dropped.GetComponent<ItemDrag>();

        // itemDrag 변수의 _parentAfterDrag 속성을 현재 스크립트가 속한 게임 오브젝트(transform)로 설정합니다.
        itemDrag._parentAfterDrag = transform;
    }
}
