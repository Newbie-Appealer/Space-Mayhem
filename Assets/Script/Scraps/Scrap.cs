using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrap : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private string _itemName;

    // 재료 아이템 정보
    [Header("아이템 정보")]
    private GameObject _etcItem_Object;
    protected string _etcItem_Name;
    private Transform _etcItem_StartPosition;
    private Rigidbody _rb;
    protected void Start()
    {
        //생성 위치 : 스폰 포인트 위치
        _etcItem_StartPosition = transform.parent.transform;

        //최초 생성 시 초기화 (오브젝트, 이름, 생성 위치)
        F_InitializeItem(this.gameObject, _etcItem_Name, _etcItem_StartPosition);

        //아이템 생성 시 이동
        _rb = GetComponent<Rigidbody>();
        _rb.velocity = new Vector3(ScrapManager.Instance._item_MoveSpeed, 0, 0);

        //시작과 동시에 거리 체크 코루틴 실행
        StartCoroutine(C_ItemDistanceCheck(_etcItem_Object));
    }

    // 오브젝트 초기화 함수 (이름, 맨 처음 생성된 위치)
    // 위치를 다시 랜덤하게 해서 다시 생성하게 해야할 지 고민중
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

        //아이템이 활성화돼있는 동안 3초에 한번씩 실행 
        // => 따라서 Player와의 거리 수치를 높여도 바로 반영되지 않음. 약간 느리게 반영되는 듯함.
        while (v_etcItem.activeSelf)
        {
            _playerTrs = ScrapManager.Instance.playerTransform;
            //거리가 멀어지면
            if (Vector3.Distance(v_etcItem.transform.position, _playerTrs.position) > _distance)
            {
                gameObject.SetActive(false);
                F_InitializeItem(_etcItem_Object, _etcItem_Name, _etcItem_StartPosition);                  // 오브젝트 초기화
                ScrapManager.Instance.F_ItemPoolingAdd(_etcItem_Object);                                  // 풀링에 추가
                StopAllCoroutines();

            }
            yield return new WaitForSeconds(3f);
        }
    }
    

}
