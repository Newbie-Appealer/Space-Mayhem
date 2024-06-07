using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalSystem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject     _journalUI_root;        // 일지 ON/OFF
    [SerializeField] private GameObject[]   _journalUI_Category;    // 일지 카테고리 ( 일반 / 특수 등등 )

    [Header("Prefab")]
    [SerializeField] private GameObject     _journalPrefab;         // 일지 프리팹 ( UI )

    private void Start()
    {
        // GameManager의 storyStep 활용

        // 스토리와 관계없는 일지에 대해서는 좀 고민을 해봐야겠군
    }
}
