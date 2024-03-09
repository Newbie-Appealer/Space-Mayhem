using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Image image;
    [HideInInspector] public Transform _parentAfterDrag;
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("BeginDrag");
        _parentAfterDrag = transform.parent; //���� �θ� ��Ƶ�
        transform.SetParent(transform.root); // �� ������Ʈ �θ� �ֻ��� ��ü �ٷ� ��(Canvas)���� ����
        transform.SetAsLastSibling(); //���� ����(ȭ�� �� �ֻ���) ������Ʈ�� ����
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        transform.position = Input.mousePosition; //���콺 ��ġ�� ����
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("EndDrag");
        transform.SetParent(_parentAfterDrag); //Ŭ���� �� �����ߴ� �θ� ������� ����
        image.raycastTarget = true;
    }
}
