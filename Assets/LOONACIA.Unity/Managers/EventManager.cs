using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EventManager
{
    public delegate void OnUnitDeath(Unit deadUnit_, AttackInfo attackInfo_);
    public delegate void OnIAttackerStartAttack(IAttacker attacker_, AttackInfo attackInfo_);
    public delegate void OnIDamageableTakeHit(IDamageable damaged_, AttackInfo attackInfo_, int modifiedDamage_, ref int finalDamage_);
    public delegate void OnUnitSpawn(Unit newUnit_);
    public delegate void OnRecallAction();
    public delegate void OnRecallLanded();
    public delegate void OnRecallStarted();
    public delegate void OnUnitAttack(Unit attackerUnit_, AttackInfo attackInfo_);
    public delegate void OnUnitGrind(Unit unit_);
    public delegate void OnUnitSoulWhip(Unit damagedUnit_, AttackInfo attackInfo_);
    public delegate void OnUnitTakeFear(Unit fearedUnit_, AttackInfo attackInfo_);
    
    public OnUnitDeath onUnitDeath;
    public OnIAttackerStartAttack onIAttackerStartAttack; //TODO: 이벤트를 호출해야함. 아직 없음.
    public OnIDamageableTakeHit onIDamageableTakeHit;
    public OnUnitSpawn onUnitSpawn;
    public OnRecallLanded onRecallLanded;
    public OnRecallAction onRecallActionEnd;
    public OnUnitAttack onUnitAttack;
    public OnUnitGrind onUnitGrind;
    public OnUnitSoulWhip onUnitSoulWhip;
    public OnRecallStarted onRecallStarted;
    public OnUnitTakeFear onUnitTakeFear;

    public void Init()
    {
        onIDamageableTakeHit += HealthDrain;
    }
    
    public void Clear()
    {
        onUnitDeath = null;
        onIAttackerStartAttack = null;
        onIDamageableTakeHit = null;
        onUnitSpawn = null;
        onRecallLanded = null;
        onRecallActionEnd = null;
        onUnitAttack = null;
        onUnitGrind = null;
        onUnitSoulWhip = null;
        onRecallStarted = null;
    }
    
    private void HealthDrain(IDamageable damaged_, AttackInfo attackInfo_, int modifiedDamage_, ref int finalDamage_)
    {
        //공격자가 유닛이라면 최종 대미지와 흡혈 비율에 비례해 체력을 회복한다.
        if (attackInfo_.attacker is Unit attackerUnit)
        {
            var drainRatio = attackerUnit.instanceStats.FinalHealthDrain / 100f;
            if (drainRatio <= 0f) drainRatio = 0;
            int healAmount = (int) (finalDamage_ * drainRatio);
            if (healAmount <= 0) return;
            (attackInfo_.attacker as IDamageable)?.TakeHeal(healAmount, null);
        }
    }
}
