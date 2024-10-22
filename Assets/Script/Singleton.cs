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
        // 더이상 필요하지않은 DontDestoryOnLoad
        // 로그인해서 데이터를 가져올때는 필요했으나
        // 현재는 로그인기능없이 로컬로 데이터를 저장/불러오기 하기때문에 
        // 필요없어짐.

        //if (transform.parent != null && transform.root != null)
        //{
        //    DontDestroyOnLoad(transform.parent);
        //}

        //DontDestroyOnLoad(gameObject);
        InitManager();
    }

    protected abstract void InitManager();  // 싱글톤 적용한 스크립트의 초기화 함수
}
