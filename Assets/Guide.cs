using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Guide : MonoBehaviour
{
    public List<Sprite> guideSprite;
    public Image guide;
    public Button guideNextButton;
    public TextMeshProUGUI guideNextButtonText;

    public int pageIndex;
    public int maxPageIndex;

    public void InitGuide()
    {
        pageIndex = 0;                                          // 가이드 현재 페이지
        maxPageIndex = guideSprite.Count - 1;                   // 가이드 최대 페이지

        guide.sprite = guideSprite[pageIndex];                  // 가이드 이미지 변경

        guideNextButtonText.text = "다음";                      // 버튼 텍스트
        guideNextButton.onClick.RemoveAllListeners();
        guideNextButton.onClick.AddListener(() => NextPage());  // 버튼 이벤트
    }

    private void NextPage()
    {
        pageIndex++;
        guide.sprite = guideSprite[pageIndex];

        // 현재 페이지가 마지막 페이지일때
        if(pageIndex == maxPageIndex)
        {
            guideNextButtonText.text = "닫기";

            guideNextButton.onClick.RemoveAllListeners();
            guideNextButton.onClick.AddListener(
                () => { gameObject.SetActive(false); });
        }
    }
}
