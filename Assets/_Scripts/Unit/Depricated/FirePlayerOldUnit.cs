// using System.Collections;
// using System.Collections.Generic;
// using LOONACIA.Unity.Managers;
// using UnityEngine;
//
// public class FirePlayerOldUnit : PlayerOldUnit
// {
//     // Start is called before the first frame update
//     public override void Start()
//     {
//         base.Start();
//         Init();
//     }
//     public override void Init()
//     {
//         base.Init();
//     }
//
//     public override void Attack(){
//         base.Attack();
//         Debug.Log("fireAttack");
//         ShootFireBall(_target.transform);
//     }
//
//
//     public void ShootFireBall(Transform target_) // todo: 스킬매니저로 이동 필요
//     {
//
//         GameObject go = ManagerRoot.Resource.Instantiate("Skills/FriendlyFireballProjectile", usePool: true);
//         FriendlyFireballProjectile fb = go.GetComponent<FriendlyFireballProjectile>();
//
//         fb.Init(transform, target_ ,speed_:2.5f, shootDelay_:1f, lifespan_:10f, damage_:6);
//     }
//     
//
//     // Update is called once per frame
//     public override void Update()
//     {
//         base.Update();
//     }
//
// }
