using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    [Header("=== Player Camera ===")]
    [SerializeField] private Camera _player_mainCamera;

    [Header("=== Spear ===")]
    [SerializeField] private Spear _spear;
    [SerializeField] private float _spearFireSpeed;
    [SerializeField] private Rigidbody _spear_rb;
    private Vector3 _spear_Firepos = new Vector3(0, 0.14f, 0.63f);
    private Quaternion _spear_FireRotate;
    private float _spear_Distance = 0.3f;

    private Animator _pistol_Animation;

    private void Start()
    {
        _pistol_Animation = GetComponent<Animator>();
        _spear_FireRotate = _spear.transform.localRotation;
        F_InitSpear();
    }
    public void F_SpearPowerCharge()
    {
        UIManager.Instance.F_GetPlayerFireGauge().fillAmount = _spearFireSpeed / 14f;
        _spearFireSpeed += Time.deltaTime * 10f;
        if (_spearFireSpeed > 14f )
        {
            _spearFireSpeed = 14f;
        }
    }
    public void F_SpearFire()
    {
        _spear.F_EnableLine();
        StartCoroutine(C_DrawLine());
        _spear_rb.isKinematic = false;
        _spear.transform.parent = null;
        _spear_rb.AddForce(_player_mainCamera.transform.forward * _spearFireSpeed, ForceMode.Impulse);
        _pistol_Animation.SetBool("Reach", true);
    }
    public void F_SpearComeBack()
    {
        _spear_rb.isKinematic = true;
        _spear.transform.parent = this.transform;
        _spear.transform.localPosition = Vector3.Lerp(_spear.transform.localPosition, _spear_Firepos, _spearFireSpeed * Time.deltaTime / 2f);
        _spear.transform.localRotation = _spear_FireRotate;
        if (Vector3.Distance(_spear.transform.localPosition, _spear_Firepos) < _spear_Distance )
        {
            for (int l = 0; l< ScrapManager.Instance._scrapHitedSpear.Count; l++)
            {
                int v_scrapNum = ScrapManager.Instance._scrapHitedSpear[l].scrapNumber;
                string v_scrapName = ItemManager.Instance.ItemDatas[v_scrapNum]._itemName;

                //아이템 획득
                ScrapManager.Instance._scrapHitedSpear[l].GetComponent<Scrap>().F_GetScrap();
                StartCoroutine(UIManager.Instance.C_GetItemUIOn(ResourceManager.Instance.F_GetInventorySprite(v_scrapNum), v_scrapName));
            }

            _pistol_Animation.SetBool("Reach", false);
            _pistol_Animation.SetTrigger("Get");
            F_InitSpear();
            StopAllCoroutines();
            _spear.F_DisableLine();
        }
    }

    private IEnumerator C_DrawLine()
    {
        while(PlayerManager.Instance._isSpearFire)
        {
            _spear.F_RestoreLine();
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public void F_InitSpear()
    {
        PlayerManager.Instance._isSpearFire = false;

        //작살에 맞은 재료가 있다면 풀링으로 return
        if (ScrapManager.Instance._scrapHitedSpear.Count > 0)
        {
            foreach (Scrap v_scrap in ScrapManager.Instance._scrapHitedSpear)
            {
                ScrapManager.Instance.F_ReturnScrap(v_scrap);
            }
        }
        //풀링으로 return 후 작살에 맞은 재료 List 초기화
        ScrapManager.Instance._scrapHitedSpear.Clear();
        UIManager.Instance.F_GetPlayerFireGauge().fillAmount = 1f;

        //움직임 초기화
        if (!_spear_rb.isKinematic)
            _spear_rb.velocity = Vector3.zero;
        _spearFireSpeed = 0f;

        //위치 및 각도 초기화
        _spear.transform.parent = this.transform;
        _spear.transform.localPosition = _spear_Firepos;
        _spear.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        _spear.F_DisableLine();
    }
}
