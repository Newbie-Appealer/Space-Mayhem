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

    // 메테오 충돌 
    public void F_CrashMeteor()
    {
        // BuildFinishedBlock 이랑만 충돌, connector이랑은 충돌 x

        // 1. 충돌시 hp 감소
        _myBlockHp--;
        if (_myBlockHp <= 0)           // block 부서지는 시점 : hp가 0이 될 때 , connector은 hp가 -100 
        {
            // 3. 해당 블럭에 대해 커넥터 업데이트 
            BuildMaster.Instance.myBuildManger.F_SettingConnectorType((SelectedBuildType)_myBlockTypeIdx, gameObject.transform);
            BuildMaster.Instance.myBuildManger.F_DestroyConnetor((SelectedBuildType)_myBlockTypeIdx, gameObject.transform.position);

            StartCoroutine(F_Test());

        }
    }

    IEnumerator F_Test()
    {
        yield return new WaitForSeconds(0.1f);

        // 2. 블럭 삭제 
        Destroy(gameObject);
    }
}
