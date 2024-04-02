using System;
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

    [Header("MoussEvent")]
    private GraphicRaycaster _gr;
    private EventSystem _es;
    private Transform _defaultParent;
    private List<RaycastResult> results;

    private bool canDrag => _usedSlot && !UIManager.Instance.slotFunctionUI.activeSelf;

    private Item[] _itemSlotRef;
    public Item[] itemSlotRef
    {
        get { return _itemSlotRef; }
        set { _itemSlotRef = value; }
    }

    private void Start()
    {
        _gr = UIManager.Instance.canvas.GetComponent<GraphicRaycaster>();
        _es = UIManager.Instance.canvas.GetComponent<EventSystem>();
        results = new List<RaycastResult>();
        _defaultParent = this.transform;
    }

    #region UI Image
    public void F_UpdateSlot(int v_code,int v_stack)
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
        if (canDrag)
        {
            _itemImage.transform.SetParent(_itemImage.transform.root);                      // root를 부모로 지정
        }
    }

    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {

        if (canDrag)
           _itemImage.transform.position = eventData.position;                             // 아이템 이미지 마우스 따라가게
    }

    //인벤토리 슬롯 아이템 드래그 끝
    public void OnEndDrag(PointerEventData eventData) 
    {
        if(canDrag)
        {
            _itemImage.transform.SetParent(_defaultParent);                                 // 원래 부모로 재설정
            _itemImage.transform.localPosition = Vector3.zero;                              // 부모 밑 위치를 0, 0, 0으로 설정

            eventData = new PointerEventData(_es);
            eventData.position = Input.mousePosition;                                       // 이벤트 데이터에 마우스 위치 넣어줌
            _gr.Raycast(eventData, results);                                                // 이벤트 데이터를 results list에 넣어줌

            if (results.Count == 0)                                                         // UI가 아닌 밖으로 드래그 했을때 예외처리.
                return;

            ItemSlot target_slot = results[0].gameObject.GetComponent<ItemSlot>();
            results.Clear();                                                                // 초기화

            if (target_slot == null)                                                        // 인벤 슬롯이 아닌 다른곳에 드래그 했을떄 예외처리.
                return;

            int targetIndex = target_slot._slotIndex;                                       // 드래그가 끝난 위치의 슬롯

            // 슬롯이 참조하는 배열과 index를 매개변수로 넘김. ( C# 배열 = 참조형식 )
            ItemManager.Instance.inventorySystem.F_SwapItem(
                _slotIndex, targetIndex,ref _itemSlotRef,ref target_slot._itemSlotRef);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_usedSlot)
        {
            if (eventData.button == PointerEventData.InputButton.Left)  // 좌클릭 ( 아이템 정보 최신화 )
            {
                int itemIndex = ItemManager.Instance.inventorySystem.inventory[_slotIndex].itemCode;
                UIManager.Instance.F_UpdateItemInformation(itemIndex);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)    // 우클릭 ( 아이템 삭제 기능)
            {
                UIManager.Instance.F_SlotFunctionUI(_slotIndex);
            }
        }
    }
}
