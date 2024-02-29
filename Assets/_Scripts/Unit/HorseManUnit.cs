using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using BehaviorDesigner.Runtime;
using LOONACIA.Unity.Managers;
using Pathfinding;

using UnityEngine;

public class HorseManUnit : Unit
{
    [Header("Charge")]
    public float chargeMoveSpeed = 5f; //달릴때 속도
    public float SpecialAttackDetectRadius = 10f; //돌진 감지 범위
    public float chargeKnockBackPower = 100f; //넉백 파워
    public float chargeKnockDownRadius = 2; //넉다운 범위
    public float ChargeCoolTime = 1f; //차지 쿨타임
    public float ChargeEnergyTime = 3f; //돌진 전에 가만히 서있는 시간
    public float chargeAttackTime = 1.5f; // 돌진 박는 시간
    private float speedMultiplier = 1f;
    private float _soundPlayDelay = 0f;
    Vector2 _chargeDirection;

    private float SetDirectionTime;
    private List<Unit> _unitsAttackedPerCharge = new();
    private bool _hasHitPlayer;


    public override void Start()
    {
        base.Start();
        reviveAudio = "Small Monster Breathing Larger 01";
        IsSpecialAttackable = true;
        SetDirectionTime = ChargeEnergyTime - .5f;
    }
    protected override void Init()
    {
        base.Init();
        if(CurrentFaction == Faction.Undead){
            UIWinPopup.IsFamilyImage[(int)UIWinPopup.Images.HorseManImage] = true;

			SteamUserData.IsHorsemanRevived = true;
		}
    }
    public override void Update()
    {
        base.Update();
        
        //SET ATTACK TARGET
        if (AttackTarget == null)
        {
            if (CurrentFaction == Faction.Human)
            {
                AttackTarget = GameManager.Instance.GetPlayer().gameObject;
            }
            else
            {
                //가장 최대체력이 높은 적을 타겟으로
                float maxHp = 0;
                foreach (var unit in ManagerRoot.Unit.GetAllAliveHumanUnits())
                {
                    if (unit.GetBaseStats().BaseMaxHp > maxHp)
                    {
                        maxHp = unit.GetBaseStats().BaseMaxHp;
                        AttackTarget = unit.gameObject;
                    }
                }
            }
        }
        
        if(!IsDead && IsSpecialAttacking && IsWalking)
        {
            if (_soundPlayDelay <= 0f)
            {
                if (CurrentFaction == Faction.Human)
                {
                    ManagerRoot.Sound.PlaySfx("Horse Footsteps Metal_4", 1f);
                }
                else
                {
                    ManagerRoot.Sound.PlaySfx("Horse Footsteps Metal_2", 1f);
                }
                _soundPlayDelay = 0.3f;
            }
            else
            {
                _soundPlayDelay -= Time.deltaTime;
            }
            Collider2D[] detectEnemies_ = new Collider2D[100];
            Physics2D.OverlapCircleNonAlloc(transform.position, DetectRadius, detectEnemies_, targetLayerMask);
            foreach (var thing in detectEnemies_)
            {
                if(thing == null) break;
                if(thing == _collider) continue;

                AttackInfo atkInfo = new AttackInfo(this, instanceStats.FinalAttackDamage, attackingMedium: transform);
                if(_collider.Distance(thing).distance <= chargeKnockDownRadius){
                    if (thing.gameObject.GetComponent<Unit>() is {} unit)
                    {
                        if (!_unitsAttackedPerCharge.Contains(unit))
                        {
                            _unitsAttackedPerCharge.Add(unit);
                            unit.TakeDamage(atkInfo);
                            var attackSounds = new List<string> {"Punch Impact (Flesh) 3", "Punch Impact (Flesh) 5", "Punch Impact (Flesh) 6"};
                            ManagerRoot.Sound.PlaySfx(attackSounds[UnityEngine.Random.Range(0, attackSounds.Count)], 1f);
                        }
                        unit.TakeKnockDown(transform, chargeKnockBackPower);
                    }
                    if (thing.gameObject.TryGetComponent(out Player player) && player != null)
                    {
                        if (_hasHitPlayer == false)
                        {
                            _hasHitPlayer = true;
                            player.TakeDamage(atkInfo);
                            var attackSounds = new List<string> {"Punch Impact (Flesh) 3", "Punch Impact (Flesh) 5", "Punch Impact (Flesh) 6"};
                            ManagerRoot.Sound.PlaySfx(attackSounds[UnityEngine.Random.Range(0, attackSounds.Count)], 1f);
                        }
                        player.TakeKnockDown(transform, chargeKnockBackPower, 2f);
                    }
                }
            }
        }

        if (!IsDead && !IsSpecialAttacking && IsSpecialAttackable && IsWalking)
        {
            // 플레이어가 특수공격 감지 범위 안에 들어오면 특수공격
            if (AttackTarget != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, AttackTarget.transform.position);
                if (distanceToPlayer <= SpecialAttackDetectRadius)
                {
                    AISpecialAttack();
                }
            }
        }
        
        if (!IsDead && IsSpecialAttacking && !IsWalking)
        {
            if (AttackTarget != null)
            {
                if (AttackTarget.transform.position.x > transform.position.x)
                {
                    SetFlipableLocalScaleX(false);
                }
                else
                {
                   SetFlipableLocalScaleX(true);
                }
            }
        }

        if (speedMultiplier > 1f)
        {
            speedMultiplier -= Time.deltaTime * 0.5f;
            if (speedMultiplier < 1f)
            {
                speedMultiplier = 1f;
            }
        }
    }
    
    public override void TurnUndeadEvent()
    {
        AttackTarget = null;
    }
    
    public override void AIDetectAround(Collider2D[] detectEnemies_){
        if(IsSpecialAttacking){
            return;
        }
        else{
            base.AIDetectAround(detectEnemies_);
        }
    }
    
    public override EnumBTState AIAttackCheck(){  
        if(IsSpecialAttacking && IsWalking){
            SetVelocity(chargeMoveSpeed * _chargeDirection);
            return EnumBTState.Running;
        }   
        return base.AIAttackCheck();    
    }
    public override bool AISpecialAttack(){
        if(IsSpecialAttacking) return true;
        if(IsSpecialAttackable){
            AIStop();
            IsSpecialAttackable = false;

            StartCoroutine(StartSpecialAttackRoutine());
            // ShakeRotate();
            return true;
        }
        return false;
    }

    protected IEnumerator SpecialAtkCooltimeRoutine(){
        yield return new WaitForSeconds(ChargeCoolTime);
        IsSpecialAttackable = true;
    }
    
    protected IEnumerator StartSpecialAttackRoutine()
    {
        IsSpecialAttacking = true;
        
        //Debug.Log("ChargeEnergy!");
        IsCCImmunity = true;
        CurrentAnim.Play(JUMP);
        GetComponent<Collider2D>().isTrigger = true;

        if (CurrentFaction == Faction.Human)
        {
            ManagerRoot.Sound.PlaySfx("Dragon Breath Fire_12", 1f);
        }
        else
        {
            ManagerRoot.Sound.PlaySfx("Small Monster Breathing Larger 01", 1f);
        }

        if (TryGetComponent(out FeedbackController feedback))
        {
            feedback.SetChargeEffect(ChargeEnergyTime);
        }
        yield return new WaitForSeconds(SetDirectionTime);
        SetDirectionToTarget();
        yield return new WaitForSeconds(ChargeEnergyTime - SetDirectionTime);
        //Debug.Log("Attack!!");
        
        IsWalking = true;
        IsCCImmunity = false;
        //애니메이션 실행
        speedMultiplier = 2f;
        SetVelocity(chargeMoveSpeed * speedMultiplier * _chargeDirection);
        
        //한번의 돌진 동안 피해 입은 유닛 리스트 초기화
        _unitsAttackedPerCharge.Clear();
        _hasHitPlayer = false;
        
        //게임패드 진동
		// ManagerRoot.Input.Vibration(0.3f, chargeAttackTime, true, false);

        yield return new WaitForSeconds(chargeAttackTime);
        
        GetComponent<Collider2D>().isTrigger = false;
        IsSpecialAttacking = false;
        AIStop();
        StartCoroutine(SpecialAtkCooltimeRoutine());
    }

    private void SetDirectionToTarget()
    {
        //방향을 플레이어 방향으로
        if (AttackTarget != null)
        {
            _chargeDirection = (AttackTarget.transform.position - transform.position).normalized;
        }
    }
}


// 과거 코드, 거리 벌리고 돌진


//    public override bool AIAttack(GameObject target_)
//     {
//         if(_isSpecialMove) return false;
//         return base.AIAttack(target_);
//     }
//     public override void TakeDamage(AttackInfo attackInfo_)
//     {
//         if(_isSpecialMove){
//             return;
//         }
//         base.TakeDamage(attackInfo_);
//     }
//     public override void AIDetectAround(Collider2D[] detectThings_){
//         if(_isSpecialMove){
//             foreach (var thing in detectThings_)
//             {
//                 if(thing == null) break;
//                 if(thing == _collider) continue;
//                 // 돌진중 적이랑 부딫혔다면
//                 bool isCrashWithEnemy = IsExistInLayerMask(thing.gameObject.layer, targetLayerMask) && _collider.Distance(thing).distance < chargeKnockDownDistance;
//                 if(isCrashWithEnemy){
//                     if( thing.gameObject.TryGetComponent(out Unit unit))
//                         unit.TakeKnockDown(transform, chargeKnockBackPower);
//                     if(!_isFirstAttack) {
//                         _isFirstAttack = true;
//                         StartCoroutine(SpecialChargeRoutine());
//                     }
//                 }
//             }
//         }
//         else{
//             base.AIDetectAround(detectThings_);
//         }
//     }
//     public override bool AISpecialMove(){
//         if(_canSpecialMove){
//             // 타겟 없으면 리턴
//             Transform target = _aiDestinationSetter.target;
//             bool canNotChargeAttackTarget = target == null || !IsExistInLayerMask(target.gameObject.layer, targetLayerMask) || target.gameObject.layer == Layers.Player;
//             if(canNotChargeAttackTarget){
//                 IsWalking = false;
//                 return false;
//             } 
                
//             if(Vector3.Distance(target.position, transform.position) < chargeMinDistance){
//                 IsWalking = true;
//                 _currentTarget = null;
//                 _rigid.velocity = Info.MoveSpeed * (transform.position - target.position).normalized;
//                 return true;
//             }
//             IsWalking = true;
//             _currentTarget = target.gameObject;
//             _chargeDirection = (target.position - transform.position).normalized;
//             _canSpecialMove = false;
//             _isSpecialMove = true;
//             _rigid.velocity = chargeMoveSpeed * _chargeDirection;
//             StartCoroutine(SpecialMoveRoutine());
//             return true;
//         }
//         if(_isSpecialMove){
//             _rigid.velocity = chargeMoveSpeed * _chargeDirection;
//             IsWalking = true;
//             ShakeRotate();
//             return true;
//         }
//         return false;
//     }

//     IEnumerator SpecialChargeRoutine(){
//         yield return new WaitForSeconds(chargeTime);
//         if(_isSpecialMove){
//             _isSpecialMove = false;
//             _isFirstAttack = false;
//             AIStop();
//         }
//     }
//     IEnumerator SpecialMoveRoutine(){
//         yield return new WaitForSeconds(maxChargeTime);
//         if(!_isFirstAttack && _isSpecialMove)
//         {
//             _isSpecialMove = false;
//             AIStop();
//         }
//     }
//     public override void AIIdle()
//     {
//         base.AIIdle();
//         _canSpecialMove = true;
//     }

