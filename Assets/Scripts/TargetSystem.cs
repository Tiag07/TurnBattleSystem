using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BattleSystem
{
    public class TargetSystem : MonoBehaviour
    {
        Camera mainCamera;
        [SerializeField] bool active = false;
        [SerializeField] LayerMask fightersLayer;
        const int heroLayer = 9, enemyLayer = 10;
        [SerializeField] ParticleSystem ptcfighterTargeted;
        [SerializeField] Color heroTargetColor, enemyTargetColor;
        [SerializeField] Transform currentTarget;

        public event Action<Fighter> targetSelected;
        void Start()
        {
            mainCamera = Camera.main;
        }

        void Update()
        {
            if (active)
            {
                MovingMouseToTarget();
            }

        }
        void MovingMouseToTarget()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, fightersLayer))
            {
                if (currentTarget != hit.transform)
                {
                    ptcfighterTargeted.transform.position = hit.transform.position;
                    ptcfighterTargeted.transform.localScale = hit.transform.localScale;
                    currentTarget = hit.transform;
                }
                if (ptcfighterTargeted.gameObject.activeSelf == false)
                {
                    ptcfighterTargeted.gameObject.SetActive(true);
                    ptcfighterTargeted.Play();
                }

                SelectingTarget(hit);
            }
            else ptcfighterTargeted.gameObject.SetActive(false);
        }

        void SelectingTarget(RaycastHit hit)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //print(hit.transform.name);
                Fighter fighterTargeted = hit.transform.GetComponent<Fighter>();
                targetSelected?.Invoke(fighterTargeted);
            }
        }

        public void EnableTarget() => active = true;
        public void DisableTarget()
        {
            active = false;
            ptcfighterTargeted.gameObject.SetActive(false);
        }
    }
}