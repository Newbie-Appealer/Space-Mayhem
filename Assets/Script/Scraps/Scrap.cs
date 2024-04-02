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

    /// <summary> Scrap 초기화 함수 </summary>
    public void F_InitScrap(Transform v_transformParent)
    {
        _scrapRigidBody.isKinematic = false;            // 움직임 초기화
        _scrapRigidBody.velocity = Vector3.zero;            

        this.transform.SetParent(v_transformParent);
        this.transform.localPosition = Vector3.zero;        // 위치   초기화
        this.gameObject.SetActive(false);                   // 오브젝트 비활성화
    }

    /// <summary> Scrap 움직임 시작 함수</summary>
    public void F_MoveScrap(Vector3 v_spawnPosition)
    {
        gameObject.SetActive(true);                                         // 오브젝트 활성화

        this.transform.position = v_spawnPosition;                          // 위치 초기화
        _scrapRigidBody.velocity = ScrapManager.Instance._scrapVelocity;    // 움직임

        //움직임 시작과 동시에 거리 체크 코루틴 실행
        StartCoroutine(C_ItemDistanceCheck());
    }

    IEnumerator C_ItemDistanceCheck()
    {
        float distance = ScrapManager.Instance._range_Distance;         // 거리
        Transform playerTrs = ScrapManager.Instance.playerTransform;    // 플레이어 위치
        //아이템이 활성화돼있는 동안 3초에 한번씩 실행 
        while (gameObject.activeSelf)
        {
            //거리가 멀어지면
            if (Vector3.Distance(gameObject.transform.position, playerTrs.position) > distance)
            {
                ScrapManager.Instance.F_ReturnScrap(this);  // 초기화 및 풀링
                StopAllCoroutines();
            }
            yield return new WaitForSeconds(3f);            // 3초에 한번씩 거리를 확인
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
