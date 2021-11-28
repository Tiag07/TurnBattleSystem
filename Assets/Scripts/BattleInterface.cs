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
        [SerializeField] TMP_Text txtChooseTarget;

        [SerializeField] Button btnStartTurn, btnSkipTurn;
        [SerializeField] GameObject pnlChooseAction;
        [SerializeField] GameObject pnlChooseTarget;
        [SerializeField] Color titleColor;
        string titleColorHex;

        public void Start()
        {
            titleColorHex = string.Concat("#", ColorUtility.ToHtmlStringRGB(titleColor));
            txtFightersSequence.enabled = false;
            pnlChooseAction.SetActive(false);
            btnSkipTurn.gameObject.SetActive(false);
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
        }

        public void EnableChooseAction()
        {
            CloseAllInterfaces();
            pnlChooseAction.SetActive(true);
        }
        public void EnableWaitTurn()
        {
            CloseAllInterfaces();
            btnSkipTurn.gameObject.SetActive(true);
        }

        public void EnableChooseTargetInterface()
        {
            CloseAllInterfaces();

            pnlChooseTarget.SetActive(true);
        }

    }
}