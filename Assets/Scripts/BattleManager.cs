using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace BattleSystem
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] Fighter currentFighterTurn;
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

        public event Action<List<Fighter>> onFightersOrderOrStatusChanged;
        public event Action onChoosingOrWaitingAction;
        public event Action onControllableTurnStarted;
        public event Action<Fighter> onAutomaticTurnStarted;
        public event Action onAttackButtonSelected;
        public event Action onItemButtonSelected;
        public event Action onTargetingFightersStarted;
        public event Action onTargetingFightersFinished;
        public event Action onActionProcessStarted;
        public enum ActionMode { attack, item }
        public delegate void ActionProcess(Fighter attacker = null, Fighter target = null, ActionMode actionMode = ActionMode.attack, string itemName = "Item");
        public event ActionProcess onFighterActionHappening;

        public void StartBattle()
        {
            onFightersOrderOrStatusChanged += RefreshCurrentFighter;
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
            onFightersOrderOrStatusChanged?.Invoke(newFightersOrder);

        }
        void RefreshCurrentFighter(List<Fighter> fighters) => currentFighterTurn = fighters[0];

        void StartTurn()
        {
            onChoosingOrWaitingAction?.Invoke();
            if (heroFighters.Contains(currentFighterTurn))
            {
                print("Hero's turn");
                if (currentFighterTurn.autoControl == false)
                    onControllableTurnStarted?.Invoke();
                else
                {
                   StartCoroutine(StartAutomaticTurnProcess());
                }
                return;
            }
            else if (enemyFighters.Contains(currentFighterTurn))
            {
                print("Enemy's turn");
                StartCoroutine(StartAutomaticTurnProcess());
            }
        }
        IEnumerator StartAutomaticTurnProcess()
        {
            onAutomaticTurnStarted?.Invoke(currentFighterTurn);
            yield return new WaitForSeconds(2f);

            List<Fighter> targetedTeam = GetTargetTeam(TargetTeam.oponent);
            Fighter lowerHpTarget = GetLowerHealthFighterFromTeam(targetedTeam);
           StartCoroutine(StartAttackProcess(lowerHpTarget));
            print("auto turn end");
        }

        public void Button_BackToChooseAction()
        {
            onControllableTurnStarted?.Invoke();
            onTargetingFightersFinished?.Invoke();
            onChoosingOrWaitingAction?.Invoke();
        }
        public void Button_Attack()
        {
            print(currentFighterTurn + " will attack");
            onAttackButtonSelected?.Invoke();
            onTargetingFightersStarted?.Invoke();

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
            onTargetingFightersFinished?.Invoke();
            onActionProcessStarted?.Invoke();
            onFighterActionHappening?.Invoke(currentFighterTurn, fighterTargeted, ActionMode.attack);
            yield return new WaitForSeconds(1f);
            Vector3 originalAttackerPosition = currentFighterTurn.transform.position;
            //Vector3 spotForAttackOponent = fighterTargeted.transform.position + fighterTargeted.transform.forward * 2;

            currentFighterTurn.SetAnimation(Fighter.AnimationMotion.walk);
            #region MovingToOponent
            while (Vector3.Distance(currentFighterTurn.transform.position, fighterTargeted.transform.position) > 1f)
            {
                currentFighterTurn.transform.position =
                    Vector3.MoveTowards(currentFighterTurn.transform.position, fighterTargeted.transform.position, movingSpeed * movingSpeed * Time.deltaTime);
                yield return new WaitForSeconds(Time.deltaTime);
            }
            #endregion
            currentFighterTurn.SetAnimation(Fighter.AnimationMotion.attack);
            fighterTargeted.TakeDamage(currentFighterTurn.attack);
            fighterTargeted.SetAnimation(Fighter.AnimationMotion.damaged);

            onFightersOrderOrStatusChanged?.Invoke(fightersOrder);
            yield return new WaitForSeconds(1f);
            currentFighterTurn.SetAnimation(Fighter.AnimationMotion.walk);
            #region ReturningToOriginalSpot
            while (Vector3.Distance(currentFighterTurn.transform.position, originalAttackerPosition) > 0)
            {
                currentFighterTurn.transform.position =
                    Vector3.MoveTowards(currentFighterTurn.transform.position, originalAttackerPosition, movingSpeed * movingSpeed * Time.deltaTime);
                yield return new WaitForSeconds(Time.deltaTime);               
            }
            #endregion
            currentFighterTurn.SetAnimation(Fighter.AnimationMotion.idle);
            yield return new WaitForSeconds(1f);

            SkipToNextTurn();
        }
        #region ACTION_ANIMATIONS

        private bool moveFighter = false;
        private Transform fighterForMoving;
        private Vector3 originalSpot;
        private Vector3 targetSpot;
        private float movingSpeed = 2.5f;
        void MoveFighterToTarget(Transform body, Vector3 originalPosition, Vector3 destinyPosition)
        {
            fighterForMoving = body;
            originalSpot = originalPosition;
            targetSpot = destinyPosition;
            moveFighter = true;
        }
        private void Update()
        {
            if (!moveFighter) return;

            if(fighterForMoving.position != targetSpot)
            {
                fighterForMoving.position = Vector3.MoveTowards(fighterForMoving.position, targetSpot, movingSpeed * Time.deltaTime);
            }
            else moveFighter = false;
        }
        #endregion

        enum TargetTeam { ally, oponent }
        List<Fighter> GetTargetTeam(TargetTeam targetTeam)
        {
            List<Fighter> listForSearch = new List<Fighter>();

            if (heroFighters.Contains(currentFighterTurn))
            {
                switch (targetTeam)
                {
                    case TargetTeam.ally:
                        listForSearch = new List<Fighter>(heroFighters);
                        break;
                    case TargetTeam.oponent:
                        listForSearch = new List<Fighter>(enemyFighters);
                        break;
                }
            }
            else if (enemyFighters.Contains(currentFighterTurn))
            {
                switch (targetTeam)
                {
                    case TargetTeam.ally:
                        listForSearch = new List<Fighter>(enemyFighters);
                        break;
                    case TargetTeam.oponent:
                        listForSearch = new List<Fighter>(heroFighters);
                        break;
                }
            }
            return listForSearch;
        }
        Fighter GetLowerHealthFighterFromTeam(List<Fighter> team)
        {
            Fighter lowerHpFighter = team[0];
            foreach(Fighter fighter in team)
            {
                if (fighter.currentHp < lowerHpFighter.currentHp)
                    lowerHpFighter = fighter;
            }
            return lowerHpFighter;
        }

        public void SkipToNextTurn()
        {
            onTargetingFightersFinished?.Invoke();
            int deadHeroes = 0;
            int deadenemies = 0;
            Fighter fighterWhoEndedHisTurn = fightersOrder[0];
            fightersOrder.Remove(fighterWhoEndedHisTurn);
            fightersOrder.Add(fighterWhoEndedHisTurn);
            onFightersOrderOrStatusChanged?.Invoke(fightersOrder);
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