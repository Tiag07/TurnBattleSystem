using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    [SerializeField] Transform arenaPoint;
    [SerializeField] float speed = 10;
    [SerializeField] BattleSystem battleSystem;
    public enum MovimentType
    {
        rotateAround, focus, idle,
    }
    public MovimentType movimentType;
    Transform fighterToFocus;

    private void Start()
    {
        StartCoroutine(CameraBehaviorLoops());
        
    }

    IEnumerator CameraBehaviorLoops()
    {
        PreparingRotateAroundCamera();
        yield return new WaitForSeconds(10f);
        PreparingFocusOnAFighter();
        yield return new WaitForSeconds(3f);
        StartCoroutine(CameraBehaviorLoops());
    }
    void LateUpdate()
    {
        switch (movimentType)
        {
            case MovimentType.rotateAround:
                transform.RotateAround(arenaPoint.position, Vector3.up, speed * Time.deltaTime);
                break;

            case MovimentType.idle:
                break;
        }
        
    }
    void PreparingRotateAroundCamera()
    {
        Vector3 offset = new Vector3(4.3f, 1.1f, -1.4f);
        transform.position = arenaPoint.position + offset;
        transform.LookAt(arenaPoint);
        movimentType = MovimentType.rotateAround;

    }
    void PreparingFocusOnAFighter()
    {
        GetFighterToFocus();
        transform.rotation = fighterToFocus.rotation;
        Vector3 offset = transform.forward * 2 + transform.up;
        transform.position = fighterToFocus.position + offset;
        transform.LookAt(fighterToFocus);
        movimentType = MovimentType.idle;
    }

    void GetFighterToFocus()
    {
       fighterToFocus = battleSystem.RandomFighter;
    }
}
