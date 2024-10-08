using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHeightGenerator : MonoBehaviour
{
    [Header("Container")]
    public float[,] noiseMap;       // 펄린 노이즈 계산한 float값이 들어있음 
    public float[,] gradientMap;    // 그라디언트 맵 

    public void GenerateMap
        (  ref float[,] v_concludeMap, int v_width, int v_height, int v_seed, float noiseScale, int v_octaves, float v_per, float v_lac, float v_devi)
    {
        // 1. 노이즈 맵 생성 ( 가로, 세로, 노이즈 크기 , 옥타브, persistance , lacunerity )
        noiseMap = Noise.GenerateNoiseMap(v_width, v_height, v_seed, noiseScale, v_octaves, v_per, v_lac);

        // 2. 그라디언트 맵 생성
        gradientMap = GenerateGradientMap(v_width, v_height);

        // 3. 최종 height 결정 
        for (int y = 0; y < v_height; y++)
        {
            for (int x = 0; x < v_width; x++)
            {
                // 그라데이션 높이 ( 1f ~ 0f ) + 노이즈 높이
                v_concludeMap[x, y] = (gradientMap[x,y] + noiseMap[x,y]) * v_devi;
            }
        }
    }

    // 그라데이션 map 만들기 
    public float[,] GenerateGradientMap(int v_mapWidth, int v_mapHeight)
    {
        float[,] _gra = new float[v_mapWidth, v_mapHeight];

        // base 채우기 ( 0이나 1 )
        for (int y = 0; y < v_mapHeight; y++)
            for (int x = 0; x < v_mapWidth; x++)
                _gra[x, y] = 0;

        int xStart = 0; int yStart = 0;

        // 그라디언트
        for (float i = 1f; i >= 0f; i -= 0.2f) // 0.8 / 0.6 / 0.4 / 0.2
        {
            for (int j = xStart; j < v_mapWidth - xStart; j++)
            {
                // 1. x 좌표 0부터 10까지
                _gra[ j, yStart] = i;

                // 3. y가 height 일때, x 좌표 0부터 10까지
                _gra[ j, v_mapHeight - 1 - yStart] = i;
            }


            for (int j = yStart; j < v_mapHeight - yStart; j++)
            {
                // 2. y 좌표 0부터 10까지
                _gra[xStart , j] = i;

                // 4. y가 0 ~ height 이고, x 좌표가 0
                _gra[v_mapWidth - 1 - xStart , j] = i;

            }

            xStart++;
            yStart++;
        }


        return _gra;
    }
}
