using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class InstallSystem : MonoBehaviour
{
    [Header("player object")]
    [SerializeField] Camera _playerCamera;
    Transform _player;
    Collider _playerCollider;

    [Header("install object")]
    [SerializeField] GameObject[] _installPrefabs;

    [SerializeField] LayerMask _floorLayer;
    GameObject _pendingObject;

    Vector3 _hitPos;
    RaycastHit _hitInfo;

    Collider[] _collider;

    private void Start()
    {
        _player = PlayerManager.Instance.playerTransform;
        _playerCollider = _player.GetComponent<Collider>();
    }

    private void Update()
    {
        F_CheckInstallPosition();
        F_OnIsntallMode();
    }

    public void F_CheckInstallPosition()
    {
        Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out _hitInfo, 5, _floorLayer))
        {
            _hitPos = _hitInfo.point;
        }
    }
    public void F_PlaceObject()
    {
        Debug.Log("Place");
        _pendingObject = null;
        foreach (Collider collider in _collider)
        {
            Physics.IgnoreCollision(_playerCollider, collider, false);
        }
    }

    public void F_SelectObject()
    {
        Debug.Log("Select");
        _pendingObject = Instantiate(_installPrefabs[0], _hitPos, transform.rotation);
        _collider = _pendingObject.GetComponentsInChildren<Collider>();
        foreach (Collider collider in _collider)
        {
            Physics.IgnoreCollision(_playerCollider, collider, true);
        }
    }
    public void F_OnIsntallMode()
    {
        if (_pendingObject != null)
        {
            _pendingObject.transform.position = _hitPos;

            if (Input.GetMouseButtonDown(1))
            {
                F_PlaceObject();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                F_SelectObject();
            }
        }
    }
}
