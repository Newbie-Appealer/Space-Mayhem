using System;
using System.Collections.Generic;
using UnityEngine;

public class InstallSystem : MonoBehaviour
{
    [Header("player object")]
    [SerializeField] Camera _mainCamera;

    [Header("install object")]
    [SerializeField] GameObject[] _installObjects;
    [SerializeField] Transform _installTransform;

    [Header("preview object")]
    [SerializeField] GameObject[] _previewObjects;
    [SerializeField] Transform _previewTransform;
    List<GameObject> _objectList_Preview;
    GameObject _selectObject_Preview;

    [Header("raycasting")]
    [SerializeField] LayerMask _PreviewObjLayer;
    Vector3 _hitPos;
    RaycastHit _hitInfo;
    int _idx;

    [Header("scripts")]
    private InventorySystem _inventorySystem;
    Install_Item _installItem;

    private Vector3 tmpVector = new Vector3(0, 0.01f, 0);
    private void Start()
    {
        // 게임 데이터 저장 델리게이트에 함수 추가
        SaveManager.Instance.GameDataSave += () => SaveManager.Instance.F_SaveFurniture(_installTransform);

        // Preview 오브젝트 배열
        _objectList_Preview = new List<GameObject>();

        // Preview 오브젝트 세팅
        F_CreatePreviewObject();

        // 인벤토리 시스템 스크립트 불러오기
        _inventorySystem = ItemManager.Instance.inventorySystem;

        // 설치물 불러오기
        SaveManager.Instance.F_LoadFurniture(_installTransform);
    }

    private void Update()
    {
        F_OnInstallMode();
    }

    /// <summary> 미리보기 오브젝트 생성 </summary>
    public void F_CreatePreviewObject()
    {
        for (int i = 0; i < _previewObjects.Length; i++)
        {
            _objectList_Preview.Add(Instantiate(_previewObjects[i], _previewTransform.position, Quaternion.identity));
            _objectList_Preview[i].transform.SetParent(_previewTransform);
            _objectList_Preview[i].SetActive(false);
        }
    }


    // 1. 플레이어 상태가 바뀌면 초기화 
    public void F_InitInstall()
    {
        // 선택된 설치물이 없을때 Return;
        if(_selectObject_Preview == null)
            return;

        for (int i = 0; i <= _selectObject_Preview.transform.childCount; i++)
        {
            //녹색 재질로 변경
            _installItem.F_ChgMaterial(); 
        }
        _selectObject_Preview.transform.rotation = Quaternion.identity; //회전값 초기화
        _selectObject_Preview.SetActive(false); // 선택했던 설치물 비활성화    
        _selectObject_Preview = null;           // 선택된 설치물 초기화
        _installItem = null;                    // 선택된 설치물의 Install_item컴포넌트 초기화
    }

    // 2. 설치물 선택시 초기화 작업 
    public void F_GetItemInfo(int v_itemCode)
    {
        _idx = v_itemCode;                                                  // 현재 선택한 설치물의 Index
        _selectObject_Preview = _objectList_Preview[_idx];                  // 현재 선택한 설치물의 Preview
        _selectObject_Preview.SetActive(true);                              // 선택한 설치물을 활성화
        _installItem = _selectObject_Preview.GetComponent<Install_Item>();  // 현재 선택한 설치물의 Install_Item 컴포넌트 초기화
    }

    public void F_OnInstallMode() //설치 기능 활성화
    {

        // 선택된 설치물이 없을때 Return;
        if (_selectObject_Preview == null)
            return;

        // 플레이어의 상태가 INSTALL이 아닐때 Return;
        if (PlayerManager.Instance.playerState != PlayerState.INSTALL)
            return;

        //쉬프트를 누르면 스냅 회전
        UIManager.Instance.F_PlayerMessagePopupTEXT("Press Shift snap rotation");

        //카메라 중심으로 레이를 쏴 미리보기 오브젝트를 충돌 지점에 따라가게 함
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out _hitInfo, 8, _installItem._installLayer))
        {
           _hitPos = _hitInfo.point;
            _selectObject_Preview.transform.position = _hitPos + tmpVector;

            // Preview Rotate
            F_RotateObject();

            // 설치 시도
            if (Input.GetMouseButtonDown(0))
                F_PlaceObject();
        }
    }

    //오브젝트 설치
    public void F_PlaceObject()
    {
        // 설치 가능하면 설치.
        if (_installItem.checkInstall)
        {
            // 설치 사운드 재생
            SoundManager.Instance.F_PlaySFX(SFXClip.INSTALL);

            // Preview 오브젝트 비활성화
            _selectObject_Preview.gameObject.SetActive(false);

            // 오브젝트 설치
            GameObject obj = Instantiate(_installObjects[_idx], _hitPos, _selectObject_Preview.transform.rotation, _installTransform);

            // 오브젝트 이름 초기화 ( 상호작용 텍스트 )
            obj.name = _installObjects[_idx].name;                          

            // 아이템 삭제
            int slotIndex = _inventorySystem.selectQuickSlotNumber;
            _inventorySystem.inventory[slotIndex] = null;                   

            // 인벤토리 업데이트 / 상화 전환 / 퀵슬롯 포커스 해제
            ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();     
            PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);     
            UIManager.Instance.F_QuickSlotFocus(-1);                        
        }

        // 설치 불가능하면 메세지 출력
        else
        {
            UIManager.Instance.F_PlayerMessagePopupTEXT("Can't install here");
        }
    }

    public void F_RotateObject() //오브젝트 회전
    {
        float _rotationSpeed = 300;
        if (_selectObject_Preview.activeSelf)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(KeyCode.R))
                    _selectObject_Preview.transform.Rotate(0, 45f, 0);
                else if (Input.GetKeyDown(KeyCode.Q))
                    _selectObject_Preview.transform.Rotate(0, -45f, 0);
            }
            else if (Input.GetKey(KeyCode.R))
                _selectObject_Preview.transform.Rotate(Vector3.up * _rotationSpeed * Time.deltaTime);
            else if (Input.GetKey(KeyCode.Q))
                _selectObject_Preview.transform.Rotate(Vector3.down * _rotationSpeed * Time.deltaTime);
        }
    }


    public void F_LoadFurnitureInstall(int v_idx, Vector3 v_pos, Vector3 v_rotate, string v_data)
    {
        Furniture furniture = Instantiate(_installObjects[v_idx], _installTransform).GetComponent<Furniture>();
        furniture.gameObject.name = _installObjects[v_idx].name;    // 이름 초기화 ( 상호작용 텍스트를 위한 )
        furniture.transform.position = v_pos;                       // 위치
        furniture.transform.rotation = Quaternion.Euler(v_rotate);  // 회전

        furniture.F_SetData(v_data);                                // 데이터
    }
}