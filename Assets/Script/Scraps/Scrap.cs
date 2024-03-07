using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrap : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private string _itemName;
    private Vector3 _etcItem_StartPosition;

    private Rigidbody _rb;

    protected void Start()
    {
        //���� ��ġ : ���� ����Ʈ ��ġ
        _etcItem_StartPosition = transform.position;

        //���� ���� �� �ʱ�ȭ (�̸�, ���� ��ġ)
        F_InitializeItem(_itemName, _etcItem_StartPosition);

        //������ ���� �� �̵�
        _rb = GetComponent<Rigidbody>();
        _rb.velocity = new Vector3(ScrapManager.Instance._item_MoveSpeed, 0, 0);

    }

    protected void OnEnable()
    {
        //Ȱ��ȭ�� ���ÿ� �Ÿ� üũ �ڷ�ƾ ����
        StartCoroutine(C_ItemDistanceCheck(gameObject));
    }
    //������Ʈ ���� ��ġ ��������
    public Vector3 F_SaveStartPoint(Vector3 v_point)
    {
        _etcItem_StartPosition = v_point;
        return _etcItem_StartPosition;
    }
    // ������Ʈ �ʱ�ȭ �Լ� (�̸�, �� ó�� ������ ��ġ)
    private void F_InitializeItem(string v_Name, Vector3 v_Trans)
    {
        gameObject.name = v_Name;
        transform.position = v_Trans;
    }

    private IEnumerator C_ItemDistanceCheck(GameObject v_etcItem)
    {
        float _distance = ScrapManager.Instance._range_Distance;
        Transform _playerTrs;

        //�������� Ȱ��ȭ���ִ� ���� 3�ʿ� �ѹ��� ���� 
        while (v_etcItem.activeSelf)
        {
            _playerTrs = ScrapManager.Instance.playerTransform;
            //�Ÿ��� �־�����
            if (Vector3.Distance(v_etcItem.transform.position, _playerTrs.position) > _distance)
            {
                gameObject.SetActive(false);
                F_InitializeItem(_itemName, _etcItem_StartPosition);                  // ������Ʈ �ʱ�ȭ
                ScrapManager.Instance.F_ItemPoolingAdd(v_etcItem);                                  // Ǯ���� �߰�
                StopAllCoroutines();
            }
            yield return new WaitForSeconds(3f);
        }
    }
    

}
