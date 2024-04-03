using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    private Rigidbody _rb;

    [SerializeField] private float _meteor_MoveSpeed; //운석 속도
    private Vector3 _meteor_StartPosition;
    
    //타겟 좌표
    private float _targetX; 
    private float _targetY;
    private float _targetZ;

    //플레이어 주변 범위 구체
    private float _player_Sphere_Radius;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _player_Sphere_Radius = MeteorManager.Instance.player_SphereCollider.radius;
        _meteor_StartPosition = transform.position;
        F_InitializeMeteor(_meteor_StartPosition);
        F_MoveMeteor(); 
    }

    private void OnEnable()
    {
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
        _rb.velocity = _targetDirection * _meteor_MoveSpeed;
    }

    //메테오 초기화
    private void F_InitializeMeteor(Vector3 v_Trans)
    {
        transform.position = v_Trans;
    }

    //메테오와 플레이어 주변 원 사이 거리 측정
    private IEnumerator C_MeteorDistanceCheck(GameObject v_Meteor) 
    {
        float _distance = MeteorManager.Instance.F_GetMeteorDistance();
        Transform _PlayerSphere;
        while(gameObject.activeSelf)
        {
            _PlayerSphere = MeteorManager.Instance.player_SphereCollider.transform;
            if (Vector3.Distance(_PlayerSphere.position, v_Meteor.transform.position) > _distance)
            {
                F_MeteorDestoryed();
            }
            yield return new WaitForSeconds(3f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        F_MeteorDestoryed();
    }

    private void F_MeteorDestoryed()
    {
        StopAllCoroutines();
        F_InitializeMeteor(_meteor_StartPosition);
        gameObject.SetActive(false);
        MeteorManager.Instance.F_MeteorPoolingAdd(this.gameObject);
    }
}
