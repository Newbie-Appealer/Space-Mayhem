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
    public LayerMask _layerMask;                       // connector이 있는 레이어로 설정

    [Header("building object")]
    [SerializeField]
    GameObject[] _floorObject;                  // 바닥 오브젝트
    [SerializeField]
    GameObject[] _cellingObject;                // 천장 오브젝트
    [SerializeField]
    GameObject[] _wallObject;                   // 벽 오브젝트

    [Header("now Select Object")]
    [SerializeField]
    GameObject _nowSelectTempObject;              // 현재 선택 된 임시 오브젝트
    [SerializeField]
    SelectBuildType _nowSelectType;               // 현재 선택 된 임시 오브젝트의 타입 저장

    [Header(" Material ")]
    [SerializeField]
    Material _tempMaterial;

    private void Start()
    {
        instance = this;        // 싱글톤
    }

    public void Update()
    {
        // 플레이어 앞으로 ray 쏘기
        Debug.DrawRay(_player.transform.position, _player.transform.forward * 10, Color.red);

        // 만약 build 할 오브젝트가 있고
        // 좌클릭을 했을 때 -> 설치
        if (_nowSelectTempObject != null) 
        {
            if (Input.GetMouseButton(0)) 
            {
                // 설치하는 함수 추가
            }
        }
    }

    // HousingUiManager에서 인덱스를 받고 나서 실행 하는 게 좋을듯?
    // -> 그래서 F_startBuilding 안에 코루틴 실행  
    public void F_startBuiling(int v_typeIdx, int v_builgIdx)  
    {
        // 만약 ui 상 아무것도 셀렉 안 되어있으면
        if (v_typeIdx == -1)
            v_typeIdx = 0;
        if (v_builgIdx == -1)
            v_builgIdx = 0;

        // 1. 반환된 오브젝트를 instantiate 함
        _nowSelectTempObject = Instantiate(F_InstanseTempGameobj( v_typeIdx, v_builgIdx) );

        // 1-1. 생성한 오브젝트의 자식 Model 안의 오브젝트의 material을 바꿈 
        Transform _child = _nowSelectTempObject.transform.GetChild(0);
        for (int i = 0; i < _child.childCount ; i++) 
        {
            F_TempChangeMaterial(_child.GetChild(i) , _tempMaterial);
        }

        // 2. 오브젝트를 화면상 띄움 ( ray 사용)
        StartCoroutine(F_SetTempBuildingObj(_nowSelectTempObject) );

    }

    // 인덱스에 해당하는 오브젝트 반환
    public GameObject F_InstanseTempGameobj(int v_typeIdx, int v_builgIDx) 
    {
        // ui 상 내가 선택한 idx에 해당하는 오브젝트 return
        switch (v_typeIdx) 
        {
            // type이 0이면 -> wall
            case 0: 
                return _floorObject[v_builgIDx];

            // type이 1이면 -> celling
            case 1:    
                return _cellingObject[v_builgIDx];

            // type이 2이면 -> wall
            case 2:     
                return _wallObject[v_builgIDx];

            // 예외 : null return
            default: 
                return null;
        }
    }

    // ray에 오브젝트 띄우기
    IEnumerator F_SetTempBuildingObj(GameObject v_tempObj) 
    {
        // 마우스 위치에서 ray 쏘기 ( housing ui 가 꺼지면 마우스 커서는 중앙에 위치함)
        while (true) 
        {
            // 카메라 기준으로 ray 쏘기
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit _hit;

            if (Physics.Raycast(_ray, out _hit))
            {
                v_tempObj.transform.position = _hit.point;
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    // 오브젝트의 material 바꾸기
    public void F_TempChangeMaterial( Transform _obj , Material _material) 
    {
        if (_obj.GetComponent<MeshRenderer>() == null )
            return;

        _obj.GetComponent<MeshRenderer>().material = _material;
    }

   

    

}
