using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour 
{
    [SerializeField] FighterData fighterData;
    [SerializeField] private string fighterName;
    [SerializeField] private int maxHp, currentHp;
    [SerializeField] private int attack, currentAttack;
    [SerializeField] private int speed, currentSpeed;
    void Start()
    {
        RefreshData();
    }

    void RefreshData()
    {
        fighterName = fighterData.fighterName;
        maxHp = fighterData.hp;
        currentHp = maxHp;
        attack = fighterData.attack;
        speed = fighterData.speed;
    }
    public void TakeDamage(int damageAmount = 0)
    {
        currentHp -= damageAmount; 
    }
}
