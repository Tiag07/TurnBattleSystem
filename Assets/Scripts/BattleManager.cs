using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace BattleSystem
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] Transform[] heroSpots, enemySpots;
        [SerializeField] List<Fighter> heroFighters;
        [SerializeField] List<Fighter> enemyFighters;
        [SerializeField] List<Fighter> fightersOrder;
        public List<Fighter> GetListWithAllFighters(List<Fighter> heroes, List<Fighter> enemies)
        {
            List<Fighter> allFightersList = new List<Fighter>();

            foreach (Fighter hero in heroes) allFightersList.Add(hero);
            foreach (Fighter enemy in enemies) allFightersList.Add(enemy);

            return allFightersList;
        }
        public Transform GetRandomFighter
        {
            get
            {
                int randomNumber = Random.Range(0, 10);
                List<Fighter> listToGetFighter = new List<Fighter>();
                listToGetFighter = randomNumber > 4 ? heroFighters : enemyFighters;

                int randomFighter = Random.Range(0, listToGetFighter.Count);
                return listToGetFighter[randomFighter].transform;

            }
        }

        public event Action<List<Fighter>> onFightersOrderSorted;
        public event Action onMainPhaseStarted;
        public event Action<BattleState> onBattleStateChanged;
        public event Action onTargetingStarted;
        public enum ActionMode { attack, item }
        public delegate void ActionProcess(string attacker = "Fighter ", ActionMode actionMode = ActionMode.attack, string target = "Oponent!", string itemName = "Item");
        public ActionProcess onActionPhase;

        public event Action onTargetingEnded;
        

        [SerializeField] Fighter currentFighter;
        public enum BattleState
        {
            TurnOfControlledFighterStarted,
            TurnOfNonControlledFighterStarted,
            MainPhase,
            TargetingAtkPhase,            
            TargetingItemPhase,
            ActionPhase, 
            WaitingAttackPhase,
        }
        
        public void StartBattle( )
        {
            onFightersOrderSorted += RefreshCurrentFighter;
            List<Fighter> heroes = heroFighters;
            List<Fighter> enemies = enemyFighters;

            EnableFighters(heroes, enemies);
            RefreshFightersStats(heroes, enemies);
            SetFightersPosition(heroes, enemies);
            SetFightersLookRotation(heroes, enemies);
            SetFightersLookRotation(enemies, heroes);
            InitialFightersOrderSort(heroes, enemies);

            StartTurn();
        }
        void EnableFighters(List<Fighter> heroes, List<Fighter> enemies)
        {
            for (int i = 0; i < heroes.Count; i++)
            {
                heroes[i].gameObject.SetActive(true);
            }
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].gameObject.SetActive(true);
            }
        }
        void RefreshFightersStats(List<Fighter> heroes, List<Fighter> enemies)
        {
            List<Fighter> allFightersList = GetListWithAllFighters(heroes, enemies);

            foreach (Fighter fighter in allFightersList)
                fighter.RefreshStats();

        }
        void SetFightersPosition(List<Fighter> heroes, List<Fighter> enemies)
        {
            for (int i = 0; i < heroes.Count; i++)
            {
                heroes[i].transform.position = heroSpots[i].position;
            }
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].transform.position = enemySpots[i].position;
            }
        }
        public void SetFightersLookRotation(List<Fighter> observers, List<Fighter> targets)
        {
            for (int i = 0; i < observers.Count; i++)
            {
                Vector3 targetEnemy = Vector3.zero;

                if (targets.Count - 1 >= i && targets[i].gameObject.activeSelf)
                    targetEnemy = targets[i].transform.position;
                else
                {
                    foreach (Fighter target in targets)
                    {
                        if (target.gameObject.activeSelf)
                        {
                            targetEnemy = target.transform.position;
                            break;
                        }
                    }
                }

                Vector3 enemyDirection = new Vector3(targetEnemy.x, observers[i].transform.position.y, targetEnemy.z);
                observers[i].transform.LookAt(enemyDirection);
            }
        }
        public void InitialFightersOrderSort(List<Fighter> heroes, List<Fighter> enemies)
        {
            List<Fighter> allFightersList = GetListWithAllFighters(heroes, enemies);
            Fighter fastestFighter;
            List<Fighter> newFightersOrder = new List<Fighter>();

            while (allFightersList.Count > 0)
            {
                fastestFighter = allFightersList[0];
                foreach (Fighter fighter in allFightersList)
                {
                    if (fighter.speed > fastestFighter.speed)
                        fastestFighter = fighter;
                }
                newFightersOrder.Add(fastestFighter);
                allFightersList.Remove(fastestFighter);
            }
            fightersOrder = new List<Fighter>(newFightersOrder);
            onFightersOrderSorted?.Invoke(newFightersOrder);

        }
        void RefreshCurrentFighter(List<Fighter> fighters) => currentFighter = fighters[0];

        void StartTurn()
        {
            if (heroFighters.Contains(currentFighter))
            {
                print("Hero's turn");
                if(currentFighter.autoControl == false)
                {
                    onBattleStateChanged?.Invoke(BattleState.TurnOfControlledFighterStarted);
                    onMainPhaseStarted?.Invoke();
                } else onBattleStateChanged?.Invoke(BattleState.TurnOfNonControlledFighterStarted);

            }

            if (enemyFighters.Contains(currentFighter)) 
            {
                print("Enemy's turn");
                onBattleStateChanged?.Invoke(BattleState.TurnOfNonControlledFighterStarted);
            } 
            
            
        }
        public void Button_BackToChooseAction()
        {
            onBattleStateChanged?.Invoke(BattleState.TurnOfControlledFighterStarted);
            onTargetingEnded?.Invoke();
            onMainPhaseStarted?.Invoke();

        }
        public void Button_Attack()
        {
            print(currentFighter + " will attack");
            onBattleStateChanged?.Invoke(BattleState.TargetingAtkPhase);
            onTargetingStarted?.Invoke();
            
        }
        public void ValidateTargetForAttack(Fighter fighterTargeted)
        {
            if (heroFighters.Contains(fighterTargeted))
            {
                print("Invalid Target");
            }
            if (enemyFighters.Contains(fighterTargeted))
            {
                print("Valid Target");
                StartCoroutine(StartAttackProcess(fighterTargeted));
            }
        }
        IEnumerator StartAttackProcess(Fighter fighterTargeted)
        {
            onTargetingEnded?.Invoke();
            onActionPhase?.Invoke(currentFighter.nickName, ActionMode.attack ,fighterTargeted.nickName);
            onBattleStateChanged?.Invoke(BattleState.ActionPhase);

            Vector3 originalAttackerPosition = currentFighter.transform.position;
            Vector3 spotForAttackOponent = fighterTargeted.transform.position + fighterTargeted.transform.forward*2;
            currentFighter.transform.position = spotForAttackOponent;

            yield return new WaitForSeconds(2f);
            fighterTargeted.TakeDamage(currentFighter.attack);
            yield return new WaitForSeconds(1f);
            currentFighter.transform.position = originalAttackerPosition;
            SkipFighterTurn();
        }

        public void SkipFighterTurn()
        {
            onTargetingEnded?.Invoke();
            int deadHeroes = 0;
            int deadenemies = 0;
            Fighter fighterWhoEndedHisTurn = fightersOrder[0];
            fightersOrder.Remove(fighterWhoEndedHisTurn);
            fightersOrder.Add(fighterWhoEndedHisTurn);
            onFightersOrderSorted?.Invoke(fightersOrder);
            StartTurn();

            foreach (Fighter hero in heroFighters)
                if (hero.isDead) deadHeroes += 1;

            foreach (Fighter enemy in enemyFighters)
                if (enemy.isDead) deadenemies += 1;

            if (deadHeroes == heroFighters.Count) LoseBattle();
            if (deadenemies == heroFighters.Count) winBattle();
            
        }

        void LoseBattle() { }
        void winBattle() { }

    }
}