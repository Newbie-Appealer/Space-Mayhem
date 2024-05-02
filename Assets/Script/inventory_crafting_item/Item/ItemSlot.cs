using System;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] private bool _usedSlot;        // ���Կ� ������ ���� ����  ������ true ������ false
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
    // ���������� ������ UI ������Ʈ �Լ�
    public void F_UpdateDurability(int v_slotIndex)
    {
        if (itemSlotRef[v_slotIndex] is Tool)
        {
            _durability.fillAmount = (itemSlotRef[v_slotIndex] as Tool).durabilityAmount;
            _durability.gameObject.SetActive(true);         // ������ UI ON
            _itemStack.gameObject.SetActive(false);         // ����   UI OFF
        }
    }

    public void F_UpdateSlot(int v_code, int v_stack, int v_slotIndex)
    {
        _itemImage.sprite = ResourceManager.Instance.F_GetInventorySprite(v_code);
        _itemStack.text = v_stack.ToString();
        _usedSlot = true;

        _durability.gameObject.SetActive(false);        // ������ UI OFF
        _itemStack.gameObject.SetActive(true);          // ����   UI ON

        F_UpdateDurability(v_slotIndex);
    }

    public void F_EmptySlot()
    {
        _itemImage.sprite = ResourceManager.Instance.emptySlotSprite;
        _itemStack.text = "";
        _usedSlot = false;
        _durability.gameObject.SetActive(false);
    }
    #endregion


    //�κ��丮 ���� ������ �巡�� ����
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canDrag)
        {
            GameManager.Instance._onDrag = true;
            _itemImage.transform.SetParent(_itemImage.transform.root);                      // root�� �θ�� ����
        }
    }

    // �巡�� ��
    public void OnDrag(PointerEventData eventData)
    {

        if (canDrag)
        {
           _itemImage.transform.position = eventData.position;                             // ������ �̹��� ���콺 ���󰡰�
        }
    }

    //�κ��丮 ���� ������ �巡�� ��
    public void OnEndDrag(PointerEventData eventData) 
    {
        if(canDrag)
        {
            GameManager.Instance._onDrag = false;
            _itemImage.transform.SetParent(_defaultParent);                                 // ���� �θ�� �缳��
            _itemImage.transform.localPosition = Vector3.zero;                              // �θ� �� ��ġ�� 0, 0, 0���� ����

            eventData = new PointerEventData(_es);
            eventData.position = Input.mousePosition;                                       // �̺�Ʈ �����Ϳ� ���콺 ��ġ �־���
            _gr.Raycast(eventData, results);                                                // �̺�Ʈ �����͸� results list�� �־���

            if (results.Count == 0)                                                         // UI�� �ƴ� ������ �巡�� ������ ����ó��.
                return;

            ItemSlot target_slot = results[0].gameObject.GetComponent<ItemSlot>();
            results.Clear();                                                                // �ʱ�ȭ

            if (target_slot == null)                                                        // �κ� ������ �ƴ� �ٸ����� �巡�� ������ ����ó��.
                return;

            int targetIndex = target_slot._slotIndex;                                       // �巡�װ� ���� ��ġ�� ����

            // ������ �����ϴ� �迭�� index�� �Ű������� �ѱ�. ( C# �迭 = �������� )
            ItemManager.Instance.inventorySystem.F_SwapItem(
                _slotIndex, targetIndex,ref _itemSlotRef,ref target_slot._itemSlotRef);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_usedSlot && !GameManager.Instance._onDrag)
        {
            if (eventData.button == PointerEventData.InputButton.Left)  // ��Ŭ�� ( ������ ���� �ֽ�ȭ )
            {
                int itemIndex = _itemSlotRef[_slotIndex].itemCode;
                UIManager.Instance.F_UpdateItemInformation(itemIndex);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)    // ��Ŭ�� ( ������ ���� ���)
            {
                int itemCode = _itemSlotRef[_slotIndex].itemCode;
                UIManager.Instance.F_SlotFunctionUI(_slotIndex, itemCode, _slotType);
            }
        }
    }
}
