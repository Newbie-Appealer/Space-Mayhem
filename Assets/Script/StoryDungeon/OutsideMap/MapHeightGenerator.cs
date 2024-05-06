using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHeightGenerator : MonoBehaviour
{
    [Header("Container")]
    public float[,] noiseMap;       // �޸� ������ ����� float���� ������� 
    public float[,] gradientMap;    // �׶���Ʈ �� 
    public float[,] concluMap;      // ���� �� 

    public float[,] GenerateMap
        (int v_width, int v_height, int v_seed, float noiseScale, int v_octaves, float v_per, float v_lac, float v_devi)
    {
        // 0. 
        concluMap = new float[v_height, v_width];

        // 1. ������ �� ���� ( ����, ����, ������ ũ�� , ��Ÿ��, persistance , lacunerity )
        noiseMap = Noise.GenerateNoiseMap(v_width, v_height, v_seed, noiseScale, v_octaves, v_per, v_lac);

        // 2. �׶���Ʈ �� ����
        gradientMap = GenerateGradientMap(v_width, v_height);

        // 3. ���� height ���� 
        for (int y = 0; y < v_height; y++)
        {
            for (int x = 0; x < v_width; x++)
            {
                // �׶��̼� ���� ( 1f ~ 0f ) + ������ ����
                concluMap[x, y] = (gradientMap[y, x] + noiseMap[y, x]) * v_devi;

            }
        }

        return concluMap;
    }

    // �׶��̼� map ����� 
    public float[,] GenerateGradientMap(int v_mapWidth, int v_mapHeight)
    {
        float[,] _gra = new float[v_mapWidth, v_mapHeight];

        // base ä��� ( 0�̳� 1 )
        for (int y = 0; y < v_mapHeight; y++)
            for (int x = 0; x < v_mapWidth; x++)
                _gra[y, x] = 0;

        int xStart = 0; int yStart = 0; // 0���� 10����

        // �׶���Ʈ
        for (float i = 1f; i >= 0f; i -= 0.3f) // 0.8 / 0.6 / 0.4 / 0.2
        {
            for (int j = xStart; j < v_mapWidth - xStart; j++)
            {
                // 1. x ��ǥ 0���� 10����
                _gra[yStart, j] = i;

                // 3. y�� height �϶�, x ��ǥ 0���� 10����
                _gra[v_mapHeight - 1 - yStart, j] = i;
            }


            for (int j = yStart; j < v_mapHeight - yStart; j++)
            {
                // 2. y ��ǥ 0���� 10����
                _gra[j, xStart] = i;

                // 4. y�� 0 ~ height �̰�, x ��ǥ�� 0
                _gra[j, v_mapWidth - 1 - xStart] = i;

            }

            xStart++;
            yStart++;
        }


        return _gra;
    }
}
