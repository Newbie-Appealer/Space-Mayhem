using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Install : UnCountableItem, UsableItem
{
    public InstallData data
    { get { return _data as InstallData; } }
    public Install(InstallData v_data) : base(v_data) { }

    public void F_UseItem()
    {
        // ItemManager���� ��ġ�� �������� �������� ��������,
        // ��ġ ����� �ٸ��Լ����� �����Ͽ� ȣ���ϸ�ɰͰ����ϴ�.
    }
}
