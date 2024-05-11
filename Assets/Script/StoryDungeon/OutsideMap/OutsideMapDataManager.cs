using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class OutsideMapDataManager 
{
    // csv �Ľ� ���Խ� 
    string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";      

    public void F_InitOutsidemapData() 
    {
        TextAsset textAsset = Resources.Load("OutsideMapData") as TextAsset;    // csv ������ �ؽ�Ʈ ����Ÿ������ �ҷ����� 
        string[] lines = Regex.Split(textAsset.text , LINE_SPLIT_RE);           // �� ������ �ڸ��� ( ���Խ��� �������� )
        string[] header = Regex.Split(lines[0], SPLIT_RE);                      // �� ó����, �ܾ�� �ڸ��� ( ���Խ��� �������� )

        for (int i = 1; i < lines.Length; i++) 
        {
            // 1. �� ���� �ܾ�� �ڸ��� ( ���Խ��� �������� )
            string[] values = Regex.Split(lines[i] ,SPLIT_RE );

            // 2. landScape ���� ,  �ʱ�ȭ
            LandScape _land = new LandScape();
            _land.F_InitLandScape(values);

            // 3. OutsideMapManager�� arr�� ���� 
            OutsideMapManager.Instance.F_InsertLandSacpeArr( values[0]  , _land);

        }
    
    }
}
