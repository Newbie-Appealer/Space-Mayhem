using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Install : UnCountableItem, UsableItem
{
    public InstallData _installData;
    public Install(InstallData v_data) : base(v_data)
    {
        _installData = v_data;
    }

    public void F_UseItem()
    {
        // ItemManager���� ��ġ�� �������� �������� ��������,
        // ��ġ ����� �ٸ��Լ����� �����Ͽ� ȣ���ϸ�ɰͰ����ϴ�.
    }
}
