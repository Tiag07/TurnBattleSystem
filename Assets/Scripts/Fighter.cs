using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour 
{
    [SerializeField] FighterData fighterData;
    [SerializeField] private string originalName;
    [SerializeField] private string fighterName;
    [SerializeField] private int maxHp;
    [SerializeField] private int currentHp;
    [SerializeField] private int attack;
    [SerializeField] public int speed /*{ get; private set; }*/;
    void Start()
    {
        RefreshData();
    }

    void RefreshData()
    {
        originalName = fighterData.baseName;
        maxHp = fighterData.baseHp;
        currentHp = maxHp;
        attack = fighterData.baseAttack;
        speed = fighterData.baseSpeed;
    }
    public void TakeDamage(int damageAmount = 0)
    {
        currentHp -= damageAmount; 
    }
}
