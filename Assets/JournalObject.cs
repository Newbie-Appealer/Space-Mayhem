using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalObject : DropObject
{
    public string _journalKey = "";
    public override void F_GetObject()
    {
        // TODO:JournalKey �� �ʱ�ȭ
        // JournalObject ��� ��ġ���� 

        // ������Ʈ ����
        Destroy(gameObject);
    }

    // 1. �����Ҷ�  JournalObject�� �������ִ� Key���� Ȱ���Ͽ�
    //    �ߺ��������� Ű���� �߷�����
    // 2. �ߺ��������� Ű���� �������� �ϳ��� Ű������ ������
    // 3. 
}
