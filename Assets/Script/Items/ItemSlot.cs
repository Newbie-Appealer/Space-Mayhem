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
    public void OnDrop(PointerEventData eventData) //���콺 Ŭ���� ���� �� ����
    {
        Debug.Log("OnDrop");
        // GameObject dropped ������ �����ϰ�, �̴� �̺�Ʈ �����ͷκ��� ������ �巡�׵� ������Ʈ�� ��Ÿ���ϴ�.
        GameObject dropped = eventData.pointerDrag;

        // dropped ������Ʈ���� ItemDrag ������Ʈ�� ������ itemDrag ������ �Ҵ��մϴ�.
        ItemDrag itemDrag = dropped.GetComponent<ItemDrag>();

        // itemDrag ������ _parentAfterDrag �Ӽ��� ���� ��ũ��Ʈ�� ���� ���� ������Ʈ(transform)�� �����մϴ�.
        itemDrag._parentAfterDrag = transform;
    }
}
