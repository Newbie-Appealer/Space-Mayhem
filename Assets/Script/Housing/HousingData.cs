using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingData
{
    private int _hp;
    private string _name;

    public string Name { get => _name; set { _name = value; } }

    public HousingData(int v_hp , string v_str ) 
    {
        this._hp = v_hp;
        this._name = v_str;
    }

}
