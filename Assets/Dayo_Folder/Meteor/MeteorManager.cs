using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class MeteorManager : Singleton<MeteorManager> 
{
    [Header("� ����")]
    [SerializeField]  private GameObject _meteor_Object; // � ���� ������
    [SerializeField]  private float _meteor_Spawn_SphereRange = 200f; // � ���� �ִ� ���� �� ������
    [SerializeField]  private int _meteor_Count; //� �ʱ� Ǯ�� ����
    [SerializeField, Range(1f, 4f)] private float _meteor_Delay;                 //� �������� �� ������
    private GameObject _meteor_Group;              //� ���� ����Ʈ ��Ƶ� �� ������Ʈ

    [Header("Ǯ��")]
    private Queue<Meteor> _pooling_Meteor;               //���׿� Ǯ��
    private List<Vector3>     _pooling_MeteorSpawner;          //���׿� ���� ��ġ

    [Header("�÷��̾�")]
    //�÷��̾� �ֺ� ���� �� ���� (Player ������Ʈ �ؿ� �� ������Ʈ �߰��ؼ� SphereCollider ���� �� Radius 10���� �������ּ���)
    [SerializeField] private SphereCollider _player_Sphere;  
    
    public SphereCollider player_SphereCollider
    { get { return _player_Sphere; } }

    protected override void InitManager()
    {
        _pooling_Meteor = new Queue<Meteor>();
        _pooling_MeteorSpawner = new List<Vector3>();

        _meteor_Group = new GameObject();
        _meteor_Group.name = "Meteor_Group";
        _meteor_Group.transform.position = new Vector3(-10, 8.4f, 19f);
        
        //���ϴ� ������ŭ ���� ����Ʈ �� � ���� ����
        for (int l = 0; l < _meteor_Count; l++) 
        {
            F_CreateMeteorSpawnPoint();
            F_CreateMeteor(l);
        }
        StartCoroutine(C_MeteorSpawn());
    }

    //� ���� ����Ʈ Ǯ��
    private Vector3 F_CreateMeteorSpawnPoint()
    {
        Vector3 _randomSpawner = Random.onUnitSphere * _meteor_Spawn_SphereRange;
        _pooling_MeteorSpawner.Add(_randomSpawner);
        return _randomSpawner;
    }

    //� Ǯ��
    private void F_CreateMeteor(int v_Index)
    {
        Meteor _Meteor = Instantiate(_meteor_Object).GetComponent<Meteor>();
        Vector3 _spawn_Point = _pooling_MeteorSpawner[v_Index];
        _Meteor.transform.SetParent(_meteor_Group.transform);
        _Meteor.F_SettingMeteor();
        _Meteor.F_InitializeMeteor(_spawn_Point);

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
            //Ǯ���� ��� ���ٸ� ���ο� ���� ����Ʈ �� � ����
            F_CreateMeteorSpawnPoint();
            F_CreateMeteor(_meteor_Count);
            _meteor_Count++;
            F_MeteorSpawn();
            yield return new WaitForSeconds(3f);
        }
    }

    private void F_MeteorSpawn()
    {
        Meteor _spawnedMeteor = _pooling_Meteor.Dequeue();
        _spawnedMeteor.gameObject.SetActive(true);
        _spawnedMeteor.F_MoveMeteor();
    }
    //������ ���׿� Ǯ��
    public void F_ReturnMeteor(Meteor v_DestroyedMeteor)
    {
        _pooling_Meteor.Enqueue(v_DestroyedMeteor);
        v_DestroyedMeteor.F_InitializeMeteor(v_DestroyedMeteor.MeteorStart);
    }
}
