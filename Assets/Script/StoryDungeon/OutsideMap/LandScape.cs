using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LandScape 
{
    [SerializeField] private PlanetType _planetType;                      // 타입

    [Header("Sacle")]
    private int _minWidth; private int _maxWidth;       // 최소, 최대 width
    private int _minHeight; private int _maxHeight;     // 최소, 최대 height
    private int _noiseScale;                             // 노이즈 크기
    private int _devigation;                            // height를 얼마나 올릴것인가?

    [Header("octaves")]
    private int _octave;             // 옥타브
    private float _persistance;      // 진폭(amplitude) 크기 (얼마나 낮~높 을지) 결정
    private float _lacunerity;       // 빈도(Frequency) 의 폭 결정

    [Header("Material")]
    private Material _defaultMaterial;                   // default Material
    private List<Tuple<float, Material>> _landHeightMaterial;    // 각 지형의 높이, Material

    #region 프로퍼티
    public PlanetType planetType { get => _planetType; }
    public List<Tuple<float, Material>> LandHeight { get => _landHeightMaterial; }
    public int minWidth => _minWidth;
    public int maxWidth => _maxWidth;
    public int minHeight => _minHeight;
    public int maxHeight => _maxHeight;
    public int noiseScale => _noiseScale;
    public int devigation => _devigation;
    public int octave => _octave;
    public float persistance => _persistance;
    public float lacunerity => _lacunerity;
    #endregion

    // 초기화
    public void F_InitLandScape(string[] v_data) 
    {
        // [0] : PlanetType 
        // [1] : minWidth
        // [2] : maxWidth
        // [3] : minHeight
        // [4] : maxheight
        // [5] : noiseScale
        // [6] : devigation
        // [7] : octave
        // [8] : persis
        // [9] : lacu
        // [10] : height / (_)로 구분 

        this._planetType     = (PlanetType)Enum.Parse(typeof(PlanetType), v_data[0]);
        this._minWidth      = int.Parse(v_data[1]); 
        this._maxWidth      = int.Parse(v_data[2]);
        this._minHeight     = int.Parse(v_data[3]); 
        this._maxHeight     = int.Parse(v_data[4]);
        this._noiseScale    = int.Parse(v_data[5]);
        this._devigation    = int.Parse(v_data[6]);
        this._octave        = int.Parse(v_data[7]);
        this._persistance   = float.Parse(v_data[8]);
        this._lacunerity    = float.Parse(v_data[9]);

        F_InitHaighMaterial(v_data[0] , v_data[10]);
    }

    // 높이와 material 초기화
    private void F_InitHaighMaterial( string v_type, string v_height ) 
    {
        // ## TODO 
        // 1. string값으로 material[] 가져오기
        // 2. tuple 만들어서 List에 넣기 

        // 1. 초기화
        _landHeightMaterial = new List<Tuple<float, Material>>();

        string _path = "OutsideMapMaterial/" + v_type;              // type에 따른 경로 접근
        Material[] _mat = Resources.LoadAll<Material>(_path);       // 경로에 있는 material 다 들고오기
        string[] heightParts = v_height.Split('_');                 // _기준으로 height string 자르기

        for (int i = 0; i < heightParts.Length; i++)        // 마지막은 default material 
        {
            // 1. Tuple 만들기
            Tuple<float , Material> _tu 
                = new Tuple<float , Material>(int.Parse(heightParts[i]) , _mat[i]);

            // 2. list에 넣기
            _landHeightMaterial.Add(_tu);   
        }

        // 3. default material 설정
        this._defaultMaterial = _mat[_mat.Length - 1];          // 마지막이 default
    }

    // 높이에 따른 material return
    public Material F_GetMaterial(float v_height)
    {
        foreach (Tuple<float, Material> _tu in _landHeightMaterial)
        {
            if (_tu.Item1 >= v_height)
                return _tu.Item2;
        }

        return _defaultMaterial;
    }

}
