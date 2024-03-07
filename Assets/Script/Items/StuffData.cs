using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StuffData : CountableData
{
    /// <summary> v_code : 아이템 코드  /  v_name : 아이템 이름  /  v_description : 아이템 설명 </summary>
    public StuffData(int v_code, string v_name, string v_description) : base(v_code, v_name, v_description)
    { }
}
