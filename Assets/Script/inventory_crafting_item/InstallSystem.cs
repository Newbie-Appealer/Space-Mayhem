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
    [SerializeField] GameObject[] _previewPrefabs;
    [SerializeField] GameObject _previewParent;
    [SerializeField] Material _greenMaterial;
    [SerializeField] Material _redMaterial;
    GameObject _pendingObject;

    [Header("floor")]
    [SerializeField] LayerMask _Layer;
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
        _previewParent.transform.GetChild(0);
        F_CheckInstallPosition();
    }
    public void F_CheckInstallPosition() //��ġ ��ġ Ȯ��
    {
        if (PlayerManager.Instance.playerState == PlayerState.INSTALL)
        {
            Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out _hitInfo, 5, _Layer))
            {
                _hitPos = _hitInfo.point; //ray�� �ε��� ����
                _previewParent.transform.GetChild(_idx).position = _hitPos;
                //������Ʈ�� �����Ǹ� ���̰� �ε����� ������ �ǽð����� ����
            }
            if (Input.GetMouseButtonDown(0)) //������ ��ġ(��ġ ����) ����
            {
                F_PlaceObject(); //������ ��ġ ����
            }
        }
        else
        {
            F_InitInstall();
        }
    }

    private void F_InitInstall()
    {
        _pendingObject.GetComponentInChildren<Transform>().gameObject.SetActive(false);
    }

    public void F_OnInstallMode() //��ġ ��� Ȱ��ȭ
    {
        _previewParent.transform.GetChild(_idx).gameObject.SetActive(true);
        F_RotateObject();
    }

    public void F_PlaceObject() //������Ʈ ��ġ
    {
        _previewParent.transform.GetChild(_idx).gameObject.SetActive(false);
        Instantiate(_installPrefabs[_idx], _hitPos, Quaternion.identity);
        ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();
    }

    public void F_RotateObject()
    {
        if (Input.GetKey(KeyCode.R))
        {
            _pendingObject.transform.Rotate(0, 0.5f, 0);
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            _pendingObject.transform.Rotate(0, -0.5f, 0);
        }
    }

    public void F_GetItemInfo(int v_itemCode)
    {
        _idx = v_itemCode - 24;
    }
}
