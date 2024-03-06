using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HousingDetailSlot : MonoBehaviour , IPointerExitHandler
{
    [SerializeField]
    int _idx;

    public int Idx { get=> _idx; set { _idx = value; } }

    public void OnPointerExit(PointerEventData eventData)
    {
        HousingUI.instance.F_PointerInSlot(_idx);
    }
}
