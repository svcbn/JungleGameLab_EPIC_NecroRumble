// using System.Collections;
// using System.Collections.Generic;
// using LOONACIA.Unity.Managers;
// using UnityEngine;
//
// public class BonePile : OldUnit
// {
//     public bool isHoverMode; // 시체 스킬 사용시 True, 스킬 사용 끝나면 False
//     public bool isSelected;
//
//     private Material _selectedMaterial;
//     private Material _unselectedMaterial;
//
//     public override void Start()
//     {
//         _selectedMaterial   = ManagerRoot.Resource.Load<Material>("Materials/CorpseUnitSelected");
//         _unselectedMaterial = ManagerRoot.Resource.Load<Material>("Materials/AllInOneNone");
//     }
//     public override void Die()
//     {
//         
//     }
//
//
//     public override void Update()
//     {
//
//         if( isHoverMode )
//         {
//             if( isSelected ) // changed 형식으로 바꾸기
//             {
//                 GetComponent<SpriteRenderer>().material = _selectedMaterial;
//             }
//             else
//             {
//                 GetComponent<SpriteRenderer>().material = _unselectedMaterial;
//             }
//         }
//
//     }
// }
