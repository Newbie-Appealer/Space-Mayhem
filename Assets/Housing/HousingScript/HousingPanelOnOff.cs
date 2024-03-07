using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingPanelOnOff : MonoBehaviour
{
    [SerializeField]
    GameObject  _craftCanvas;

    private void Update()
    {
        if (Input.GetMouseButton(1))        // ��Ŭ���� �ϰ� �ִ� ����
            F_OnOffCraftCanvas(true);       // canvas ���̰�
        else if (Input.GetMouseButtonUp(1)) // ��Ŭ�� ����
            F_OnOffCraftCanvas(false);      // cavas �Ⱥ��̰�

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
            Cursor.lockState = CursorLockMode.Locked;        // Ŀ���� 'ȭ�� ���߾�'�� ������Ŵ
            Cursor.visible = false;                          // Ŀ�� �� ���̰�
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;             // Ŀ�� �������
            Cursor.visible = true;                              // Ŀ�� ���̰�
        }
    }
}
