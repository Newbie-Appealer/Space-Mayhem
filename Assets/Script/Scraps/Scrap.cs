using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Scrap : MonoBehaviour
{
    [Header("Scrap Information")]
    [SerializeField] private string _itemName;
    [SerializeField] private int _scrapNumber;
    public int scrapNumber => _scrapNumber;

    private Rigidbody _scrapRigidBody;
    public void F_SettingScrap()
    {
        _scrapRigidBody = GetComponent<Rigidbody>();
    }

    /// <summary> Scrap �ʱ�ȭ �Լ� </summary>
    public void F_InitScrap(Transform v_transformParent)
    {
        _scrapRigidBody.isKinematic = false;            // ������ �ʱ�ȭ
        _scrapRigidBody.velocity = Vector3.zero;            

        this.transform.SetParent(v_transformParent);
        this.transform.localPosition = Vector3.zero;        // ��ġ   �ʱ�ȭ
        this.gameObject.SetActive(false);                   // ������Ʈ ��Ȱ��ȭ
    }

    /// <summary> Scrap ������ ���� �Լ�</summary>
    public void F_MoveScrap(Vector3 v_spawnPosition)
    {
        gameObject.SetActive(true);                                         // ������Ʈ Ȱ��ȭ

        this.transform.position = v_spawnPosition;                          // ��ġ �ʱ�ȭ
        _scrapRigidBody.velocity = ScrapManager.Instance._scrapVelocity;    // ������

        //������ ���۰� ���ÿ� �Ÿ� üũ �ڷ�ƾ ����
        StartCoroutine(C_ItemDistanceCheck());
    }

    IEnumerator C_ItemDistanceCheck()
    {
        float distance = ScrapManager.Instance._range_Distance;         // �Ÿ�
        Transform playerTrs = ScrapManager.Instance.playerTransform;    // �÷��̾� ��ġ
        //�������� Ȱ��ȭ���ִ� ���� 3�ʿ� �ѹ��� ���� 
        while (gameObject.activeSelf)
        {
            //�Ÿ��� �־�����
            if (Vector3.Distance(gameObject.transform.position, playerTrs.position) > distance)
            {
                ScrapManager.Instance.F_ReturnScrap(this);  // �ʱ�ȭ �� Ǯ��
                StopAllCoroutines();
            }
            yield return new WaitForSeconds(3f);            // 3�ʿ� �ѹ��� �Ÿ��� Ȯ��
        }
    }

    public void F_GetScrap()
    {
        if(scrapNumber == 3)
        {
            for(int i = 0; i < scrapNumber; i++)
            {
                
            }
        }
        ItemManager.Instance.inventorySystem.F_GetItem(scrapNumber);
        ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();
        ScrapManager.Instance.F_ReturnScrap(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Spear"))
        {
            ScrapManager.Instance._scrapHitedSpear.Add(this);
            _scrapRigidBody.isKinematic = true; 
            transform.SetParent(other.transform);
            transform.localPosition = Vector3.zero;
            //_scrapRigidBody.velocity = Vector3.zero;
        }
    }
}
