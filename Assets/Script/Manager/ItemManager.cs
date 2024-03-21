using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public enum ItemType
{
    NONE,           // ����ó����
    STUFF,          // ��������
    FOOD,           // ����(�Һ�)������
    TOOL,           // ����������
    INSTALL         // ��ġ������
}
public class ItemManager : Singleton<ItemManager>
{
    string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

    [Header("Systems")]
    [SerializeField] private InventorySystem _inventorySystem;
    [SerializeField] private CraftSystem _craftSystem;
    public InventorySystem inventorySystem => _inventorySystem;
    public CraftSystem craftSystem => _craftSystem;

    [Header("Datas")]
    [SerializeField] private List<ItemData> _itemDatas;
    [SerializeField] private List<Recipe> _recipes;
    public List<ItemData> ItemDatas => _itemDatas;
    public List<Recipe> recipes => _recipes;
    protected override void InitManager() 
    { 
        F_InitItemDatas();
        F_initRecipeDatas();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {

            _inventorySystem.F_GetItem(0);
            _inventorySystem.F_GetItem(1);
            _inventorySystem.F_GetItem(20);
            _inventorySystem.F_GetItem(21);

            _inventorySystem.F_InventoryUIUpdate();
        }
    }

    #region ���� csv �Ľ�
    // ������ ������ ���̺�
    public void F_InitItemDatas()
    {
        _itemDatas = new List<ItemData>();

        TextAsset data = Resources.Load("ItemData") as TextAsset;           // ���� �ҷ�����
        var lines = Regex.Split(data.text, LINE_SPLIT_RE);                  // �� ������ �ڸ���
        var header = Regex.Split(lines[0], SPLIT_RE);                       // �ܾ�� �ڸ��� �� ��� ������ ��

        for(int i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);                   // �ܾ�� �ڸ���

            ItemData i_data = new ItemData();
            i_data.F_initData(values);
            _itemDatas.Add(i_data);
        }
    }

    // ������ ������ ���̺�
    public void F_initRecipeDatas()
    {
        TextAsset data = Resources.Load("RecipeData") as TextAsset;           // ���� �ҷ�����
        var lines = Regex.Split(data.text, LINE_SPLIT_RE);                  // �� ������ �ڸ���
        var header = Regex.Split(lines[0], SPLIT_RE);                       // �ܾ�� �ڸ��� �� ��� ������ ��

        for (int i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);

            Recipe r_data = new Recipe();
            if (r_data.F_InitRecipe(values))
                _recipes.Add(r_data);
        }
    }
    #endregion
}
