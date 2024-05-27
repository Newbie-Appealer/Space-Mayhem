using UnityEngine;

public class DropItem : DropObject
{
    private int _randomItemCode_MIN;
    private int _randomItemCode_MAX;

    [SerializeField] private bool _isRandomItem;    // 랜덤 아이템을 얻을지에 대한 bool타입 변수
    [SerializeField] private int _itemCode;         // 획득할 아이템 코드

    private void Start()
    {
        _randomItemCode_MIN = 0;
        _randomItemCode_MAX = ItemManager.Instance.ItemDatas.Count;

        // 랜덤 아이템을 얻는 아이템일 경우 
        if (_isRandomItem)
            _itemCode = Random.Range(_randomItemCode_MIN, _randomItemCode_MAX);
    }
    public override void F_GetObject()
    {
        // _itemCode에 해당하는 아이템을 획득 시도
        if(ItemManager.Instance.inventorySystem.F_GetItem(_itemCode))
        {
            // 인벤토리 업데이트
            ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();

            // 아이템 획득 UI 출력
            F_GetObjectUI(_itemCode);

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

