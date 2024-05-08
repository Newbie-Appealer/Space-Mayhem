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

    /// <summary> Scrap 초기화 함수 </summary>
    public void F_InitScrap(Transform v_transformParent)
    {
        _scrapRigidBody.isKinematic = false;            // 움직임 초기화
        _scrapRigidBody.velocity = Vector3.zero;
        _isHited = false;

        this.transform.SetParent(v_transformParent);
        this.transform.localPosition = Vector3.zero;        // 위치   초기화
        this.gameObject.SetActive(false);                   // 오브젝트 비활성화
    }

    /// <summary> Scrap 움직임 시작 함수</summary>
    public void F_MoveScrap(Vector3 v_spawnPosition, Vector3 v_scrapVelocity)
    {
        gameObject.SetActive(true);                                         // 오브젝트 활성화

        this.transform.position = v_spawnPosition;                          // 위치 초기화
        //_scrapRigidBody.velocity = ScrapManager.Instance._scrapVelocity;    // 움직임
        _scrapRigidBody.velocity = v_scrapVelocity * ScrapManager.Instance._item_MoveSpeed;    // 움직임
        StartCoroutine(C_ItemDistanceCheck());
    }

    IEnumerator C_ItemDistanceCheck()
    {
        //Scrap 생성 10초 후 거리 검사 시작 
        yield return new WaitForSeconds(10f);

        float distance = ScrapManager.Instance._range_Distance;         //재료가 존재할 수 있는 최대 거리
        Transform playerTrs = ScrapManager.Instance.playerTransform;    // 플레이어 위치

        while (gameObject.activeSelf)
        {
            //거리가 멀어지면
            if (Vector3.Distance(gameObject.transform.position, playerTrs.position) > distance)
            {
                StopAllCoroutines();
                ScrapManager.Instance.F_ReturnScrap(this);  // 초기화 및 풀링
            }
            yield return new WaitForSeconds(3f);            // 3초에 한번씩 거리를 확인
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
                //박스 먹었을 때 아이템 획득 작성
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
