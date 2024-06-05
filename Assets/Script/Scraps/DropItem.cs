using UnityEngine;

public enum ItemDropCode
{
    Random = -1,
    Plastic = 0,        Fiber = 1,      Scrap = 2,              Dirt = 3,
    Sand = 4,           Stone = 5,      IronOre = 6,            SpaceFood = 20,
    Potato = 21,        Bottle = 36,    SimpleoxygenPack = 37,  PotatoBurger = 38,
    PotatoPizza = 39,	PotatoNoodle = 40,
}

public class DropItem : DropObject
{
    [SerializeField] private ItemDropCode _itemCode; // 획득할 아이템 코드

    private ItemDropCode[] _dropCodes =
    {
        ItemDropCode.Plastic,           ItemDropCode.Fiber,         ItemDropCode.Scrap,             ItemDropCode.Dirt,
        ItemDropCode.Sand,              ItemDropCode.Stone,         ItemDropCode.IronOre,           ItemDropCode.SpaceFood,
        ItemDropCode.Potato,            ItemDropCode.Bottle,        ItemDropCode.SimpleoxygenPack,  ItemDropCode.PotatoBurger,
        ItemDropCode.PotatoPizza,       ItemDropCode.PotatoNoodle
    };

    private void Start()
    {
        // 랜덤 아이템을 얻는 아이템일 경우 
        if (_itemCode == ItemDropCode.Random)
            _itemCode = _dropCodes[Random.Range(0, _dropCodes.Length)];
    }
    public override void F_GetObject()
    {
        // _itemCode에 해당하는 아이템을 획득 시도
        if(ItemManager.Instance.inventorySystem.F_GetItem((int)_itemCode))
        {
            // 인벤토리 업데이트
            ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();

            // 아이템 획득 UI 출력
            F_GetObjectUI((int)_itemCode);

            // 오브젝트 삭제
            Destroy(this.gameObject);
        }
    }

    //위에 F_GetObject에서 _itemCode를 매개변수로 쓰면 댐니다!
    private void F_GetObjectUI(int v_itemCodeIndex)
    {
        // UI 실행에 필요한 데이터
        Sprite v_getItemSpr = ResourceManager.Instance.F_GetInventorySprite(v_itemCodeIndex);
        string v_getItemName = ItemManager.Instance.ItemDatas[v_itemCodeIndex]._itemName;

        // UI 실행
        ItemManager.Instance.dropItemSystem.F_GetObjectUI(v_getItemSpr, v_getItemName);
    }
}

