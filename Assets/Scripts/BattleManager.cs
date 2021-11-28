using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
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
        public Transform RandomFighter
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

        public event Action<List<Fighter>> fightersOrderSorted;
        public event Action battleStarted;
        public event Action controlledFighterTurn;
        public event Action nonControlledFighterTurn;
        public event Action actionAttackSelected;
        public event Action backToChooseActionSelected;
        public event Action targetingEnded;

        [SerializeField] Fighter currentFighter;
        [SerializeField] TargetSystem targetSystem;
        public enum CurrentAttackAllowedTargets
        {
            onlyAllies, onlyEnemies, allFighters
        } 
        CurrentAttackAllowedTargets currentAttackAllowedTargets;
        public enum BattleState
        {
           choosingAction, choosingAtkTarget
        }
        BattleState battleState = BattleState.choosingAction;
        
        void Start()
        {
            //StartBattleTest(heroFighters, enemyFighters);
        }
        public void StartBattle( )
        {
            fightersOrderSorted += RefreshCurrentFighter;
            List<Fighter> heroes = heroFighters;
            List<Fighter> enemies = enemyFighters;

            EnableFighters(heroes, enemies);
            RefreshFightersStats(heroes, enemies);
            SetFightersPosition(heroes, enemies);
            SetFightersLookRotation(heroes, enemies);
            SetFightersLookRotation(enemies, heroes);
            InitialFightersOrderSort(heroes, enemies);
  
            battleStarted?.Invoke();

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
            fightersOrderSorted?.Invoke(newFightersOrder);

        }
        void RefreshCurrentFighter(List<Fighter> fighters) => currentFighter = fighters[0];

        void StartTurn()
        {
            if (heroFighters.Contains(currentFighter))
            {
                print("Hero's turn");
                if(currentFighter.autoControl == false)
                {
                    controlledFighterTurn?.Invoke();
                } else nonControlledFighterTurn?.Invoke();

            }

            if (enemyFighters.Contains(currentFighter)) 
            {
                print("Enemy's turn");
                nonControlledFighterTurn?.Invoke();
            } 
            
            
        }
        public void Button_BackToChooseAction()
        {
            backToChooseActionSelected?.Invoke();
            targetingEnded?.Invoke();
        }
        public void Button_Attack()
        {
            print(currentFighter + " will attack");
            actionAttackSelected?.Invoke();
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
                fighterTargeted.TakeDamage(currentFighter.attack);
                SkipFighterTurn();
            }
        }

        public void SkipFighterTurn()
        {
            targetingEnded?.Invoke();
            int deadHeroes = 0;
            int deadenemies = 0;
            Fighter fighterWhoEndedHisTurn = fightersOrder[0];
            fightersOrder.Remove(fighterWhoEndedHisTurn);
            fightersOrder.Add(fighterWhoEndedHisTurn);
            fightersOrderSorted?.Invoke(fightersOrder);
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