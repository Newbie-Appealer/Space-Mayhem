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
    GameObject _pendingObject;
    GameObject _previewChild;

    [Header("material")]
    [SerializeField] Material _greenMaterial;

    [Header("raycasting")]
    [SerializeField] LayerMask _PreviewObjLayer;
    Vector3 _hitPos;
    RaycastHit _hitInfo;
    int _idx;
    private InventorySystem _inventorySystem;
    bool _checkInstall;

    private void Start()
    {
        F_CreatePreviewObject();
        _inventorySystem = ItemManager.Instance.inventorySystem;
    }
    private void Update()
    {
        F_CheckInstallPosition();
        F_RotateObject();
    }
    public void F_CreatePreviewObject() //�̸����� ������Ʈ �����س���
    {
        for (int i = 0; i < _previewPrefabs.Length; i++)
        {
            _pendingObject = Instantiate(_previewPrefabs[i], _previewParent.transform.position, Quaternion.identity);
            _pendingObject.SetActive(false);
            _pendingObject.transform.SetParent(_previewParent.transform);
            Physics.IgnoreLayerCollision(6, 12, true); //�÷��̾���� �浹 ����
        }
    }

    public void F_CheckInstallPosition() //��ġ ��ġ Ȯ��
    {
        if (PlayerManager.Instance.playerState == PlayerState.INSTALL)
        {
            //ī�޶� �߽����� ���̸� �� �̸����� ������Ʈ�� �浹 ������ ���󰡰� ��
            Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out _hitInfo, 8, _PreviewObjLayer))
            {
                _hitPos = _hitInfo.point;
                _previewChild.transform.position = _hitPos;
            }
        }
    }

    public void F_InitInstall()
    {
        _previewChild = null;
        for (int i = 0; i < _previewPrefabs.Length; i++)
        {
            _previewParent.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void F_OnInstallMode() //��ġ ��� Ȱ��ȭ
    {
        if (_previewChild == null)
            return;

        _previewChild.SetActive(true);

        _checkInstall = _previewChild.GetComponent<Install_Item>()._checkInstall;

        if (Input.GetMouseButtonDown(0) && _checkInstall) //������ ��ġ(��ġ ����) ����
            F_PlaceObject(); //������ ��ġ ����
    }

    public void F_PlaceObject() //������Ʈ ��ġ
    {
        _previewChild.gameObject.SetActive(false);
        Instantiate(_installPrefabs[_idx], _hitPos, _previewChild.transform.rotation, _parentTransform);

        int slotIndex = _inventorySystem.selectQuickSlotNumber;
        _inventorySystem.inventory[slotIndex] = null;                   // ������ ����

        ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();     // �κ��丮 ������Ʈ
        PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);     // ���º�ȯ
        UIManager.Instance.F_QuickSlotFocus(-1);                        // ��Ŀ�� UI ����
    }

    public void F_RotateObject()
    {
        if (_previewChild == null)
            return;

        if (_previewChild.activeSelf)
        {
            if (Input.GetKey(KeyCode.R))
                _previewChild.transform.Rotate(0, 0.5f, 0);

            else if (Input.GetKey(KeyCode.Q))
                _previewChild.transform.Rotate(0, -0.5f, 0);
        }
        else
            _previewChild.transform.Rotate(0, 0, 0);
    }

    public void F_GetItemCode(int v_itemCode)
    {
        _idx = v_itemCode - 24;
        _previewChild = _previewParent.transform.GetChild(_idx).gameObject;
    }
}