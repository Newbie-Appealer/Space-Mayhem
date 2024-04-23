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

    // ������ �浹�� Ŀ���͵� ������Ʈ
    public void F_BlockCollisionConnector(bool v_flag) 
    {
        Collider[] _colls = Physics.OverlapSphere(transform.position , 1f , BuildMaster.Instance.myBuildManger._tempWholeLayer);

        foreach (Collider col in _colls) 
        {
            // �� ���̶� �浹�� Ŀ���͵��� �� canconntor�� false��
            col.GetComponent<MyConnector>()._canConnect = v_flag;
        }
    }

    public void F_CrashMeteor() 
    {
        _myBlockHp--;
       Debug.Log("�ǹ� Ÿ��, ���� ü�� : " + _myBlockHp);
        if (_myBlockHp <= 0)
        {
            Debug.Log("�ǹ� �ı�");
            Destroy(gameObject);
        }
    }
}
