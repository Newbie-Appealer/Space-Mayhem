using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBuildingBlock : MonoBehaviour
{
    [Header("Block Field")]
    [SerializeField] int _myBlockTypeIdx;       // �� type �ε���
    [SerializeField] int _myBlockDetailIdx;     // �� detail �ε���
    [SerializeField] int _myBlockHp;            // hp  
    [SerializeField] int _myBlockMaxHp;         // �� �� max hp
    [SerializeField] Vector3 _myPosition;       // ��ġ
    [SerializeField] Vector3 _myRotation;       // ȸ�� 

    private void Awake()
    {
        _myPosition = transform.position;
    }

    // ������Ƽ
    public int MyBlockTypeIdx { get => _myBlockTypeIdx;}
    public int MyBlockDetailIdx { get=> _myBlockDetailIdx; }   
    public int MyBlockHp { get => _myBlockHp; set { _myBlockHp = value; } } 
    public int MyBlockMaxHp { get => _myBlockMaxHp; set { _myBlockMaxHp = value; } }

    public Vector3 MyPosition { get => _myPosition; }
    public Vector3 MyRotation { get => _myRotation;  }

    // �ʵ� ���� 
    public void F_SetBlockFeild( int v_type , int v_det , int v_hp , int v_maxHp) 
    { 
        this._myBlockTypeIdx        = v_type;
        this._myBlockDetailIdx      = v_det;
        this._myBlockHp             = v_hp;
        this._myBlockMaxHp          = v_maxHp;
        _myPosition = gameObject.transform.position;
        _myRotation = gameObject.transform.rotation.eulerAngles;
    }

    // ���׿� �浹 
    public void F_CrashMeteor()
    {
        // BuildFinishedBlock �̶��� �浹, connector�̶��� �浹 x

        // 1. �浹�� hp ����
        _myBlockHp--;
        if (_myBlockHp <= 0)           // block �μ����� ���� : hp�� 0�� �� �� , connector�� hp�� -100 
        {
            // 3. �ش� ���� ���� Ŀ���� ������Ʈ 
            BuildMaster.Instance.myBuildManger.F_SettingConnectorType((SelectedBuildType)_myBlockTypeIdx, gameObject.transform);
            BuildMaster.Instance.myBuildManger.F_DestroyConnetor((SelectedBuildType)_myBlockTypeIdx, gameObject.transform.position);

            StartCoroutine(F_Test());

        }
    }

    IEnumerator F_Test()
    {
        yield return new WaitForSeconds(0.1f);

        // 2. �� ���� 
        Destroy(gameObject);
    }
}
