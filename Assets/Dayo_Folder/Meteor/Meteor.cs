using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
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

    //아이템 확률 및 코드
    private float[] _drop_Chance;
    private int _itemCode = 3;
    public int ItemCode => _itemCode;

    //플레이어 주변 범위 구체
    private float _player_Sphere_Radius;


    public void F_SettingMeteor()
    {
        _rb = GetComponent<Rigidbody>();
        _player_Sphere_Radius = MeteorManager.Instance.player_SphereCollider.radius;
        gameObject.name = "Meteor";
        _drop_Chance = new float[] { 40f, 40f, 10f, 8f, 2f };
    }

    public void F_MoveMeteor()
    {
        // 플레이어 주변 구를 기준으로 그 안의 범위 랜덤한 좌표 뽑기
        float _targetX = Random.Range(-_player_Sphere_Radius, _player_Sphere_Radius);
        float _targetY = Random.Range(-_player_Sphere_Radius, _player_Sphere_Radius);
        float _targetZ = Random.Range(-_player_Sphere_Radius, _player_Sphere_Radius);
        Vector3 _targetDirection = (new Vector3(_targetX, _targetY, _targetZ) - transform.position).normalized;
        _rb.velocity = _targetDirection * MeteorManager.Instance.meteorSpeed;
        StartCoroutine(C_MeteorDistanceCheck(gameObject));
    }

    //메테오와 플레이어 주변 원 사이 거리 측정
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

    public int F_SettingItemCode()
    {
        _itemCode = 3;
        float _randomChance = Mathf.Floor(Random.value * 100);
        Debug.Log("이번 숫자 : " + _randomChance);
        for (int l = 0; l < _drop_Chance.Length; l++)
        {
            if (_randomChance < _drop_Chance[l])
                return _itemCode;
            else
            {
                _randomChance -= _drop_Chance[l];
                _itemCode++;
            }
        }
        return _itemCode = 7;
    }
    public void F_GetMeteor(int v_itemCode)
    {
        ItemManager.Instance.inventorySystem.F_GetItem(v_itemCode);
        ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();
        MeteorManager.Instance.F_ReturnMeteor(this);
    }

    // 충돌 일어나는 곳에서 작업해주기
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("운석과 충돌");
        MeteorManager.Instance.F_ReturnMeteor(this);
    }

}
