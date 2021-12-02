using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

namespace BattleSystem
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] Fighter currentTurnFighter;
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
        //public event Action onAutoControlButtonSelected;
        public event Action<Fighter> onAutomaticTurnStarted;
        public event Action onEnemyTurnStarted;
        public event Action onAttackButtonSelected;
        public event Action onItemButtonSelected;
        public event Action onTargetingFightersStarted;
        public event Action onTargetingFightersFinished;
        public event Action onActionProcessStarted;
        public enum ActionMode { attack, item }
        public delegate void ActionProcess(Fighter attacker = null, Fighter target = null, ActionMode actionMode = ActionMode.attack, string itemName = "Item");
        public event ActionProcess onFighterActionHappening;
        public enum BattleResult { victory, defeat, draw }
        public event Action<BattleResult> onBattleEnds;

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
                Fighter targetOponent = null;

                if (targets.Count - 1 >= i && targets[i].isDead == false)
                    targetOponent = targets[i];            
                else
                {
                    foreach (Fighter target in targets)
                    {
                        if (target.isDead == false)
                        {
                            targetOponent = target;
                            break;
                        }
                    }
                }
                FighterLookAt(observers[i], targetOponent);
            }
        }
        void FighterLookAt(Fighter observer, Fighter target)
        {
            Vector3 targetDirection =
                new Vector3(target.transform.position.x, observer.transform.position.y, target.transform.position.z);
            observer.transform.LookAt(targetDirection);
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
        void RefreshCurrentFighter(List<Fighter> fighters) => currentTurnFighter = fighters[0];

        void StartTurn()
        {
            if (currentTurnFighter.isDead)
            {
                SkipToNextTurn();
                return;
            }
                
            onChoosingOrWaitingAction?.Invoke();
            if (heroFighters.Contains(currentTurnFighter))
            {
                print("Hero's turn");
                if (currentTurnFighter.autoControl == false)
                    onControllableTurnStarted?.Invoke();
                else
                {
                   StartCoroutine(StartAutomaticTurnProcess());
                }
                return;
            }
            else if (enemyFighters.Contains(currentTurnFighter))
            {
                print("Enemy's turn");
                onEnemyTurnStarted?.Invoke();
                StartCoroutine(StartAutomaticTurnProcess());
            }
        }
        //public void Button_AutomaticControl()
        //{
        //    if(actingCoroutine != null) StopCoroutine(actingCoroutine);

        //    currentTurnFighter.SwitchAutoControl();
        //    onAutoControlButtonSelected?.Invoke();
        //    StartTurn();
        //}
        #region AUTOMATIC TURN
        IEnumerator StartAutomaticTurnProcess()
        {
            onAutomaticTurnStarted?.Invoke(currentTurnFighter);
            yield return new WaitForSeconds(2f);

            List<Fighter> targetedTeam = GetTargetTeam(TargetTeam.oponent);
            Fighter lowerHpTarget = GetLowerHealthFighterFromTeam(targetedTeam);
            StartCoroutine(StartAttackProcess(lowerHpTarget));
        }
        enum TargetTeam { ally, oponent }
        List<Fighter> GetTargetTeam(TargetTeam targetTeam)
        {
            List<Fighter> listForSearch = new List<Fighter>();

            if (heroFighters.Contains(currentTurnFighter))
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
            else if (enemyFighters.Contains(currentTurnFighter))
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
            Fighter lowerHpLiveFighter = null;

            foreach (Fighter fighter in team)
            {
                if (fighter.isDead == false) 
                {
                    lowerHpLiveFighter = fighter;
                    break;
                }
                
            }
                
            foreach (Fighter fighter in team)
            {
                if (fighter.currentHp < lowerHpLiveFighter.currentHp && fighter.isDead == false)
                    lowerHpLiveFighter = fighter;
            }
            return lowerHpLiveFighter;
        }
        #endregion
        public void Button_BackToChooseAction()
        {
            onControllableTurnStarted?.Invoke();
            onTargetingFightersFinished?.Invoke();
            onChoosingOrWaitingAction?.Invoke();
        }
        public void Button_Attack()
        {
            print(currentTurnFighter + " will attack");
            onAttackButtonSelected?.Invoke();
            onTargetingFightersStarted?.Invoke();

        }
        public void ValidateTargetForAttack(Fighter fighterTargeted)
        {
            if (fighterTargeted.isDead)
            {
                print("Invalid Target. This fighter is Dead.");
                return;
            }
                
            if (heroFighters.Contains(fighterTargeted))
            {
                print("Invalid Target");
                return;
            }
            if (enemyFighters.Contains(fighterTargeted))
            {
                print("Valid Target");
                StartCoroutine(StartAttackProcess(fighterTargeted));
                return;
            }
        }

        IEnumerator StartAttackProcess(Fighter fighterTargeted)
        {
            onTargetingFightersFinished?.Invoke();
            onActionProcessStarted?.Invoke();
            onFighterActionHappening?.Invoke(currentTurnFighter, fighterTargeted, ActionMode.attack);
            yield return new WaitForSeconds(1f);
            Vector3 originalAttackerPosition = currentTurnFighter.transform.position;

            FighterLookAt(currentTurnFighter, fighterTargeted);
            currentTurnFighter.SetAnimation(Fighter.AnimationMotion.walk);
            #region MovingToOponent
            while (Vector3.Distance(currentTurnFighter.transform.position, fighterTargeted.transform.position) > 1f)
            {
                currentTurnFighter.transform.position =
                    Vector3.MoveTowards(currentTurnFighter.transform.position, fighterTargeted.transform.position, movingSpeed * movingSpeed * Time.deltaTime);
                yield return new WaitForSeconds(Time.deltaTime);
            }
            #endregion
            currentTurnFighter.SetAnimation(Fighter.AnimationMotion.attack);
            fighterTargeted.TakeDamage(currentTurnFighter.attack);
            //fighterTargeted.SetAnimation(Fighter.AnimationMotion.damaged);

            onFightersOrderOrStatusChanged?.Invoke(fightersOrder);
            print("order Sorted");
            yield return new WaitForSeconds(1f);
            currentTurnFighter.SetAnimation(Fighter.AnimationMotion.walk);
            #region ReturningToOriginalSpot
            while (Vector3.Distance(currentTurnFighter.transform.position, originalAttackerPosition) > 0)
            {
                currentTurnFighter.transform.position =
                    Vector3.MoveTowards(currentTurnFighter.transform.position, originalAttackerPosition, movingSpeed * movingSpeed * Time.deltaTime);
                yield return new WaitForSeconds(Time.deltaTime);               
            }
            #endregion
            currentTurnFighter.SetAnimation(Fighter.AnimationMotion.idle);
            yield return new WaitForSeconds(1f);

            CheckTeamsStatus();
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

       

        void CheckTeamsStatus()
        {
            int deadHeroes = 0;
            int deadenemies = 0;

            foreach (Fighter hero in heroFighters)
                if (hero.isDead) deadHeroes += 1;

            foreach (Fighter enemy in enemyFighters)
                if (enemy.isDead) deadenemies += 1;

            

            if (deadenemies == enemyFighters.Count)
            {
                onBattleEnds?.Invoke(BattleResult.victory);
                return;
            }
            if (deadHeroes == heroFighters.Count)
            {
                onBattleEnds?.Invoke(BattleResult.defeat);
                return;
            }
            if (deadenemies > 0) SetFightersLookRotation(heroFighters, enemyFighters);
            if (deadenemies > 0) SetFightersLookRotation(enemyFighters, heroFighters);

            SkipToNextTurn();
        }
        public void SkipToNextTurn()
        {
            //onTargetingFightersFinished?.Invoke();
            
            Fighter fighterWhoEndedHisTurn = fightersOrder[0];
            fightersOrder.Remove(fighterWhoEndedHisTurn);
            fightersOrder.Add(fighterWhoEndedHisTurn);
            onFightersOrderOrStatusChanged?.Invoke(fightersOrder);
            StartTurn();
      
        }

        public void ReloadGame()
        {
            Scene thisScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(thisScene.name);
        }
        
    }
}