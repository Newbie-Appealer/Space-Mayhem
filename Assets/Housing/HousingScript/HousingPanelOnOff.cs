using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingPanelOnOff : MonoBehaviour
{
    [SerializeField]
    GameObject  _craftCanvas;

    private void Update()
    {
        if (Input.GetMouseButton(1))        // 우클릭을 하고 있는 동안
            F_OnOffCraftCanvas(true);       // canvas 보이게
        else if (Input.GetMouseButtonUp(1)) // 우클릭 떼면
            F_OnOffCraftCanvas(false);      // cavas 안보이게

    }
    public void F_OnOffCraftCanvas(bool v_check)
    {
        _craftCanvas.SetActive(v_check);
        F_SetMouseMove(v_check);
    }

    public void F_SetMouseMove(bool v_mode)
    {
        if (!v_mode)
        {
            Cursor.lockState = CursorLockMode.Locked;        // 커서를 '화면 정중앙'에 고정시킴
            Cursor.visible = false;                          // 커서 안 보이게
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;             // 커서 원래대로
            Cursor.visible = true;                              // 커서 보이게
        }
    }
}
