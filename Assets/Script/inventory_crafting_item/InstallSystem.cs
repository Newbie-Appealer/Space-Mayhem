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

    public void F_CreatePreviewObject() //�̸����� ������Ʈ �����س���
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

    public void F_InitInstall() //�÷��̾� Ÿ���� �ٲ�� �ʱ�ȭ
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

    public void F_OnInstallMode() //��ġ ��� Ȱ��ȭ
    {
        if (_previewChild == null)
            return;

        if (PlayerManager.Instance.playerState == PlayerState.INSTALL)
        {
            //ī�޶� �߽����� ���̸� �� �̸����� ������Ʈ�� �浹 ������ ���󰡰� ��
            Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out _hitInfo, 8, _PreviewObjLayer))
            {
                _hitPos = _hitInfo.point;
                _previewChild.transform.position = _hitPos;
            }

            _previewChild.SetActive(true);

            F_RotateObject();

            _installItem = _previewChild.GetComponent<Install_Item>();

            if (Input.GetMouseButtonDown(0) && _installItem._checkInstall) //������ ��ġ(��ġ ����) ����
                F_PlaceObject(); //������ ��ġ ����
        }
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

    public void F_RotateObject() //������Ʈ ȸ��
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