using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public enum ItemType
{
    NONE,           // 예외처리용
    STUFF,          // 재료아이템
    FOOD,           // 음식(소비)아이템
    TOOL,           // 도구아이템
    INSTALL         // 설치아이템
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
        // 1. 현재 가지고 있는 아이템을 배열로 정리하는 함수를 UiManager의 delegate에 추가.
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
        // 1. 값을 0으로 초기화
        for (int i = 0; i < _itemCounter.Length; i++)
            _itemCounter[i] = 0;

        // 2. 인벤토리내 아이템의 개수를 정리.
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
    #region 엑셀 csv 파싱
    // 아이템 데이터 테이블
    public void F_InitItemDatas()
    {
        _itemDatas = new List<ItemData>();

        TextAsset data = Resources.Load("ItemData") as TextAsset;           // 파일 불러오기
        var lines = Regex.Split(data.text, LINE_SPLIT_RE);                  // 줄 단위로 자르기
        var header = Regex.Split(lines[0], SPLIT_RE);                       // 단어로 자르기 및 상단 데이터 명

        for(int i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);                   // 단어로 자르기

            ItemData i_data = new ItemData();
            i_data.F_initData(values);
            _itemDatas.Add(i_data);
        }
    }

    // 레시피 데이터 테이블
    public void F_initRecipeDatas()
    {
        TextAsset data = Resources.Load("RecipeData") as TextAsset;           // 파일 불러오기
        var lines = Regex.Split(data.text, LINE_SPLIT_RE);                  // 줄 단위로 자르기
        var header = Regex.Split(lines[0], SPLIT_RE);                       // 단어로 자르기 및 상단 데이터 명

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
