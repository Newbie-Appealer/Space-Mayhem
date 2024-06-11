using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RecipeType
{
    UNLOCK,
    JOURNAL
}

public class DropRecipe : DropObject
{
    [SerializeField] private RecipeType _type;
    [SerializeField] private string _journalKey = "recipe > No modification";
    public override void F_GetObject()
    {
        switch(_type)
        {
            case RecipeType.UNLOCK:
                F_UnlockRecipe();
                break;

            case RecipeType.JOURNAL:
                F_GetJournal();
                break;
        }

        // 오브젝트 삭제
        Destroy(gameObject);
    }

    private void F_UnlockRecipe()
    {
        // 해금 단계 + 1
        GameManager.Instance.unlockRecipeStep++;

        // 해금 단계에 맞는 레시피를 제작UI에 추가
        ItemManager.Instance.craftSystem.F_AddUnlockRecipe(GameManager.Instance.unlockRecipeStep);
    }

    private void F_GetJournal()
    {

    }
}