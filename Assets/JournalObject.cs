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

    // 1. 생성할때  JournalObject가 가질수있는 Key값을 활용하여
    //    중복되지않은 키값을 추려내기
    // 2. 중복되지않은 키값중 랜덤으로 하나를 키값으로 가지기
    // 3. 
}
