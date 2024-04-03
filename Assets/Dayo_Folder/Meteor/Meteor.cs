using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    private Rigidbody _rb;

    [SerializeField] private float _meteor_MoveSpeed; //� �ӵ�
    private Vector3 _meteor_StartPosition;
    
    //Ÿ�� ��ǥ
    private float _targetX; 
    private float _targetY;
    private float _targetZ;

    //�÷��̾� �ֺ� ���� ��ü
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
        _rb.velocity = _targetDirection * _meteor_MoveSpeed;
    }

    //���׿� �ʱ�ȭ
    private void F_InitializeMeteor(Vector3 v_Trans)
    {
        transform.position = v_Trans;
    }

    //���׿��� �÷��̾� �ֺ� �� ���� �Ÿ� ����
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
