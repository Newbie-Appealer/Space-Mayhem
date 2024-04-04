using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMaster : Singleton<BuildMaster>
{
    protected override void InitManager()
    {

    }

    public MyBuildManager myBuildManger;
    public MyBuildCheck mybuildCheck;
    public HousingDataManager housingDataManager;
    public HousingUiManager housingUiManager;

    // 블럭 sprite 
    [SerializeField]
    public List<Sprite> _blockSprite;

    // 현재 짓고 있는 블럭의 데이터 
    [SerializeField]
    private HousingBlock _currBlockData;

    public HousingBlock currBlockData { get => _currBlockData;  }

    public void F_SetBlockData( HousingBlock v_block) 
    {
        this._currBlockData = v_block;
    }

}
