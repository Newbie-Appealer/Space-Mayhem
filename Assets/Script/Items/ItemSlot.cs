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

    public void OnBeginDrag(PointerEventData eventData) //인벤토리 슬롯 아이템 드래그 시작
    {
        if(_itemImage.sprite.name != "InputThemeYellow")
        {
            Debug.Log(_slotIndex);
            ItemManager.Instance.inventorySystem.F_GetBeginIndex(_slotIndex);
            _startParent = _itemImage.transform.parent;
            _itemImage.transform.SetParent(_itemImage.transform.root); //root를 부모로 지정
            _itemImage.transform.SetAsLastSibling(); //드래그 하는 동안 가장 상위에 보이게 함
            _itemImage.raycastTarget = false; //드래그 동안 ray에 인식 안되게 함
        }
    }

    public void OnDrag(PointerEventData eventData) // 드래그 중
    {
        _itemImage.transform.position = eventData.position; //아이템 이미지 마우스 따라가게
    }

    public void OnEndDrag(PointerEventData eventData) //인벤토리 슬롯 아이템 드래그 끝
    {
        _itemImage.transform.SetParent(_startParent); //원래 부모로 재설정
        _itemImage.transform.localPosition = Vector3.zero; //부모 밑 위치를 0, 0, 0으로 설정
        _itemImage.raycastTarget = true;
        eventData = new PointerEventData(_es);
        eventData.position = Input.mousePosition; //이벤트 데이터에 마우스 위치 넣어줌
        _gr.Raycast(eventData, results); //이벤트 데이터를 results list에 넣어줌
        int index = results[0].gameObject.GetComponent<ItemSlot>()._slotIndex;
        Debug.Log(index);
        ItemManager.Instance.inventorySystem.F_GetEndIndex(index);
        results.Clear();
    }
}
