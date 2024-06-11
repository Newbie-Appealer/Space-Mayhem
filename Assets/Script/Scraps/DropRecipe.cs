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

        // ������Ʈ ����
        Destroy(gameObject);
    }

    private void F_UnlockRecipe()
    {
        // �ر� �ܰ� + 1
        GameManager.Instance.unlockRecipeStep++;

        // �ر� �ܰ迡 �´� �����Ǹ� ����UI�� �߰�
        ItemManager.Instance.craftSystem.F_AddUnlockRecipe(GameManager.Instance.unlockRecipeStep);
    }

    private void F_GetJournal()
    {

    }
}