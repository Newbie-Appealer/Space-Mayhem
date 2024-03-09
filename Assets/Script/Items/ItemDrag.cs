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
        _parentAfterDrag = transform.parent; //현재 부모를 담아둠
        transform.SetParent(transform.root); // 이 오브젝트 부모를 최상위 개체 바로 밑(Canvas)으로 지정
        transform.SetAsLastSibling(); //가장 하위(화면 상 최상위) 오브젝트로 지정
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        transform.position = Input.mousePosition; //마우스 위치를 따라감
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("EndDrag");
        transform.SetParent(_parentAfterDrag); //클릭할 때 변경했던 부모를 원래대로 변경
        image.raycastTarget = true;
    }
}
