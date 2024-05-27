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
    }
    public override void F_GetObject()
    {
        // 랜덤 아이템을 얻는 아이템일 경우 
        if(_isRandomItem)
            _itemCode = Random.Range(_randomItemCode_MIN, _randomItemCode_MAX);

        // _itemCode에 해당하는 아이템을 획득 시도
        if(ItemManager.Instance.inventorySystem.F_GetItem(_itemCode))
        {
            // 인벤토리 업데이트
            ItemManager.Instance.inventorySystem.F_InventoryUIUpdate(); 

            // 오브젝트 삭제
            Destroy(this.gameObject);
        }
    }
}

