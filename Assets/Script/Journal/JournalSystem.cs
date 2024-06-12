using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class JournalSystem : MonoBehaviour
{
    string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

    private Dictionary<string, string> _journalData;            // 게임 내 일지 데이터( key, value )

    [Header("Transform")]
    [SerializeField] private Transform  _journalslot_Parent;    // 일지 slot parent

    [Header("Prefab")]
    [SerializeField] private GameObject _journalPrefab;         // 일지 프리팹 ( UI )

    [Header("Player Survival Time")]
    [SerializeField] private int             _surDay;    // 생존일수
    [SerializeField] private int             _surTime;   // 생존시간 ( 1800 -> 하루 )
    [SerializeField] private List<string>    _myKeys;    // 플에이어가 얻은 일지 ( 얻은 순서대로 )
    HashSet<string> _myJournalUniquKeys;                 // 플레이어의 일지에 추가된 Key HashSet

    public int surDay { get=> _surDay; set => _surDay = value; }
    public int surTime { get => _surTime; set => _surTime = value; }
    public List<string> myKeys { get => _myKeys; set => _myKeys = value; }
    private void Start()
    {
        F_initJournalSystem();
    }

    public void F_initJournalSystem()
    {
        // 1. 일지 데이터 테이블 파싱
        F_InitJournalData();

        // 중복 방지 hashSet
        _myJournalUniquKeys = new HashSet<string>();

        // Key가 비어있을때 예외처리
        if(myKeys == null)
            myKeys = new List<string>();

        for(int i = 0; i < _myKeys.Count; i++)
        {
            F_GetJournal(_myKeys[i]);
        }

        StartCoroutine(C_SurvivalTime());
    }

    /// <summary> 일지 키를 가지고있는지 확인 </summary>
    public bool F_CheckKey(string v_key)
    {
        if(_myJournalUniquKeys.Contains(v_key))
            return true;
        return false;
    }

    #region Get Journal
    /// <summary>
    /// Key값에 해당하는 일지를 추가하고, 성공여부를 반환함.
    /// </summary>
    public bool F_GetJournal(string v_key)
    {
        // 삽입 성공 ( 중복 키 방지 )
        if (_myJournalUniquKeys.Add(v_key))
        {
            string journalTEXT;
            // v_key에 해당하는 Journal 받아오기 
            if (F_GetJournalData(v_key, out journalTEXT))
            {
                // 1. journalPrefab 생성 및 초기화
                JournalSlot journal = Instantiate(_journalPrefab, _journalslot_Parent).GetComponent<JournalSlot>();
                journal.F_InitSlot(_surDay, journalTEXT);
                // 일지 추가 성공
                return true;
            }

            // 일지 추가 실패 ( Key값에 해당하는 데이터가없음 )
            _myJournalUniquKeys.Remove(v_key);
            return false;
        }
        // 삽입 실패 ( 중복 )
        return false;
    }

    /// <summary>
    /// Key값에 해당하는 데이터를 얻을수있음
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

        TextAsset data = Resources.Load("JournalData") as TextAsset;    // 파일 불러오기
        var lines = Regex.Split(data.text, LINE_SPLIT_RE);          // 줄 단위로 자르기

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
            // 1초 대기
            yield return new WaitForSeconds(1f);

            // 죽은 상태일때 시간추가 X
            if (PlayerManager.Instance.PlayerController._isPlayerDead)
                continue;

            // 시간 증가
            _surTime++;

            // 1800초 이후 생존일 +1
            if(_surTime >= 1800)
            {
                _surTime = 0;
                _surDay++;
            }
        }
    }
}