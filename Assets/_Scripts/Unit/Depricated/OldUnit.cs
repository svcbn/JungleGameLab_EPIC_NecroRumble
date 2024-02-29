// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Sirenix.Utilities;
// using Sirenix.OdinInspector;
// using UnityEngine;
//
//
//
// public abstract class OldUnit : MonoBehaviour
// {
//     #region Member Variables
//     public EnumUnitType unitType = EnumUnitType.skeleton_knight;
//     [ReadOnly] public uint _unitID;
//     [HeaderAttribute("Stats")]
//     protected float _curHp;
//     protected bool _isElite = true;
//     [HeaderAttribute("Attack")]
//     public float idleMoveSpeed = 0.5f;
//     public LayerMask targetLayer;
//     public bool isPlayerTargetFirst = false;
//     public bool isGoToEnemyCaptin = true;
//     [HeaderAttribute("Movement")]
//     protected Vector3 _moveTargetPosition;
//     [HeaderAttribute("Components")]
//     private GameObject _flipable;
//     private GameObject _unitRenderer;
//     private SpriteRenderer _spriteRenderer;
//     protected GameObject _target;
//     protected GameObject _targetCaptain;
//     protected GameObject _weapon;
//     protected Collider2D _collider;
//     [HeaderAttribute("Prefabs")]
//     private GameObject _hpBarContainer;
//     private GameObject _hpBar;
//     [HeaderAttribute("Delays")]
//     protected float _currentAttackTimer;
//     protected float _hpBarAppearDelay;
//     [HeaderAttribute("Data")]
//     private DataDB _dataDB;
//     private string _name;
//     [SerializeField,ReadOnly] private UnitInfo _info;
//     [HeaderAttribute("Lifespan")]
//     protected LifespanType lifespanType;
//     protected int lifespanDotDamage;
//     protected float lifespanTotalTime     = 15f;
//     protected float lifespanDotDamageInterval = 1f;
//     protected float lifespanTotalTimer;
//     protected float lifespanDotDamageTimer;
//     [HeaderAttribute("Animation")]
//     private AnimatorOverrideController _overrideController;
//     private Animator _weaponAnimator;
//     private AnimationClip _idleAnimation;
//     private AnimationClip _attackAnimation;
//     #endregion
//     #region Properties
//     public GameObject Captain => _targetCaptain;
//     public UnitInfo Info => _info;
//     #endregion
//     public enum LifespanType
//     {
//         None,
//         Time,
//         DotDamage
//     }
//     public enum EnumUnitType
//     {
//         skeleton_knight,
//         skeleton_archer,
//         skeleton_magic,
//         skeleton_deathKnight,
//     }
//     public virtual void Start()
//     {
//         Init();
//     }
//     public virtual void Init() //캐릭터마다 다를 예정, 엑셀이나 DB에서 불러올 예정
//     {
//         _flipable = transform.Find("Flipable").gameObject;
//         _unitRenderer = _flipable.transform.Find("UnitRenderer").gameObject;
//         _spriteRenderer = _unitRenderer.GetComponent<SpriteRenderer>();
//         _collider = GetComponent<Collider2D>();
//         _hpBarContainer = transform.Find("HpBarContainer").gameObject;
//         _hpBar = _hpBarContainer.transform.Find("HpBarValue").gameObject;
//         _dataDB = Resources.Load<DataDB>("Data/DataDB");
//         //TODO: 나중에 이름 받아서 적용 -> 이거만 바꾸면 알아서 궁수랑 기사랑 구분됨
//         _name = unitType.ToString();
//         SetUnitInfoByName();
//
//         lifespanDotDamage = Mathf.RoundToInt(_info.MaxHp / lifespanTotalTime);
//         lifespanDotDamageInterval = lifespanDotDamage * lifespanTotalTime / _info.MaxHp;
//     }
//     public virtual void Update()
//     {
//         if (_hpBarAppearDelay > 0)
//         {
//             _hpBarAppearDelay -= Time.deltaTime;
//             if (_hpBarAppearDelay <= 0)
//             {
//                 _hpBarContainer.SetActive(false);
//             }
//         }
//         if (_currentAttackTimer > 0)
//         {
//             _currentAttackTimer -= Time.deltaTime;
//             if (_currentAttackTimer <= 0)
//             {
//                 _currentAttackTimer = 0;
//             }
//         }
//         if (Vector3.Distance(transform.position,_moveTargetPosition) > 0.01f)
//         {
//             ShakeRotate();
//         }
//         else
//         {
//             transform.rotation = Quaternion.Euler(0f, 0f, Mathf.LerpAngle(transform.rotation.eulerAngles.z, 0, 20f * Time.deltaTime));
//         }
//         UpdateLifespan();
//     }
//     public virtual bool MoveTo(){
//         if (_target == null)
//         {
//             _moveTargetPosition = transform.position;
//             return true; // EndState in BT
//         }
//         else
//         {
//             _moveTargetPosition = _target.transform.position;
//             return false;
//         }
//     }
//     public virtual bool AttackTo(){
//         if (transform.position.x - _target.transform.position.x > 0)
//         {
//             FlipScale(-1);
//         }
//         else
//         {
//             FlipScale(1);
//         }
//         if (_currentAttackTimer <= 0f)
//         {
//             _currentAttackTimer = 1 / _info.AttackPerSec;
//             Attack();
//         }
//         #region Weapon Rotation Setting
//         Vector3 targetDirection = _target.transform.position - transform.position;
//         float targetZRotation = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
//         float xScale = _unitRenderer.transform.localScale.x;
//         if (xScale < 0)
//         {
//             targetZRotation += 180f;
//         }
//         Vector3 currentEulerAngles = _weapon.transform.parent.transform.eulerAngles;
//         currentEulerAngles.z = Mathf.MoveTowardsAngle(currentEulerAngles.z, targetZRotation, 100f * Time.deltaTime);
//         _weapon.transform.parent.transform.eulerAngles = currentEulerAngles;
//         #endregion
//         return false;
//     }
//     public virtual void TakeHit(float damage_)
//     {
//         _curHp -= damage_;
//         CreateText(damage_.ToString(), new Color32(164, 35, 35, 255));
//         if (TryGetComponent(out DamageFlash damageFlash))
//         {
//             damageFlash.CallDamageFlash();
//         }
//         if (_isElite)
//         {
//             _hpBarContainer.SetActive(true);
//             _hpBarAppearDelay = 1f;
//             _hpBar.transform.localScale = new Vector3(_curHp / _info.MaxHp, 1f, 1f);
//         }
//         if (_curHp <= 0)
//         {
//             //죽음
//             Die();
//         }
//     }
//     public abstract void Die();
//     public virtual void CreateText(string text, Color32 color = default)
//     {
//         GameObject damageTextPrefab = Resources.Load("Prefabs/UI/DamageText") as GameObject;
//         GameObject damageText = Instantiate(damageTextPrefab, transform.position + new Vector3(0f, .5f, 0f), Quaternion.identity);
//         // damageText.GetComponent<MoveAndDestroy>().Text = text;
//         // damageText.GetComponent<MoveAndDestroy>().Color = color;
//     }
//     public virtual GameObject GetTarget()
//     {
//         return _target;
//     }
//     public virtual void SetTarget(GameObject target_)
//     {
//         _target = target_;
//     }
//     public virtual float GetAtk()
//     {
//         return _info.Damage;
//     }
//     public virtual void Attack()
//     {
//         _weaponAnimator.SetTrigger("Attack");
//     }
//     private void OnDrawGizmos() //Debug
//     {
//         Gizmos.color = Color.red;
//         Gizmos.DrawWireSphere(transform.position, _info.EnemyDetectRadius);
//     }
//     private void ShakeRotate()
//     {
//         transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Sin(Time.time * 10f) * 5f);
//     }
//     public void FlipScale(int dir_)
//     {
//         // 1이면 오른쪽, -1이면 왼쪽
//        if(dir_ == 0) return;
//         float scale = Mathf.Abs(_unitRenderer.transform.localScale.x);
//         _unitRenderer.transform.localScale = new Vector3(scale * dir_, _unitRenderer.transform.localScale.y, _unitRenderer.transform.localScale.z);
//        }
//     private void SetUnitInfoByName()
//     {
//         for (int i = 0; i < _dataDB.UnitSheet.Count; i++)
//         {
//             if (_dataDB.UnitSheet[i].Name == _name)
//             {
//                 _info = _dataDB.UnitSheet[i];
//                 _curHp = _info.MaxHp;
//                 break;
//             }
//         }
//         #region Weapon Animation Setting
//         _weapon = _unitRenderer.transform.Find("WeaponContainer").GetChild(0).gameObject;
//         _weaponAnimator = _weapon.GetComponent<Animator>();
//         SetWeaponSpriteBasedOnType();
//         string animationPath = "Animations/" + _info.Type;
//         _idleAnimation = (AnimationClip)Resources.Load(animationPath + "IdleAnim");
//         _attackAnimation = (AnimationClip)Resources.Load(animationPath + "AttackAnim");
//         //RuntimeAnimatorController currentController = _weaponAnimator.runtimeAnimatorController;
//         _overrideController = new AnimatorOverrideController(_weaponAnimator.runtimeAnimatorController);
//         _weaponAnimator.runtimeAnimatorController = _overrideController;
//         var clipOverrides = new AnimationClipOverrides(_overrideController.overridesCount);
//         _overrideController.GetOverrides(clipOverrides);
//         if (_idleAnimation != null) clipOverrides["Idle"] = _idleAnimation;
//         if (_attackAnimation != null) clipOverrides["Attack"] = _attackAnimation;
//         _overrideController.ApplyOverrides(clipOverrides);
//         #endregion
//     }
//     private void SetWeaponSpriteBasedOnType()
//     {
//         _weapon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Weapons/" + _info.Type);
//     }
//     protected void UpdateLifespan()
//     {
//         switch(lifespanType)
//         {
//             case LifespanType.None:
//                 break;
//             case LifespanType.Time:
//                 lifespanTotalTimer += Time.deltaTime;
//                 if( lifespanTotalTimer > lifespanTotalTime)
//                 {
//                     Debug.Log("Dead by lifespan");
//                     Destroy(this.gameObject);
//                 }
//                 break;
//             case LifespanType.DotDamage:
//                 lifespanDotDamageTimer += Time.deltaTime;
//                 if( lifespanDotDamageTimer > lifespanDotDamageInterval ){ // lifespanDotDamageTime초마다 lifespanDotDamage 입음
//                     lifespanDotDamageTimer = 0;
//                     TakeHit(lifespanDotDamage);
//                     //Debug.Log($"Dot Damage {lifespanDotDamage} by lifespan");
//                 }
//                 break;
//         }
//     }
//     public void SetLifespanType(LifespanType type_)
//     {
//         lifespanType = type_;
//     }
//         public bool IsInAttackRange(GameObject target_){
//         if(_collider.Distance(target_.GetComponent<Collider2D>()).distance > _info.AttackRange){
//             return false;
//         }
//         return true;
//     }
// }