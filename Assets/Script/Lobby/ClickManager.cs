using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClickManager : MonoBehaviour
{
    [SerializeField] private GameObject _optionPopUp;
    private bool _OnPopUp = true;
    public Button _quitBtn;

    private void Awake()
    {
        _quitBtn.onClick.AddListener(F_QuitOptionPopUp);
    }
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0) && _OnPopUp)
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.name == "NewGame")
                {
                    SceneManager.LoadScene("01_Main");
                }
                if (hit.transform.gameObject.name == "Continue")
                {
                    //저장 기능 완성 후 작업
                }
                if (hit.transform.gameObject.name == "Option")
                {
                    if (!_optionPopUp.activeSelf)
                    {
                        _optionPopUp.SetActive(true);
                        _OnPopUp = false;
                    }
                }
                if (hit.transform.gameObject.name == "Exit")
                {
                    Application.Quit();
                }
            }
        }
    }

    public void F_QuitOptionPopUp()
    {
        _OnPopUp = true;
        _optionPopUp.SetActive(false);
    }

    
}
