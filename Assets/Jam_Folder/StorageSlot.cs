using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StorageSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("UI")]
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _itemStack;

    [Header("Slot Information")]
    [SerializeField] private bool _usedSlot;
    private bool canDrag => _usedSlot;
    

    [Header("MoussEvent")]
    private GraphicRaycaster _gr;
    private EventSystem _es;
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


    public void F_UpdateSlot(int v_code, int v_stack)
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
    #endregion    //인벤토리 슬롯 아이템 드래그 시작


    public void OnBeginDrag(PointerEventData eventData)
    {
        if(canDrag)
        {
            _itemImage.transform.SetParent(_itemImage.transform.root);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canDrag)
            _itemImage.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(canDrag)
        {
            _itemImage.transform.SetParent(_defaultParent);
            _itemImage.transform.localPosition = Vector3.zero;

            eventData = new PointerEventData(_es);
            eventData.position = Input.mousePosition;
            _gr.Raycast(eventData, results);

            if (results.Count == 0)
                return;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
}
