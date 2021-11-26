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
         

        void Awake()
        {
            battleManager = GetComponent<BattleManager>();
            battleInterface = GetComponent<BattleInterface>();
            battleCamera = FindObjectOfType<BattleCamera>();

            battleManager.fightersOrderSorted += battleInterface.OnFightersOrderSorted;
            battleManager.controlledFighterTurn += battleInterface.EnableOrDisableControlUI;
            battleManager.battleStarted += battleCamera.StartCameraBehavior;
        }
        
    }
}