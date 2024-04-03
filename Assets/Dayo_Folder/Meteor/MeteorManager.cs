using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class MeteorManager : Singleton<MeteorManager> 
{
    [Header("운석 정보")]
    [SerializeField]  private GameObject _meteor_Object; // 운석 원본 프리팹
    [SerializeField]  private float _meteor_Spawn_SphereRange = 200f; // 운석 생성 최대 범위 원 반지름
    [SerializeField]  private int _meteor_Count; //운석 초기 풀링 개수
    [SerializeField, Range(1f, 4f)] private float _meteor_Delay;                 //운석 떨어지게 할 딜레이
    private GameObject _meteor_Group;              //운석 스폰 포인트 모아둘 빈 오브젝트

    [Header("풀링")]
    private Queue<Meteor> _pooling_Meteor;               //메테오 풀링
    private List<Vector3>     _pooling_MeteorSpawner;          //메테오 스폰 위치

    [Header("플레이어")]
    //플레이어 주변 범위 원 범위 (Player 오브젝트 밑에 새 오브젝트 추가해서 SphereCollider 생성 및 Radius 10으로 생성해주세요)
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
        
        //원하는 개수만큼 스폰 포인트 및 운석 개수 조정
        for (int l = 0; l < _meteor_Count; l++) 
        {
            F_CreateMeteorSpawnPoint();
            F_CreateMeteor(l);
        }
        StartCoroutine(C_MeteorSpawn());
    }

    //운석 스폰 포인트 풀링
    private Vector3 F_CreateMeteorSpawnPoint()
    {
        Vector3 _randomSpawner = Random.onUnitSphere * _meteor_Spawn_SphereRange;
        _pooling_MeteorSpawner.Add(_randomSpawner);
        return _randomSpawner;
    }

    //운석 풀링
    private void F_CreateMeteor(int v_Index)
    {
        Meteor _Meteor = Instantiate(_meteor_Object).GetComponent<Meteor>();
        Vector3 _spawn_Point = _pooling_MeteorSpawner[v_Index];
        _Meteor.transform.SetParent(_meteor_Group.transform);
        _Meteor.F_SettingMeteor();
        _Meteor.F_InitializeMeteor(_spawn_Point);

        _pooling_Meteor.Enqueue( _Meteor );
    }

    //Delay마다 1개씩 운석 Dequeue
    private IEnumerator C_MeteorSpawn()
    {
        while (true)
        {
            while (_pooling_Meteor.Count > 0)
            {
                F_MeteorSpawn();
                yield return new WaitForSeconds(_meteor_Delay);
            }
            //풀링에 운석이 없다면 새로운 스폰 포인트 및 운석 생성
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
    //삭제된 메테오 풀링
    public void F_ReturnMeteor(Meteor v_DestroyedMeteor)
    {
        _pooling_Meteor.Enqueue(v_DestroyedMeteor);
        v_DestroyedMeteor.F_InitializeMeteor(v_DestroyedMeteor.MeteorStart);
    }
}
