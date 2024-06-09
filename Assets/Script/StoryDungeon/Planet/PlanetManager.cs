using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;

public class PlanetManager : MonoBehaviour
{
    [SerializeField] TeleportController _teleportController;
    [SerializeField] GameObject[] _planetPrefList;
    [SerializeField] GameObject _teleport;
    GameObject _planetObj;

    [HideInInspector] int _planetIndex;
    [SerializeField] int _waitCreatePlanet; //15��
    [SerializeField] int _waitDeletePlanet; //1��30��
    
    [SerializeField] bool _joinPlanet; //�༺���� �����ϸ� true
    public bool joinPlanet { get => _joinPlanet; set => _joinPlanet = value; }
    public Transform teleport => _teleport.transform;
    public int planetIdx => _planetIndex;

    private void Start()
    {
        _teleport.SetActive(false);
        _joinPlanet = false;

        StartCoroutine(F_CheckCurrentTime());
    }

    IEnumerator F_CheckCurrentTime()
    {
        while (true)
        {
            // _waitCreate Planet �ð���ŭ ��� ( �༺ ���� ���ð� )
            yield return new WaitForSeconds(_waitCreatePlanet);

            // ���� Ȱ��ȭ / �༺ ������Ʈ ����
            _teleport.SetActive(true);
            _teleport.transform.position = _teleportController._defalutPostion_teleport;
            F_CreatePlanet();

            UIManager.Instance.F_PlayerMessagePopupTEXT("Use the portal to enter the planet");

            // _waitDeletePlanet �ð���ŭ ��� ( �༺ �ı� ���ð� )
            for (int i = 0; i < _waitDeletePlanet; i++)
            {
                // 1�ʾ� _waitDeletePlanet�� ���
                yield return new WaitForSeconds(1f);

                // �ı� �� �����ϸ�
                if (joinPlanet)
                {
                    // �༺���� ���ö� ���� ��� �� While ó������ �ǵ��ư�
                    yield return new WaitWhile(() => joinPlanet);
                    break;
                }
            }
            // �༺ �ı� ���ð����� �༺�� ���������ʾ�����.
            F_DeletePlanet();   // �༺ ������Ʈ �ı� / �ڷ���Ʈ ��Ȱ��ȭ
        }
    }

    public void F_CreatePlanet()
    {
        // �༺ ��ȣ ����
        _planetIndex = Random.Range(0, Enum.GetValues(typeof(PlanetType)).Length);

        // �༺ �༺
        _planetObj = Instantiate(_planetPrefList[_planetIndex], new Vector3(-1800, 0, 1100), Quaternion.identity); //�༺ ������Ʈ ����
        _planetObj.GetComponent<Rigidbody>().velocity = Vector3.right * 15;
    }

    public void F_DeletePlanet()
    {
        UIManager.Instance.F_PlayerMessagePopupTEXT("Close portal");
        _teleport.SetActive(false);
        Destroy(_planetObj); //�༺ ������Ʈ ����
    }
}
