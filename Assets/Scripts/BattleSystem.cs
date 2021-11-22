using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] Transform[] heroSpots, enemySpots;
    [SerializeField] GameObject[] heroesTest;
    [SerializeField] GameObject[] enemiesTest;
    void Start()
    {
        StartBattleTest(heroesTest, enemiesTest);
    }

    public void StartBattleTest(GameObject[] heroes, GameObject[] enemies)
    { 
        SetFightersPosition(heroes, enemies);
        SetFightersLookRotation(heroes, enemies);
        EnableFighters(heroes, enemies);
    }

    void SetFightersPosition(GameObject[] heroes, GameObject[] enemies)
    {
        for (int i = 0; i < heroes.Length; i++)
        {
            heroes[i].transform.position = heroSpots[i].position;
        }
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].transform.position = enemySpots[i].position;
        }
    }

    void SetFightersLookRotation(GameObject[] heroes, GameObject[] enemies)
    {
        
    }
    void EnableFighters(GameObject[] heroes, GameObject[] enemies)
    {
        for (int i = 0; i < heroes.Length; i++)
        {
            heroes[i].SetActive(true);
        }
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].SetActive(true);
        }
    }
}
