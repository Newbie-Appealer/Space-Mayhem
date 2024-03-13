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


    //�κ��丮 ���� ������ �巡�� ����
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_usedSlot)
        {
            Debug.Log("Begin index : " + _slotIndex);
            _itemImage.transform.SetParent(_itemImage.transform.root);                      // root�� �θ�� ����
        }
    }

    // �巡�� ��
    public void OnDrag(PointerEventData eventData)
    {

        if (_usedSlot)
           _itemImage.transform.position = eventData.position;                             // ������ �̹��� ���콺 ���󰡰�
    }

    //�κ��丮 ���� ������ �巡�� ��
    public void OnEndDrag(PointerEventData eventData) 
    {
        if(_usedSlot)
        {

            _itemImage.transform.SetParent(_defaultParent);                                 // ���� �θ�� �缳��
            _itemImage.transform.localPosition = Vector3.zero;                              // �θ� �� ��ġ�� 0, 0, 0���� ����

            // _itemImage.raycastTarget = true;

            eventData = new PointerEventData(_es);
            eventData.position = Input.mousePosition;                                       // �̺�Ʈ �����Ϳ� ���콺 ��ġ �־���
            _gr.Raycast(eventData, results);                                                // �̺�Ʈ �����͸� results list�� �־���

            if (results.Count == 0)                                                         // UI�� �ƴ� ������ �巡�� ������ ����ó��.
                return;

            ItemSlot tmp_slot = results[0].gameObject.GetComponent<ItemSlot>();
            results.Clear();                                                                // �ʱ�ȭ

            if (tmp_slot == null)                                                           // �κ� ������ �ƴ� �ٸ����� �巡�� ������ ����ó��.
                return;

            int index = tmp_slot._slotIndex;                                                // �巡�װ� ���� ��ġ�� ����
            Debug.Log("end index : " + index);
            ItemManager.Instance.inventorySystem.F_SwapItem(_slotIndex, index);             // ���� �õ�
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_usedSlot)
        {
            if (eventData.button == PointerEventData.InputButton.Left)  // ��Ŭ�� ( ������ ���� �ֽ�ȭ )
            {
                int itemIndex = ItemManager.Instance.inventorySystem.inventory[_slotIndex].itemCode;
                UIManager.Instance.F_UpdateInventoryInformation(itemIndex);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)    // ��Ŭ�� ( ������ ����/��� ���)
            {
                UIManager.Instance.F_SlotFunctionUI(_slotIndex);
            }            
        }
    }
}
