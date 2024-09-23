using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalObject : DropObject
{
    private string[] KEY = {
        "otherServiver1",
        "otherServiver2",
        "otherServiver3",
        "otherServiver4",
        "otherServiver5",
        "otherServiver6"
    };
    private List<int> _availableKEY;
    public string _journalKey = "";

    private void Start()
    {
        _availableKEY = new List<int>();

        if(!F_GetKey(out _journalKey))
            Destroy(gameObject);

    }

    public override void F_GetObject()
    {
        // Ű ȹ�� / // ���� ȹ�� �޼��� �˾�
        if (GameManager.Instance.journalSystem.F_GetJournal(_journalKey))
            UIManager.Instance.F_PlayerMessagePopupTEXT("������ ȹ���Ͽ����ϴ�. 'B'�� ���� ������ Ȯ���ϼ���.", 2f);

        // ������Ʈ ����
        Destroy(gameObject);
    }

    private bool F_GetKey(out string key)
    {
        key = "";
        for(int index = 0; index < KEY.Length; index++)
        {
            // Key[index] �� ���� ����Ҽ��ִ��� üũ ( �ߺ� Ȯ�� )
            bool check = GameManager.Instance.journalSystem.F_CheckKey(KEY[index]);

            // ��밡���� Ű�� _availableKEY�� �߰�
            if (!check)
                _availableKEY.Add(index);
        }

        // ��밡���� Ű�� ���ٸ� ������Ʈ ����
        if(_availableKEY.Count == 0)
            return false;


        int randomIndex = _availableKEY[Random.Range(0, _availableKEY.Count)];
        key = KEY[randomIndex];
        return true;
    }
}