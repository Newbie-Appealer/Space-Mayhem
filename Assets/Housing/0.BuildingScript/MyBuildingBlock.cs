using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBuildingBlock : MonoBehaviour
{
    [Header("Block Field")]
    [SerializeField] int _myBlockTypeIdx;       // 블럭 type 인덱스
    [SerializeField] int _myBlockDetailIdx;     // 블럭 detail 인덱스
    [SerializeField] int _myBlockHp;            // hp  
    [SerializeField] int _myBlockMaxHp;         // 내 블럭 max hp
    [SerializeField] Vector3 _myPosition;       // 위치
    [SerializeField] Vector3 _myRotation;       // 회전 

    private void Awake()
    {
        _myPosition = transform.position;
    }

    // 프로퍼티
    public int MyBlockTypeIdx { get => _myBlockTypeIdx;}
    public int MyBlockDetailIdx { get=> _myBlockDetailIdx; }   
    public int MyBlockHp { get => _myBlockHp; set { _myBlockHp = value; } } 
    public int MyBlockMaxHp { get => _myBlockMaxHp; set { _myBlockMaxHp = value; } }

    public Vector3 MyPosition { get => _myPosition; }
    public Vector3 MyRotation { get => _myRotation;  }

    // 필드 세팅 
    public void F_SetBlockFeild( int v_type , int v_det , int v_hp , int v_maxHp) 
    { 
        this._myBlockTypeIdx        = v_type;
        this._myBlockDetailIdx      = v_det;
        this._myBlockHp             = v_hp;
        this._myBlockMaxHp          = v_maxHp;
        _myPosition = gameObject.transform.position;
        _myRotation = gameObject.transform.rotation.eulerAngles;
    }

    // 나한테 충돌한 커넥터들 업데이트
    public void F_BlockCollisionConnector(bool v_flag) 
    {
        Collider[] _colls = Physics.OverlapSphere(transform.position , 1f , BuildMaster.Instance.myBuildManger._tempWholeLayer);

        foreach (Collider col in _colls) 
        {
            // 내 블럭이랑 충돌한 커넥터들을 다 canconntor를 false로
            col.GetComponent<MyConnector>()._canConnect = v_flag;
        }
    }

    public void F_CrashMeteor() 
    {
        _myBlockHp--;
       Debug.Log("건물 타격, 현재 체력 : " + _myBlockHp);
        if (_myBlockHp <= 0)
        {
            Debug.Log("건물 파괴");
            Destroy(gameObject);
        }
    }
}
