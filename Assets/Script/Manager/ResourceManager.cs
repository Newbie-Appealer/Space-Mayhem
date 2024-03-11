using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    public Sprite emptySlotSprite => _emptySlotSprite;
    [SerializeField] private Sprite _emptySlotSprite;
    [SerializeField] private List<Sprite> _inventorySprites;

    protected override void InitManager() {  }

    public Sprite F_GetInventorySprite(int v_code)
    {
        return _inventorySprites[v_code];
    }
}
