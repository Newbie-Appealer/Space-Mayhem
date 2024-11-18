using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ttt : MonoBehaviour
{
    private void FixedUpdate()
    {
        transform.position = new Vector3(0, 20, -20);
        transform.rotation = Quaternion.Euler(40, 0, 0);
    }
}
