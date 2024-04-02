using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Furniture : MonoBehaviour
{
    [Header("Initialize inspector")]
    [SerializeField] private int _InstantiateIndex;         // �ҷ����⿡ �ʿ��� index

    private void Start()
    {
        F_InitFurniture();   
    }

    protected abstract void F_InitFurniture();
    public virtual void F_Interaction() 
    {
        // ��ȣ�ۿ� ���� �������� �������̵� �����ʱ�
    }
}
