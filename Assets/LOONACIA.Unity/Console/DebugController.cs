using System;
using System.Collections.Generic;
using System.Linq;
using LOONACIA.Unity.Managers;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

namespace LOONACIA.Unity.Console
{
	public class DebugController : MonoBehaviour
	{
		private List<DebugCommandBase> _commands;

		private bool _isToggled;

		private bool _showHelp;
		private bool _showAllResolutions;
		private bool _showCurrentResolution;

		private Resolution[] _availableResolutions;
		private Resolution _currentResolution;
		private FullScreenMode _fullScreenMode;

		private string _commandString;

		private Vector2 _scrollView;

		private Action _enablePlayerInput;
		private Action _disablePlayerInput;

		private void Awake()
		{
			Init();
		}
#if UNITY_EDITOR
		private void OnEnable()
		{
			ManagerRoot.Input.OnTogglePressed += ToggleDebugConsole;
			ManagerRoot.Input.OnReturnPressed += ExecuteCommand;
		}

		private void OnDisable()
		{
			if(ManagerRoot.Instance == null || !ManagerRoot.Instance.enabled ){ return; }

			ManagerRoot.Input.OnTogglePressed -= ToggleDebugConsole;
			ManagerRoot.Input.OnReturnPressed -= ExecuteCommand;
			
		}
#endif
		void ToggleDebugConsole()
		{
			Debug.Log( $"DebugController | ToggleDebugConsole {_isToggled}" );
			if( !_isToggled )
			{
				_isToggled = true;
				ManagerRoot.Input.EnableDebugMode();
			}
			else if( _isToggled )
			{
				_isToggled = false;
				ManagerRoot.Input.DisableDebugMode();
			}
		}


		private void OnGUI()
		{
			if (!_isToggled)
			{
				_commandString = string.Empty;
				return;
			}

			int fontSize = Mathf.Max(15, (int)(Screen.height / 54f));
			GUI.skin.textField.fontSize = fontSize;
			GUI.skin.label.fontSize     = fontSize;

			float y = 0f;

			// 도움말 표시
			if (_showHelp)
			{
				float helpBoxHeight = fontSize * 5f;
				GUI.Box(        new(0, y, Screen.width, helpBoxHeight), string.Empty              );
				Rect viewport = new(0, 0, Screen.width - 30,  (fontSize + 12)* _commands.Count ); // 여백 12만큼 더해줘야함

				_scrollView = GUI.BeginScrollView(new(0, y + 5f, Screen.width, helpBoxHeight - 10f), _scrollView, viewport);
				foreach ((DebugCommandBase command, int index) in _commands.Select((command, index) => (command, index)))
				{
					Rect labelRect = new(5, (fontSize * 1.5f) * index, viewport.width - 100, fontSize * 1.5f);
					GUI.Label(labelRect, $"{command.Format} - {command.Description}");
				}

				GUI.EndScrollView();
				y += helpBoxHeight;
			}

			// 현재 해상도 표시
			if (_showCurrentResolution)
			{
				float resolBoxHeight = fontSize * 5f;
				GUI.Box(        new(0, y, Screen.width, resolBoxHeight), string.Empty              );
				Rect viewport = new(0, 0, Screen.width - 30,  (fontSize + 12)* _availableResolutions.Length ); // 여백 12만큼 더해줘야함

				_scrollView = GUI.BeginScrollView(new(0, y + 5f, Screen.width, resolBoxHeight - 10f), _scrollView, viewport);
				
				Rect labelRect = new(5, (fontSize * 1.5f), viewport.width - 100, fontSize * 1.5f);
				GUI.Label(labelRect, $"Current Resolution: {_currentResolution.ToString()}  ScreenMode: {_fullScreenMode}" );


				GUI.EndScrollView();
				y += resolBoxHeight;
				
			}

			// 모든 해상도 표시
			if (_showAllResolutions)
			{
				float resolBoxHeight = fontSize * 5f;
				GUI.Box(        new(0, y, Screen.width, resolBoxHeight), string.Empty              );
				Rect viewport = new(0, 0, Screen.width - 30,  (fontSize + 12)* _availableResolutions.Length ); // 여백 12만큼 더해줘야함

				_scrollView = GUI.BeginScrollView(new(0, y + 5f, Screen.width, resolBoxHeight - 10f), _scrollView, viewport);
				
				foreach ( (Resolution res, int index ) in _availableResolutions.Select((res, index) => (res, index)))
				{
					Rect labelRect = new(5, (fontSize * 1.5f) * index, viewport.width - 100, fontSize * 1.5f);
					GUI.Label(labelRect, res.ToString() );
				}

				GUI.EndScrollView();
				y += resolBoxHeight;
			}

			GUI.Box(new(0, y, Screen.width, fontSize * 2f), string.Empty);
			GUI.backgroundColor = new();
			GUI.SetNextControlName("DebugConsole");
			_commandString = GUI.TextField(new(10f, y + (fontSize / 2f), Screen.width - 20f, fontSize * 1.5f), _commandString);
			GUI.FocusControl("DebugConsole");
		}

		private void ExecuteCommand()
		{
			if(!_isToggled){ return; }

			Debug.Log($"DebugController | ExecuteCommand | Cmd:{_commandString}");
			_showHelp = false;
			_showAllResolutions = false;
			_showCurrentResolution = false;

			(string id, string parameter) = ParseCommandString();
			foreach (var command in _commands.Where(command => command.Id.Equals(id, StringComparison.OrdinalIgnoreCase)))
			{
				command.Execute(parameter);
			}

			_commandString = string.Empty;
			
			ToggleDebugConsole();
		}

		private (string Id, string Parameter) ParseCommandString()
		{
			int index;
			return (index = _commandString.IndexOf(' ')) == -1
				? (_commandString, string.Empty)
				: (_commandString[..index], _commandString[index..].TrimStart());
		}

		private void Init()
		{
			_commands = new()
			{
				new TransformObjectCommand(
					id: "move",
					description: "Move the position of specified GameObject",
					format: "move <name> <position>",
					type: TransformObjectCommand.TransformType.Position),

				new TransformObjectCommand(
					id: "rotate",
					description: "Rotate the specified GameObject",
					format: "rotate <name> <rotation>",
					type: TransformObjectCommand.TransformType.Rotation),

				new DebugCommand<float>(
					id: "time_scale",
					description: "Change the time scale",
					format: "time_scale <scale>",
					execute: (scale) => {Time.timeScale = scale;},
					parser: float.TryParse),

				new DebugCommand(
					id: "reload",
					description: "Reload the current scene",
					format: "reload",
					execute: () => SceneManagerEx.LoadScene(SceneManager.GetActiveScene().name)),
				
				new DebugCommand<uint>(
					id: "",
					description: "Add or Upgrade Skill ",
					format: " <skill_id>",
					execute: (skill_id) => 
					{
						//실제 스킬 추가
						ManagerRoot.Skill.AddOrUpgradeSkill(skill_id , UnitType.SwordMan);
					},
					parser: uint.TryParse),

				new DebugCommand<int, int>(
					id: "reward",
					description: "Show RewardPanel by rank and dispersion ",
					format: "reward <rank> <dispersion>",
					execute: (rank, dispersion) => 
					{
						GameUIManager.Instance.ShowRewardPopupByRankDispersion(rank, dispersion, UnitType.None);
					},
					parser: ArgumentParserBag.TryParseTwoIntegers ),

				new DebugCommand(
					id: "win",
					description: "Win the game",
					format: "win",
					execute: () =>
					{
						GameManager.Instance.WinGame();
					}),
				new DebugCommand(
					id: "defeat",
					description: "Defeat the game",
					format: "defeat",
					execute: () =>
					{
						GameManager.Instance.DefeatGame();
					}),        
					
				new DebugCommand(
					id: "hp_max",
					description: "Make Player HP 99999999",
					format: "hp_max",
					execute: () =>
					{
						GameManager.Instance.GetPlayer().IncreaseMaxHp(99999999);
					}), 
		
				new DebugCommand(
					id: "lv_sword",
					description: "Level Up Sword Man",
					format: "lv_sword",
					execute: () =>
					{
						UnitType unitType_ = UnitType.SwordMan;
						var centerPos = GameManager.Instance.GetPlayer().transform.position;
					   	Vector2 spawnPos = (Vector2)centerPos + new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
						ManagerRoot.Unit.InstantiateUnit(unitType_, spawnPos, Faction.Human, out var unit);
						unit.Start();
						unit.DieWithNoEvent();
						unit.TurnUndead();
						unit.LevelUp(unitType_);
						unit.DieWithNoEvent();
					}),
				new DebugCommand(
					id: "lv_archer",
					description: "Level Up Archer Man",
					format: "lv_archer",
					execute: () =>
					{
						UnitType unitType_ = UnitType.ArcherMan;
						var centerPos = GameManager.Instance.GetPlayer().transform.position;
					   	Vector2 spawnPos = (Vector2)centerPos + new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
						ManagerRoot.Unit.InstantiateUnit(unitType_, spawnPos, Faction.Human, out var unit);
						unit.Start();
						unit.DieWithNoEvent();
						unit.TurnUndead();
						unit.LevelUp(unitType_);
						unit.DieWithNoEvent();

						// 일단 언데드 유닛을 바로 소환하면 null refer 에러가 뜬다
					}),
				new DebugCommand(
					id: "lv_assassin",
					description: "Level Up Assassin",
					format: "lv_assassin",
					execute: () =>
					{
						UnitType unitType_ = UnitType.Assassin;
						var centerPos = GameManager.Instance.GetPlayer().transform.position;
					   	Vector2 spawnPos = (Vector2)centerPos + new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
						ManagerRoot.Unit.InstantiateUnit(unitType_, spawnPos, Faction.Human, out var unit);
						unit.Start();
						unit.DieWithNoEvent();
						unit.TurnUndead();
						unit.LevelUp(unitType_);

						unit.DieWithNoEvent();
					}),

		
				new DebugCommand<uint>(
					id: "set_ppc_ppu",
					description: "Set Pixel Per Unit of PixelPerfectCamera",
					format: "set_ppc_ppu <ppu_value>",
					execute: (ppu_value) => 
					{
						CameraController cameraController = FindObjectOfType<CameraController>();
						cameraController.SetPPU( (int)ppu_value ) ;
					},
					parser: uint.TryParse),

				new DebugCommand<int, int>(
					id: "ppc_resol",
					description: "Change Resolution of PixelPerfectCamera to (width x height)  ",
					format: "ppc_resol <width> <height>",
					execute: (width, height) => 
					{
						CameraController cameraController = FindObjectOfType<CameraController>();
						cameraController.SetRefResolution( new Vector2Int(width, height) );
					},
					parser: ArgumentParserBag.TryParseTwoIntegers ),

		
				new DebugCommand(
					id: "all_resol",
					description: "Show all resolutions of current monitor",
					format: "all_resol",
					execute: () =>
					{
						_showAllResolutions = true;
						// Returns all full-screen resolutions that the monitor supports.
						_availableResolutions = Screen.resolutions;
					}),

				new DebugCommand(
					id: "cur_resol",
					description: "Show current resolutions of current monitor",
					format: "cur_resol",
					execute: () =>
					{
						_showCurrentResolution = true;
						_fullScreenMode    = Screen.fullScreenMode;
						_currentResolution = Screen.currentResolution; 
						// fullscreen mode 일때만 유효
						// fullscreen 이 아니면, fullScreen시의 native resolution을 반환한다.
					}),

				new DebugCommand<int, int>(
					id: "set_resol",
					description: "Change Resolution to width x height  ",
					format: "set_resol <width> <height>",
					execute: (width, height) => 
					{
						// windown mode 로 고정
						Screen.SetResolution(width, height, FullScreenMode.Windowed);
					},
					parser: ArgumentParserBag.TryParseTwoIntegers ),


				new DebugCommand(
					id: "set_fullscreen",
					description: "to FullScreenMode",
					format: "set_fullscreen",
					execute: () =>
					{
						Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.FullScreenWindow);
					}),

				new DebugCommand(
					id: "exit",
					description: "Exit the game",
					format: "exit",
					execute: () =>
					{
#if UNITY_EDITOR
						UnityEditor.EditorApplication.isPlaying = false;
#else
						Application.Quit();
#endif
					}),

				new DebugCommand(
					id:"help",
					description: "Shows a list of commands",
					format: "help",
					execute: () => _showHelp = true),
			};
		}
	}
}