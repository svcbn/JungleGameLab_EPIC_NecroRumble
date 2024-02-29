// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class TestOldUnit : EnemyOldUnit
// {
//     float _attackTime = 1f;
//     public override void Init()
//     {
//         base.Init();
//         // enemyDetectRadius = 10;
//         // attackDetectRadius = 10;
//         // _maxAttackDelay = 5f;
//     }
//     public override void Attack(){
//         Debug.Log("attack");
//         base.Attack();
//         StartCoroutine(DashAttack());
//     }
//     IEnumerator DashAttack(){
//         float timer = 0f;
//         while(true){
//             timer += Time.deltaTime;
//             if(timer > _attackTime) break;
//             transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, 5 * Time.deltaTime);
//             yield return null;
//         }
//     }
// }
