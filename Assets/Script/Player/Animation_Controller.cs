using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Animation_Controller : MonoBehaviour
{
    [SerializeField]
    private Animator Anim_Arms;
    [SerializeField]
    private Animator Anim_Pistol;

    [SerializeField]
    private Button[] btns;
    private void Start()
    {
        for (int i = 0; i < this.btns.Length; i++)
        {
            int index = i;
            btns[index].onClick.AddListener(() => this.TaskOnClick(btns[index].name));
        }
    }

    private void TaskOnClick(string btn_name)
    {
        if (btn_name == "isGround")
        {
            Anim_Arms.SetBool("isGround", true);
        }
        if (btn_name == "isGroundOff")
        {
            Anim_Arms.SetBool("isGround", false);
        }
        if (btn_name == "Walk")
        {
            Anim_Arms.SetBool("Walk", true);
        }
        if (btn_name == "WalkOff")
        {
            Anim_Arms.SetBool("Walk", false);
        }
        if (btn_name == "Jump")
        {
            Anim_Arms.SetTrigger("Jump");
        }
        if (btn_name == "Fire")
        {
            Anim_Arms.SetTrigger("Fire");
            Anim_Pistol.SetTrigger("Fire");
        }
        if (btn_name == "Reach")
        {
            Anim_Arms.SetBool("Reach",true);
            Anim_Pistol.SetBool("Reach",true);
        }
        if (btn_name == "ReachOff")
        {
            Anim_Arms.SetBool("Reach", false);
            Anim_Pistol.SetBool("Reach", false);
        }
        if (btn_name == "Get")
        {
            Anim_Arms.SetBool("Get", true);
            Anim_Pistol.SetBool("Get", true);
        }
        if (btn_name == "GetOff")
        {
            Anim_Arms.SetBool("Get", false);
            Anim_Pistol.SetBool("Get", false);
        }
    }
}
