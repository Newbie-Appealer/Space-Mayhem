using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class MeteorManager : Singleton<MeteorManager> 
{
    [Header("운석 정보")]
    [SerializeField]       private GameObject _Meteor_Object; // 운석 원본 프리팹
    [Range(1f, 4f)]         public float _Meteor_Delay; //운석 떨어지게 할 딜레이
    [Range(300f, 500f)] public float _Meteor_Distance = 300f; //플레이어 주변 범위랑 운석 사이 거리
    [SerializeField]       private float _Meteor_Count; //운석 총 개수
    [SerializeField]       private float _Meteor_Spawn_SphereRange = 200f; // 운석 생성 최대 범위 원 반지름

    [Header("풀링")]
    private Queue<GameObject> _Pooling_Meteor;               //메테오 풀링
    private Queue<Transform>     _Pooling_MeteorSpawner; //메테오 스포너 풀링

    [Header("플레이어")]
    //플레이어 주변 범위 원 범위 (Player 오브젝트 밑에 새 오브젝트 추가해서 SphereCollider 생성 및 Radius 10으로 생성해주세요)
    [SerializeField] private SphereCollider _Player_Sphere;  
    
    public SphereCollider Player_SphereCollider
    { get { return _Player_Sphere; } }

    protected override void InitManager()
    {
        _Pooling_Meteor = new Queue<GameObject>();
        _Pooling_MeteorSpawner = new Queue<Transform>();

        //스폰 포인트 및 운석 개수만큼
        for (int l = 0; l < _Meteor_Count; l++) 
        {
            F_CreateMeteor(F_CreateMeteorSpawnPoint());
        }
        StartCoroutine(C_MeteorSpawn());
    }

    //운석 풀링
    private void F_CreateMeteor(Transform v_SpanwerTransform)
    {
        GameObject _Meteor = Instantiate(_Meteor_Object, v_SpanwerTransform.position, Quaternion.identity);
        _Meteor.transform.SetParent(v_SpanwerTransform);
        _Meteor.name = "Meteor";
        _Pooling_Meteor.Enqueue( _Meteor );
        _Meteor.SetActive(false);
    }

    //운석 스폰 포인트 풀링
    private Transform F_CreateMeteorSpawnPoint()
    {
        GameObject _MeteorSpawner = new GameObject();
        Vector3 _randomSpawner = Random.onUnitSphere * _Meteor_Spawn_SphereRange;
        _MeteorSpawner.transform.position = _randomSpawner;
        _Pooling_MeteorSpawner.Enqueue(_MeteorSpawner.transform);
        _MeteorSpawner.name = "MeteorSpawner";
        return _MeteorSpawner.transform;
    }

    //Delay마다 1개씩 운석 Dequeue
    private IEnumerator C_MeteorSpawn()
    {
        while (true)
        {
            while (_Pooling_Meteor.Count > 0)
            {
                GameObject _spawnedMeteor = _Pooling_Meteor.Dequeue();
                _spawnedMeteor.SetActive(true);
                yield return new WaitForSeconds(_Meteor_Delay);
            }
            //풀링에 더 이상 운석이 없다면 새로운 스폰 포인트 및 운석 생성
            F_CreateMeteor(F_CreateMeteorSpawnPoint());
            yield return new WaitForSeconds(3f);
        }
    }

    //삭제된 메테오 풀링
    public void F_MeteorPoolingAdd(GameObject v_DestroyedMeteor)
    {
        Debug.Log("메테오 풀링 중");
        _Pooling_Meteor.Enqueue(v_DestroyedMeteor);
    }
}
