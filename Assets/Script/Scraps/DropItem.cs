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
    [SerializeField] private ItemDropCode _itemCode; // ȹ���� ������ �ڵ�

    private ItemDropCode[] _dropCodes =
    {
        ItemDropCode.Plastic,           ItemDropCode.Fiber,         ItemDropCode.Scrap,             ItemDropCode.Dirt,
        ItemDropCode.Sand,              ItemDropCode.Stone,         ItemDropCode.IronOre,           ItemDropCode.SpaceFood,
        ItemDropCode.Potato,            ItemDropCode.Bottle,        ItemDropCode.SimpleoxygenPack,  ItemDropCode.PotatoBurger,
        ItemDropCode.PotatoPizza,       ItemDropCode.PotatoNoodle
    };

    private void Start()
    {
        // ���� �������� ��� �������� ��� 
        if (_itemCode == ItemDropCode.Random)
            _itemCode = _dropCodes[Random.Range(0, _dropCodes.Length)];
    }
    public override void F_GetObject()
    {
        // _itemCode�� �ش��ϴ� �������� ȹ�� �õ�
        if(ItemManager.Instance.inventorySystem.F_GetItem((int)_itemCode))
        {
            // �κ��丮 ������Ʈ
            ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();

            // ������ ȹ�� UI ���
            F_GetObjectUI((int)_itemCode);

            // ������Ʈ ����
            Destroy(this.gameObject);
        }
    }

    //���� F_GetObject���� _itemCode�� �Ű������� ���� ��ϴ�!
    private void F_GetObjectUI(int v_itemCodeIndex)
    {
        // UI ���࿡ �ʿ��� ������
        Sprite v_getItemSpr = ResourceManager.Instance.F_GetInventorySprite(v_itemCodeIndex);
        string v_getItemName = ItemManager.Instance.ItemDatas[v_itemCodeIndex]._itemName;

        // UI ����
        ItemManager.Instance.dropItemSystem.F_GetObjectUI(v_getItemSpr, v_getItemName);
    }
}

