using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalObject : DropObject
{
    public string _journalKey = "";
    public override void F_GetObject()
    {
        // TODO:JournalKey 값 초기화
        // JournalObject 어디에 배치할지 

        // 오브젝트 삭제
        Destroy(gameObject);
    }
}
