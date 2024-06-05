using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class MeteorManager : Singleton<MeteorManager> 
{
    [Header("Meteor Information")]
    [SerializeField]  private float _meteor_Spawn_SphereRange = 200f;   // 운석 생성 최대 범위 원 반지름
    [SerializeField, Range(0.01f, 60f)] private float _meteor_Delay;    // 운석 생성 주기 ( defalut )
    [SerializeField, Range(1,20)] private int _meteor_Count;            // 운석 생성 개수 ( defalut )
    private float _meteor_DelayLimit;
    private int _meteor_CountLimit;
    public float[] _drop_Chance;                                        // 운석 아이템 획득 확률

    [Header("풀링")]
    [SerializeField]  private GameObject[] _meteorPrefabs;  // 운석 원본 프리팹
    private List<Queue<Meteor>> _poolingMeteor;             // 메테오 풀링
    private GameObject _meteor_Group;                       // 메테오 오브젝트를 모아둘 오브젝트

    [Header("플레이어")]
    [SerializeField] private SphereCollider _player_Sphere; // 플레이어 주변 범위 원 범위 

    // Get Set
    public SphereCollider player_SphereCollider
    { get { return _player_Sphere; } }
    public float meteorDelay
    { get => _meteor_Delay; set => _meteor_Delay = value; }
    public int meteorCount
    { get => _meteor_Count; set => _meteor_Count = value; }

    protected override void InitManager()
    {
        _meteor_DelayLimit = 10f;
        _meteor_CountLimit = 10;

        _poolingMeteor = new List<Queue<Meteor>>();

        _meteor_Group = new GameObject();
        _meteor_Group.name = "MeteorGroup";
        _meteor_Group.transform.position = Vector3.zero;
        
        _drop_Chance = new float[] { 40f, 40f, 10f, 8f, 2f };

        for(int i = 0; i < _meteorPrefabs.Length; i++)
        {
            _poolingMeteor.Add(new Queue<Meteor>());
            for(int j = 0; j < 3; j++)
            {
                F_CreateMeteor(i);
            }
        }
        StartCoroutine(C_MeteorSpawn());
    }

    //운석 풀링
    private void F_CreateMeteor(int v_index)
    {
        Meteor _Meteor = Instantiate(_meteorPrefabs[v_index], _meteor_Group.transform).GetComponent<Meteor>();
        _Meteor.F_SettingMeteor(v_index);               // 메테오 초기화
        _Meteor.gameObject.SetActive(false);            // 메테오 비활성화
        _poolingMeteor[v_index].Enqueue( _Meteor );     // 풀링 추가
    }

    //Delay마다 1개씩 운석 Dequeue
    private IEnumerator C_MeteorSpawn()
    {
        while (true)
        {
            // _meteor_Count 만큼 메테오를 생성 ( 1 ~ 20 )
            for(int i = 0; i < _meteor_Count; i++)
            {
                // 운석 번호 랜덤 
                int rnd = Random.Range(0, _poolingMeteor.Count);

                // 풀링이 비어있을때
                if(!(_poolingMeteor[rnd].Count > 0))
                {
                    // 풀링에 운석이 없으면 새로 생성
                    F_CreateMeteor(rnd);
                }
                // 운석 생성
                F_MeteorSpawn(rnd);
            }
            yield return new WaitForSeconds(_meteor_Delay);
        }
    }

    private void F_MeteorSpawn(int v_index)
    {
        Meteor _spawnedMeteor = _poolingMeteor[v_index].Dequeue();

        Vector3 _spawn_Point = Random.onUnitSphere * _meteor_Spawn_SphereRange;

        //y좌표 절대값으로 위에서만 생성
        float _spawn_Point_y = Mathf.Abs(_spawn_Point.y);

        _spawn_Point = new Vector3(_spawn_Point.x, _spawn_Point_y, _spawn_Point.z);
        _spawnedMeteor.transform.position = _spawn_Point;
        _spawnedMeteor.gameObject.SetActive(true);

        _spawnedMeteor.F_MoveMeteor();
    }

    // Meteor 풀링으로 되돌려주는 함수
    public void F_ReturnMeteor(Meteor v_DestroyedMeteor, int v_index)
    {
        v_DestroyedMeteor.F_StopMeteorCoroutine();
        _poolingMeteor[v_index].Enqueue(v_DestroyedMeteor);
        v_DestroyedMeteor.gameObject.SetActive(false);
    }

    public void F_DifficultyUpdate()
    {
        int difficultyValue = GameManager.Instance.storyStep;

        float tmp_Delay = 1 - (0.03f * difficultyValue);    // 1당 0.03 만큼 => 시간이 3%씩 감소함.
        int tmp_Count   = difficultyValue / 3;              // 3클리어당 1 추가

        _meteor_Delay = 35 * tmp_Delay;                     // 60 -> 기본값
        _meteor_Count = 1 + tmp_Count;                      // 1  -> 기본값

        // 제한 수치를 넘어가면
        if (_meteor_Delay < _meteor_DelayLimit)                 
            _meteor_Delay = _meteor_DelayLimit;

        // 제한 수치를 넘어가면
        if (_meteor_Count > _meteor_CountLimit)                  
            _meteor_Count = _meteor_CountLimit;
        /*
            던전 1회 클리어당 메테오 생성주기 '3' % 감소
            던전 3회 클리어당 메테오 생성개수 '1' 개 추가

            메테오 생성주기 제한 : '10' 초까지 감소
            메테오 생성개수 제한 : '10' 개까지 생성
        */
    }
}
