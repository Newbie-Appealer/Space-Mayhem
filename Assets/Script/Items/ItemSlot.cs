using System.Collections.Generic;
using System.Threading.Tasks.Sources;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image _itemImage;
    public TextMeshProUGUI _itemStack;

    [Header("Item Slot")]
    [SerializeField] private GameObject _canvas;
    private GraphicRaycaster _gr;
    private EventSystem _es;
    public int _slotIndex;
    Transform _startParent;
    List<RaycastResult> results;
    
    private void Start()
    {
        _gr = _canvas.GetComponent<GraphicRaycaster>();
        _es = _canvas.GetComponent<EventSystem>();
        results = new List<RaycastResult>();
    }

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

    public void OnBeginDrag(PointerEventData eventData) //�κ��丮 ���� ������ �巡�� ����
    {
        if(_itemImage.sprite.name != "InputThemeYellow")
        {
            Debug.Log(_slotIndex);
            ItemManager.Instance.inventorySystem.F_GetBeginIndex(_slotIndex);
            _startParent = _itemImage.transform.parent;
            _itemImage.transform.SetParent(_itemImage.transform.root); //root�� �θ�� ����
            _itemImage.transform.SetAsLastSibling(); //�巡�� �ϴ� ���� ���� ������ ���̰� ��
            _itemImage.raycastTarget = false; //�巡�� ���� ray�� �ν� �ȵǰ� ��
        }
    }

    public void OnDrag(PointerEventData eventData) // �巡�� ��
    {
        _itemImage.transform.position = eventData.position; //������ �̹��� ���콺 ���󰡰�
    }

    public void OnEndDrag(PointerEventData eventData) //�κ��丮 ���� ������ �巡�� ��
    {
        _itemImage.transform.SetParent(_startParent); //���� �θ�� �缳��
        _itemImage.transform.localPosition = Vector3.zero; //�θ� �� ��ġ�� 0, 0, 0���� ����
        _itemImage.raycastTarget = true;
        eventData = new PointerEventData(_es);
        eventData.position = Input.mousePosition; //�̺�Ʈ �����Ϳ� ���콺 ��ġ �־���
        _gr.Raycast(eventData, results); //�̺�Ʈ �����͸� results list�� �־���
        int index = results[0].gameObject.GetComponent<ItemSlot>()._slotIndex;
        Debug.Log(index);
        ItemManager.Instance.inventorySystem.F_GetEndIndex(index);
        results.Clear();
    }
}
