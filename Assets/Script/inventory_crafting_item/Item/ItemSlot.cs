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


    //�κ��丮 ���� ������ �巡�� ����
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canDrag)
        {
            _itemImage.transform.SetParent(_itemImage.transform.root);                      // root�� �θ�� ����
        }
    }

    // �巡�� ��
    public void OnDrag(PointerEventData eventData)
    {

        if (canDrag)
           _itemImage.transform.position = eventData.position;                             // ������ �̹��� ���콺 ���󰡰�
    }

    //�κ��丮 ���� ������ �巡�� ��
    public void OnEndDrag(PointerEventData eventData) 
    {
        if(canDrag)
        {
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
        if (_usedSlot)
        {
            if (eventData.button == PointerEventData.InputButton.Left)  // ��Ŭ�� ( ������ ���� �ֽ�ȭ )
            {
                int itemIndex = ItemManager.Instance.inventorySystem.inventory[_slotIndex].itemCode;
                UIManager.Instance.F_UpdateItemInformation(itemIndex);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)    // ��Ŭ�� ( ������ ���� ���)
            {
                UIManager.Instance.F_SlotFunctionUI(_slotIndex);
            }
        }
    }
}
