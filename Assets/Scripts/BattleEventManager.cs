﻿using System.Collections;
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

        void Awake()
        {
            battleManager = GetComponent<BattleManager>();
            battleInterface = GetComponent<BattleInterface>();
            battleCamera = FindObjectOfType<BattleCamera>();
            targetSystem = GetComponent<TargetSystem>();

            battleManager.fightersOrderSorted += battleInterface.OnFightersOrderSorted;

            battleManager.controlledFighterTurn += battleInterface.EnableChooseAction;
            battleManager.nonControlledFighterTurn += battleInterface.EnableWaitTurn;

            battleManager.battleStarted += battleCamera.StartCameraBehavior;

            battleManager.actionAttackSelected += battleInterface.EnableChooseTargetInterface;
            battleManager.actionAttackSelected += targetSystem.EnableTarget;
            battleManager.actionAttackSelected += battleCamera.GeneralVision;

            battleManager.targetingEnded += targetSystem.DisableTarget;
            battleManager.targetingEnded += battleCamera.StartCameraBehavior;
            battleManager.backToChooseActionSelected += battleInterface.EnableChooseAction;
            battleManager.backToChooseActionSelected += battleCamera.StartCameraBehavior;

            targetSystem.targetSelected += battleManager.ValidateTargetForAttack;

        }
        
    }
}