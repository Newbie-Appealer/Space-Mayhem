using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class OutsideMapDataManager 
{
    // csv 파싱 정규식 
    string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";      

    public void F_InitOutsidemapData() 
    {
        TextAsset textAsset = Resources.Load("OutsideMapData") as TextAsset;    // csv 파일을 텍스트 에셋타입으로 불러오기 
        string[] lines = Regex.Split(textAsset.text , LINE_SPLIT_RE);           // 줄 단위로 자르기 ( 정규식을 기준으로 )
        string[] header = Regex.Split(lines[0], SPLIT_RE);                      // 맨 처음줄, 단어로 자르기 ( 정규식을 기준으로 )

        for (int i = 1; i < lines.Length; i++) 
        {
            // 1. 각 열을 단어로 자르기 ( 정규식을 기준으로 )
            string[] values = Regex.Split(lines[i] ,SPLIT_RE );

            // 2. landScape 생성 ,  초기화
            LandScape _land = new LandScape();
            _land.F_InitLandScape(values);

            // 3. OutsideMapManager의 arr에 저장 
            OutsideMapManager.Instance.F_InsertLandSacpeArr( values[0]  , _land);

        }
    
    }
}
