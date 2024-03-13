using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class MeteorManager : Singleton<MeteorManager> 
{
    [Header("운석 정보")]
    [SerializeField]       private GameObject _Meteor_Object; // 운석 원본 프리팹
    [Range(1f, 4f)]         public float _Meteor_Delay;                 //운석 떨어지게 할 딜레이
    [Range(300f, 500f)] public float _Meteor_Distance = 200f; //플레이어 주변 범위랑 운석 사이 거리
    [SerializeField]       private float _Meteor_Spawn_SphereRange = 200f; // 운석 생성 최대 범위 원 반지름
    [SerializeField]       private int _Meteor_Count; //운석 초기 풀링 개수
    GameObject _Meteor_Group;              //운석 스폰 포인트 모아둘 빈 오브젝트

    [Header("풀링")]
    private Queue<GameObject> _Pooling_Meteor;               //메테오 풀링
    private List<Vector3>     _Pooling_MeteorSpawner;          //메테오 스폰 위치

    [Header("플레이어")]
    //플레이어 주변 범위 원 범위 (Player 오브젝트 밑에 새 오브젝트 추가해서 SphereCollider 생성 및 Radius 10으로 생성해주세요)
    [SerializeField] private SphereCollider _Player_Sphere;  
    
    public SphereCollider Player_SphereCollider
    { get { return _Player_Sphere; } }

    protected override void InitManager()
    {
        _Pooling_Meteor = new Queue<GameObject>();
        _Pooling_MeteorSpawner = new List<Vector3>();

        _Meteor_Group = new GameObject();
        _Meteor_Group.name = "Meteor_Group";
        _Meteor_Group.transform.position = Vector3.zero;
        
        //원하는 개수만큼 스폰 포인트 및 운석 개수 조정
        for (int l = 0; l < _Meteor_Count; l++) 
        {
            F_CreateMeteorSpawnPoint();
            F_CreateMeteor(l);
        }
        StartCoroutine(C_MeteorSpawn());
    }

    //운석 스폰 포인트 풀링
    private Vector3 F_CreateMeteorSpawnPoint()
    {
        Vector3 _randomSpawner = Random.onUnitSphere * _Meteor_Spawn_SphereRange;
        _Pooling_MeteorSpawner.Add(_randomSpawner);
        return _randomSpawner;
    }

    //운석 풀링
    private void F_CreateMeteor(int v_Index)
    {
        GameObject _Meteor = Instantiate(_Meteor_Object);
        _Meteor.name = "Meteor";
        _Meteor.transform.SetParent(_Meteor_Group.transform);
        Vector3 _spawn_Point = _Pooling_MeteorSpawner[v_Index];
        _Meteor.transform.position = _spawn_Point;
        _Pooling_Meteor.Enqueue( _Meteor );
        _Meteor.SetActive(false);
    }

    //Delay마다 1개씩 운석 Dequeue
    private IEnumerator C_MeteorSpawn()
    {
        while (true)
        {
            while (_Pooling_Meteor.Count > 0)
            {
                F_MeteorSpawn();
                yield return new WaitForSeconds(_Meteor_Delay);
            }
            //풀링에 운석이 없다면 새로운 스폰 포인트 및 운석 생성
            F_CreateMeteorSpawnPoint();
            F_CreateMeteor(_Meteor_Count);
            _Meteor_Count++;
            F_MeteorSpawn();
            yield return new WaitForSeconds(3f);
        }
    }

    private void F_MeteorSpawn()
    {
        GameObject _spawnedMeteor = _Pooling_Meteor.Dequeue();
        _spawnedMeteor.SetActive(true);
    }
    //삭제된 메테오 풀링
    public void F_MeteorPoolingAdd(GameObject v_DestroyedMeteor)
    {
        _Pooling_Meteor.Enqueue(v_DestroyedMeteor);
    }
}
