using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] Transform[] heroSpots, enemySpots;
    [SerializeField] List<GameObject> heroFighters;
    [SerializeField] List<GameObject> enemyFighters;

    public Transform RandomFighter
    {
        get
        {
            int randomNumber = Random.Range(0, 10);
            List<GameObject> listToGetFighter = new List<GameObject>();
            listToGetFighter = randomNumber > 4 ? heroFighters : enemyFighters;

            int randomFighter = Random.Range(0, listToGetFighter.Count);
            return listToGetFighter[randomFighter].transform;
            
        }
    }
    void Start()
    {
        StartBattleTest(heroFighters, enemyFighters);
    }
    public void StartBattleTest(List<GameObject> heroes, List<GameObject> enemies)
    {
        EnableFighters(heroes, enemies);
        SetFightersPosition(heroes, enemies);
        SetFightersLookRotation(heroes, enemies);
        SetFightersLookRotation(enemies, heroes);

    }
    void EnableFighters(List<GameObject> heroes, List<GameObject> enemies)
    {
        for (int i = 0; i < heroes.Count; i++)
        {
            heroes[i].SetActive(true);
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].SetActive(true);
        }
    }
    void SetFightersPosition(List<GameObject> heroes, List<GameObject> enemies)
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
    public void SetFightersLookRotation(List<GameObject> observers, List<GameObject> targets)
    {
        for (int i = 0; i < observers.Count; i++)
        {
            Vector3 targetEnemy = Vector3.zero;

            if (targets.Count - 1 >= i && targets[i].activeSelf)
                targetEnemy = targets[i].transform.position;
            else
            {
                foreach (GameObject enemy in targets)
                {
                    if (enemy.activeSelf)
                    {
                        targetEnemy = enemy.transform.position;
                        break;
                    }
                }
            }

            Vector3 enemyDirection = new Vector3(targetEnemy.x, observers[i].transform.position.y, targetEnemy.z);
            observers[i].transform.LookAt(enemyDirection);
        }
    }


}
