using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyModelBlock : MonoBehaviour
{
    /// <summary>
    /// Building BLock ������ ���� ,  Model ����, ������Ʈ�� �߰�
    /// 
    /// #Todo
    /// � �浹����
    /// 
    /// </summary>

    // Model �̶� �ٸ� ������Ʈ�� �浹 ���� ��.
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == BuildMaster.Instance.placedItemLayerInt )       // ��ġ�� ������Ʈ ���̾� 
        {
            Debug.Log("�浹�ǰ�����" + other.transform.position);

            // MyBuildingManager �� ���� �ٲٱ�
            BuildMaster.Instance.housingSnapBuild.isntColliderPlacedItem = false;

            // Material�� ����������
            BuildMaster.Instance.myBuildManger.F_IsntCollChagneMaterail(1);
        }
    }   

    // Model �̶� �浹�ϰ� exit ���� ��
    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.layer == BuildMaster.Instance.placedItemLayerInt)       // ��ġ�� ������Ʈ ���̾� 
        {
            // MyBuildingManager �� ���� �ٲٱ�
            BuildMaster.Instance.housingSnapBuild.isntColliderPlacedItem = true;

            // Material�� �ʷϻ� ����
            BuildMaster.Instance.myBuildManger.F_IsntCollChagneMaterail(0);
        }
    }
    
}
