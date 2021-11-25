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
            battleManager = FindObjectOfType<BattleManager>();
            battleInterface = FindObjectOfType<BattleInterface>();
            battleCamera = FindObjectOfType<BattleCamera>();

            battleManager.fightersOrderSorted += battleInterface.OnFightersOrderSorted;
            battleManager.battleStarted += battleInterface.OnFightStarted;
            battleManager.battleStarted += battleCamera.StartCameraBehavior;
            
        }
        
    }
}