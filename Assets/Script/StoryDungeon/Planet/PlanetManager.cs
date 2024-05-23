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

    int _planetIndex;
    [SerializeField] int _waitCreatePlanet; //15��
    [SerializeField] int _waitDeletePlanet; //5��
    
    [SerializeField] bool _joinPlanet; //�༺���� �����ϸ� true
    public bool joinPlanet { get => _joinPlanet; set => _joinPlanet = value; }

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

            // ���� ���� / �༺ ������Ʈ ����
            F_OnTeleport(true, _teleportController._defalutPostion_teleport);
            UIManager.Instance.F_PlayerMessagePopupTEXT("Use the portal to enter the planet");
            F_CreatePlanet();

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
                    continue;
                }
            }
            // �༺ �ı� ���ð����� �༺�� ���������ʾ�����.
            F_OnTeleport(false, _teleportController._defalutPostion_teleport);  // ���� ��Ȱ��ȭ
            F_DeletePlanet();                                                   // �༺ ������Ʈ �ı�
            UIManager.Instance.F_PlayerMessagePopupTEXT("Close portal");
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
        _teleport.SetActive(false);
        Destroy(_planetObj); //�༺ ������Ʈ ����
    }

    public void F_OnTeleport(bool v_state, Vector3 v_pos)
    {
        // ��Ż On / OFF
        _teleport.SetActive(v_state);

        // ��Ż ��ġ �Ű��������� While������ ����������
        while(Vector3.Distance(v_pos, _teleport.transform.position) >= 1f)
            _teleport.transform.position = v_pos;
        // �÷��̾ �̵������ʴ� ���׸� �ذ��ϱ����� ����� ������.
        // �� ���� �ذ��������� �����ϸ�ɵ�
    }
}
