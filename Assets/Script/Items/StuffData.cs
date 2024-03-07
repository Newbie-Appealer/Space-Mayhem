using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StuffData : CountableData
{
    /// <summary> v_code : ������ �ڵ�  /  v_name : ������ �̸�  /  v_description : ������ ���� </summary>
    public StuffData(int v_code, string v_name, string v_description) : base(v_code, v_name, v_description)
    { }
}
