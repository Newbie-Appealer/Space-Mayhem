using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public int id;
    public string name;
    public string description;

    public Item(int i, string s, string s2)
    {
        id = i;
        name = s;
        description = s2;
    }
}
