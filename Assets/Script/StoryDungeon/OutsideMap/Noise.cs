using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Noise 
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octave, float persistance, float lacunarity)
    {
        // scale : �������� ũ�� , �������� �ڼ��ϰ� ����(�� ������) / Ŭ���� �� �ڼ��ϰ� (�ε巯����)
        // octave : �ĵ��� ����,����(?) ������ �������� �ڼ��ϰ� return��
        // persistance : ����(amplitude) ũ�� (�󸶳� ��~�� ����) ����
        // lacunarity : ��(Frequency) �� �� ����

        float[,] noiseMap = new float[mapWidth, mapHeight]; 

        // seed�� ���� / �� ��Ÿ�꿡 seed�� ���� 
        System.Random rand = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octave];
        for (int i = 0; i < octave; i++)
        {
            float offsetX = rand.Next(-100000, 100000);    // -100,000 ~ 100,000 ������ ������ ��ȯ
            float offsetY = rand.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }


        if (scale <= 0)
            scale = 0.0001f;

        float maxNoiseHeight = float.MinValue;      // ������ �ִ� (����ȭ�� ����)
        float minNoiseHeight = float.MaxValue;      // ������ �ּڰ� ( `` )

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;        // ����
                float frequency = 1;        // ��
                float noiseHeight = 0;      // ���� ���ϴ� �������� ���� 

                for (int i = 0; i < octave; i++)        // ex) 4 ,  �� ĭ�� ����, ��� ��Ÿ�� ���� ���ϴµ� 
                {
                    // x�� y�� 1���� �Ҽ��� ���ڷ� �ٲٱ� ���ؼ� 
                    float sampleX = x / scale * frequency;      // �ش� ��ǥ���� �󵵿� ������ ���� (�󵵰� ũ�� ���� �� �ָ� ����������) + seed�� ����
                    float sampleY = y / scale * frequency;      // + octaveOffsets[i].x

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; // �ް��� ��ȭ�� �ֱ����� -1���� 1���� ���� �� �ְ�
                    noiseHeight += perlinValue * amplitude;     // �������� ���̴� (0~1������ perline ��) * (����)�� ������ ����

                    // ������ �󵵿� ���� ������ ����
                    amplitude *= persistance;     // �󵵸� �ø��� ������ �۾�����, �󵵸� ���̸� ������ Ŀ���� 
                    // �󵵴� Lacunarity(�󸶳� ��������)�� ������ ��ħ
                    frequency *= lacunarity;
                }

                maxNoiseHeight = noiseHeight > maxNoiseHeight ? noiseHeight : maxNoiseHeight;
                minNoiseHeight = minNoiseHeight > noiseHeight ? noiseHeight : minNoiseHeight;

                noiseMap[x, y] = noiseHeight;
            }
        }

        // noiseMap���� -? ���� ? ������ ���� ������ -> 0���� 1���� ����ȭ �ؾ��� 
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // x,y �� ���� min~max ���̿� �����ġ�� �ִ��� ( 0~1�� ��)
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
