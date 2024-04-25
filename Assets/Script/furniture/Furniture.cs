using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

public abstract class Furniture : MonoBehaviour
{
    [Header("Initialize inspector")]
    [SerializeField] protected int _itemCode;                 // 아이템 회수시 획득하게될 아이템번호
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

    public virtual void F_ChangeEnergyState(bool v_state)
    {
        
    }

    public virtual void F_ChangeFilterState(bool v_state)
    {
        
    }

    public virtual void F_TakeFurniture()
    {
        if(ItemManager.Instance.inventorySystem.F_GetItem(_itemCode))   // 인벤토리에 아이템 추가 시도
        {
            ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();
            Destroy(this.gameObject);                                   // 아이템 획득 성공
                                                                        // 시 오브젝트 파괴
            // 일부 오브젝트는 회수가 되지않거나,
            // 회수되어야할때 필요한 동작이 있음.
            // virtual 함수로 선언했으니 동작이 다르게 되어야하는 오브젝트는 override 해줍시다.
        }
    }
}

// TODO:설치해둔 아이템 회수기능
//   -일부 아이템 ( ex 필터기 ) 는 회수시 즉시 파괴해야함. -> 가상함수로 하면될듯
    