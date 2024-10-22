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
        pageIndex = 0;                                          // ���̵� ���� ������
        maxPageIndex = guideSprite.Count - 1;                   // ���̵� �ִ� ������

        guide.sprite = guideSprite[pageIndex];                  // ���̵� �̹��� ����

        guideNextButtonText.text = "����";                      // ��ư �ؽ�Ʈ
        guideNextButton.onClick.RemoveAllListeners();
        guideNextButton.onClick.AddListener(() => NextPage());  // ��ư �̺�Ʈ
    }

    private void NextPage()
    {
        pageIndex++;
        guide.sprite = guideSprite[pageIndex];

        // ���� �������� ������ �������϶�
        if(pageIndex == maxPageIndex)
        {
            guideNextButtonText.text = "�ݱ�";

            guideNextButton.onClick.RemoveAllListeners();
            guideNextButton.onClick.AddListener(
                () => { gameObject.SetActive(false); });
        }
    }
}
