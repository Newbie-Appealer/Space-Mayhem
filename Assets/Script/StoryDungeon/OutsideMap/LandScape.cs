using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LandScape 
{
    // 각 지형에 대한 정보
    private List<Tuple<float, Material>> _landHeight;

    // 프로퍼티
    public List<Tuple<float, Material>> LandHeight { get => _landHeight; }

    public LandScape(List<Tuple<float, Material>> v_list)
    {
        _landHeight = v_list;
    }

    public Material F_GetMaterial(float v_height)
    {
        foreach (Tuple<float, Material> _tu in _landHeight)
        {
            if (_tu.Item1 >= v_height)
                return _tu.Item2;
        }

        return OutsideMapManager.Instance._defultMaterial;
    }

}
