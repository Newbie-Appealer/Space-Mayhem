using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class Scrap : MonoBehaviour
{
    [Header("Scrap Information")]
    [SerializeField] private string _itemName;
    [SerializeField] private int _scrapNumber;
    public int scrapNumber => _scrapNumber;
    private bool _isHited = false;
    private Rigidbody _scrapRigidBody;

    private Vector3 _refVector3 = Vector3.zero;
    public void F_SettingScrap()
    {
        _scrapRigidBody = GetComponent<Rigidbody>();
    }

    /// <summary> Scrap �ʱ�ȭ �Լ� </summary>
    public void F_InitScrap(Transform v_transformParent)
    {
        _scrapRigidBody.isKinematic = false;            // ������ �ʱ�ȭ
        _scrapRigidBody.velocity = Vector3.zero;
        _isHited = false;

        this.transform.SetParent(v_transformParent);
        this.transform.localPosition = Vector3.zero;        // ��ġ   �ʱ�ȭ
        this.gameObject.SetActive(false);                   // ������Ʈ ��Ȱ��ȭ
    }

    /// <summary> Scrap ������ ���� �Լ�</summary>
    public void F_MoveScrap(Vector3 v_spawnPosition, Vector3 v_scrapVelocity)
    {
        gameObject.SetActive(true);                                         // ������Ʈ Ȱ��ȭ

        this.transform.position = v_spawnPosition;                          // ��ġ �ʱ�ȭ
        //_scrapRigidBody.velocity = ScrapManager.Instance._scrapVelocity;    // ������
        _scrapRigidBody.velocity = v_scrapVelocity * ScrapManager.Instance._item_MoveSpeed;    // ������
        StartCoroutine(C_ItemDistanceCheck());
    }

    IEnumerator C_ItemDistanceCheck()
    {
        //Scrap ���� 10�� �� �Ÿ� �˻� ���� 
        yield return new WaitForSeconds(10f);

        float distance = ScrapManager.Instance._range_Distance;         //��ᰡ ������ �� �ִ� �ִ� �Ÿ�
        Transform playerTrs = ScrapManager.Instance.playerTransform;    // �÷��̾� ��ġ

        while (gameObject.activeSelf)
        {
            //�Ÿ��� �־�����
            if (Vector3.Distance(gameObject.transform.position, playerTrs.position) > distance)
            {
                StopAllCoroutines();
                ScrapManager.Instance.F_ReturnScrap(this);  // �ʱ�ȭ �� Ǯ��
            }
            yield return new WaitForSeconds(3f);            // 3�ʿ� �ѹ��� �Ÿ��� Ȯ��
        }
    }

    public IEnumerator C_ItemVelocityChange(Vector3 v_newVelocity, float v_changeSpeed)
    {
        while (_scrapRigidBody.velocity != v_newVelocity)
        {
            _scrapRigidBody.velocity
                    = Vector3.SmoothDamp(_scrapRigidBody.velocity, v_newVelocity, ref _refVector3, v_changeSpeed).normalized * ScrapManager.Instance._item_MoveSpeed;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public void F_GetScrap()
    {
        if(scrapNumber == 3)
        {
            for(int i = 0; i < scrapNumber; i++)
            {
                //�ڽ� �Ծ��� �� ������ ȹ�� �ۼ�
            }
        }
        ItemManager.Instance.inventorySystem.F_GetItem(scrapNumber);
        ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();
        ScrapManager.Instance.F_ReturnScrap(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Spear") && !_isHited)
        {
            _isHited = true;
            ScrapManager.Instance._scrapHitedSpear.Add(this);
            _scrapRigidBody.isKinematic = true; 
            transform.SetParent(other.transform);
            transform.localPosition = Vector3.zero;
        }
    }
}
