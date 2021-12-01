using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    Rigidbody rb;
    void Start()
    {
        if (!GetComponent<Rigidbody>())
            rb = GetComponent<Rigidbody>();
        
        rb?.AddForce(Vector3.forward * 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
