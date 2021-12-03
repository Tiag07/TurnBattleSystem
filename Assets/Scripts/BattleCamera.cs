using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
    public class BattleCamera : MonoBehaviour
    {
        Vector3 arenaCenterPoint = Vector3.zero;
        [SerializeField] float rotatingAroundSpeed = 10f;
        [SerializeField] float approachingSpeed = 10f;
        [SerializeField] Transform generalVisionPoint;
        [SerializeField] Transform[] middleCameraPoints;
        [SerializeField] BattleManager battleSystem;

        public enum MovimentType
        {
            idle, rotateAround, focus, approaching,
        }
        public MovimentType movimentType = MovimentType.idle;
        Transform fighterToFocus;

        Coroutine cameraBehaviorLoop;

        public void RefreshArenaCenterPoint(List<Fighter> allFighterPositions)
        {
            
            for (int i = 0; i < allFighterPositions.Count; i++)
            {
                arenaCenterPoint += allFighterPositions[i].transform.position;
            }
            arenaCenterPoint /= allFighterPositions.Count;

            transform.position = arenaCenterPoint;
        }
        public void StartCameraBehavior()
        {
            if (movimentType == MovimentType.idle)
            {
                if (cameraBehaviorLoop != null)
                    StopCoroutine(cameraBehaviorLoop);
                
                cameraBehaviorLoop = StartCoroutine(CameraBehaviorLoops());
            }

        }

        IEnumerator CameraBehaviorLoops()
        {
            while (true)
            {
                PreparingRotateAroundCamera();
                yield return new WaitForSeconds(3f);
                PreparingFocusOnAFighter();
                yield return new WaitForSeconds(5f);
            }
        }
        void LateUpdate()
        {
            switch (movimentType)
            {
                case MovimentType.rotateAround:
                    transform.RotateAround(arenaCenterPoint, Vector3.up, rotatingAroundSpeed * Time.deltaTime);
                    break;

                case MovimentType.approaching:
                    transform.position += -transform.forward * 0.5f * Time.deltaTime;
                    break;

                case MovimentType.idle:
                    break;
            }

        }

        public void GeneralVision()
        {
            transform.position = generalVisionPoint.position;
            transform.rotation = generalVisionPoint.rotation;
            if (cameraBehaviorLoop != null) StopCoroutine(cameraBehaviorLoop);
            movimentType = MovimentType.idle;
        }
        void PreparingRotateAroundCamera()
        {
            int randomCameraPoint = Random.Range(0, middleCameraPoints.Length);
            transform.position = middleCameraPoints[randomCameraPoint].position;
            transform.LookAt(arenaCenterPoint);
            movimentType = MovimentType.rotateAround;

        }
        void PreparingFocusOnAFighter()
        {
            fighterToFocus = battleSystem.GetRandomFighter;

            transform.rotation = fighterToFocus.rotation;
            Vector3 offset = (transform.forward * 2) + transform.up;
            transform.position = fighterToFocus.position + offset;
            transform.LookAt(fighterToFocus);
            movimentType = MovimentType.approaching;
        }

    }
}