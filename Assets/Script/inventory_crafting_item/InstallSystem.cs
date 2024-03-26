using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

public class InstallSystem : MonoBehaviour
{
    [Header("player object")]
    [SerializeField] Camera _playerCamera;

    [Header("install object")]
    [SerializeField] GameObject[] _installPrefabs;

    [Header("preview object")]
    [SerializeField] GameObject[] _previewPrefabs;
    [SerializeField] GameObject _previewParent;
    GameObject _pendingObject;
    GameObject _previewChild;

    [Header("material")]
    [SerializeField] Material _greenMaterial;
    [SerializeField] Material _redMaterial;

    [Header("raycasting")]
    [SerializeField] LayerMask _PreviewObjLayer;
    [SerializeField] LayerMask _installCondition;
    Vector3 _hitPos;
    RaycastHit _hitInfo;
    int _idx;

    private void Start()
    {
        for (int i = 0; i < _previewPrefabs.Length; i++)
        {
            _pendingObject = Instantiate(_previewPrefabs[i], _previewParent.transform.position, Quaternion.identity);
            _pendingObject.SetActive(false);
            _pendingObject.transform.SetParent(_previewParent.transform);
            Physics.IgnoreLayerCollision(6, 12, true);
        }
    }
    private void Update()
    {
        F_CheckInstallPosition();
    }
    public void F_CheckInstallPosition() //설치 위치 확인
    {
        if (PlayerManager.Instance.playerState == PlayerState.INSTALL)
        {
            Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out _hitInfo, 8, _PreviewObjLayer))
            {
                _hitPos = _hitInfo.point; //ray가 부딪힌 지점
                _previewChild.transform.position = _hitPos;
                //오브젝트가 생성되면 레이가 부딪히는 지점을 실시간으로 따라감
            }
        }
    }

    private void F_InitInstall()
    {
        _previewParent.GetComponentInChildren<Transform>().gameObject.SetActive(false);
    }

    public void F_OnInstallMode() //설치 기능 활성화
    {
        _previewChild.SetActive(true);
        Bounds _bounds = _previewChild.GetComponent<Collider>().bounds;
        Vector3 _center = _bounds.center;
        
        if (Input.GetMouseButtonDown(0)) //아이템 설치(위치 고정) 조건
        {
            F_PlaceObject(); //아이템 위치 고정
        }
        
        F_RotateObject();
    }

    public void F_PlaceObject() //오브젝트 설치
    {
        _previewChild.gameObject.SetActive(false);
        Instantiate(_installPrefabs[_idx], _hitPos, Quaternion.identity);
        ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();
    }

    public void F_RotateObject()
    {
        if (Input.GetKey(KeyCode.R))
        {
            _previewChild.transform.Rotate(0, 0.5f, 0);
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            _previewChild.transform.Rotate(0, -0.5f, 0);
        }
    }

    public void F_GetItemInfo(int v_itemCode)
    {
        _idx = v_itemCode - 24;
        _previewChild = _previewParent.transform.GetChild(_idx).gameObject;
    }
    //int _slotIndex;

    //public void F_GetSlotIndex(int v_slotIndex)
    //{
    //    _slotIndex = v_slotIndex;
    //    Debug.Log(_slotIndex);
    //}
}
