using System.IO;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimator;
using LOONACIA.Unity.Pool;
using UnityEngine;

namespace LOONACIA.Unity.Managers
{
	public class ResourceManager
	{
		public T Load<T>(string path) where T : Object
		{
			if (Resources.Load<T>(path) is not T result)
			{
				Debug.LogWarning($"ResourceManager | Type '{typeof(T).Name}' does not exist in the path; Resources/{path}");
				return null;
			}
			else return result;
		}

		public GameObject Instantiate(string name, Transform parent = null, bool usePool = true)
		{
			if (usePool)
			{
				var poolable = ManagerRoot.Pool.Get(name);
				poolable.transform.SetParent(parent);
				return poolable.gameObject;
			}
			else
			{
				GameObject original = Load<GameObject>($"Prefabs/{name}");
				if (original == null)
				{
					UnityEngine.Debug.Log($"ResourceManager | Failed to load: {original}");
					return null;
				}

				GameObject go = Object.Instantiate(original, parent);
				int index = go.name.IndexOf("(Clone)");
				if (index > 0)
				{
					go.name = go.name.Substring(0, index);
				}

				return go;
			}
		}

		public GameObject Instantiate(GameObject prefab, Transform parent = null, bool usePool = true)
		{
			if (usePool)
			{
				var poolable = ManagerRoot.Pool.Get(prefab);
				poolable.transform.parent = parent;
				poolable.transform.localPosition = Vector3.zero;
				return poolable.gameObject;
			}
			else
			{
				return Object.Instantiate(prefab, parent);
			}
		}

		public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null, bool usePool = true)
		{
			GameObject go = Instantiate(prefab, parent, usePool);
			go.transform.position = position;
			go.transform.rotation = rotation;

			return go;
		}

		public void Release(GameObject go)
		{
			if (go == null)
			{
				return;
			}

			if (go.TryGetComponent<Poolable>(out var poolable))
			{
				ManagerRoot.Pool.Release(poolable);
			}
			else
			{
				Object.Destroy(go);
			}
		}


		public static Object LoadAny(string resourceName, System.Type systemTypeInstance) 
		{
			string ResourcesPath = Application.dataPath+"/Resources";
			string[] directories = Directory.GetDirectories(ResourcesPath,"*",SearchOption.AllDirectories);
			foreach (var item in directories)
			{
				string itemPath = item.Substring(ResourcesPath.Length+1);
				Object result = Resources.Load(itemPath+"\\"+resourceName,systemTypeInstance);
				if(result!=null)
					return result;
			}

			//Debug.Log(resourceName+" is not found in Resources folder");
			return null;
		}
	}
}