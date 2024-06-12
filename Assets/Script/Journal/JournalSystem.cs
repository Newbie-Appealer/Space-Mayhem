using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class JournalSystem : MonoBehaviour
{
    string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

    private Dictionary<string, string> _journalData;            // ���� �� ���� ������( key, value )

    [Header("Transform")]
    [SerializeField] private Transform  _journalslot_Parent;    // ���� slot parent

    [Header("Prefab")]
    [SerializeField] private GameObject _journalPrefab;         // ���� ������ ( UI )

    [Header("Player Survival Time")]
    [SerializeField] private int             _surDay;    // �����ϼ�
    [SerializeField] private int             _surTime;   // �����ð� ( 1800 -> �Ϸ� )
    [SerializeField] private List<string>    _myKeys;    // �ÿ��̾ ���� ���� ( ���� ������� )
    HashSet<string> _myJournalUniquKeys;                 // �÷��̾��� ������ �߰��� Key HashSet

    public int surDay { get=> _surDay; set => _surDay = value; }
    public int surTime { get => _surTime; set => _surTime = value; }
    public List<string> myKeys { get => _myKeys; set => _myKeys = value; }
    private void Start()
    {
        F_initJournalSystem();
    }

    public void F_initJournalSystem()
    {
        // 1. ���� ������ ���̺� �Ľ�
        F_InitJournalData();

        // �ߺ� ���� hashSet
        _myJournalUniquKeys = new HashSet<string>();

        // Key�� ��������� ����ó��
        if(myKeys == null)
            myKeys = new List<string>();

        for(int i = 0; i < _myKeys.Count; i++)
        {
            F_GetJournal(_myKeys[i]);
        }

        StartCoroutine(C_SurvivalTime());
    }

    /// <summary> ���� Ű�� �������ִ��� Ȯ�� </summary>
    public bool F_CheckKey(string v_key)
    {
        if(_myJournalUniquKeys.Contains(v_key))
            return true;
        return false;
    }

    #region Get Journal
    /// <summary>
    /// Key���� �ش��ϴ� ������ �߰��ϰ�, �������θ� ��ȯ��.
    /// </summary>
    public bool F_GetJournal(string v_key)
    {
        // ���� ���� ( �ߺ� Ű ���� )
        if (_myJournalUniquKeys.Add(v_key))
        {
            string journalTEXT;
            // v_key�� �ش��ϴ� Journal �޾ƿ��� 
            if (F_GetJournalData(v_key, out journalTEXT))
            {
                // 1. journalPrefab ���� �� �ʱ�ȭ
                JournalSlot journal = Instantiate(_journalPrefab, _journalslot_Parent).GetComponent<JournalSlot>();
                journal.F_InitSlot(_surDay, journalTEXT);
                // ���� �߰� ����
                return true;
            }

            // ���� �߰� ���� ( Key���� �ش��ϴ� �����Ͱ����� )
            _myJournalUniquKeys.Remove(v_key);
            return false;
        }
        // ���� ���� ( �ߺ� )
        return false;
    }

    /// <summary>
    /// Key���� �ش��ϴ� �����͸� ����������
    /// </summary>
    private bool F_GetJournalData(string v_key, out string v_journaldata)
    {
        if (_journalData.TryGetValue(v_key, out v_journaldata))
            return true;

        Debug.LogError("Key [" + v_key + "] is not found");
        return false;
    }
    #endregion

    #region Data Table
    private void F_InitJournalData()
    {
        _journalData = new Dictionary<string, string>();

        TextAsset data = Resources.Load("JournalData") as TextAsset;    // ���� �ҷ�����
        var lines = Regex.Split(data.text, LINE_SPLIT_RE);          // �� ������ �ڸ���

        for (int i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);

            _journalData.Add(values[0], values[1]);
        }
    }
    #endregion

    IEnumerator C_SurvivalTime()
    {
        while(true)
        {
            // 1�� ���
            yield return new WaitForSeconds(1f);

            // ���� �����϶� �ð��߰� X
            if (PlayerManager.Instance.PlayerController._isPlayerDead)
                continue;

            // �ð� ����
            _surTime++;

            // 1800�� ���� ������ +1
            if(_surTime >= 1800)
            {
                _surTime = 0;
                _surDay++;
            }
        }
    }
}