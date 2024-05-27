using UnityEngine;

public class DropItem : DropObject
{
    private int _randomItemCode_MIN;
    private int _randomItemCode_MAX;

    [SerializeField] private bool _isRandomItem;    // ���� �������� �������� ���� boolŸ�� ����
    [SerializeField] private int _itemCode;         // ȹ���� ������ �ڵ�

    private void Start()
    {
        _randomItemCode_MIN = 0;
        _randomItemCode_MAX = ItemManager.Instance.ItemDatas.Count;
    }
    public override void F_GetObject()
    {
        // ���� �������� ��� �������� ��� 
        if(_isRandomItem)
            _itemCode = Random.Range(_randomItemCode_MIN, _randomItemCode_MAX);

        // _itemCode�� �ش��ϴ� �������� ȹ�� �õ�
        if(ItemManager.Instance.inventorySystem.F_GetItem(_itemCode))
        {
            // �κ��丮 ������Ʈ
            ItemManager.Instance.inventorySystem.F_InventoryUIUpdate(); 

            // ������Ʈ ����
            Destroy(this.gameObject);
        }
    }
}

