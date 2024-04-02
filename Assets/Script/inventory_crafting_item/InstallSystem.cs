using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

public class InstallSystem : MonoBehaviour
{
    [Header("object Parent Transform")]
    [SerializeField] Transform _parentTransform;

    [Header("player object")]
    [SerializeField] Camera _playerCamera;

    [Header("install object")]
    [SerializeField] GameObject[] _installPrefabs;

    [Header("preview object")]
    [SerializeField] GameObject[] _previewPrefabs;
    [SerializeField] GameObject _previewParent;
    List<GameObject> _pendingObject;
    GameObject _previewChild;

    [Header("material")]
    [SerializeField] Material _greenMaterial;

    [Header("raycasting")]
    [SerializeField] LayerMask _PreviewObjLayer;
    Vector3 _hitPos;
    RaycastHit _hitInfo;
    int _idx;
    private InventorySystem _inventorySystem;
    Install_Item _installItem;

    private void Start()
    {
        _pendingObject = new List<GameObject>();
        F_CreatePreviewObject();
        _inventorySystem = ItemManager.Instance.inventorySystem;
    }

    private void Update()
    {
        F_OnInstallMode();
    }

    public void F_CreatePreviewObject() //미리보기 오브젝트 생성해놓기
    {
        for (int i = 0; i < _previewPrefabs.Length; i++)
        {
            _pendingObject.Add(Instantiate(_previewPrefabs[i], _previewParent.transform.position, Quaternion.identity));
            _pendingObject[i].transform.SetParent(_previewParent.transform);
            _pendingObject[i].SetActive(false);
        }
    }

    public void F_GetItemInfo(int v_itemCode)
    {
        _idx = v_itemCode - 24;
        _previewChild = _pendingObject[_idx];
    }

    public void F_InitInstall() //플레이어 타입이 바뀌면 초기화
    {
        if(_previewChild == null)
            return;

        for (int i = 0; i < _previewChild.transform.childCount+1; i++)
        {
            _installItem.F_ChgMaterial();
        }
        _previewChild.transform.rotation = Quaternion.identity;
        _previewChild.SetActive(false);
        _previewChild = null;
    }

    public void F_OnInstallMode() //설치 기능 활성화
    {
        if (_previewChild == null)
            return;

        if (PlayerManager.Instance.playerState == PlayerState.INSTALL)
        {
            //카메라 중심으로 레이를 쏴 미리보기 오브젝트를 충돌 지점에 따라가게 함
            Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out _hitInfo, 8, _PreviewObjLayer))
            {
                _hitPos = _hitInfo.point;
                _previewChild.transform.position = _hitPos;
            }

            _previewChild.SetActive(true);

            F_RotateObject();

            _installItem = _previewChild.GetComponent<Install_Item>();

            if (Input.GetMouseButtonDown(0) && _installItem._checkInstall) //아이템 설치(위치 고정) 조건
                F_PlaceObject(); //아이템 위치 고정
        }
    }

    public void F_PlaceObject() //오브젝트 설치
    {
        _previewChild.gameObject.SetActive(false);
        Instantiate(_installPrefabs[_idx], _hitPos, _previewChild.transform.rotation, _parentTransform);

        int slotIndex = _inventorySystem.selectQuickSlotNumber;
        _inventorySystem.inventory[slotIndex] = null;                   // 아이템 삭제

        ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();     // 인벤토리 업데이트
        PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);     // 상태변환
        UIManager.Instance.F_QuickSlotFocus(-1);                        // 포커스 UI 해제
    }

    public void F_RotateObject() //오브젝트 회전
    {
        if (_previewChild.activeSelf)
        {
            if (Input.GetKey(KeyCode.R))
                _previewChild.transform.Rotate(0, 0.5f, 0);

            else if (Input.GetKey(KeyCode.Q))
                _previewChild.transform.Rotate(0, -0.5f, 0);
        }
    }
}