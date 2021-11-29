using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BattleSystem
{
    public class BattleEventManager : MonoBehaviour
    {
        BattleManager battleManager;
        BattleInterface battleInterface;
        BattleCamera battleCamera;
        TargetSystem targetSystem;

        private void Awake()
        {
            battleManager = GetComponent<BattleManager>();
            battleInterface = GetComponent<BattleInterface>();
            battleCamera = FindObjectOfType<BattleCamera>();
            targetSystem = GetComponent<TargetSystem>();
        }
        void OnEnable()
        {
            battleManager.onFightersOrderSorted += battleInterface.OnFightersOrderSorted;

            battleManager.onTurnOfControlledFighterStarted += battleInterface.EnableChooseAction;
            battleManager.onTurnOfNonControlledFighterStarted += battleInterface.EnableWaitTurn;

            battleManager.onBattleStarted += battleCamera.StartCameraBehavior;

            battleManager.onAttackButtonSelected += battleInterface.EnableChooseTargetInterface;
            battleManager.onAttackButtonSelected += targetSystem.EnableTarget;
            battleManager.onAttackButtonSelected += battleCamera.GeneralVision;

            battleManager.onTargetingFighterEnded += targetSystem.DisableTarget;
            battleManager.onTargetingFighterEnded += battleCamera.StartCameraBehavior;
            battleManager.onBackToMainInterfaceSelected += battleInterface.EnableChooseAction;
            battleManager.onBackToMainInterfaceSelected += battleCamera.StartCameraBehavior;
            battleManager.onAttackProcessStarted += battleInterface.EnableFighterIsAttackingOponentText;

            targetSystem.onTargetSelected += battleManager.ValidateTargetForAttack;
        }

        void OnDisable()
        {
            battleManager.onFightersOrderSorted -= battleInterface.OnFightersOrderSorted;

            battleManager.onTurnOfControlledFighterStarted -= battleInterface.EnableChooseAction;
            battleManager.onTurnOfNonControlledFighterStarted -= battleInterface.EnableWaitTurn;

            battleManager.onBattleStarted -= battleCamera.StartCameraBehavior;

            battleManager.onAttackButtonSelected -= battleInterface.EnableChooseTargetInterface;
            battleManager.onAttackButtonSelected -= targetSystem.EnableTarget;
            battleManager.onAttackButtonSelected -= battleCamera.GeneralVision;

            battleManager.onTargetingFighterEnded -= targetSystem.DisableTarget;
            battleManager.onTargetingFighterEnded -= battleCamera.StartCameraBehavior;
            battleManager.onBackToMainInterfaceSelected -= battleInterface.EnableChooseAction;
            battleManager.onBackToMainInterfaceSelected -= battleCamera.StartCameraBehavior;

            targetSystem.onTargetSelected += battleManager.ValidateTargetForAttack;
        }

    }
}