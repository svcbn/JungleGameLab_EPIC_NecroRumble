// using System;
// using System.Collections;
// using System.Collections.Generic;
// using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
// using LOONACIA.Unity.Managers;
// using UnityEngine;
// using UnityEngine.PlayerLoop;
//
// public class EnemyOldUnit : OldUnit
// {
//     public GameObject _bang;
//     int _playerLayer;
//     bool isGoToPlayer = false;
//     public override void Start()
//     {
//         base.Start();
//         Init();
//         
//     }
//
//     public override void Init()
//     {
//         base.Init();
//         _playerLayer = LayerMask.NameToLayer("Player");
//         _targetCaptain = GameObject.Find("Player");
//     }
//     private void Awake()
//     {
//
//     }
//
//     public override void Update()
//     {
//         base.Update();
//         if(_target != null && _target.layer == _playerLayer){
//             _bang.SetActive(true);
//         }
//         else{
//             _bang.SetActive(false);
//         }
//         if(isGoToPlayer){
//             if(Vector3.Distance(_targetCaptain.transform.position, transform.position) <= Info.EnemyDetectRadius){
//                 isGoToPlayer = false;
//             }
//             else{
//                 transform.position = Vector3.MoveTowards(transform.position, _targetCaptain.transform.position, Info.MoveSpeed * Time.deltaTime);
//             }
//         }
//     }
//     public void GoToPlayer(){
//         isGoToPlayer = true;
//         _target = _targetCaptain;
//     }
//
//     public override void Die()
//     {
//         //Debug.Log($"Enemy Die : {this.name}");
//
//         //시체 오브젝트 생성
//         //ManagerRoot.Unit.CreateCorpse(this);
//         //ManagerRoot.Unit.DestroyEnemyUnit(this);
//         
//     }
// }
