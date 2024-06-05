using System.Collections;
using UnityEngine;

public enum ScrapType
{
    PLASTIC,
    FIBER,
    SCRAP,
    BOX
}

public class Scrap : MonoBehaviour
{
    [Header("Scrap Information")]
    [SerializeField] private string _itemName;
    [SerializeField] ScrapType _scrapType;
    public ScrapType scrapType => _scrapType;

    private bool _isHited = false;
    private Rigidbody _scrapRigidBody;
    private Vector3 _refVector3 = Vector3.zero;

    public void F_SettingScrap()
    {
        _scrapRigidBody = GetComponent<Rigidbody>();
    }

    /// <summary> Scrap �ʱ�ȭ �Լ� </summary>
    public void F_InitScrap(Transform v_transformParent)
    {
        _scrapRigidBody.isKinematic = false;            // Rigidbody �ʱ�ȭ
        _scrapRigidBody.velocity = Vector3.zero;
        _isHited = false;

        this.transform.SetParent(v_transformParent);
        this.transform.localPosition = Vector3.zero;        // ��ġ   �ʱ�ȭ
        
        //��ũ�� ��ȣ���� ũ�� �ٸ��� �ʱ�ȭ
        switch(_scrapType)
        {
            case ScrapType.PLASTIC: 
                this.transform.localScale = new Vector3(10f, 10f, 10f);
                break;
            case ScrapType.FIBER:
                this.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
                break;
            case ScrapType.SCRAP:
                this.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                break;
            case ScrapType.BOX:
                goto case ScrapType.FIBER;
        }

        float _randomX = Random.Range(0f, 360f);
        float _randomY = Random.Range(0f, 360f);
        float _randomZ = Random.Range(0f, 360f);
        this.transform.localRotation = Quaternion.Euler(_randomX, _randomY, _randomZ);
        this.gameObject.SetActive(false);                   // ������Ʈ ��Ȱ��ȭ
    }

    /// <summary> Scrap ������ ���� �Լ�</summary>
    public void F_MoveScrap(Vector3 v_spawnPosition, Vector3 v_scrapVelocity)
    {
        gameObject.SetActive(true);                                         // ������Ʈ Ȱ��ȭ

        this.transform.localPosition = v_spawnPosition;                          // ��ġ �ʱ�ȭ
        _scrapRigidBody.velocity = v_scrapVelocity * ScrapManager.Instance._item_MoveSpeed;    // ������
        StartCoroutine(C_ItemDistanceCheck());
    }

    IEnumerator C_ItemDistanceCheck()
    {
        //Scrap ���� 10�� �� �Ÿ� �˻� ���� 
        yield return new WaitForSeconds(10f);

        float distance = ScrapManager.Instance._range_Distance;         //��ᰡ ������ �� �ִ� �ִ� �Ÿ�
        Transform playerTrs = ScrapManager.Instance.playerTransform;    // �÷��̾� ��ġ

        while (gameObject.activeSelf)
        {
            //�Ÿ��� �־�����
            if (Vector3.Distance(gameObject.transform.position, playerTrs.position) > distance)
            {
                StopAllCoroutines();
                ScrapManager.Instance.F_ReturnScrap(this);  // �ʱ�ȭ �� Ǯ��
            }
            yield return new WaitForSeconds(2f);            // 2�ʿ� �ѹ��� �Ÿ��� Ȯ��
        }
    }

    public IEnumerator C_ItemVelocityChange(Vector3 v_newVelocity, float v_changeSpeed)
    {
        while (_scrapRigidBody.velocity != v_newVelocity && !_scrapRigidBody.isKinematic)
        {
            _scrapRigidBody.velocity
                    = Vector3.SmoothDamp(_scrapRigidBody.velocity, v_newVelocity, ref _refVector3, v_changeSpeed).normalized * ScrapManager.Instance._item_MoveSpeed;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public void F_GetScrap()
    {
        if(_scrapType == ScrapType.BOX)
        {
            for (int l = 0; l < Random.Range(1, 4); l++)
            {
                int _randomScrap = Random.Range(0, 3);
                ItemManager.Instance.inventorySystem.F_GetItem(_randomScrap);
                ScrapManager.Instance.F_GetScrapBox(_randomScrap, ItemManager.Instance.ItemDatas[_randomScrap]._itemName);
            }
        }
        else
        {
            ItemManager.Instance.inventorySystem.F_GetItem((int)_scrapType);
        }

        ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();
        //������ ȹ�� ����
        SoundManager.Instance.F_PlaySFX(SFXClip.USEHAND);
        //Scrap Ǯ������ �ʱ�ȭ
        ScrapManager.Instance.F_ReturnScrap(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Spear") && !_isHited)
        {
            _isHited = true;
            ScrapManager.Instance._scrapHitedSpear.Add(this);
            _scrapRigidBody.isKinematic = true; 
            transform.SetParent(other.transform);
            transform.localPosition = Vector3.zero;
        }
    }
}
