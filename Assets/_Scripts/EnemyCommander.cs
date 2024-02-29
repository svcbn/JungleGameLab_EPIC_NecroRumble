// using LOONACIA.Unity.Managers;
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class EnemyCommander : MonoBehaviour
// {
//     private enum Pattern
//     {
//         Idle = 1,
//         Fireball,
//         BackUp,
//         CycleEnd,
//     }
//     private Pattern _currentPattern = Pattern.Fireball;
//
//     private bool _isPatternEnd = false;
//
//     private void Start()
//     {
//         StartCoroutine(PatternCycle());
//         
//     }
//
//     private IEnumerator PatternCycle()
//     {
//         while (true)
//         {
//             ChangePattern(_currentPattern);
//
//             yield return new WaitForSeconds(5);
//
//             yield return new WaitUntil(() => _isPatternEnd);
//         }
//     }
//
//     private void ChangePattern(Pattern currentPattern_)
//     {
//
//         _currentPattern = ++currentPattern_ == Pattern.CycleEnd ? Pattern.Idle : currentPattern_;
//         _isPatternEnd = false;
//
//         //Debug.Log(_currentPattern);
//
//         switch (_currentPattern)
//         {
//             case Pattern.Idle:
//                 CommanderIdle();
//                 break;
//             case Pattern.Fireball:
//                 CommanderFireball();
//                 break;
//             case Pattern.BackUp:
//                 CommanderBackUp();
//                 break;
//         }
//
//     }
//
//     private void CommanderIdle()
//     {
//         StartCoroutine(Idle());
//     }
//
//     private IEnumerator Idle()
//     {
//
//
//         yield return new WaitForSeconds(3);
//
//         _isPatternEnd = true;
//     }
//
//     private void CommanderFireball()
//     {
//         StartCoroutine(FireBall());
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
//         _isPatternEnd = true;
//     }
//
//     private void CommanderBackUp()
//     {
//         StartCoroutine(BackUp());
//     }
//
//     private IEnumerator BackUp()
//     {
//         //ManagerRoot.Unit.MakeWave(GameManager.Instance.GetPlayer().transform.position, 10f, 5);
//
//         yield return new WaitForSeconds(15);
//
//         _isPatternEnd = true;
//     }
//
//
//     public void ShootFireBall(Transform shooter_) // todo: 스킬매니저로 이동 필요
//     {
//         GameObject go = ManagerRoot.Resource.Instantiate("Skills/FireballProjectile", usePool: true);
//         FireballProjectile fb = go.GetComponent<FireballProjectile>();
//
//         Transform playerTrans = FindObjectOfType<Player>().transform;
//         fb.Init(shooter_, playerTrans ,speed_:2.5f, shootDelay_:1f, lifespan_:10f, damage_:6);
//     }
//
// }