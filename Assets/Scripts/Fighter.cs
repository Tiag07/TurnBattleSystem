using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour 
{
    [SerializeField] FighterData fighterData;
    [SerializeField] public string originalName { get; private set; }
    [SerializeField] public string nickName;
    [SerializeField] public string currentLevel; /**not used still*/
    [SerializeField] public int maxHp { get; private set; }
    [SerializeField] public int currentHp { get; private set; }
    [SerializeField] public int attack { get; private set; }
    [SerializeField] public int speed { get; private set; }
    public bool autoControl { get; private set; } = false;
    public bool isDead { get; private set; } = false;

private Animator animator;
    public enum AnimationMotion
    {
        idle, walk, attack, damaged, death
    }
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

        if (GetComponent<Animator>())
            animator = GetComponent<Animator>();
    }
    public void SwitchAutoControl()
    {
        autoControl = !autoControl;
    }

    public void SetAnimation(AnimationMotion motion)
    {
        switch (motion)
        {
            case AnimationMotion.idle:
                animator?.SetTrigger("idle");
                break;
            case AnimationMotion.walk:
                animator?.SetTrigger("walk");
                break;
            case AnimationMotion.attack:
                animator?.SetTrigger("attack");
                break;
            case AnimationMotion.damaged:
                animator?.SetTrigger("damaged");
                break;
            case AnimationMotion.death:
                animator?.SetTrigger("death");
                break;
        }
    }



    public void TakeDamage(int damageAmount = 0)
    {
        currentHp -= damageAmount;
        print("hp decreased");
        SetAnimation(AnimationMotion.damaged);
        if (currentHp <= 0)
        {
            print(currentHp);
            currentHp = 0;
            SetAnimation(AnimationMotion.death);
            isDead = true;
            return;
        }
        else SetAnimation(AnimationMotion.idle);
    }

}
