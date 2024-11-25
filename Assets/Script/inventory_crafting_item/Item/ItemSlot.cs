using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SlotType
{
    NONE,
    STORAGE,
    INVENTORY
}

public class ItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public SlotType _slotType;

    [Header("UI")]
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _itemStack;
    [SerializeField] private Image _durability;
    
    [Header("Slot Information")]
    [SerializeField] private bool _usedSlot;        // 슬롯에 아이템 존재 여부 ( 있으면 true 없으면 false )
    [SerializeField] public int _slotIndex;

    [Header("MoussEvent")]
    private GraphicRaycaster _gr;
    private EventSystem _es;
    private Transform _defaultParent;
    private List<RaycastResult> results;

    // Get / Set
    private bool canDrag => _usedSlot && !UIManager.Instance.slotFunctionUI.activeSelf;

    // 인벤토리 배열 참조
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
    /// <summary> 도구아이템 내구도 게이지 업데이트</summary>
    public void F_UpdateDurability(int v_slotIndex)
    {
        if (itemSlotRef[v_slotIndex] is Tool)
        {
            _durability.fillAmount = (itemSlotRef[v_slotIndex] as Tool).durabilityAmount;
            _durability.gameObject.SetActive(true);         // 내구도 UI ON
            _itemStack.gameObject.SetActive(false);         // 스택   UI OFF
        }
    }

    /// <summary> 아이템 슬롯의 이미지, 스택, 게이지 등 업데이트</summary>
    public void F_UpdateSlot(int v_code, int v_stack, int v_slotIndex)
    {
        _itemImage.sprite = ResourceManager.Instance.F_GetInventorySprite(v_code);
        _itemStack.text = v_stack.ToString();
        _usedSlot = true;

        _durability.gameObject.SetActive(false);        // 내구도 UI OFF
        _itemStack.gameObject.SetActive(true);          // 스택   UI ON

        F_UpdateDurability(v_slotIndex);
    }

    /// <summary> 아이템 슬롯이 비었을때 슬롯 초기화 함수</summary>
    public void F_EmptySlot()
    {
        _itemImage.sprite = ResourceManager.Instance.emptySlotSprite;
        _itemStack.text = "";
        _usedSlot = false;
        _durability.gameObject.SetActive(false);
    }
    #endregion

    // 인벤토리 슬롯 아이템 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canDrag)
        {
            GameManager.Instance.onDrag = true;
            _itemImage.transform.SetParent(_itemImage.transform.root);                      // root를 부모로 지정
        }
    }

    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {

        if (canDrag)
        {
           _itemImage.transform.position = eventData.position;                             // 아이템 이미지 마우스 따라가게
        }
    }

    // 인벤토리 슬롯 아이템 드래그 끝
    public void OnEndDrag(PointerEventData eventData) 
    {
        if(canDrag)
        {
            GameManager.Instance.onDrag = false;
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

    // 아이템 좌클릭(정보보기) 우클릭 ( 아이템 삭제,나누기 기능 )
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_usedSlot && !GameManager.Instance.onDrag)
        {
            if (eventData.button == PointerEventData.InputButton.Left)  // 좌클릭 ( 아이템 정보 최신화 )
            {
                SoundManager.Instance.F_PlaySFX(SFXClip.CLICK3);

                int itemIndex = _itemSlotRef[_slotIndex].itemCode;
                UIManager.Instance.F_UpdateItemInformation(itemIndex);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)    // 우클릭 ( 아이템 삭제 기능)
            {
                SoundManager.Instance.F_PlaySFX(SFXClip.CLICK4);

                int itemCode = _itemSlotRef[_slotIndex].itemCode;
                UIManager.Instance.F_SlotFunctionUI(_slotIndex, itemCode, _slotType);
            }
        }
    }
}
