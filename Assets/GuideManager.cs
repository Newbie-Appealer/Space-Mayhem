using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideManager : MonoBehaviour
{
    public Guide guide;
    void Start()
    {
        // OnGuide();
    }

    public void OnGuide()
    {
        guide.gameObject.SetActive(true);   // ���̵� ON
        guide.InitGuide();                  // ���̵� �ʱ�ȭ
    }
}
