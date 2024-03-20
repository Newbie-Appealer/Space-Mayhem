using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public enum MySelectedBuildType
{
    defaultFloor,
    floor,
    celling,
    wall
}

public class MyBuildManager : MonoBehaviour
{
    public static MyBuildManager instance;

    private void Awake()
    {
        instance = this;
    }

    [Header("Build Object")]
    [SerializeField] private List<GameObject> _floorList;
    [SerializeField] private List<GameObject> _cellingList;
    [SerializeField] private List<GameObject> _wallList;

    [Header("LayerMask")]
    // [SerializeField] public LayerMask _buildShpereLayer;
    [SerializeField] public int _buildBlockLayer;

    [Header( "Type")]
    [SerializeField] private MySelectedBuildType _mySelectBuildType;
   
    [Header("Snap")]    
    [SerializeField] private MyConnectorTpye _MyConnectorTpye;

    [Header("Temp Object Setting")]
    [SerializeField] Material _validBuildMaterial;
    [SerializeField] int _buildTypeIdx;     // ���� Ÿ������
    [SerializeField] int _buildDetailIdx;   // �� Ÿ�Ծ� ���° ������Ʈ ����`

    [Header("Tesmp Object Building")]
    [SerializeField] GameObject _TempObjectBuilding;        // �ӽ� ������Ʈ
    [SerializeField] MyBuildingBlock _tempBuildingBlock;    // �ӽ� ������Ʈ�� , �� block�� ����ִ� block ��ũ��Ʈ
    [SerializeField] bool _isTempValidPosigion;             // �ӽ� ������Ʈ�� ������ �� �ִ���


    private void Start()
    {
        _buildBlockLayer = LayerMask.NameToLayer("BuildingBlock");
    }

    public void F_GetbuildType( int v_type , int v_detail) 
    {
        _buildTypeIdx = v_type;
        _buildDetailIdx = v_detail;

        StopAllCoroutines();
        StartCoroutine(F_TEmpBuild());
    }

    IEnumerator F_TEmpBuild() 
    {
        GameObject _currBuild = F_GetCurBuild();
        while (true) 
        {
            F_CreateTempPrefab(_currBuild);

            F_MoveTempObjectToRaycast();
            yield return null;      // update ȿ��   
        }
    }

    private void F_MoveTempObjectToRaycast() 
    {
        Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit _hit;

        if (Physics.Raycast(_ray, out _hit)) 
        {
            // 0. block�̶� �浹�ϸ� ��ġ �̵�
            if (_hit.collider.CompareTag("Block") )
            {
                _TempObjectBuilding.transform.position = _hit.point;        
            }
            // 1. �ƴϸ� �Ⱥ��̰�
            else
                F_ChangeMeshRenderer(_tempBuildingBlock.transform, null , false);
        }
    }

    public void F_TempBlockTriggerOther( MyConnector v_otherConnet) 
    {
        // �Ű����� : �� ���̶� �浹 �� other Ŀ����

        // 1. ���� ���ϸ�
        if (v_otherConnet._canConnect != true)
            return;

        // 2. �� Ÿ���̶� ������
        if (_mySelectBuildType == MySelectedBuildType.wall && v_otherConnet._isConnectWall
            && _mySelectBuildType == MySelectedBuildType.floor && v_otherConnet._isConnectFloor)
            return;

        F_SnapTempConnector( v_otherConnet );

    }

    private void F_SnapTempConnector( MyConnector v_type) 
    {
        // other Ŀ������ MyConnector Ÿ��
        Debug.Log( v_type._myConnectorType );
        _MyConnectorTpye = F_FindMyConnecType( v_type._myConnectorType );
        Transform trs = _tempBuildingBlock.F_FintTrsSameToType( _MyConnectorTpye );

        _TempObjectBuilding.transform.position
            = v_type.transform.position
                - (trs.position - _TempObjectBuilding.transform.position );
        
    }

    private MyConnectorTpye F_FindMyConnecType( MyConnectorTpye v_other) 
    {
        switch (v_other) 
        {
            case MyConnectorTpye.top:
                return MyConnectorTpye.bottom;
            case MyConnectorTpye.bottom:
                return MyConnectorTpye.top;
            case MyConnectorTpye.left:
                return MyConnectorTpye.right;
            case MyConnectorTpye.right:
                return MyConnectorTpye.left;
            default:
                return MyConnectorTpye.bottom;
        }
    }


    private GameObject F_GetCurBuild() 
    {
        switch (_buildTypeIdx)
        {
            case 0:
                _mySelectBuildType = MySelectedBuildType.floor;
                return _floorList[_buildDetailIdx];
            case 1:
                _mySelectBuildType = MySelectedBuildType.celling; 
                return _cellingList[_buildDetailIdx];
            case 2:
                _mySelectBuildType = MySelectedBuildType.wall;
                return _wallList[_buildDetailIdx];
            default:
                return null;
        }
    }

    private void F_CreateTempPrefab( GameObject v_temp) 
    {
        // ��������
        // temp������Ʈ�� null�̵Ǹ� �ٷ� ������!
        if (_TempObjectBuilding == null)
        {
            
            _TempObjectBuilding = Instantiate(v_temp);

            // temp ���� block�� �ִ� ��ũ��Ʈ ��������
            _tempBuildingBlock = _TempObjectBuilding.transform.GetChild(0).GetComponent<MyBuildingBlock>();

            // ���̾� �ٲٱ�
            F_ChangeLayer( _tempBuildingBlock.transform , _buildBlockLayer );

            // Meterial �ٲٱ�
            F_ChangeMeshRenderer( _tempBuildingBlock.transform, _validBuildMaterial);
        }
    }

    private void F_ChangeMeshRenderer( Transform v_pa , Material material , bool v_flag = false ) 
    {
        foreach ( MeshRenderer msr in v_pa.GetComponentsInChildren<MeshRenderer>()) 
        {
            if (material != null)
                msr.material = material;

            else
                msr.enabled = v_flag;
        }
    }

    private void F_ChangeLayer( Transform v_pa , int v_n) 
    {
        v_pa.gameObject.layer = v_n;
    }

}
