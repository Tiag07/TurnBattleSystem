using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BattleSystem
{
    public class BattleInterface : MonoBehaviour
    {
        [SerializeField] TMP_Text txtFightersSequence;
        [SerializeField] TMP_Text txtCurrentFighter;
        [SerializeField] TMP_Text txtFighterActing;

        [SerializeField] Button btnStartTurn, btnSkipTurn;
        [SerializeField] GameObject pnlChooseAction;
        [SerializeField] GameObject pnlChooseTarget;
        [SerializeField] Color titleColor;
        string titleColorHex;

        public void Start()
        {
            titleColorHex = string.Concat("#", ColorUtility.ToHtmlStringRGB(titleColor));
            txtFightersSequence.enabled = false;
            CloseAllInterfaces();
            btnStartTurn.gameObject.SetActive(true);
        }
        public void OnFightersOrderSorted(List<Fighter> fighters)
        {
            txtFightersSequence.text = string.Concat("<color=",titleColorHex,">Order:</color><br>");
            foreach(Fighter fighter in fighters)
            {
                txtFightersSequence.text += string.Concat(fighter.nickName);
                txtFightersSequence.text += string.Concat(" <color=", titleColorHex, ">Lv. </color>", fighter.currentLevel);
                txtFightersSequence.text += string.Concat(" <color=", titleColorHex, ">HP: </color>", fighter.currentHp, "/", fighter.maxHp);
                txtFightersSequence.text += "<br>";
            }
            txtFightersSequence.enabled = true;

            Fighter currentFighter = fighters[0];
            RefreshCurrentFighterInterface(currentFighter);
        }

        void RefreshCurrentFighterInterface(Fighter currentFighter)
        {     
            txtCurrentFighter.text = string.Concat("<color=", titleColorHex, ">Fighter Turn:</color><br>", currentFighter.nickName);
            txtCurrentFighter.text += string.Concat("<br><color=", titleColorHex, ">HP:</color><br>", currentFighter.maxHp, "/", currentFighter.currentHp);
        }
        public void CloseAllInterfaces()
        {
            btnStartTurn.gameObject.SetActive(false);
            btnSkipTurn.gameObject.SetActive(false);
            pnlChooseAction.SetActive(false);
            pnlChooseTarget.SetActive(false);
            txtFighterActing.gameObject.SetActive(false);
        }


        public void RefreshBattleStateInterface(BattleManager.BattleState battleState)
        {
            CloseAllInterfaces();
            switch (battleState)
            {
                case BattleManager.BattleState.TurnOfControlledFighterStarted:
                    pnlChooseAction.SetActive(true);
                    break;
                case BattleManager.BattleState.TurnOfNonControlledFighterStarted:
                    btnSkipTurn.gameObject.SetActive(true);
                    break;
                case BattleManager.BattleState.MainPhase:
                    pnlChooseAction.SetActive(true);
                    break;
                case BattleManager.BattleState.TargetingAtkPhase:
                    pnlChooseTarget.SetActive(true);
                    break;
                case BattleManager.BattleState.ActionPhase:
                    txtFighterActing.gameObject.SetActive(true);
                    break;
                case BattleManager.BattleState.TargetingItemPhase:
                    
                    break;
                case BattleManager.BattleState.WaitingAttackPhase:
                    txtFighterActing.gameObject.SetActive(true);
                    break;
            }
        }
        public void ShowActionMessage(string attacker, BattleManager.ActionMode actionMode, string target, string item)
        {
            string actionMessage = "";
            switch (actionMode)
            {
                case BattleManager.ActionMode.attack:
                    actionMessage = " is attacking ";
                    break;
                case BattleManager.ActionMode.item:
                    actionMessage = string.Concat(" is using ", item, " in ");
                    break;
            }
            txtFighterActing.text = string.Concat(attacker, actionMessage, target);
        }

    }
}