using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalSystem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject     _journalUI_root;        // ���� ON/OFF
    [SerializeField] private GameObject[]   _journalUI_Category;    // ���� ī�װ� ( �Ϲ� / Ư�� ��� )

    [Header("Prefab")]
    [SerializeField] private GameObject     _journalPrefab;         // ���� ������ ( UI )

    private void Start()
    {
        // GameManager�� storyStep Ȱ��

        // ���丮�� ������� ������ ���ؼ��� �� ����� �غ��߰ڱ�
    }
}
