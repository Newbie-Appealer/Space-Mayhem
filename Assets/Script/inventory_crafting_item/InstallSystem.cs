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
    List<GameObject> _pendingObject;
    GameObject _pendingChild;

    [Header("raycasting")]
    [SerializeField] LayerMask _PreviewObjLayer;
    Vector3 _hitPos;
    RaycastHit _hitInfo;
    int _idx;

    [Header("scripts")]
    private InventorySystem _inventorySystem;
    Install_Item _installItem;

    private void Start()
    {
        _pendingObject = new List<GameObject>();
        F_CreatePreviewObject();
        _inventorySystem = ItemManager.Instance.inventorySystem;

        // 불러오기
        SaveManager.Instance.F_LoadFurniture(_installTransform);
    }

    private void Update()
    {
        F_OnInstallMode();

        // 저장
        if(Input.GetKeyDown(KeyCode.P))
        {
            SaveManager.Instance.F_SaveFurniture(_installTransform);
        }
    }

    public void F_CreatePreviewObject() //미리보기 오브젝트 생성해놓기
    {
        for (int i = 0; i < _previewObjects.Length; i++)
        {
            _pendingObject.Add(Instantiate(_previewObjects[i], _previewTransform.position, Quaternion.identity));
            _pendingObject[i].transform.SetParent(_previewTransform);
            _pendingObject[i].SetActive(false);
        }
    }

    public void F_GetItemInfo(int v_itemCode)
    {
        _idx = v_itemCode - 24;
        _pendingChild = _pendingObject[_idx]; //현재 선택한 오브젝트
    }

    public void F_InitInstall() //플레이어 상태가 바뀌면 초기화
    {
        if(_pendingChild == null)
            return;

        for (int i = 0; i <= _pendingChild.transform.childCount; i++)
        {
            _installItem.F_ChgMaterial(); //녹색 재질로 변경
        }
        _pendingChild.transform.rotation = Quaternion.identity; //회전값 초기화
        _pendingChild.SetActive(false);
        _pendingChild = null;
    }

    public void F_OnInstallMode() //설치 기능 활성화
    {
        if (_pendingChild == null)
            return;

        if (PlayerManager.Instance.playerState == PlayerState.INSTALL)
        {
            //카메라 중심으로 레이를 쏴 미리보기 오브젝트를 충돌 지점에 따라가게 함
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out _hitInfo, 8, _PreviewObjLayer))
            {
                _hitPos = _hitInfo.point;
                _pendingChild.transform.position = _hitPos;
            }

            _pendingChild.SetActive(true);

            F_RotateObject();

            _installItem = _pendingChild.GetComponent<Install_Item>();

            if (Input.GetMouseButtonDown(0) && _installItem._checkInstall) //아이템 설치(위치 고정) 조건
                F_PlaceObject(); //아이템 위치 고정
        }
    }

    public void F_PlaceObject() //오브젝트 설치
    {
        _pendingChild.gameObject.SetActive(false);
        Instantiate(_installObjects[_idx], _hitPos, _pendingChild.transform.rotation, _installTransform);

        int slotIndex = _inventorySystem.selectQuickSlotNumber;
        _inventorySystem.inventory[slotIndex] = null;                   // 아이템 삭제

        ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();     // 인벤토리 업데이트
        PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);     // 상태변환
        UIManager.Instance.F_QuickSlotFocus(-1);                        // 포커스 UI 해제
    }

    public void F_RotateObject() //오브젝트 회전
    {
        float _rotationSpeed = 300;
        if (_pendingChild.activeSelf)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(KeyCode.R))
                    _pendingChild.transform.Rotate(0, 45f, 0);
                else if (Input.GetKeyDown(KeyCode.Q))
                    _pendingChild.transform.Rotate(0, -45f, 0);
            }
            else if (Input.GetKey(KeyCode.R))
                _pendingChild.transform.Rotate(Vector3.up * _rotationSpeed * Time.deltaTime);
            else if (Input.GetKey(KeyCode.Q))
                _pendingChild.transform.Rotate(Vector3.down * _rotationSpeed * Time.deltaTime);
        }
    }


    public void F_LoadFurnitureInstall(int v_idx, Vector3 v_pos, Vector3 v_rotate, string v_data)
    {
        Furniture furniture = Instantiate(_installObjects[v_idx], _installTransform).GetComponent<Furniture>();
        furniture.transform.position = v_pos;                       // 위치
        furniture.transform.rotation = Quaternion.Euler(v_rotate);  // 회전

        furniture.F_SetData(v_data);                                // 데이터
    }
}