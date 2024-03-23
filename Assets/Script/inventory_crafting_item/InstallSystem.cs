using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

public class InstallSystem : MonoBehaviour
{
    [Header("player object")]
    [SerializeField] Camera _playerCamera;
    //Transform _player;

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
        //_player = PlayerManager.Instance.playerTransform;
    }

    private void Update()
    {
        F_CheckInstallPosition();
    }
    public void F_CheckInstallPosition() //설치 위치 확인
    {
        Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out _hitInfo, 5, _Layer))
        {
            _hitPos = _hitInfo.point; //ray가 부딪힌 지점
        }
        F_OnInstallMode();
    }
    public void F_PlaceObject() //오브젝트 설치
    {
        _pendingObject.layer = 11; //설치 후 레이어 변경
        foreach (Collider collider in _pendingColliders)
        {
            Physics.IgnoreLayerCollision(6, 12, false);
            //플레이어와 설치 아이템의 충돌 활성화
        }
        foreach (MeshRenderer meshRenderer in _changeMaterials)
        {
            meshRenderer.material = _defaultMaterial;
            //오브젝트 material 선명하게
        }
        _pendingObject = null;
    }
    public void F_SelectObject() //오브젝트 선택
    {
        _pendingObject = Instantiate(_installPrefabs[0], _hitPos, Quaternion.identity);
        _pendingColliders = _pendingObject.GetComponentsInChildren<Collider>();
        _changeMaterials = _pendingObject.GetComponentsInChildren<MeshRenderer>();
        _defaultMaterial = _changeMaterials[0].material;

        foreach (Collider collider in _pendingColliders)
        {
            Physics.IgnoreLayerCollision(6, 12, true);
            //플레이어와 설치 아이템의 충돌 비활성화
        }
        foreach (MeshRenderer meshRenderer in _changeMaterials)
        {
            meshRenderer.material = _greenMaterial;
            //오브젝트 material 반투명하게
        }
    }
    public void F_OnInstallMode() //설치 기능 활성화
    {
        if (_pendingObject != null)
        {
            Vector3 range = _pendingColliders[0].bounds.size;
            Vector3 center = _pendingColliders[0].bounds.center;
            Debug.DrawLine(center, range);
            _pendingObject.transform.position = _hitPos;
            //오브젝트가 생성되면 레이가 부딪히는 지점을 실시간으로 따라감

            if (Input.GetMouseButtonDown(1)) //아이템 설치(위치 고정) 조건
            {
                F_PlaceObject(); //아이템 위치 고정
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0)) //아이템 생성 조건
            {
                F_SelectObject(); 
                //해당 함수가 활성화되면 _pendingObject에 오브젝트 생성
            }
        }
    }
}
