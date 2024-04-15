using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class MeteorManager : Singleton<MeteorManager> 
{
    [Header("� ����")]
    [SerializeField]  private GameObject _meteor_Object; // � ���� ������
    [SerializeField]  private float _meteor_Spawn_SphereRange = 150f; // � ���� �ִ� ���� �� ������
    [SerializeField]  private int _meteor_Count; //� �ʱ� Ǯ�� ����
    [SerializeField, Range(1f, 4f)] private float _meteor_Delay;                 //� �������� �� ������
    private GameObject _meteor_Group;              //� ���� ����Ʈ ��Ƶ� �� ������Ʈ
    [SerializeField] private float _meteor_MoveSpeed;
    public float meteorSpeed => _meteor_MoveSpeed;

    [Header("Ǯ��")]
    private Queue<Meteor> _pooling_Meteor;               //���׿� Ǯ��

    [Header("�÷��̾�")]
    //�÷��̾� �ֺ� ���� �� ���� 
    [SerializeField] private SphereCollider _player_Sphere;  
    
    public SphereCollider player_SphereCollider
    { get { return _player_Sphere; } }

    protected override void InitManager()
    {
        _pooling_Meteor = new Queue<Meteor>();

        _meteor_Group = new GameObject();
        _meteor_Group.name = "MeteorGroup";
        _meteor_Group.transform.position = Vector3.zero;
        
        for (int l = 0; l < _meteor_Count; l++) 
        {
            F_CreateMeteor();
        }
        StartCoroutine(C_MeteorSpawn());
    }

    //� Ǯ��
    private void F_CreateMeteor()
    {
        Meteor _Meteor = Instantiate(_meteor_Object).GetComponent<Meteor>();
        _Meteor.F_SettingMeteor();
        _Meteor.transform.SetParent(_meteor_Group.transform);
        _Meteor.gameObject.SetActive(false);
        _pooling_Meteor.Enqueue( _Meteor );
    }

    //Delay���� 1���� � Dequeue
    private IEnumerator C_MeteorSpawn()
    {
        while (true)
        {
            while (_pooling_Meteor.Count > 0)
            {
                F_MeteorSpawn();
                yield return new WaitForSeconds(_meteor_Delay);
            }
            //Ǯ���� ��� ���ٸ� 3�ʿ� 1���� ���ο� � ����
            F_CreateMeteor();
            F_MeteorSpawn();
            yield return new WaitForSeconds(3f);
        }
    }

    private void F_MeteorSpawn()
    {
        Meteor _spawnedMeteor = _pooling_Meteor.Dequeue();
        Vector3 _spawn_Point = Random.onUnitSphere * _meteor_Spawn_SphereRange;
        _spawnedMeteor.transform.position = _spawn_Point;
        _spawnedMeteor.gameObject.SetActive(true);
        _spawnedMeteor.F_MoveMeteor();
    }

    //������ ���׿� Ǯ��
    public void F_ReturnMeteor(Meteor v_DestroyedMeteor)
    {
        _pooling_Meteor.Enqueue(v_DestroyedMeteor);
        v_DestroyedMeteor.gameObject.SetActive(false);
    }
}
