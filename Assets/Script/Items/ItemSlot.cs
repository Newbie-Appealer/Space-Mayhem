using System.Collections.Generic;
using System.Threading.Tasks.Sources;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("UI")]
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _itemStack;

    [Header("Slot Information")]
    [SerializeField] private bool _usedSlot;        // 슬롯에 아이템 존재 여부  있으면 true 없으면 false
    [SerializeField] public int _slotIndex;

    [SerializeField] private GraphicRaycaster _gr;
    [SerializeField] private EventSystem _es;
    private Transform _defaultParent;
    private List<RaycastResult> results;
    
    private void Start()
    {
        _gr = UIManager.Instance.canvas.GetComponent<GraphicRaycaster>();
        _es = UIManager.Instance.canvas.GetComponent<EventSystem>();
        results = new List<RaycastResult>();
        _defaultParent = this.transform;
    }

    #region UI Image
    public void F_UpdateSlost(int v_code,int v_stack)
    {
        _itemImage.sprite = ResourceManager.Instance.F_GetInventorySprite(v_code);
        _itemStack.text = v_stack.ToString();
        _usedSlot = true;
    }

    public void F_EmptySlot()
    {
        _itemImage.sprite = ResourceManager.Instance.emptySlotSprite;
        _itemStack.text = "";
        _usedSlot = false;
    }
    #endregion


    //인벤토리 슬롯 아이템 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_usedSlot)
        {
            Debug.Log("Begin index : " + _slotIndex);
            _itemImage.transform.SetParent(_itemImage.transform.root);                      // root를 부모로 지정
        }
    }

    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {

        if (_usedSlot)
           _itemImage.transform.position = eventData.position;                             // 아이템 이미지 마우스 따라가게
    }

    //인벤토리 슬롯 아이템 드래그 끝
    public void OnEndDrag(PointerEventData eventData) 
    {
        if(_usedSlot)
        {

            _itemImage.transform.SetParent(_defaultParent);                                 // 원래 부모로 재설정
            _itemImage.transform.localPosition = Vector3.zero;                              // 부모 밑 위치를 0, 0, 0으로 설정

            // _itemImage.raycastTarget = true;

            eventData = new PointerEventData(_es);
            eventData.position = Input.mousePosition;                                       // 이벤트 데이터에 마우스 위치 넣어줌
            _gr.Raycast(eventData, results);                                                // 이벤트 데이터를 results list에 넣어줌

            if (results.Count == 0)                                                         // UI가 아닌 밖으로 드래그 했을때 예외처리.
                return;

            ItemSlot tmp_slot = results[0].gameObject.GetComponent<ItemSlot>();
            results.Clear();                                                                // 초기화

            if (tmp_slot == null)                                                           // 인벤 슬롯이 아닌 다른곳에 드래그 했을떄 예외처리.
                return;

            int index = tmp_slot._slotIndex;                                                // 드래그가 끝난 위치의 슬롯
            Debug.Log("end index : " + index);
            ItemManager.Instance.inventorySystem.F_SwapItem(_slotIndex, index);             // 스왑 시도
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_usedSlot)
        {
            if (eventData.button == PointerEventData.InputButton.Left)  // 좌클릭 ( 아이템 정보 최신화 )
            {
                int itemIndex = ItemManager.Instance.inventorySystem.inventory[_slotIndex].itemCode;
                UIManager.Instance.F_UpdateInventoryInformation(itemIndex);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)    // 우클릭 ( 아이템 삭제/사용 기능)
            {
                UIManager.Instance.F_SlotFunctionUI(_slotIndex);
            }            
        }
    }
}
