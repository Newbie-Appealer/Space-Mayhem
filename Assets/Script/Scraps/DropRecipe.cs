using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRecipe : DropObject
{
    public override void F_GetObject()
    {
        // �ر� �ܰ� + 1
        GameManager.Instance.unlockRecipeStep++;

        // �ر� �ܰ迡 �´� �����Ǹ� ����UI�� �߰�
        ItemManager.Instance.craftSystem.F_AddUnlockRecipe(GameManager.Instance.unlockRecipeStep);

        // ������Ʈ ����
        Destroy(gameObject);
    }
}