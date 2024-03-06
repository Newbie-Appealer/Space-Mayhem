using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrap : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private string _itemName;

    // ��� ������ ����
    [Header("������ ����")]
    private GameObject _etcItem_Object;
    protected string _etcItem_Name;
    private Transform _etcItem_StartPosition;
    private Rigidbody _rb;
    protected void Start()
    {
        //���� ��ġ : ���� ����Ʈ ��ġ
        _etcItem_StartPosition = transform.parent.transform;

        //���� ���� �� �ʱ�ȭ (������Ʈ, �̸�, ���� ��ġ)
        F_InitializeItem(this.gameObject, _etcItem_Name, _etcItem_StartPosition);

        //������ ���� �� �̵�
        _rb = GetComponent<Rigidbody>();
        _rb.velocity = new Vector3(ScrapManager.Instance._item_MoveSpeed, 0, 0);

        //���۰� ���ÿ� �Ÿ� üũ �ڷ�ƾ ����
        StartCoroutine(C_ItemDistanceCheck(_etcItem_Object));
    }

    // ������Ʈ �ʱ�ȭ �Լ� (�̸�, �� ó�� ������ ��ġ)
    // ��ġ�� �ٽ� �����ϰ� �ؼ� �ٽ� �����ϰ� �ؾ��� �� �����
    private void F_InitializeItem(GameObject v_GameObject, string v_Name, Transform v_Trans)
    {
        _etcItem_Object = v_GameObject;
        _etcItem_Object.name = v_Name;
        _etcItem_StartPosition = v_Trans;
    }

    private IEnumerator C_ItemDistanceCheck(GameObject v_etcItem)
    {
        float _distance = ScrapManager.Instance._range_Distance;
        Transform _playerTrs;

        //�������� Ȱ��ȭ���ִ� ���� 3�ʿ� �ѹ��� ���� 
        // => ���� Player���� �Ÿ� ��ġ�� ������ �ٷ� �ݿ����� ����. �ణ ������ �ݿ��Ǵ� ����.
        while (v_etcItem.activeSelf)
        {
            _playerTrs = ScrapManager.Instance.playerTransform;
            //�Ÿ��� �־�����
            if (Vector3.Distance(v_etcItem.transform.position, _playerTrs.position) > _distance)
            {
                gameObject.SetActive(false);
                F_InitializeItem(_etcItem_Object, _etcItem_Name, _etcItem_StartPosition);                  // ������Ʈ �ʱ�ȭ
                ScrapManager.Instance.F_ItemPoolingAdd(_etcItem_Object);                                  // Ǯ���� �߰�
                StopAllCoroutines();

            }
            yield return new WaitForSeconds(3f);
        }
    }
    

}
