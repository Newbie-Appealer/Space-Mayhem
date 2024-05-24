using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRecipe : DropObject
{
    public override void F_GetObject()
    {
        // 해금 단계 + 1
        GameManager.Instance.unlockRecipeStep++;

        // 해금 단계에 맞는 레시피를 제작UI에 추가
        ItemManager.Instance.craftSystem.F_AddUnlockRecipe(GameManager.Instance.unlockRecipeStep);

        // 오브젝트 삭제
        Destroy(gameObject);
    }
}