using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class MeteorManager : Singleton<MeteorManager> 
{
    [Header("� ����")]
    [SerializeField]       private GameObject _Meteor_Object; // � ���� ������
    [Range(1f, 4f)]         public float _Meteor_Delay; //� �������� �� ������
    [Range(300f, 500f)] public float _Meteor_Distance = 300f; //�÷��̾� �ֺ� ������ � ���� �Ÿ�
    [SerializeField]       private float _Meteor_Count; //� �� ����
    [SerializeField]       private float _Meteor_Spawn_SphereRange = 200f; // � ���� �ִ� ���� �� ������

    [Header("Ǯ��")]
    private Queue<GameObject> _Pooling_Meteor;               //���׿� Ǯ��
    private Queue<Transform>     _Pooling_MeteorSpawner; //���׿� ������ Ǯ��

    [Header("�÷��̾�")]
    //�÷��̾� �ֺ� ���� �� ���� (Player ������Ʈ �ؿ� �� ������Ʈ �߰��ؼ� SphereCollider ���� �� Radius 10���� �������ּ���)
    [SerializeField] private SphereCollider _Player_Sphere;  
    
    public SphereCollider Player_SphereCollider
    { get { return _Player_Sphere; } }

    protected override void InitManager()
    {
        _Pooling_Meteor = new Queue<GameObject>();
        _Pooling_MeteorSpawner = new Queue<Transform>();

        //���� ����Ʈ �� � ������ŭ
        for (int l = 0; l < _Meteor_Count; l++) 
        {
            F_CreateMeteor(F_CreateMeteorSpawnPoint());
        }
        StartCoroutine(C_MeteorSpawn());
    }

    //� Ǯ��
    private void F_CreateMeteor(Transform v_SpanwerTransform)
    {
        GameObject _Meteor = Instantiate(_Meteor_Object, v_SpanwerTransform.position, Quaternion.identity);
        _Meteor.transform.SetParent(v_SpanwerTransform);
        _Meteor.name = "Meteor";
        _Pooling_Meteor.Enqueue( _Meteor );
        _Meteor.SetActive(false);
    }

    //� ���� ����Ʈ Ǯ��
    private Transform F_CreateMeteorSpawnPoint()
    {
        GameObject _MeteorSpawner = new GameObject();
        Vector3 _randomSpawner = Random.onUnitSphere * _Meteor_Spawn_SphereRange;
        _MeteorSpawner.transform.position = _randomSpawner;
        _Pooling_MeteorSpawner.Enqueue(_MeteorSpawner.transform);
        _MeteorSpawner.name = "MeteorSpawner";
        return _MeteorSpawner.transform;
    }

    //Delay���� 1���� � Dequeue
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
            //Ǯ���� �� �̻� ��� ���ٸ� ���ο� ���� ����Ʈ �� � ����
            F_CreateMeteor(F_CreateMeteorSpawnPoint());
            yield return new WaitForSeconds(3f);
        }
    }

    //������ ���׿� Ǯ��
    public void F_MeteorPoolingAdd(GameObject v_DestroyedMeteor)
    {
        Debug.Log("���׿� Ǯ�� ��");
        _Pooling_Meteor.Enqueue(v_DestroyedMeteor);
    }
}
