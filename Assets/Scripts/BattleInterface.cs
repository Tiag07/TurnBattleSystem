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

        public void Start()
        {
            txtFightersSequence.enabled = false;
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
    }
}