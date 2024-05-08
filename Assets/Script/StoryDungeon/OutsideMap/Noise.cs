using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Noise 
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octave, float persistance, float lacunarity)
    {
        // scale : 노이즈의 크기 , 작을수록 자세하게 생성(빈도 높아짐) / 클수록 덜 자세하게 (부드러워짐)
        // octave : 파동의 갯수,종류(?) 많으면 많을수록 자세하게 return됨
        // persistance : 진폭(amplitude) 크기 (얼마나 낮~높 을지) 결정
        // lacunarity : 빈도(Frequency) 의 폭 결정

        float[,] noiseMap = new float[mapWidth, mapHeight]; 

        // seed값 적용 / 각 옥타브에 seed값 적용 
        System.Random rand = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octave];
        for (int i = 0; i < octave; i++)
        {
            float offsetX = rand.Next(-100000, 100000);    // -100,000 ~ 100,000 사이의 랜덤값 반환
            float offsetY = rand.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }


        if (scale <= 0)
            scale = 0.0001f;

        float maxNoiseHeight = float.MinValue;      // 높이의 최댓값 (정규화를 위한)
        float minNoiseHeight = float.MaxValue;      // 높이의 최솟값 ( `` )

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;        // 진폭
                float frequency = 1;        // 빈도
                float noiseHeight = 0;      // 새로 정하는 노이즈의 높이 

                for (int i = 0; i < octave; i++)        // ex) 4 ,  한 칸에 대한, 모든 옥타브 수를 구하는듯 
                {
                    // x와 y를 1이하 소숫점 숫자로 바꾸기 위해서 
                    float sampleX = x / scale * frequency;      // 해당 좌표들은 빈도에 영향을 받음 (빈도가 크면 서로 더 멀리 떨어져있음) + seed값 적용
                    float sampleY = y / scale * frequency;      // + octaveOffsets[i].x

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; // 급격한 변화를 주기위해 -1부터 1까지 있을 수 있게
                    noiseHeight += perlinValue * amplitude;     // 노이즈의 높이는 (0~1사이의 perline 값) * (진폭)에 영향을 받음

                    // 진폭은 빈도에 따라 영향을 받음
                    amplitude *= persistance;     // 빈도를 늘리면 진폭이 작아지고, 빈도를 줄이면 진폭이 커진다 
                    // 빈도는 Lacunarity(얼마나 촘촘한지)에 영향을 미침
                    frequency *= lacunarity;
                }

                maxNoiseHeight = noiseHeight > maxNoiseHeight ? noiseHeight : maxNoiseHeight;
                minNoiseHeight = minNoiseHeight > noiseHeight ? noiseHeight : minNoiseHeight;

                noiseMap[x, y] = noiseHeight;
            }
        }

        // noiseMap에는 -? 부터 ? 까지의 수가 들어가있음 -> 0부터 1가지 정규화 해야함 
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // x,y 의 값이 min~max 사이에 어느위치에 있는지 ( 0~1의 값)
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
