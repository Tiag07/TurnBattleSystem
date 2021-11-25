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
        [SerializeField] Button btnStartBattle;
        [SerializeField] GameObject pnlChooseAction;

        public void Start()
        {
            txtFightersSequence.enabled = false;
            pnlChooseAction.SetActive(false);
            btnStartBattle.gameObject.SetActive(true);
        }
        public void OnFightersOrderSorted(List<Fighter> fighters)
        {
            txtFightersSequence.text = "<color=yellow>Order:</color><br>";
            foreach(Fighter fighter in fighters)
            {
                txtFightersSequence.text += fighter.nickName + "<br>";
            }
            txtFightersSequence.enabled = true;
        }

        public void OnFightStarted()
        {
            btnStartBattle.gameObject.SetActive(false);
            pnlChooseAction.SetActive(true);
        }
    }
}