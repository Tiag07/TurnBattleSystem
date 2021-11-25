using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTest : MonoBehaviour
{
    [SerializeField] Transform[] obj;
    [SerializeField] Vector3 pos;
    void Start()
    {
        RefreshArenaCenterPoint();
    }

    public void RefreshArenaCenterPoint()
    {
        for (int i = 0; i < obj.Length; i++)
        {
            pos += obj[i].position;
        }
        pos /= obj.Length;

        transform.position = pos;
    }
}
