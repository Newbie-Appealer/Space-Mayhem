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
        guide.gameObject.SetActive(true);   // 가이드 ON
        guide.InitGuide();                  // 가이드 초기화
    }
}
