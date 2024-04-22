using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

    [Header("Sprite")]
    [SerializeField] private Sprite _emptySlotSprite;
    [SerializeField] private List<Sprite> _inventorySprites;
    public Sprite emptySlotSprite => _emptySlotSprite;

    [Header("install Item Intercation TEXT")]
    [SerializeField] private Dictionary<string, string> _intercationTEXT_install;

    protected override void InitManager() 
    {
        F_InitIntercationTEXTData();
    }

    #region Sprite
    public Sprite F_GetInventorySprite(int v_code)
    {
        return _inventorySprites[v_code];
    }
    #endregion

    #region TEXT
    private void F_InitIntercationTEXTData()
    {
        _intercationTEXT_install = new Dictionary<string, string>();

        TextAsset data = Resources.Load("IntercationTEXT") as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        var header = Regex.Split(lines[0], SPLIT_RE);

        for(int i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);

            string objectName = values[0];
            string eng = values[1];
            string kor = values[2];

            _intercationTEXT_install.Add(objectName, eng);
        }
    }

    public string F_GetIntercationTEXT(string v_objectName)
    {
        return _intercationTEXT_install[v_objectName];
    }
    #endregion
}
