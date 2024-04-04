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
 
    // 설치가 끝나면 true로 
    [SerializeField] private bool _isModelBuild = false;

    public bool isModelBuild { get => _isModelBuild; set { _isModelBuild = value; } }

    // Model 이랑 다른 오브젝트랑 충돌 했을 떄
    private void OnTriggerStay(Collider other)
    {
        if ( !_isModelBuild) 
        {
            if (other.gameObject.layer == 11)
            {
                // MyBuildingManager 의 변수 바꾸기
                BuildMaster.Instance.myBuildManger.IsntColliderOther = false;

                // Material을 빨간색으로
                BuildMaster.Instance.myBuildManger.F_IsntCollChagneMaterail(1);
            }
        }

    }   

    // Model 이랑 충돌하고 exit 됐을 때
    private void OnTriggerExit(Collider other)
    {
        if (!_isModelBuild)
        {
            if (other.gameObject.layer == 11)
            {
                // MyBuildingManager 의 변수 바꾸기
                BuildMaster.Instance.myBuildManger.IsntColliderOther = true;

                // Material을 초록색 으로
                BuildMaster.Instance.myBuildManger.F_IsntCollChagneMaterail(0);
            }

        }

    }
}
