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
        // ���� ������ ���� ��������Ʈ�� �Լ� �߰�
        SaveManager.Instance.GameDataSave += () => SaveManager.Instance.F_SaveFurniture(_installTransform);

        // Preview ������Ʈ �迭
        _objectList_Preview = new List<GameObject>();

        // Preview ������Ʈ ����
        F_CreatePreviewObject();

        // �κ��丮 �ý��� ��ũ��Ʈ �ҷ�����
        _inventorySystem = ItemManager.Instance.inventorySystem;

        // ��ġ�� �ҷ�����
        SaveManager.Instance.F_LoadFurniture(_installTransform);
    }

    private void Update()
    {
        F_OnInstallMode();
    }

    /// <summary> �̸����� ������Ʈ ���� </summary>
    public void F_CreatePreviewObject()
    {
        for (int i = 0; i < _previewObjects.Length; i++)
        {
            _objectList_Preview.Add(Instantiate(_previewObjects[i], _previewTransform.position, Quaternion.identity));
            _objectList_Preview[i].transform.SetParent(_previewTransform);
            _objectList_Preview[i].SetActive(false);
        }
    }


    // 1. �÷��̾� ���°� �ٲ�� �ʱ�ȭ 
    public void F_InitInstall()
    {
        // ���õ� ��ġ���� ������ Return;
        if(_selectObject_Preview == null)
            return;

        for (int i = 0; i <= _selectObject_Preview.transform.childCount; i++)
        {
            //��� ������ ����
            _installItem.F_ChgMaterial(); 
        }
        _selectObject_Preview.transform.rotation = Quaternion.identity; //ȸ���� �ʱ�ȭ
        _selectObject_Preview.SetActive(false); // �����ߴ� ��ġ�� ��Ȱ��ȭ    
        _selectObject_Preview = null;           // ���õ� ��ġ�� �ʱ�ȭ
        _installItem = null;                    // ���õ� ��ġ���� Install_item������Ʈ �ʱ�ȭ
    }

    // 2. ��ġ�� ���ý� �ʱ�ȭ �۾� 
    public void F_GetItemInfo(int v_itemCode)
    {
        _idx = v_itemCode;                                                  // ���� ������ ��ġ���� Index
        _selectObject_Preview = _objectList_Preview[_idx];                  // ���� ������ ��ġ���� Preview
        _selectObject_Preview.SetActive(true);                              // ������ ��ġ���� Ȱ��ȭ
        _installItem = _selectObject_Preview.GetComponent<Install_Item>();  // ���� ������ ��ġ���� Install_Item ������Ʈ �ʱ�ȭ
    }

    public void F_OnInstallMode() //��ġ ��� Ȱ��ȭ
    {

        // ���õ� ��ġ���� ������ Return;
        if (_selectObject_Preview == null)
            return;

        // �÷��̾��� ���°� INSTALL�� �ƴҶ� Return;
        if (PlayerManager.Instance.playerState != PlayerState.INSTALL)
            return;

        //����Ʈ�� ������ ���� ȸ��
        UIManager.Instance.F_PlayerMessagePopupTEXT("Press Shift snap rotation");

        //ī�޶� �߽����� ���̸� �� �̸����� ������Ʈ�� �浹 ������ ���󰡰� ��
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out _hitInfo, 8, _installItem._installLayer))
        {
           _hitPos = _hitInfo.point;
            _selectObject_Preview.transform.position = _hitPos + tmpVector;

            // Preview Rotate
            F_RotateObject();

            // ��ġ �õ�
            if (Input.GetMouseButtonDown(0))
                F_PlaceObject();
        }
    }

    //������Ʈ ��ġ
    public void F_PlaceObject()
    {
        // ��ġ �����ϸ� ��ġ.
        if (_installItem.checkInstall)
        {
            // ��ġ ���� ���
            SoundManager.Instance.F_PlaySFX(SFXClip.INSTALL);

            // Preview ������Ʈ ��Ȱ��ȭ
            _selectObject_Preview.gameObject.SetActive(false);

            // ������Ʈ ��ġ
            GameObject obj = Instantiate(_installObjects[_idx], _hitPos, _selectObject_Preview.transform.rotation, _installTransform);

            // ������Ʈ �̸� �ʱ�ȭ ( ��ȣ�ۿ� �ؽ�Ʈ )
            obj.name = _installObjects[_idx].name;                          

            // ������ ����
            int slotIndex = _inventorySystem.selectQuickSlotNumber;
            _inventorySystem.inventory[slotIndex] = null;                   

            // �κ��丮 ������Ʈ / ��ȭ ��ȯ / ������ ��Ŀ�� ����
            ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();     
            PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);     
            UIManager.Instance.F_QuickSlotFocus(-1);                        
        }

        // ��ġ �Ұ����ϸ� �޼��� ���
        else
        {
            UIManager.Instance.F_PlayerMessagePopupTEXT("Can't install here");
        }
    }

    public void F_RotateObject() //������Ʈ ȸ��
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
        furniture.gameObject.name = _installObjects[v_idx].name;    // �̸� �ʱ�ȭ ( ��ȣ�ۿ� �ؽ�Ʈ�� ���� )
        furniture.transform.position = v_pos;                       // ��ġ
        furniture.transform.rotation = Quaternion.Euler(v_rotate);  // ȸ��

        furniture.F_SetData(v_data);                                // ������
    }
}