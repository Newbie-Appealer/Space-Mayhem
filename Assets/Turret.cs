using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public enum TurretType
{
    MACHINEGUN,
    ROCKET
}

public class Turret : Furniture
{
    IEnumerator _turretAction;

    [Header("��������!")]
    [SerializeField] private TurretType _turretType;        // �ͷ� ����
    [SerializeField] private int _shootingDelay;            // ���� ������
    [SerializeField] private float _shootingRange;          // � ���� ����

    [SerializeField] private LayerMask _meteorLayer;        // ���׿� ���̾�
    [SerializeField] private LayerMask _obstacleLayer;      // ���� ��ֹ� ���̾� ( �÷��̾ ��ġ�� ������Ʈ )

    [SerializeField] private Transform _rayPoint;           // ���� �߽� ��ġ
    [SerializeField] private Transform _rotateObject;       // ȸ�� ������Ʈ ( ���׿� �ٶ󺸱� )

    [Header("=== Effect ===")]
    [SerializeField] private ParticleSystem _shotEffect;    // �߽� ����Ʈ
    [SerializeField] private ParticleSystem _bulletEffect;  // �Ѿ� ����Ʈ

    [Header("=== turret information ===")]
    [SerializeField] private Collider[] _meteors;           // ������ � �迭
    [SerializeField] private bool        _canShooting;


    private RaycastHit _rayHit;
    protected override void F_InitFurniture()
    {
        _turretAction = C_TurretAction();
        StartCoroutine(_turretAction);
    }

    IEnumerator C_TurretAction()
    {
        _canShooting = true;
        yield return new WaitForSeconds(_shootingDelay);

        while (true)
        {
            if (!_onEnergy)
                yield return new WaitForSeconds(_shootingDelay);
            
            if(_canShooting)
            {
                yield return null;

                _meteors = Physics.OverlapSphere(transform.position, _shootingRange, _meteorLayer);

                foreach (Collider meteor in _meteors)
                {
                    // 1. ���׿� �ٶ󺸱�
                    F_RotateTurret(meteor.transform);

                    // 2. ���׿� �������� ������Ʈ�� �ִ��� Ȯ��
                    if (F_RaycastObstacle())
                        continue;



                    // 3. ���� ( ����Ʈ ���� )
                    F_EffectPlay();

                    yield return new WaitForSeconds(0.5f);

                    // 4. Ȯ���� ���� � �ı�
                    F_DestoryMeteor(meteor.transform);

                    // Ž��/���� �Ұ��� ���·� ����
                    _canShooting = false;
                    break;
                }
            }
            else
            {
                yield return new WaitForSeconds(_shootingDelay);
                _canShooting = true;
            }
        }
    }

    private void F_RotateTurret(Transform v_target)
    {
        Vector3 dir = v_target.transform.position - transform.position;
        _rotateObject.rotation = Quaternion.LookRotation(dir.normalized);
    }

    private bool F_RaycastObstacle()
    {
        return Physics.Raycast(_rayPoint.position, _rayPoint.forward, out _rayHit, 30, _obstacleLayer);
    }

    private void F_EffectPlay()
    {
        if (!_shotEffect.isPlaying)
            _shotEffect.Play();
        if (!_bulletEffect.isPlaying)
            _bulletEffect.Play();
    }

    private void F_DestoryMeteor(Transform v_meteor)
    {
        int rnd = Random.Range(0, 1000);
        switch(_turretType)
        {
            // Ÿ�Կ� ���� Ȯ�� ���߿� �������!
            case TurretType.MACHINEGUN:
                if (rnd % 3 != 0)   // ���� 33%
                    return;
                break;
            case TurretType.ROCKET:
                if (rnd % 2 != 0)   // ���� 50%
                    return;
                break;
        }

        Meteor tmpMeteor = v_meteor.GetComponent<Meteor>();
        if (tmpMeteor != null)
        {
            if(tmpMeteor.gameObject.activeSelf)
            {
                int dropitemCode = tmpMeteor.F_SettingItemCode();
                tmpMeteor.F_GetMeteor(dropitemCode);
                ResourceManager.Instance.F_GetEffect(EffectType.EXPLOSION, tmpMeteor.transform.position);
            }
        }
    }

    public override void F_ChangeEnergyState(bool v_state)
    {
        _onEnergy = v_state;
    }

    #region ���� / �ҷ����� 
    public override string F_GetData()
    {
        return "";
    }

    public override void F_SetData(string v_data)
    {

    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _shootingRange);
    }
}
