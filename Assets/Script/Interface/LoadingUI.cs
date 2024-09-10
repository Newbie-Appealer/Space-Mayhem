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
    "[TIP] ��ٸ� Ÿ��, ���� ������ [w]�� ������, �Ʒ��� ������ [s]�� ��������." ,
    "[TIP] �޸����� [shift]�� ������, ��ũ���� �ʹٸ� [c]�� ������, �����Ϸ��� [space]�� ��������.",
    "[TIP] [E]�� ���� ��ȣ�ۿ��ϼ���..",
    "[TIP] ESC�� ���� ���콺 ������ ���带 ������ �� �ֽ��ϴ�.",
    "[TIP] ��ũ���� �������� ���̳� �Ĺֵ����� ����ϼ���.",
    "[TIP] ����κ��� ������ ��ȣ�Ϸ��� ��ž�� �Ǽ��ϼ���.",
    "[TIP] �Ϻ� �������� �༺���������� ���� �� �ֽ��ϴ�.",
    "[TIP] �Ϻ� �������� �����Ǵ� �༺�������� ���� �� �ֽ��ϴ�."};

    private void OnEnable()
    {
        //�ε� ���� �ʱ�ȭ
        _loading_Text.text = "�ε���";
        F_SetLoadingUI();
        StartCoroutine(C_LoadingStart());
    }

    private IEnumerator C_LoadingStart()
    {
        while (gameObject.activeSelf)
        {
            _loading_Text.text += ".";
            if (_loading_Text.text == "�ε���....")
                _loading_Text.text = "�ε���";

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void F_SetLoadingUI()
    {
        //����� ����
        int _randomImg = Random.Range(0, _thumbnail_Image.Length);
        _thumbnail_Obj.sprite = _thumbnail_Image[_randomImg];
        if (_randomImg != _thumbnail_Image.Length-1)
            _mapName_Text.text = _thumbnail_Obj.sprite.name + " [Sample] ";
        else
            _mapName_Text.text = "main theme";

        //TIP �ؽ�Ʈ ����
        _tip_Text.text = _tip_Array[Random.Range(0, _tip_Array.Length)];
    }
}
