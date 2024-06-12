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
    [SerializeField] private List<string>    _myKeys;    // 플레이어가 얻은 일지 ( 얻은 순서대로 )
    [SerializeField] private List<int>       _myKeydays; // 플레이어가 얻은 일지의 날짜 ( 얻은 순서대로 )
    HashSet<string> _myJournalUniquKeys;                 // 플레이어의 일지에 추가된 Key HashSet

    public int surDay { get=> _surDay; set => _surDay = value; }
    public int surTime { get => _surTime; set => _surTime = value; }
    public List<string> myKeys { get => _myKeys; set => _myKeys = value; }
    public List<int> myKeydays { get => _myKeydays; set => _myKeydays = value; }
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
        if(_myKeys == null)
            _myKeys = new List<string>();
        if(_myKeydays == null)
            _myKeydays = new List<int>();

        for (int i = 0; i < _myKeys.Count; i++)
            F_GetJournal(_myKeys[i], false, _myKeydays[i]);


        F_Getjournal_SurvivalTime(0);

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
    public bool F_GetJournal(string v_key, bool v_init = true, int v_surDay = 0)
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

                // v_init가 True일때 -> 신규 일지 등록 ( 기본값, 매개변수 v_key만 넣어야함 )
                //   - myKeys, myKeydays에 값 추가
                //   - 생존일자는 현재 생존일 ( v_surDay를 현재 생존일로 초기화 )
                // v_init이 False일때 -> 기존 일지 등록 ( 불러오기, 매개변수 3개 다넣어야함 )
                //   - myKeys, myKeydays에 값 추가 X
                //   - 생존일자는 매개변수로 받은 생존일
                if (v_init)
                {
                    v_surDay = _surDay;
                    _myKeys.Add(v_key);
                    _myKeydays.Add(v_surDay);
                }

                journal.F_InitSlot(v_surDay, journalTEXT);
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

            // 900초(15분) 이후 생존일 +1
            if(_surTime >= 900)
            {
                _surTime = 0;
                _surDay++;
                
                //하루 지날 때마다 Scrap 나오는 방향 변경
                ScrapManager.Instance.F_ScrapMoveChange();

                F_Getjournal_SurvivalTime(_surDay); // 생존시간에 따른 일지 추가
            }
        }
    }


    private void F_Getjournal_SurvivalTime(int v_survivalTime)
    {
        //100, 101, 102, 103, 104, 105, 106, 107, 110, 115, 130, 200
        int survivalTimeKey = v_survivalTime + 100;
        switch (survivalTimeKey)
        {
            case 100: case 101: case 102: case 103: case 104: case 105:
            case 106: case 107: case 110: case 115: case 130: case 200:
                if(F_GetJournal(survivalTimeKey.ToString()))
                    UIManager.Instance.F_PlayerMessagePopupTEXT("got the journal.Press 'B' to check your journals", 2f);
                break;
        }
    }

    public void F_GetJournal_DungeonExit(int v_ExitCount)
    {
        // 1,2,3,4 번째 탈출 
        switch (v_ExitCount)
        {
            case 1: case 2: case 3: case 4:
                if(F_GetJournal(v_ExitCount.ToString()))
                    UIManager.Instance.F_PlayerMessagePopupTEXT("got the journal.Press 'B' to check your journals", 2f);
                break;
        }
    }
}
