using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LandScape 
{
    [SerializeField] private PlanetType _planetType;                      // Ÿ��

    [Header("Sacle")]
    private int _minWidth; private int _maxWidth;       // �ּ�, �ִ� width
    private int _minHeight; private int _maxHeight;     // �ּ�, �ִ� height
    private int _noiseScale;                             // ������ ũ��
    private int _devigation;                            // height�� �󸶳� �ø����ΰ�?

    [Header("octaves")]
    private int _octave;             // ��Ÿ��
    private float _persistance;      // ����(amplitude) ũ�� (�󸶳� ��~�� ����) ����
    private float _lacunerity;       // ��(Frequency) �� �� ����

    [Header("Material")]
    private Material _defaultMaterial;                   // default Material
    private List<Tuple<float, Material>> _landHeightMaterial;    // �� ������ ����, Material

    #region ������Ƽ
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

    // �ʱ�ȭ
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
        // [10] : height / (_)�� ���� 

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

    // ���̿� material �ʱ�ȭ
    private void F_InitHaighMaterial( string v_type, string v_height ) 
    {
        // ## TODO 
        // 1. string������ material[] ��������
        // 2. tuple ���� List�� �ֱ� 

        // 1. �ʱ�ȭ
        _landHeightMaterial = new List<Tuple<float, Material>>();

        string _path = "OutsideMapMaterial/" + v_type;              // type�� ���� ��� ����
        Material[] _mat = Resources.LoadAll<Material>(_path);       // ��ο� �ִ� material �� ������
        string[] heightParts = v_height.Split('_');                 // _�������� height string �ڸ���

        for (int i = 0; i < heightParts.Length; i++)        // �������� default material 
        {
            // 1. Tuple �����
            Tuple<float , Material> _tu 
                = new Tuple<float , Material>(int.Parse(heightParts[i]) , _mat[i]);

            // 2. list�� �ֱ�
            _landHeightMaterial.Add(_tu);   
        }

        // 3. default material ����
        this._defaultMaterial = _mat[_mat.Length - 1];          // �������� default
    }

    // ���̿� ���� material return
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
