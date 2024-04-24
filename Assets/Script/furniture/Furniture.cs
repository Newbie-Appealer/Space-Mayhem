using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

public abstract class Furniture : MonoBehaviour
{
    [Header("Initialize inspector")]
    [SerializeField] private int _InstantiateIndex;         // �ҷ����⿡ �ʿ��� index
    public int InstantiateIndex => _InstantiateIndex;

    [Header("=== Furniture Energy State ===")]
    [SerializeField] protected bool _onEnergy;
    [SerializeField] protected bool _onFilter;
    public bool onEnergy => _onEnergy;
    public bool onFilter => _onFilter;
    private void Awake()
    {
        F_InitFurniture();   
    }

    protected abstract void F_InitFurniture();
    public virtual void F_Interaction()
    {
        // ��ȣ�ۿ� ���� �������� �������̵� �����ʱ�
    }

    /// <summary>
    /// ��ġ�� ���� �����͸� ��ȯ�޴� �Լ� 
    /// </summary>
    /// <returns>��ġ���� ���� ������ (json) </returns>
    public abstract string F_GetData();

    /// <summary>
    /// �Ű������� ���� �����͸� ��ġ�� �����Ϳ� �����Ű�� �Լ�
    /// </summary>
    /// <param name="v_data"> Json ������ </param>
    public abstract void F_SetData(string v_data);

    public virtual void F_ChangeEnergyState(bool v_state)
    {
        
    }

    public virtual void F_ChangeFilterState(bool v_state)
    {
        
    }
}

// TODO:��ġ�ص� ������ ȸ�����
//   -�Ϻ� ������ ( ex ���ͱ� ) �� ȸ���� ��� �ı��ؾ���. -> �����Լ��� �ϸ�ɵ�
