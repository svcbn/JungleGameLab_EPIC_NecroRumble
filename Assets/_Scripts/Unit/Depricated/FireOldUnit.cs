// using System;
// using System.Collections;
// using System.Collections.Generic;
// using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
// using LOONACIA.Unity.Managers;
// using UnityEngine;
// using UnityEngine.PlayerLoop;
//
// public class FireOldUnit : EnemyOldUnit
// {
//     public override void Start()
//     {
//         base.Start();
//         Init();
//     }
//
//     public override void Init()
//     {
//         base.Init();
//
//     }
//
//     public override void Attack(){
//         base.Attack();
//         ShootFireBall(_target.transform);
//     }
//
//
//     public void ShootFireBall(Transform target_) // todo: 스킬매니저로 이동 필요
//     {
//         GameObject go = ManagerRoot.Resource.Instantiate("Skills/FireballProjectile", usePool: true);
//         FireballProjectile fb = go.GetComponent<FireballProjectile>();
//
//         fb.Init(transform, target_ ,speed_:2.5f, shootDelay_:1f, lifespan_:10f, damage_:6);
//     }
//
//     private IEnumerator FireBall()
//     {
//
//         for(int i=0; i<3; i++){
//             ShootFireBall(transform);
//             yield return new WaitForSeconds(0.5f);
//         }
//
//         yield return new WaitForSeconds(8);
//
//     }
//     
//     public override void Die()
//     {
//         //Debug.Log($"Enemy Die : {this.name}");
//
//         //시체 오브젝트 생성
//         //ManagerRoot.Unit.CreateFireCorpse(this);
//         //ManagerRoot.Unit.DestroyEnemyUnit(this);
//         
//     }
//
// }
