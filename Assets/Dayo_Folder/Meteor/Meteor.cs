using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    private Rigidbody _rb;

    [SerializeField] private float _Meteor_MoveSpeed; //� �ӵ�
    private Vector3 _Meteor_StartPosition;
    
    //Ÿ�� ��ǥ
    private float _targetX; 
    private float _targetY;
    private float _targetZ;

    //�÷��̾� �ֺ� ���� ��ü
    private float _player_Sphere_Radius;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _player_Sphere_Radius = MeteorManager.Instance.Player_SphereCollider.radius;
        _Meteor_StartPosition = transform.position;
        F_InitializeMeteor(_Meteor_StartPosition);
        F_MoveMeteor(); 
    }

    private void OnEnable()
    {
        //��� �����۰� ���������� �ڷ�ƾ���� �Ÿ� ����
        StartCoroutine(C_MeteorDistanceCheck(gameObject));
    }

    //���׿� �̵�
    private void F_MoveMeteor()
    {
        // �÷��̾� �ֺ� ���� �������� �� ���� ���� ������ ��ǥ �̱�
        float _targetX = Random.Range(-_player_Sphere_Radius, _player_Sphere_Radius);
        float _targetY = Random.Range(-_player_Sphere_Radius, _player_Sphere_Radius);
        float _targetZ = Random.Range(-_player_Sphere_Radius, _player_Sphere_Radius);
        Vector3 _targetDirection = (new Vector3(_targetX, _targetY, _targetZ) - transform.position).normalized;
        _rb.velocity = _targetDirection * _Meteor_MoveSpeed;
    }

    //���׿� �ʱ�ȭ
    private void F_InitializeMeteor(Vector3 v_Trans)
    {
        transform.position = v_Trans;
    }

    //���׿��� �÷��̾� �ֺ� �� ���� �Ÿ� ����
    private IEnumerator C_MeteorDistanceCheck(GameObject v_Meteor) 
    {
        float _distance = MeteorManager.Instance._Meteor_Distance;
        Transform _PlayerSphere;
        while(gameObject.activeSelf)
        {
            _PlayerSphere = MeteorManager.Instance.Player_SphereCollider.transform;
            if (Vector3.Distance(_PlayerSphere.position, v_Meteor.transform.position) > _distance)
            {
                F_InitializeMeteor(_Meteor_StartPosition);
                gameObject.SetActive(false);
                MeteorManager.Instance.F_MeteorPoolingAdd(v_Meteor);
                StopAllCoroutines();
            }
            yield return new WaitForSeconds(3f);
        }
    }
}
