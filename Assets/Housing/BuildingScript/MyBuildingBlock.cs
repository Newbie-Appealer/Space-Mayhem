using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBuildingBlock : MonoBehaviour
{
    // Block�� �浹����
    // -> model�� �ݶ��̴��� �浹�ϸ� ( transform.parent ), MyBuilidngBlock�� �Լ� ���� �ϸ�ɵ�

    [Header("Block Field")]
    [SerializeField] int _myBlockTypeIdx;       // �� type �ε���
    [SerializeField] int _myBlockDetailIdx;     // �� detail �ε���
    [SerializeField] int _myBlockHp;            // hp  
    [SerializeField] Vector3 _myPosition;       // ��ġ

    private void Start()
    {
        _myPosition = transform.position;
    }

    // ������Ƽ
    public int MyBlockTypeIdx { get => _myBlockTypeIdx; }
    public int MyBlockDetailIdx { get=> _myBlockDetailIdx; }   
    public int MyBlockHp { get => _myBlockHp; }
    public Vector3 MyPosition { get => _myPosition; }

    // �ʵ� ���� 
    public void F_SetBlockFeild( int v_type , int v_det , int v_hp) 
    { 
        this._myBlockTypeIdx = v_type;
        this._myBlockDetailIdx = v_det;
        this._myBlockHp = v_hp;
        _myPosition = gameObject.transform.position;
    }
    
}
