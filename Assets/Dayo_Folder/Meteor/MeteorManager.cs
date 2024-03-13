using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class MeteorManager : Singleton<MeteorManager> 
{
    [Header("� ����")]
    [SerializeField]       private GameObject _Meteor_Object; // � ���� ������
    [Range(1f, 4f)]         public float _Meteor_Delay;                 //� �������� �� ������
    [Range(300f, 500f)] public float _Meteor_Distance = 200f; //�÷��̾� �ֺ� ������ � ���� �Ÿ�
    [SerializeField]       private float _Meteor_Spawn_SphereRange = 200f; // � ���� �ִ� ���� �� ������
    [SerializeField]       private int _Meteor_Count; //� �ʱ� Ǯ�� ����
    GameObject _Meteor_Group;              //� ���� ����Ʈ ��Ƶ� �� ������Ʈ

    [Header("Ǯ��")]
    private Queue<GameObject> _Pooling_Meteor;               //���׿� Ǯ��
    private List<Vector3>     _Pooling_MeteorSpawner;          //���׿� ���� ��ġ

    [Header("�÷��̾�")]
    //�÷��̾� �ֺ� ���� �� ���� (Player ������Ʈ �ؿ� �� ������Ʈ �߰��ؼ� SphereCollider ���� �� Radius 10���� �������ּ���)
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
        
        //���ϴ� ������ŭ ���� ����Ʈ �� � ���� ����
        for (int l = 0; l < _Meteor_Count; l++) 
        {
            F_CreateMeteorSpawnPoint();
            F_CreateMeteor(l);
        }
        StartCoroutine(C_MeteorSpawn());
    }

    //� ���� ����Ʈ Ǯ��
    private Vector3 F_CreateMeteorSpawnPoint()
    {
        Vector3 _randomSpawner = Random.onUnitSphere * _Meteor_Spawn_SphereRange;
        _Pooling_MeteorSpawner.Add(_randomSpawner);
        return _randomSpawner;
    }

    //� Ǯ��
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

    //Delay���� 1���� � Dequeue
    private IEnumerator C_MeteorSpawn()
    {
        while (true)
        {
            while (_Pooling_Meteor.Count > 0)
            {
                F_MeteorSpawn();
                yield return new WaitForSeconds(_Meteor_Delay);
            }
            //Ǯ���� ��� ���ٸ� ���ο� ���� ����Ʈ �� � ����
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
    //������ ���׿� Ǯ��
    public void F_MeteorPoolingAdd(GameObject v_DestroyedMeteor)
    {
        _Pooling_Meteor.Enqueue(v_DestroyedMeteor);
    }
}
