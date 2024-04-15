using System.Collections;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    [Header("=== Player Camera ===")]
    [SerializeField] private Camera _player_mainCamera;

    [Header("=== Spear ===")]
    [SerializeField] private Spear _spear;
    public Spear spear => _spear;
    private Animator _pistol_Animation;
    public Animator pistolAni => _pistol_Animation;
    [SerializeField] private Rigidbody _spear_rb;
    [SerializeField] private float _spearFireSpeed;
    private float _spear_Distance = 1f;
    private Vector3 _spear_Firepos = new Vector3(0, 0.14f, 0.63f);
    private IEnumerator _draw_LIne_Coroutine;

    private void Start()
    {
        _pistol_Animation = GetComponent<Animator>();
        _draw_LIne_Coroutine = C_DrawLine();
    }

    private void OnEnable()
    {
        F_InitSpear();
    }
    public void F_SpearPowerCharge()
    {
        _pistol_Animation.SetBool("Get", false);
        UIManager.Instance.F_GetPlayerFireGauge().color = Color.white;
        _spearFireSpeed += Time.deltaTime * 20f;
        UIManager.Instance.F_GetPlayerFireGauge().fillAmount = _spearFireSpeed / 20f;
        if (_spearFireSpeed > 20f )
               _spearFireSpeed = 20f;
    }
    public void F_SpearFire()
    {
        _spear.F_EnableLine();
        StartCoroutine(C_DrawLine());
        _spear.transform.parent = null;
        _spear_rb.isKinematic = false;
        _spear_rb.AddForce(_player_mainCamera.transform.forward * _spearFireSpeed, ForceMode.Impulse);
        _spear.transform.Rotate(-14f, -6f, 0f);
        _pistol_Animation.SetBool("Reach", true);

    }
    public void F_SpearComeBack()
    {
        if (_spear.transform.parent != this.transform)
        {
            _spear_rb.isKinematic = true;
            Vector3 _pistol_Muzzle = _spear.F_GetFirePos();
            _spear.transform.position = Vector3.Lerp(_spear.transform.position, _pistol_Muzzle, _spearFireSpeed * Time.deltaTime / 4f);
            if (Vector3.Distance(_spear.transform.position, _pistol_Muzzle) < _spear_Distance )
            {
                F_GetScrapFromSpear();
                _pistol_Animation.SetBool("Reach", false);
                _pistol_Animation.SetTrigger("Get");
                F_InitSpear();
                StartCoroutine(UIManager.Instance.C_FireGaugeFadeOut());
                StopCoroutine(_draw_LIne_Coroutine);
                _spear.F_DisableLine();

                int idx = ItemManager.Instance.inventorySystem.selectQuickSlotNumber;               // 선택된 슬롯 ( 도구 )
                (ItemManager.Instance.inventorySystem.inventory[idx] as Tool).F_CheckDurability();    // 도구 내구도 사용
            }
        }
        else
        {
            F_GetScrapFromSpear();
            F_InitSpear();
        }
    }

    private void F_GetScrapFromSpear()
    {
        for (int l = 0; l < ScrapManager.Instance._scrapHitedSpear.Count; l++)
        {
            int v_scrapNum = ScrapManager.Instance._scrapHitedSpear[l].scrapNumber;
            string v_scrapName = ItemManager.Instance.ItemDatas[v_scrapNum]._itemName;
            StartCoroutine(UIManager.Instance.C_GetItemUIOn(ResourceManager.Instance.F_GetInventorySprite(v_scrapNum), v_scrapName));
            ScrapManager.Instance._scrapHitedSpear[l].GetComponent<Scrap>().F_GetScrap();
        }
        ScrapManager.Instance._scrapHitedSpear.Clear();
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

        //움직임 초기화
        if (!_spear_rb.isKinematic)
        {
            _spear_rb.velocity = Vector3.zero;
            _spear_rb.isKinematic = true;
        }
        _spearFireSpeed = 0f;

        //위치 및 각도 초기화
        _spear.transform.parent = this.transform;
        _spear.transform.localPosition = _spear_Firepos;
        _spear.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        _spear.F_DisableLine();
    }
}
