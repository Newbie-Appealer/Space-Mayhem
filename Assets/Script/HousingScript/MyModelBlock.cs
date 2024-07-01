using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyModelBlock : MonoBehaviour
{
    /// <summary>
    /// Building BLock 프리팹 밑의 ,  Model 밑의, 오브젝트에 추가
    /// 
    /// #Todo
    /// 운석 충돌감지
    /// 
    /// </summary>

    // Model 이랑 다른 오브젝트랑 충돌 했을 때.
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == BuildMaster.Instance.placedItemLayerInt )       // 설치된 오브젝트 레이어 
        {
            Debug.Log("충돌되고있음" + other.transform.position);

            // MyBuildingManager 의 변수 바꾸기
            BuildMaster.Instance.housingSnapBuild.isntColliderPlacedItem = false;

            // Material을 빨간색으로
            BuildMaster.Instance.myBuildManger.F_IsntCollChagneMaterail(1);
        }
    }   

    // Model 이랑 충돌하고 exit 됐을 때
    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.layer == BuildMaster.Instance.placedItemLayerInt)       // 설치된 오브젝트 레이어 
        {
            // MyBuildingManager 의 변수 바꾸기
            BuildMaster.Instance.housingSnapBuild.isntColliderPlacedItem = true;

            // Material을 초록색 으로
            BuildMaster.Instance.myBuildManger.F_IsntCollChagneMaterail(0);
        }
    }
    
}
