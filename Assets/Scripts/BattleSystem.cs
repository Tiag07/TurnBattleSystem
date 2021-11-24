using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] Transform[] heroSpots, enemySpots;
    [SerializeField] List<Fighter> heroFighters;
    [SerializeField] List<Fighter> enemyFighters;
    [SerializeField] List<Fighter> fighterSequence;
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
    void Start()
    {
        StartBattleTest(heroFighters, enemyFighters);
    }
    public void StartBattleTest(List<Fighter> heroes, List<Fighter> enemies)
    {
        EnableFighters(heroes, enemies);
        SetFightersPosition(heroes, enemies);
        SetFightersLookRotation(heroes, enemies);
        SetFightersLookRotation(enemies, heroes);
        SetFightersSequence(heroes, enemies);
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

    void SetFightersSequence(List<Fighter> heroes, List<Fighter> enemies)
    {
        List<Fighter> allFightersList = new List<Fighter>();

        foreach (Fighter hero in heroes) allFightersList.Add(hero);
        foreach (Fighter enemy in enemies) allFightersList.Add(enemy);

        foreach (Fighter fighter in allFightersList)
        {           
            print(fighter.gameObject.name + " " + fighter.speed);
        } 
            
    }
}

