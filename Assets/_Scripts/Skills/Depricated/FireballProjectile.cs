// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Sirenix.OdinInspector;
//
// public class FireballProjectile : MonoBehaviour
// {
// 	[ReadOnly][SerializeField]private float _speed;
// 	[ReadOnly][SerializeField]private float _shootDelay;
// 	[ReadOnly][SerializeField]private float _lifespan;
// 	[ReadOnly][SerializeField]private int   _damage;
//
// 	[ReadOnly][SerializeField]private Vector2 _dir;
// 	[ReadOnly][SerializeField]private float   _actualSpeed;
// 	[ReadOnly][SerializeField]private float   _startTime;
//
//     private bool isHit;
//     private Transform _target;
//
// 	private void Update()
// 	{
//         if(_target == null || transform == null)
//         {
// 			gameObject.SetActive(false);
// 			Destroy(gameObject);
//             return;
// 		}
//         Vector2 dir = (_target.position - transform.position).normalized;
//         SetDirection(dir);
//
// 		transform.position += (Vector3)_dir * (_actualSpeed * Time.deltaTime);
//
// 		if (Time.time > _startTime + _lifespan)
// 		{
// 			gameObject.SetActive(false);
// 			Destroy(gameObject);
// 		}
// 	}
//
//     public void Init(Transform shooter_, Transform target_, float speed_, float shootDelay_, float lifespan_, int damage_)
//     {
//         transform.position = shooter_.position;
//         _target = target_;
//
//         transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
//
// 		_startTime   = Time.time;
//
//         _speed      = speed_;
//         _shootDelay = shootDelay_;
//         _lifespan   = lifespan_;
//         _damage     = damage_;
//
// 		Invoke("ShootProjectile", _shootDelay);
//     }
//
//     public void SetDirection(Vector2 dir_)
//     {
//         _dir = dir_;
// 		float angle = Mathf.Atan2(_dir.y,_dir.x) * Mathf.Rad2Deg;
// 		transform.rotation = Quaternion.AngleAxis(angle + 180, Vector3.forward);
//     }
//
// 	void ShootProjectile()
// 	{
// 		_actualSpeed = _speed;
// 	}
//
//
// 	private void OnTriggerEnter2D(Collider2D collision)
// 	{
//         
// 		if (collision.TryGetComponent<PlayerOldUnit>(out var hitPlayerUnit))
// 		{
//             isHit = true;
//             Debug.Log($"TriggerEnter: {hitPlayerUnit.name}");
// 			
//             hitPlayerUnit.TakeHit(_damage);
// 		}
//         else if(collision.TryGetComponent<Player>(out var hitPlayer))
//         {
//             isHit = true;
//             Debug.Log($"TriggerEnter: {hitPlayer.name}");
//
//             //hitPlayer.TakeHit(_damage); // todo Player 히트 판정
//         }
//
//
//         if(isHit){
//             gameObject.SetActive(false);
//             Destroy(gameObject);
//         }
// 	}
// }