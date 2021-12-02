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
            battleManager.onFightersOrderOrStatusChanged += battleInterface.RefreshFightersOrderListInterface;

            
            battleManager.onControllableTurnStarted += battleInterface.ShowControllableTurnInterface;
            battleManager.onAutomaticTurnStarted += battleInterface.ShowAutomaticTurnInterface;

            battleManager.onChoosingOrWaitingAction += battleCamera.StartCameraBehavior;

            battleManager.onAttackButtonSelected += battleInterface.ShowTargetingFighterInterface;

            battleManager.onTargetingFightersStarted += targetSystem.EnableTargeting;
            battleManager.onTargetingFightersStarted += battleCamera.GeneralVision;

            battleManager.onTargetingFightersFinished += targetSystem.DisableTargeting;

            battleManager.onActionProcessStarted += battleCamera.GeneralVision;

            battleManager.onFighterActionHappening += battleInterface.ShowFighterActingMessage;

            targetSystem.onTargetSelected += battleManager.ValidateTargetForAttack;
            battleManager.onBattleEnds += battleInterface.ShowBattleResults;
        }


    }
}