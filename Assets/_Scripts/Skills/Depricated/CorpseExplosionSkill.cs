// using LOONACIA.Unity;
// using LOONACIA.Unity.Managers;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class CorpseExplosionSkill : MonoBehaviour
// {
//     Player _player;
//     bool isCorpseHoveringMode;
//     BonePile selectedCorpse;
//     float _corpseExpolsionRadius = 3f;
//
//     void Start()
//     {
//
//     }
//
//     void Update()
//     {
//
//         //시체 폭발 스킬
//         if (Input.GetKeyDown(KeyCode.Alpha3))
//         {
//             // SkillIconContainer currentIconContainer = OldSkillManager.Instance.skillIconContainerList[2];
//             // float cost = currentIconContainer.GetCurSkill().info.Cost;
//             // if (currentIconContainer._currentCooltime > 0f || _player.GetMpValue() < cost)
//             // {
//             //     return;
//             // }
//             // // 호버링 모드 -> 클릭시 폭발
//             // isCorpseHoveringMode = true;
//             // //ManagerRoot.Unit.SetCorpseHoverMode(true);
//             // _player.GetComponent<PlayerMovement>().CanMove = false;
//         }
//
//         if (Input.GetMouseButtonDown(0))
//         {
//             if (isCorpseHoveringMode)
//             {
//                 isCorpseHoveringMode = false;
//                 CorpseExplosion(selectedCorpse, _corpseExpolsionRadius);
//                 //ManagerRoot.Unit.SetCorpseHoverMode(false);
//                 _player.GetComponent<PlayerMovement>().CanMove = true;
//             }
//         }
//
//         if (isCorpseHoveringMode)
//         {
//             selectedCorpse = GetNearestCorpse();
//         }
//
//     }
//
//
//     // 시체 폭발 관련 함수 2개
//     BonePile GetNearestCorpse(float radius_ = 0.5f)
//     {
//         int closestIndex = -1;
//         float closestDistance = float.MaxValue; // 거리 비교를 위한 변수 추가
//         Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//
//         // 시체 검색
//         var corpses = Physics2D.OverlapCircleAll(mousePos, radius_, Layers.HumanCorpse.ToMask());
//         for (int i = 0; i < corpses.Length; i++)
//         {
//             float distance = Vector2.Distance(mousePos, corpses[i].transform.position);
//             if (distance < closestDistance)
//             {
//                 if (closestIndex != -1)
//                 {
//                     // 이전 가장 가까운 시체의 isSelected를 false로 설정
//                     corpses[closestIndex].GetComponent<BonePile>().isSelected = false;
//                 }
//                 closestDistance = distance;
//                 closestIndex = i;
//             }
//         }
//
//         // 선택된 시체 외에 모든 시체의 isSelected를 false로 설정
//         for (int i = 0; i < corpses.Length; i++)
//         {
//             if (i != closestIndex)
//             {
//                 corpses[i].GetComponent<BonePile>().isSelected = false;
//             }
//         }
//
//
//         if (closestIndex != -1)
//         {
//             // 가장 가까운 시체의 isSelected를 true로 설정
//             corpses[closestIndex].GetComponent<BonePile>().isSelected = true;
//             return corpses[closestIndex].GetComponent<BonePile>();
//         }
//
//         if (closestIndex == -1)
//         {
//             //ManagerRoot.Unit.UnitListCorpse.ForEach(x => x.isSelected = false);
//         }
//
//         return null; // 가장 가까운 시체가 없는 경우
//     }
//
//
//
//     void CorpseExplosion(BonePile corpse_, float radius_)
//     {
//         if (corpse_ == null) { return; }
//         Debug.Log("CorpseExplosionSkill ");
//
//
//         SkillIconContainer currentIconContainer = OldSkillManager.Instance.skillIconContainerList[2]; //3번스킬, 지금은 하드코딩임
//         float damage = currentIconContainer.GetCurSkill().info.UpgradeValues[currentIconContainer.GetCurSkill().currentupgradeNum - 1];
//         currentIconContainer._currentCooltime = currentIconContainer.maxCooltime;
//         //_player.ChangeMpValue((-1) * currentIconContainer.GetCurSkill().info.Cost);
//
//         // 이펙트        
//         GameObject go = ManagerRoot.Resource.Instantiate("Skills/BoneExplosion", usePool: true);
//         go.transform.position = corpse_.transform.position;
//
//         //ManagerRoot.Unit.DestroyCorpse(corpse_); // 폭발할 시체 
//
//         // 근처 적들 폭파데미지
//         var enemyUnits = Physics2D.OverlapCircleAll(corpse_.transform.position, radius_, Layers.HumanUnit.ToMask());
//         for (int i = 0; i < enemyUnits.Length; i++)
//         {
//             enemyUnits[i].GetComponent<EnemyOldUnit>().TakeHit(damage);
//
//         }
//
//     }
//
//     void OnDrawGizmos()
//     {
//         if (Camera.main != null)
//         {
//             Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
//             Gizmos.color = Color.red;
//             Gizmos.DrawWireSphere(mousePos, 0.5f); // 원의 반지름을 0.5 단위로 설정합니다.
//         }
//     }
// }
