using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    [SerializeField] Transform arenaPoint;
    [SerializeField] float speed = 10;


    // Update is called once per frame
    void LateUpdate()
    {
        transform.RotateAround(arenaPoint.position, Vector3.up, speed * Time.deltaTime);
    }
}
