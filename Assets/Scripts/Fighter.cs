using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour 
{
    [SerializeField] FighterData fighterData;
    [SerializeField] public string originalName { get; private set; }
    [SerializeField] public string nickName;
    [SerializeField] public string currentLevel;
    [SerializeField] public int maxHp { get; private set; }
    [SerializeField] public int currentHp { get; private set; }
    [SerializeField] public int attack { get; private set; }
    [SerializeField] public int speed { get; private set; }
    public bool autoControl = false;
    public bool isDead = false;
    void Start()
    {
        //RefreshData();
    }

    public void RefreshStats()
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
