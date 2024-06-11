using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class JournalSystem : MonoBehaviour
{
    string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

    private Dictionary<string, string> _journalData;            // ���� �� ���� ������( key, value )

    [Header("UI")]
    [SerializeField] private GameObject _journalUI_root;        // ���� ON/OFF
    [SerializeField] private Transform  _journalslot_Parent;    // ���� ON/OFF

    [Header("Prefab")]
    [SerializeField] private GameObject _journalPrefab;         // ���� ������ ( UI )

    HashSet<string> _myJournalUniquKeys;                        // �÷��̾��� ������ �߰��� Key HashSet

    // �÷��̾ �����ؾ��� ������
    [Header("Player Survival Time")]
    [SerializeField] private int             _surDay;    // �����ϼ�
    [SerializeField] private int             _surTime;   // �����ð� ( 1800 -> �Ϸ� )
    [SerializeField] private List<string>    _myKeys;    // �ÿ��̾ ���� ���� ( ���� ������� )


    public int surDay { get=> _surDay; set => _surDay = value; }
    public int surTime { get => _surTime; set => _surTime = value; }

    private void Start()
    {
        F_initJournalSystem();
    }

    public void F_initJournalSystem()
    {
        // 1. ���� ������ ���̺� �Ľ�
        F_InitJournalData();

        _myJournalUniquKeys = new HashSet<string>();    // ���� ������ �߰��� 
        _myKeys = new List<string>();                   // ����/�ҷ����� �ؾ��ϴ°�

        for(int i = 0; i < _myKeys.Count; i++)
        {
            if(F_GetJournal(_myKeys[i]))
            {
                Debug.Log(_myKeys[i] + " - Successfully added journal");
            }
            else
            {
                Debug.Log("Failed to add journal");
            }
        }

        StartCoroutine(C_SurvivalTime());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if(F_GetJournal("1"))
            {
                Debug.Log("�߰� Ű�� : 1");
            }
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (F_GetJournal("2"))
            {
                Debug.Log("�߰� Ű�� : 2");
            }
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (F_GetJournal("Dead"))
            {
                Debug.Log("�߰� Ű�� : Dead");
            }
        }
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

                _myKeys.Add(v_key);
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