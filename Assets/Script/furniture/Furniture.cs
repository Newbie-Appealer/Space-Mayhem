using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Furniture : MonoBehaviour
{
    [Header("Initialize inspector")]
    [SerializeField] private int _InstantiateIndex;         // �ҷ����⿡ �ʿ��� index
    public int InstantiateIndex => _InstantiateIndex;

    protected string _data;
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

    public abstract void F_SetData(string v_data);
}
