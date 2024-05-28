using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : Furniture
{
    [Range(0f, 30f)]
    public float _jumpSpeed;

    protected override void F_InitFurniture() { }

    private void OnCollisionEnter(Collision collision)
    {
        // �÷��̾� �浹��
        if(collision.collider.CompareTag("Player"))
        {
            PlayerManager.Instance.PlayerController.playerRigidbody.
                AddForce(Vector3.up * _jumpSpeed, ForceMode.Impulse);
        }
    }

    #region ����/�ҷ�����
    public override string F_GetData()
    {
        return "NONE";
    }

    public override void F_SetData(string v_data)
    {
        
    }
    #endregion
}
