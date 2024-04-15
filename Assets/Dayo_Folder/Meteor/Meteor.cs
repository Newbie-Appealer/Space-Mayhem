using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    [Header("=== ABOUT METEOR ===")]
    [SerializeField, Range(300f, 500f)] private float _meteor_Distance = 250f;
    private Rigidbody _rb;
    private Vector3 _meteor_StartPosition;
    public Vector3 MeteorStart => _meteor_StartPosition;

    private float _targetX; 
    private float _targetY;
    private float _targetZ;
    private int _itemCode = 0;
    public int ItemCode => _itemCode;

    //�÷��̾� �ֺ� ���� ��ü
    private float _player_Sphere_Radius;
    public void F_SettingMeteor()
    {
        _rb = GetComponent<Rigidbody>();
        _player_Sphere_Radius = MeteorManager.Instance.player_SphereCollider.radius;
        gameObject.name = "Meteor";
    }

    public void F_MoveMeteor()
    {
        // �÷��̾� �ֺ� ���� �������� �� ���� ���� ������ ��ǥ �̱�
        float _targetX = Random.Range(-_player_Sphere_Radius, _player_Sphere_Radius);
        float _targetY = Random.Range(-_player_Sphere_Radius, _player_Sphere_Radius);
        float _targetZ = Random.Range(-_player_Sphere_Radius, _player_Sphere_Radius);
        Vector3 _targetDirection = (new Vector3(_targetX, _targetY, _targetZ) - transform.position).normalized;
        _rb.velocity = _targetDirection * MeteorManager.Instance.meteorSpeed;
        StartCoroutine(C_MeteorDistanceCheck(gameObject));
    }

    //���׿��� �÷��̾� �ֺ� �� ���� �Ÿ� ����
    private IEnumerator C_MeteorDistanceCheck(GameObject v_Meteor) 
    {
        Transform _PlayerSphere;
        while(gameObject.activeSelf)
        {
            _PlayerSphere = MeteorManager.Instance.player_SphereCollider.transform;
            if (Vector3.Distance(_PlayerSphere.position, v_Meteor.transform.position) > _meteor_Distance)
            {
                MeteorManager.Instance.F_ReturnMeteor(this);
            }
            yield return new WaitForSeconds(3f);
        }
    }


    public void F_SettingItemCode()
    {
        float _randomFloat = Mathf.Floor(Random.value * 100);
        Debug.Log(_randomFloat);
        if (_randomFloat <= 40)
            _itemCode = 3;
        else if (_randomFloat > 40 && _randomFloat <= 80)
            _itemCode = 4;
        else if (_randomFloat > 80 && _randomFloat <= 90)
            _itemCode = 5;
        else if (_randomFloat > 90 && _randomFloat <= 98)
            _itemCode = 6;
        else
            _itemCode = 7;
    }
    public void F_GetMeteor(int v_itemCode)
    {
        ItemManager.Instance.inventorySystem.F_GetItem(v_itemCode);
        ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();
        MeteorManager.Instance.F_ReturnMeteor(this);
    }

    // �浹 �Ͼ�� ������ �۾����ֱ�
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("��� �浹");
        MeteorManager.Instance.F_ReturnMeteor(this);
    }

}
