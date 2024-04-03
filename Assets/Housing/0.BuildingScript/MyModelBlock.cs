using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyModelBlock : MonoBehaviour
{
    /// <summary>
    /// Building BLock ������ ���� ,  Model ����, ������Ʈ�� �߰�
    /// </summary>
 
    // ��ġ�� ������ true�� 
    private bool _isModelBuild = false;

    public bool isModelBuild { get => _isModelBuild; set { _isModelBuild = value; } }

    // Model �̶� �ٸ� ������Ʈ�� �浹 ���� ��
    private void OnTriggerStay(Collider other)
    {
        if ( !_isModelBuild) 
        {
            if (other.gameObject.layer == 11)
            {
                // MyBuildingManager �� ���� �ٲٱ�
                MyBuildManager.Instance.IsntColliderOther = false;

                // Material�� ����������
                MyBuildManager.Instance.F_IsntCollChagneMaterail(1);
            }
        }

    }   

    // Model �̶� �浹�ϰ� exit ���� ��
    private void OnTriggerExit(Collider other)
    {
        if (!_isModelBuild)
        {
            if (other.gameObject.layer == 11)
            {
                // MyBuildingManager �� ���� �ٲٱ�
                MyBuildManager.Instance.IsntColliderOther = true;

                // Material�� �ʷϻ� ����
                MyBuildManager.Instance.F_IsntCollChagneMaterail(0);
            }

        }

    }
}
