using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Furniture : MonoBehaviour
{
    [Header("Initialize inspector")]
    [SerializeField] private int _InstantiateIndex;         // 불러오기에 필요한 index
    public int InstantiateIndex => _InstantiateIndex;

    protected string _data;
    private void Awake()
    {
        F_InitFurniture();   
    }

    protected abstract void F_InitFurniture();
    public virtual void F_Interaction()
    {
        // 상호작용 없는 구조물은 오버라이딩 하지않기
    }

    /// <summary>
    /// 설치류 내부 데이터를 반환받는 함수 
    /// </summary>
    /// <returns>설치류의 내부 데이터 (json) </returns>
    public abstract string F_GetData();

    public abstract void F_SetData(string v_data);
}
