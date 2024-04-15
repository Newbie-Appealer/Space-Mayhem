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

        // �ҷ�����
        SaveManager.Instance.F_LoadFurniture(_installTransform);
    }

    private void Update()
    {
        F_OnInstallMode();

        // ����
        if(Input.GetKeyDown(KeyCode.P))
        {
            SaveManager.Instance.F_SaveFurniture(_installTransform);
        }
    }

    public void F_CreatePreviewObject() //�̸����� ������Ʈ �����س���
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
        _pendingChild = _pendingObject[_idx]; //���� ������ ������Ʈ
    }

    public void F_InitInstall() //�÷��̾� ���°� �ٲ�� �ʱ�ȭ
    {
        if(_pendingChild == null)
            return;

        for (int i = 0; i <= _pendingChild.transform.childCount; i++)
        {
            _installItem.F_ChgMaterial(); //��� ������ ����
        }
        _pendingChild.transform.rotation = Quaternion.identity; //ȸ���� �ʱ�ȭ
        _pendingChild.SetActive(false);
        _pendingChild = null;
    }

    public void F_OnInstallMode() //��ġ ��� Ȱ��ȭ
    {
        if (_pendingChild == null)
            return;

        if (PlayerManager.Instance.playerState == PlayerState.INSTALL)
        {
            //ī�޶� �߽����� ���̸� �� �̸����� ������Ʈ�� �浹 ������ ���󰡰� ��
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out _hitInfo, 8, _PreviewObjLayer))
            {
                _hitPos = _hitInfo.point;
                _pendingChild.transform.position = _hitPos;
            }

            _pendingChild.SetActive(true);

            F_RotateObject();

            _installItem = _pendingChild.GetComponent<Install_Item>();

            if (Input.GetMouseButtonDown(0) && _installItem._checkInstall) //������ ��ġ(��ġ ����) ����
                F_PlaceObject(); //������ ��ġ ����
        }
    }

    public void F_PlaceObject() //������Ʈ ��ġ
    {
        _pendingChild.gameObject.SetActive(false);
        Instantiate(_installObjects[_idx], _hitPos, _pendingChild.transform.rotation, _installTransform);

        int slotIndex = _inventorySystem.selectQuickSlotNumber;
        _inventorySystem.inventory[slotIndex] = null;                   // ������ ����

        ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();     // �κ��丮 ������Ʈ
        PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);     // ���º�ȯ
        UIManager.Instance.F_QuickSlotFocus(-1);                        // ��Ŀ�� UI ����
    }

    public void F_RotateObject() //������Ʈ ȸ��
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
        furniture.transform.position = v_pos;                       // ��ġ
        furniture.transform.rotation = Quaternion.Euler(v_rotate);  // ȸ��

        furniture.F_SetData(v_data);                                // ������
    }
}