using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    private Rigidbody _rb;

    [SerializeField] private float _Meteor_MoveSpeed; //운석 속도
    private Transform _Meteor_StartPosition;
    
    //타겟 좌표
    private float _targetX; 
    private float _targetY;
    private float _targetZ;

    //플레이어 주변 범위 구체
    private float _player_Sphere_Radius;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _player_Sphere_Radius = MeteorManager.Instance.Player_SphereCollider.radius;
        F_InitializeMeteor(transform.parent.transform);
        F_MoveMeteor(); 
        //재료 아이템과 마찬가지로 코루틴으로 거리 측정
        StartCoroutine(C_MeteorDistanceCheck(gameObject));
    }

    //메테오 이동
    private void F_MoveMeteor()
    {
        // 플레이어 주변 구를 기준으로 그 안의 범위 랜덤한 좌표 뽑기
        float _targetX = Random.Range(-_player_Sphere_Radius, _player_Sphere_Radius);
        float _targetY = Random.Range(-_player_Sphere_Radius, _player_Sphere_Radius);
        float _targetZ = Random.Range(-_player_Sphere_Radius, _player_Sphere_Radius);
        Vector3 _targetDirection = (new Vector3(_targetX, _targetY, _targetZ) - transform.position).normalized;
        _rb.velocity = _targetDirection * _Meteor_MoveSpeed;
    }

    //메테오 초기화
    private void F_InitializeMeteor(Transform v_Trans)
    {
        _Meteor_StartPosition = v_Trans;
    }

    //메테오와 플레이어 주변 원 사이 거리 측정
    private IEnumerator C_MeteorDistanceCheck(GameObject v_Meteor) 
    {
        float _distance = MeteorManager.Instance._Meteor_Distance;
        Transform _PlayerSphere;
        while(gameObject.activeSelf)
        {
            _PlayerSphere = MeteorManager.Instance.Player_SphereCollider.transform;
            if (Vector3.Distance(_PlayerSphere.position, v_Meteor.transform.position) > _distance)
            {
                gameObject.SetActive(false);
                F_InitializeMeteor(_Meteor_StartPosition);
                MeteorManager.Instance.F_MeteorPoolingAdd(v_Meteor);
            }
            yield return new WaitForSeconds(3f);
        }
    }
}
