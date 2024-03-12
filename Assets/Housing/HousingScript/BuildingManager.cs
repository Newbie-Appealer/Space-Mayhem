using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum SelectBuildType 
{
    floor,
    celling,
    wall
}

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;

    [Header("Player")]
    public GameObject _player;

    [Header("Build Setting")]
    [SerializeField]
    public LayerMask _layerMask;                       // connector�� �ִ� ���̾�� ����

    [Header("building object")]
    [SerializeField]
    GameObject[] _floorObject;                  // �ٴ� ������Ʈ
    [SerializeField]
    GameObject[] _cellingObject;                // õ�� ������Ʈ
    [SerializeField]
    GameObject[] _wallObject;                   // �� ������Ʈ

    [Header("now Select Object")]
    [SerializeField]
    GameObject _nowSelectTempObject;              // ���� ���� �� �ӽ� ������Ʈ
    [SerializeField]
    SelectBuildType _nowSelectType;               // ���� ���� �� �ӽ� ������Ʈ�� Ÿ�� ����

    [Header(" Material ")]
    [SerializeField]
    Material _tempMaterial;

    private void Start()
    {
        instance = this;        // �̱���
    }

    public void Update()
    {
        // �÷��̾� ������ ray ���
        Debug.DrawRay(_player.transform.position, _player.transform.forward * 10, Color.red);

        // ���� build �� ������Ʈ�� �ְ�
        // ��Ŭ���� ���� �� -> ��ġ
        if (_nowSelectTempObject != null) 
        {
            if (Input.GetMouseButton(0)) 
            {
                // ��ġ�ϴ� �Լ� �߰�
            }
        }
    }

    // HousingUiManager���� �ε����� �ް� ���� ���� �ϴ� �� ������?
    // -> �׷��� F_startBuilding �ȿ� �ڷ�ƾ ����  
    public void F_startBuiling(int v_typeIdx, int v_builgIdx)  
    {
        // ���� ui �� �ƹ��͵� ���� �� �Ǿ�������
        if (v_typeIdx == -1)
            v_typeIdx = 0;
        if (v_builgIdx == -1)
            v_builgIdx = 0;

        // 1. ��ȯ�� ������Ʈ�� instantiate ��
        _nowSelectTempObject = Instantiate(F_InstanseTempGameobj( v_typeIdx, v_builgIdx) );

        // 1-1. ������ ������Ʈ�� �ڽ� Model ���� ������Ʈ�� material�� �ٲ� 
        Transform _child = _nowSelectTempObject.transform.GetChild(0);
        for (int i = 0; i < _child.childCount ; i++) 
        {
            F_TempChangeMaterial(_child.GetChild(i) , _tempMaterial);
        }

        // 2. ������Ʈ�� ȭ��� ��� ( ray ���)
        StartCoroutine(F_SetTempBuildingObj(_nowSelectTempObject) );

    }

    // �ε����� �ش��ϴ� ������Ʈ ��ȯ
    public GameObject F_InstanseTempGameobj(int v_typeIdx, int v_builgIDx) 
    {
        // ui �� ���� ������ idx�� �ش��ϴ� ������Ʈ return
        switch (v_typeIdx) 
        {
            // type�� 0�̸� -> wall
            case 0: 
                return _floorObject[v_builgIDx];

            // type�� 1�̸� -> celling
            case 1:    
                return _cellingObject[v_builgIDx];

            // type�� 2�̸� -> wall
            case 2:     
                return _wallObject[v_builgIDx];

            // ���� : null return
            default: 
                return null;
        }
    }

    // ray�� ������Ʈ ����
    IEnumerator F_SetTempBuildingObj(GameObject v_tempObj) 
    {
        // ���콺 ��ġ���� ray ��� ( housing ui �� ������ ���콺 Ŀ���� �߾ӿ� ��ġ��)
        while (true) 
        {
            // ī�޶� �������� ray ���
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit _hit;

            if (Physics.Raycast(_ray, out _hit))
            {
                v_tempObj.transform.position = _hit.point;
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    // ������Ʈ�� material �ٲٱ�
    public void F_TempChangeMaterial( Transform _obj , Material _material) 
    {
        if (_obj.GetComponent<MeshRenderer>() == null )
            return;

        _obj.GetComponent<MeshRenderer>().material = _material;
    }

   

    

}
