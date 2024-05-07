using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderGenerator : MonoBehaviour
{
    public void F_CreateCollider(int v_width, int v_height, List<Vector3> v_pointlist)
    {
        for (int y = 0; y < v_height - 1; y++)  // 마지막줄은 생성 x
        {
            for (int x = 0; x < v_width - 1; x++)   // 마지막줄은 생성 x
            {
                int idx = y * v_height + x;

                F_SetGradient(idx, v_width, v_pointlist);
            }
        }
    }

    private void F_SetGradient(int v_idx, int v_width, List<Vector3> v_pointlist)
    {
        Vector3 _a = v_pointlist[v_idx];
        Vector3 _b = v_pointlist[v_idx + 1];
        Vector3 _c = v_pointlist[v_idx + v_width];
        Vector3 _d = v_pointlist[v_idx + v_width + 1];

        // 1. 오브젝트 생성
        GameObject _obj = OutsideMapManager.Instance.outsideMapPooling.F_GetColliderObject();
        _obj.transform.localPosition = (_a + _b + _c + _d) / 4 + OutsideMapManager.Instance._Offset;
        _obj.transform.localScale
            = new Vector3(Vector3.Distance(_a, _b), 0.1f, Vector3.Distance(_a, _c));

        // 2. x 기울기 구하기
        Vector3 spot1 = (_a + _b) / 2;
        Vector3 spot2 = (_c + _d) / 2;

        // 2-1. 아래쪽 좌표가 작을 때
        if (spot1.y <= spot2.y)
        {
            Vector3 orthogonal = new Vector3(spot1.x, spot1.y, spot2.z);

            float spot1ToOr = Vector3.Distance(spot1, orthogonal);   // 탄젠트 공식에서 a 에 해당
            float orToSpot2 = Vector3.Distance(orthogonal, spot2);   // `` b ``

            float _angle = Mathf.Atan2(orToSpot2, spot1ToOr) * Mathf.Rad2Deg; // b , a 순으로
            F_SetRotataion(_obj, new Vector3(_angle * -1, 0, 0));               // * -1
        }
        // 2-2. 아래쪽 좌표가 클 때 
        else
        {
            Vector3 orthogonal = new Vector3(spot1.x, spot2.y, spot1.z);

            float spot2ToOr = Vector3.Distance(spot2, orthogonal); // 탄젠트 공식에서 a 에 해당
            float orTospot1 = Vector3.Distance(orthogonal, spot1);  // `` b ``

            float _angle = Mathf.Atan2(orTospot1, spot2ToOr) * Mathf.Rad2Deg;
            F_SetRotataion(_obj, new Vector3(_angle, 0, 0));
        }

        // 3. z 기울기 구하기 
        Vector3 spot3 = (_a + _c) / 2;
        Vector3 spot4 = (_b + _d) / 2;

        // 조건1. 왼쪽의 z 좌표가 작을 때 
        if (spot3.y <= spot4.y)
        {
            Vector3 orthogonal = new Vector3(spot4.x, spot3.y, spot4.z);

            float spot3ToOr = Vector3.Distance(spot3, orthogonal);     // a
            float orToSpot4 = Vector3.Distance(orthogonal, spot4);   // b

            float _angle = Mathf.Atan2(orToSpot4, spot3ToOr) * Mathf.Rad2Deg;
            F_SetRotataion(_obj, new Vector3(_obj.transform.eulerAngles.x, 0, _angle));
        }
        // 왼쪽의 z 좌표가 클 때
        else
        {
            Vector3 orthogonal = new Vector3(spot3.x, spot4.y, spot3.z);

            float spot4ToOr = Vector3.Distance(spot4, orthogonal);      // a
            float orToSpot3 = Vector3.Distance(orthogonal, spot3);    // b

            float _angle = Mathf.Atan2(orToSpot3, spot4ToOr) * Mathf.Rad2Deg;
            F_SetRotataion(_obj, new Vector3(_obj.transform.eulerAngles.x, 0, _angle * -1));         // * -1
        }
    }

    private void F_SetRotataion(GameObject v_obj, Vector3 v_angle)
    {
        Quaternion rotation = Quaternion.Euler(v_angle.x, v_angle.y, v_angle.z);
        v_obj.transform.rotation = rotation;
    }
}
