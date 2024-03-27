using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBuildingBlock : MonoBehaviour
{
    // Block의 충돌감지
    // -> model의 콜라이더가 충돌하면 ( transform.parent ), MyBuilidngBlock의 함수 실행 하면될듯

    [Header("Block Field")]
    [SerializeField] int _myBlockTypeIdx;       // 블럭 type 인덱스
    [SerializeField] int _myBlockDetailIdx;     // 블럭 detail 인덱스
    [SerializeField] int _myBlockHp;            // hp  
    [SerializeField] Vector3 _myPosition;       // 위치

    private void Start()
    {
        _myPosition = transform.position;
    }

    // 프로퍼티
    public int MyBlockTypeIdx { get => _myBlockTypeIdx; }
    public int MyBlockDetailIdx { get=> _myBlockDetailIdx; }   
    public int MyBlockHp { get => _myBlockHp; }
    public Vector3 MyPosition { get => _myPosition; }

    // 필드 세팅 
    public void F_SetBlockFeild( int v_type , int v_det , int v_hp) 
    { 
        this._myBlockTypeIdx = v_type;
        this._myBlockDetailIdx = v_det;
        this._myBlockHp = v_hp;
        _myPosition = gameObject.transform.position;
    }
    
}
