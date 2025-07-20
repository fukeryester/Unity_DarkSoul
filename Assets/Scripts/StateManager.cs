using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : IActorManagerInterface
{
    public float HP = 100;
    public float HPMax = 100;

    [Header("1st order state flags")]
    public bool isGround;
    public bool isJump;
    public bool isRoll;
    public bool isJab;
    public bool isFall;
    public bool isAttack;
    public bool isHit;
    public bool isDie;
    public bool isBlocked;
    public bool isDefence;

    [Header("2nd order state flags")]
    public bool isAllowDefence;
    public bool isImmortal;

    private void Start()
    {
        InitializeHP();
    }

    private void Update()
    {
        isGround = am.ac.CheckState("Ground");
        isJump = am.ac.CheckState("Jump");
        isRoll = am.ac.CheckState("Roll");
        isJab = am.ac.CheckState("Jab");
        isFall = am.ac.CheckState("Fall");
        isHit = am.ac.CheckState("Hit");
        isBlocked = am.ac.CheckState("Blocked");
        isDie = am.ac.CheckState("Die");
        isAttack = am.ac.CheckStateTag("attackR") || am.ac.CheckStateTag("attackL");

        isAllowDefence = isGround || isBlocked;
        isDefence = isAllowDefence && am.ac.CheckState("Defence", "Defence");
        isImmortal = isRoll || isJab;
    }

    public void InitializeHP()
    {
        HP = HPMax;
    }

    public void ChangeHP(float value)
    {
        HP += value;
        HP = Mathf.Clamp(HP, 0, HPMax);
    }
}
