using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [Header("=== ABOUT IMAGE ===")]
    [SerializeField] private Sprite[] _thumbnail_Image;
    [SerializeField] private Image _thumbnail_Obj;

    [Header("=== ABOUT TEXT ===")]
    [SerializeField] private TextMeshProUGUI _loading_Text;
    [SerializeField] private TextMeshProUGUI _mapName_Text;
    [SerializeField] private TextMeshProUGUI _tip_Text;
    private string[] _tip_Array = new string[] {
    "[TIP] 사다리 타기, 위로 가려면 [w]를 누르고, 아래로 가려면 [s]를 누르세요." ,
    "[TIP] 달리려면 [shift]를 누르고, 웅크리고 싶다면 [c]를 누르고, 점프하려면 [space]를 누르세요.",
    "[TIP] [E]를 눌러 상호작용하세요..",
    "[TIP] ESC를 눌러 마우스 감도와 사운드를 변경할 수 있습니다.",
    "[TIP] 스크랩을 얻으려면 손이나 파밍도구를 사용하세요.",
    "[TIP] 운석으로부터 기지를 보호하려면 포탑을 건설하세요.",
    "[TIP] 일부 아이템은 행성던전에서만 얻을 수 있습니다.",
    "[TIP] 일부 아이템의 레시피는 행성던전에서 얻을 수 있습니다."};

    private void OnEnable()
    {
        //로딩 문구 초기화
        _loading_Text.text = "로딩중";
        F_SetLoadingUI();
        StartCoroutine(C_LoadingStart());
    }

    private IEnumerator C_LoadingStart()
    {
        while (gameObject.activeSelf)
        {
            _loading_Text.text += ".";
            if (_loading_Text.text == "로딩중....")
                _loading_Text.text = "로딩중";

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void F_SetLoadingUI()
    {
        //썸네일 설정
        int _randomImg = Random.Range(0, _thumbnail_Image.Length);
        _thumbnail_Obj.sprite = _thumbnail_Image[_randomImg];
        if (_randomImg != _thumbnail_Image.Length-1)
            _mapName_Text.text = _thumbnail_Obj.sprite.name + " [Sample] ";
        else
            _mapName_Text.text = "main theme";

        //TIP 텍스트 설정
        _tip_Text.text = _tip_Array[Random.Range(0, _tip_Array.Length)];
    }
}
