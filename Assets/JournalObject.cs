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
        _journalKey = F_Key();
    }

    public override void F_GetObject()
    {
        // 키 획득
        GameManager.Instance.journalSystem.F_GetJournal(_journalKey);

        // 일지 획득 메세지 팝업
        UIManager.Instance.F_PlayerMessagePopupTEXT(
            "got the journal. Press B to check your journals", 2f);

        // 오브젝트 삭제
        Destroy(gameObject);
    }

    private string F_Key()
    {
        for(int index = 0; index < KEY.Length; index++)
        {
            // Key[index] 의 값을 사용할수있는지 체크 ( 중복 확인 )
            bool check = GameManager.Instance.journalSystem.F_CheckKey(KEY[index]);

            // 사용가능한 키를 _availableKEY에 추가
            if (check)
                _availableKEY.Add(index);
        }

        // 사용가능한 키가 없다면 오브젝트 삭제
        if(_availableKEY.Count == 0)
            Destroy(gameObject);

        int randomIndex = _availableKEY[Random.Range(0, _availableKEY.Count)];
        string ret = KEY[randomIndex];
        return ret;
    }
}