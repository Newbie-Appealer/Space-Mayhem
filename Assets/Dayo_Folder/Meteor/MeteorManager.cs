using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class MeteorManager : Singleton<MeteorManager> 
{
    [Header("Meteor Information")]
    [SerializeField]  private float _meteor_Spawn_SphereRange = 150f;   // � ���� �ִ� ���� �� ������
    [SerializeField, Range(1f, 4f)] private float _meteor_Delay;        // � ���� �ֱ�
    public float[] _drop_Chance;                                        // � ������ ȹ�� Ȯ��

    [Header("Ǯ��")]
    [SerializeField]  private GameObject[] _meteorPrefabs;  // � ���� ������
    private List<Queue<Meteor>> _poolingMeteor;             // ���׿� Ǯ��
    private GameObject _meteor_Group;                       // ���׿� ������Ʈ�� ��Ƶ� ������Ʈ

    [Header("�÷��̾�")]
    [SerializeField] private SphereCollider _player_Sphere; // �÷��̾� �ֺ� ���� �� ���� 

    public SphereCollider player_SphereCollider
    { get { return _player_Sphere; } }

    protected override void InitManager()
    {
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

    //� Ǯ��
    private void F_CreateMeteor(int v_index)
    {
        Meteor _Meteor = Instantiate(_meteorPrefabs[v_index], _meteor_Group.transform).GetComponent<Meteor>();
        _Meteor.F_SettingMeteor(v_index);               // ���׿� �ʱ�ȭ
        _Meteor.gameObject.SetActive(false);            // ���׿� ��Ȱ��ȭ
        _poolingMeteor[v_index].Enqueue( _Meteor );     // Ǯ�� �߰�
    }

    //Delay���� 1���� � Dequeue
    private IEnumerator C_MeteorSpawn()
    {
        while (true)
        {
            int rnd = Random.Range(0, _poolingMeteor.Count);

            if(!(_poolingMeteor[rnd].Count > 0))
            {
                F_CreateMeteor(rnd);    // Ǯ���� ��� ������ ���� ����
            }
            F_MeteorSpawn(rnd);

            yield return new WaitForSeconds(_meteor_Delay);
        }
    }

    private void F_MeteorSpawn(int v_index)
    {
        Meteor _spawnedMeteor = _poolingMeteor[v_index].Dequeue();

        Vector3 _spawn_Point = Random.onUnitSphere * _meteor_Spawn_SphereRange;

        float _spawn_Point_y = Mathf.Abs(_spawn_Point.y);
        _spawn_Point = new Vector3(_spawn_Point.x, _spawn_Point_y, _spawn_Point.z);
        _spawnedMeteor.transform.position = _spawn_Point;
        _spawnedMeteor.gameObject.SetActive(true);

        _spawnedMeteor.F_MoveMeteor();
    }

    // Meteor Ǯ������ �ǵ����ִ� �Լ�
    public void F_ReturnMeteor(Meteor v_DestroyedMeteor, int v_index)
    {
        v_DestroyedMeteor.F_StopMeteorCoroutine();
        _poolingMeteor[v_index].Enqueue(v_DestroyedMeteor);
        v_DestroyedMeteor.gameObject.SetActive(false);
    }

    public float F_GetMeteorSpeed()
    {
        float _moveSpeed = Random.Range(10f, 20f);
        return _moveSpeed;
    }
}
