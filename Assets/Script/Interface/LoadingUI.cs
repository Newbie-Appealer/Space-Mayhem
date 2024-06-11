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
    "[TIP] Riding Ladder, Press [w] to go up, [s] to go down." ,
    "[TIP] Press [shift] to Run, [c] to Crouch, [space] to Jump.",
    "[TIP] Press [E] to interaction somthing in front of you.",
    "[TIP] In Option, you can change mouse sensitivity and sounds.",
    "[TIP] To get scrap, use your hand or farming gun.",
    "[TIP] To guard your station from meteor, you can build meteor Tower.",
    "[TIP] Some Items only can get in planet Dungeon."};

    private void OnEnable()
    {
        //로딩 문구 초기화
        _loading_Text.text = "Loading";
        F_SetLoadingUI();
        StartCoroutine(C_LoadingStart());
    }

    private IEnumerator C_LoadingStart()
    {
        while (gameObject.activeSelf)
        {
            _loading_Text.text += ".";
            if (_loading_Text.text == "Loading....")
                _loading_Text.text = "Loading";

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
