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
    [SerializeField] private InstallSystem _installSystem;
    public InventorySystem inventorySystem => _inventorySystem;
    public CraftSystem craftSystem => _craftSystem;
    public InstallSystem installSystem => _installSystem;

    [Header("Datas")]
    [SerializeField] private List<ItemData> _itemDatas;
    [SerializeField] private List<Recipe> _recipes;
    public List<ItemData> ItemDatas => _itemDatas;
    public List<Recipe> recipes => _recipes;

    [SerializeField] private int[] _itemCounter;
    public int[] itemCounter => _itemCounter;

    [Header("Storage")]
    [SerializeField] private Storage _selectedStorage;
    public Storage selectedStorage => _selectedStorage;

    protected override void InitManager() 
    { 
        F_InitItemDatas();
        F_initRecipeDatas();

        _itemCounter = new int[ItemDatas.Count];
    }
    private void Start()
    {
        // 1. ���� ������ �ִ� �������� �迭�� �����ϴ� �Լ��� UiManager�� delegate�� �߰�.
        UIManager.Instance.F_AddInventoryFunction(
            new UIManager.inventoryDelegate(F_UpdateItemCounter));
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {

            _inventorySystem.F_GetItem(0);
            _inventorySystem.F_GetItem(1);
            _inventorySystem.F_GetItem(20);
            _inventorySystem.F_GetItem(21);
            _inventorySystem.F_GetItem(24);
            _inventorySystem.F_GetItem(25);
            _inventorySystem.F_GetItem(26);

            _inventorySystem.F_InventoryUIUpdate();
        }
    }

    public void F_UpdateItemCounter()
    {
        // 1. ���� 0���� �ʱ�ȭ
        for (int i = 0; i < _itemCounter.Length; i++)
            _itemCounter[i] = 0;

        // 2. �κ��丮�� �������� ������ ����.
        for (int index = 0; index < _inventorySystem.inventory.Length; index++)
        {
            if (_inventorySystem.inventory[index] == null)
                continue;
            if (_inventorySystem.inventory[index].currentStack == 0)
                continue;

            int item = _inventorySystem.inventory[index].itemCode;
            int itemStack = _inventorySystem.inventory[index].currentStack;
            _itemCounter[item] += itemStack;
        }
    }

    public void F_SelectStorage(Storage v_storage)
    {
        _selectedStorage = v_storage;
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
