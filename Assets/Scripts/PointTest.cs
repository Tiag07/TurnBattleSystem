using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTest : MonoBehaviour
{
    [SerializeField] Transform[] obj;
    [SerializeField] Vector3 pos;
    void Start()
    {
        for (int i = 0; i < obj.Length; i++)
        {
            pos += obj[i].position;
        }
        pos /= obj.Length;
    }

    // Update is called once per frame
    void Update()
    {      
        transform.position = pos;
    }
}
