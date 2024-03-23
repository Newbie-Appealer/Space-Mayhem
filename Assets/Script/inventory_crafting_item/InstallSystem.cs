using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

public class InstallSystem : MonoBehaviour
{
    [Header("player object")]
    [SerializeField] Camera _playerCamera;
    Transform _player;

    [Header("install object")]
    [SerializeField] GameObject[] _installPrefabs;
    [SerializeField] Material _greenMaterial;
    [SerializeField] Material _redMaterial;
    Material _defaultMaterial;
    MeshRenderer[] _changeMaterials;
    GameObject _pendingObject;
    Collider[] _pendingColliders;

    [Header("floor")]
    [SerializeField] LayerMask _Layer;
    Vector3 _hitPos;
    RaycastHit _hitInfo;

    private void Start()
    {
        _player = PlayerManager.Instance.playerTransform;
    }

    private void Update()
    {
        F_CheckInstallPosition();
    }
    Collider _hitCollider;
    public void F_CheckInstallPosition() //��ġ ��ġ Ȯ��
    {
        Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out _hitInfo, 5, _Layer))
        {
            _hitPos = _hitInfo.point; //ray�� �ε��� ����\
            _hitCollider = _hitInfo.collider;
        }
        F_OnInstallMode();
    }
    public void F_PlaceObject() //������Ʈ ��ġ
    {
        _pendingObject.layer = 11;
        foreach (Collider collider in _pendingColliders)
        {
            Physics.IgnoreLayerCollision(6, 12, false);
            //�÷��̾�� ��ġ �������� �浹 Ȱ��ȭ
        }
        foreach (MeshRenderer meshRenderer in _changeMaterials)
        {
            meshRenderer.material = _defaultMaterial;
            //������Ʈ material �����ϰ�
        }
        _pendingObject = null;
    }
    public void F_SelectObject() //������Ʈ ����
    {
        _pendingObject = Instantiate(_installPrefabs[0], _hitPos, Quaternion.identity);
        _pendingColliders = _pendingObject.GetComponentsInChildren<Collider>();
        _changeMaterials = _pendingObject.GetComponentsInChildren<MeshRenderer>();
        _defaultMaterial = _changeMaterials[0].material;

        foreach (Collider collider in _pendingColliders)
        {
            Physics.IgnoreLayerCollision(6, 12, true);
            //�÷��̾�� ��ġ �������� �浹 ��Ȱ��ȭ
        }
        foreach (MeshRenderer meshRenderer in _changeMaterials)
        {
            meshRenderer.material = _greenMaterial;
            //������Ʈ material �������ϰ�
        }
    }
    public void F_OnInstallMode() //��ġ ��� Ȱ��ȭ
    {
        if (_pendingObject != null)
        {
            _pendingObject.transform.position = _hitPos; 
            //������Ʈ�� �����Ǹ� ���콺 ��ġ�� �ٴ� ���̾�� �ε����� ������ �ǽð����� ����

            if (Input.GetMouseButtonDown(1)) //������ ��ġ(��ġ ����) ����
            {
                F_PlaceObject();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0)) //������ ���� ����
            {
                F_SelectObject(); 
                //�ش� �Լ��� Ȱ��ȭ�Ǹ� _pendingObject�� ������Ʈ ����
            }
        }
    }
}
