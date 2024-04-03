using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    [Header("=== ABOUT METEOR ===")]
    [SerializeField] private float _meteor_MoveSpeed;
    [SerializeField, Range(300f, 500f)] private float _meteor_Distance = 250f;
    private Rigidbody _rb;
    private Vector3 _meteor_StartPosition;
    public Vector3 MeteorStart => _meteor_StartPosition;

    private float _targetX; 
    private float _targetY;
    private float _targetZ;

    //�÷��̾� �ֺ� ���� ��ü
    private float _player_Sphere_Radius;
    public void F_SettingMeteor()
    {
        _rb = GetComponent<Rigidbody>();
        _player_Sphere_Radius = MeteorManager.Instance.player_SphereCollider.radius;
        gameObject.name = "Meteor";
    }

    public void F_InitializeMeteor(Vector3 v_Trans)
    {
        _meteor_StartPosition = v_Trans;
        transform.position = _meteor_StartPosition;
        gameObject.SetActive(false);
    }

    public void F_MoveMeteor()
    {
        // �÷��̾� �ֺ� ���� �������� �� ���� ���� ������ ��ǥ �̱�
        float _targetX = Random.Range(-_player_Sphere_Radius, _player_Sphere_Radius);
        float _targetY = Random.Range(-_player_Sphere_Radius, _player_Sphere_Radius);
        float _targetZ = Random.Range(-_player_Sphere_Radius, _player_Sphere_Radius);
        Vector3 _targetDirection = (new Vector3(_targetX, _targetY, _targetZ) - transform.position).normalized;
        _rb.velocity = _targetDirection * _meteor_MoveSpeed;
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
                F_InitializeMeteor(_meteor_StartPosition);
            }
            yield return new WaitForSeconds(3f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        F_InitializeMeteor(_meteor_StartPosition);
        MeteorManager.Instance.F_ReturnMeteor(this);
    }

}
