using System.Collections;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    [Header("=== Player Camera ===")]
    [SerializeField] private Camera _player_mainCamera;

    [Header("=== Spear ===")]
    [SerializeField] private Spear _spear;
    public Spear spear => _spear;
    [SerializeField] private Rigidbody _spear_rb;
    [SerializeField] private float _spearFireSpeed;
    private float _spear_Distance = 1f;
    private Vector3 _spear_Firepos = new Vector3(0, 0.14f, 0.63f);


    [Header("=== ABOUT ANIMATION ===")]
    private Animator _pistol_Animation;
    public Animator pistolAni => _pistol_Animation;

    private IEnumerator _draw_Line_Coroutine;

    private void Start()
    {
        _pistol_Animation = GetComponent<Animator>();
        _draw_Line_Coroutine = C_DrawLine();
    }

    private void OnEnable()
    {
        F_InitSpear();
        if (PlayerManager.Instance.playerState == PlayerState.FARMING)
            Invoke("F_InitCanShootPistol", 0.5f);
    }

    private void F_InitCanShootPistol()
    {
        PlayerManager.Instance._canShootPistol = true;
    }
    public void F_SpearPowerCharge()
    {
        _pistol_Animation.SetBool("Get", false);
        _spearFireSpeed += Time.deltaTime * 10f;
        UIManager.Instance.F_GetPlayerFireGauge().color = Color.white;
        UIManager.Instance.F_GetPlayerFireGauge().fillAmount = _spearFireSpeed / 10f;
        if (_spearFireSpeed > 10f )
               _spearFireSpeed = 10f;
    }
    public void F_SpearFire()
    {
        _spear.F_EnableLine();
        StartCoroutine(C_DrawLine());

        //�ۻ� ���� ����
        _spear.transform.parent = null;
        _spear_rb.isKinematic = false;
        if (_spearFireSpeed <= 3f)
        {
                _spearFireSpeed = 3f;
        }
        _spear_rb.AddForce(_player_mainCamera.transform.forward * _spearFireSpeed * 6f, ForceMode.Impulse);
        _spear.transform.Rotate(-14f, -6f, 0f);
        _pistol_Animation.SetBool("Reach", true);

    }
    public void F_SpearComeBack()
    {
        if (_spear.transform.parent != this.transform)
        {
            _spear_rb.isKinematic = true;
            Vector3 _pistol_Muzzle = _spear.F_GetFirePos();
            _spear.transform.position = Vector3.Lerp(_spear.transform.position, _pistol_Muzzle, _spearFireSpeed * Time.deltaTime);
            if (Vector3.Distance(_spear.transform.position, _pistol_Muzzle) < _spear_Distance )
            {
                // ������ ȹ�� ����
                F_GetScrapFromSpear();

                // �ִϸ��̼�
                _pistol_Animation.SetBool("Reach", false);
                _pistol_Animation.SetTrigger("Get");

                // �ʱ�ȭ
                F_InitSpear();

                // ������ Fade Out
                StartCoroutine(UIManager.Instance.C_FireGaugeFadeOut());

                // â �� �ڷ�ƾ ����
                StopCoroutine(_draw_Line_Coroutine);

                // â �� �����
                _spear.F_DisableLine();

                // ���� ������ ����
                int idx = ItemManager.Instance.inventorySystem.selectQuickSlotNumber;               // ���õ� ������ ��ȣ
                (ItemManager.Instance.inventorySystem.inventory[idx] as Tool).F_CheckDurability();  // ���� ������ Ȯ��
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
            ScrapType _scrapType = ScrapManager.Instance._scrapHitedSpear[l].scrapType;
            string v_scrapName = ItemManager.Instance.ItemDatas[(int)_scrapType]._itemName;
            //�ڽ� ȹ�� �� Dirt�� UI�� ������ �� ����
            if (_scrapType != ScrapType.BOX)
                UIManager.Instance.F_GetScrapUIOn(_scrapType, v_scrapName);

            ScrapManager.Instance._scrapHitedSpear[l].GetComponent<Scrap>().F_GetScrap();
        }
        ScrapManager.Instance._scrapHitedSpear.Clear();
    }
    private IEnumerator C_DrawLine()
    {
        while(!PlayerManager.Instance._canShootPistol)
        {
            _spear.F_RestoreLine();
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public void F_InitSpear()
    {
        if(_spearFireSpeed > 0f)
        {
            _spearFireSpeed = 0f;
            StartCoroutine(UIManager.Instance.C_FireGaugeFadeOut());
        }

        //�ۻ쿡 ���� ��ᰡ �ִٸ� Ǯ������ return
        if (ScrapManager.Instance._scrapHitedSpear.Count > 0)
        {
            foreach (Scrap v_scrap in ScrapManager.Instance._scrapHitedSpear)
            {
                ScrapManager.Instance.F_ReturnScrap(v_scrap);
            }
        }
        ScrapManager.Instance._scrapHitedSpear.Clear();

        //������ �ʱ�ȭ
        if (!_spear_rb.isKinematic)
        {
            _spear_rb.velocity = Vector3.zero;
            _spear_rb.isKinematic = true;
        }
        _spearFireSpeed = 0f;

        //��ġ �� ���� �ʱ�ȭ
        _spear.transform.parent = this.transform;
        _spear.transform.localPosition = _spear_Firepos;
        _spear.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        _spear.F_DisableLine();
    }
}
