using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : UnCountableItem, UsableItem
{
    public ToolData data
    { get { return _data as ToolData; } }
    public Tool(ToolData v_data) : base(v_data)
    { }
    public void F_UseItem()
    {
        // �÷��̾ �������� ���(����) �ϸ� PlayerManager �Ǵ� Controller �� ������� �Լ� ȣ���ϰ�, 
        // ȣ���� �Լ����� ������ ����� Ȱ��ȭ�����ָ� �ɰŰ����ϴ�.
    }
}
