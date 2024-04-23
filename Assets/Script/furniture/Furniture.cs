using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

public abstract class Furniture : MonoBehaviour
{
    [Header("Initialize inspector")]
    [SerializeField] private int _InstantiateIndex;         // 불러오기에 필요한 index
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
        // 상호작용 없는 구조물은 오버라이딩 하지않기
    }

    /// <summary>
    /// 설치류 내부 데이터를 반환받는 함수 
    /// </summary>
    /// <returns>설치류의 내부 데이터 (json) </returns>
    public abstract string F_GetData();

    /// <summary>
    /// 매개변수로 받은 데이터를 설치류 데이터에 적용시키는 함수
    /// </summary>
    /// <param name="v_data"> Json 데이터 </param>
    public abstract void F_SetData(string v_data);

    public void F_ChangeEnergyState(bool v_state)
    {
        _onEnergy = v_state;
    }
}