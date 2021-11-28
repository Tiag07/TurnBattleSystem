using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
    public class BattleCamera : MonoBehaviour
    {
        [SerializeField] Transform arenaPoint;
        [SerializeField] float rotatingAroundSpeed = 10f;
        [SerializeField] float approachingSpeed = 10f;
        [SerializeField] BattleManager battleSystem;
        public enum MovimentType
        {
            idle, rotateAround, focus, approaching,
        }
        public MovimentType movimentType = MovimentType.idle;
        Transform fighterToFocus;

        Coroutine cameraBehaviorLoop;
        public void StartCameraBehavior()
        {
            if(cameraBehaviorLoop == null)
           cameraBehaviorLoop = StartCoroutine(CameraBehaviorLoops());
        }

        IEnumerator CameraBehaviorLoops()
        {
            PreparingRotateAroundCamera();
            yield return new WaitForSeconds(3f);
            //PreparingFocusOnAFighter();
            yield return new WaitForSeconds(5f);
            StartCoroutine(CameraBehaviorLoops());
        }
        void LateUpdate()
        {
            switch (movimentType)
            {
                case MovimentType.rotateAround:
                    transform.RotateAround(arenaPoint.position, Vector3.up, rotatingAroundSpeed * Time.deltaTime);
                    break;

                case MovimentType.approaching:
                    transform.position += -transform.forward * 0.5f * Time.deltaTime;
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
            movimentType = MovimentType.approaching;
        }

        void GetFighterToFocus()
        {
            fighterToFocus = battleSystem.RandomFighter;
        }
    }
}