using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));

                if (_instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    _instance = obj.GetComponent<T>();
                }
            }
            return _instance;
        }
    }

    public void Awake()
    {
        if (transform.parent != null && transform.root != null)
        {
            DontDestroyOnLoad(transform.parent);
        }

        DontDestroyOnLoad(gameObject);
        InitManager();
    }

    protected abstract void InitManager();  // 싱글톤 적용한 스크립트의 초기화 함수
}
