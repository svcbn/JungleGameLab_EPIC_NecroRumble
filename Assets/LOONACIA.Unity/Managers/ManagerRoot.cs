using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Honeti;


namespace LOONACIA.Unity.Managers
{
	public class ManagerRoot : MonoBehaviour
	{
		public enum ManagerOperationMode
		{
			Always,
			DestroyWhenSceneLoaded
		}
		public static bool IsGameQuit;
		
		private readonly InputManager       _input    = new();
		private readonly UnitManager        _unit     = new();
		private readonly PoolManager        _pool     = new();
		private readonly ResourceManager    _resource = new();
		private readonly SoundManager	    _sound    = new();
		private readonly UIManager          _ui       = new();
		private readonly All1ShaderManager  _shader   = new();
		private readonly WaveManager        _wave     = new();
		private readonly SkillManager       _skill    = new();
		private readonly EventManager	    _event    = new();
		private readonly UnitUpgradeManager _unitUpgrade = new();
		private readonly ChunkManager 	    _chunk    = new();
		private readonly Settings   	    _settings = new();
		private readonly I18N   	        _i18n     = new();

		private static ManagerRoot s_instance;
		public  static ManagerRoot Instance
		{
			get
			{
				if(IsGameQuit){ return null; }

				Init();
				return s_instance;
			}
		}

		public ManagerOperationMode OperationMode { get; set; }

		public static InputManager    Input          => Instance._input;
		public static UnitManager     Unit           => Instance._unit;
		public static PoolManager     Pool           => Instance._pool;
		public static ResourceManager Resource       => Instance._resource;
		public static SoundManager    Sound          => Instance._sound;
		public static UIManager       UI             => Instance._ui;
		public static All1ShaderManager Shader       => Instance._shader;
		public static WaveManager     Wave           => Instance._wave;
		public static SkillManager    Skill          => Instance._skill;
		public static EventManager    Event          => Instance._event;
		public static UnitUpgradeManager UnitUpgrade => Instance._unitUpgrade;
		public static ChunkManager    Chunk          => Instance._chunk;
		public static Settings        Settings       => Instance._settings;
		public static I18N            I18N           => Instance._i18n;
		

		public static void Clear(bool destroyAssociatedObject = false)
		{
			UI.Clear(destroyAssociatedObject);
			Pool.Clear(destroyAssociatedObject);
			Sound.Clear();
			Unit.Clear();
			Wave.Clear();
			Input.Clear();
			UnitUpgrade.Clear();
			Chunk.Clear();
			Event.Clear();
			I18N.Clear();
			Settings.Clear();
			
			if(SceneManagerEx.CurrentScene.SceneType is SceneType.Game)
			{
				GameUIManager.Instance.Clear();
				GameManager.Instance.Clear();
			}
			else if(SceneManagerEx.CurrentScene.SceneType is SceneType.Tutorial)
			{
				TutorialManager.Instance.Clear();
				GameUIManager.Instance.Clear();
				GameManager.Instance.Clear();
			}
		}

		private static void Init()
		{
			if (s_instance != null)
			{
				return;
			}
			
			if (GameObject.Find("@Managers") is not { } managersRoot)
			{
				managersRoot = new() { name = "@Managers" };
			}

			s_instance = managersRoot.GetOrAddComponent<ManagerRoot>();
			
			DontDestroyOnLoad(s_instance);
			SceneManagerEx.OnSceneChanging  += s_instance.OnSceneChanging;
		}

		static void InitInstances()
		{
			Debug.Log("ManagerRoot | Init Instances");

			s_instance._input.Init();
			s_instance._pool.Init();
			s_instance._unit.Init();
			s_instance._ui.Init();
			s_instance._settings.Init(); // 여기 위치가 맞는가?
			s_instance._sound.Init();
			s_instance._i18n.Init();     // 여기 위치가 맞는가?
			s_instance._wave.Init();
			s_instance._skill.Init();
			s_instance._event.Init();
			s_instance._unitUpgrade.Init();
			s_instance._chunk.Init();



			if(SceneManagerEx.CurrentScene.SceneType is SceneType.Game)
			{
				GameManager.Instance.Init();
				GameUIManager.Instance.Init();
			}
			else if(SceneManagerEx.CurrentScene.SceneType is SceneType.Tutorial)
			{
				GameManager.Instance.Init();
				GameUIManager.Instance.Init();
				TutorialManager.Instance.Init();
			}

			SteamUserData.ResetAll();
		}

		private void OnEnable()
		{
			Debug.Log("ManagerRoot | ManagerRoot OnEnable");
			SceneManager.activeSceneChanged += OnSceneChanged;
		}
		private void OnDisable()
		{
			Debug.Log("ManagerRoot | ManagerRoot OnDisable");
		}

		string _prevSceneName;
		private void OnSceneChanging(Scene prevScene_)
		{
			Debug.LogFormat("ManagerRoot | OnSceneChanging from | {0} |", prevScene_.name);
			_prevSceneName = prevScene_.name;

			switch (OperationMode)
			{
				case ManagerOperationMode.Always:
					Clear(false);
					break;
				case ManagerOperationMode.DestroyWhenSceneLoaded:
					SceneManagerEx.OnSceneChanging -= OnSceneChanging;
					Clear(true);
					Destroy(gameObject);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void OnSceneChanged(Scene preScene, Scene newScene) // preScene is destroyed already
		{
			Debug.LogFormat("ManagerRoot | ---------------------------------------");
			Debug.LogFormat("ManagerRoot | | Scene changed from | {0} | to | {1} |", _prevSceneName, newScene.name);
			Debug.LogFormat("ManagerRoot | ---------------------------------------");

			InitInstances();

			SceneManagerEx.CurrentScene.SceneChanged(_prevSceneName, newScene.name);
		}

		void OnApplicationQuit()
		{
			IsGameQuit = true;
			Debug.Log($"ManagerRoot | OnApplicationQuit | IsGameQuit : {IsGameQuit}");
		}

	}
}