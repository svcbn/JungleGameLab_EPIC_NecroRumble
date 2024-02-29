using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LOONACIA.Unity.Managers;
using UnityEngine.EventSystems;

using UnityEngine.UI;
using UnityEditor;
using Honeti;
using System;

public class UITitleScene : UIScene
{
	enum Texts
	{
	}
	enum Images
	{
		Background,
	}

	enum Elements
	{
		TitleTextPanel,
		LoadPanel,
	}
	enum GridPanels
	{
		MenuGridPanel,
		LanguageGridPanel,
	}
	enum Buttons
	{
		CreditButton,
	}

	GridLayoutGroup _menuGridPanel, _languageGridPanel;
	GameObject _loadPanel, _menuPanel;

	UIPopupButton _buttonStart, _buttonTutorial, _buttonKeyMap, _buttonSetting, _buttonExit;
	UIPopupButton _buttonKorean, _buttonEnglish, _buttonJapan;

	List<UIPopupButton> _btns = new List<UIPopupButton>();
	Button _creditButton;

	protected override void Init()
	{
		base.Init();

		Debug.Log("UITitleScene | Init");   


		Bind<Image, Images>();
		Bind<GameObject, Elements>();
		Bind<GridLayoutGroup, GridPanels>();
		Bind<Button, Buttons>();

		_loadPanel = Get<GameObject>((int)Elements.LoadPanel);
		_creditButton = Get<Button>((int)Buttons.CreditButton);

		SetMenuPanel();
		SetLanguagePanel();
		ManagerRoot.I18N.setLanguage(ManagerRoot.Settings.CurrentLanguage.ToString());
	}

	public void Redraw()
	{
		SetGrid(_menuGridPanel);
		SetLanguageGrid(_languageGridPanel);
	}

	private void OnEnable() {
		Debug.Log("UITitleScene | On Enable");

		EnableBtns();  // 다른 팝업에서 떠있을때 Navi select 방지
		_loadPanel.SetActive(false);

		EventSystem eventSystem = FindObjectOfType(typeof(@EventSystem)) as EventSystem; // todo 고도화
		eventSystem.SetSelectedGameObject(_buttonStart.gameObject);
	}

	private void OnDisable() {
		Debug.Log("UITitleScene | On Disable");

		_loadPanel.SetActive(true);
		DisableBtns();
	}

	public void SetGridSize( GridLayoutGroup grid_, Vector2 cellSize_, Vector4 padding_, Vector2 spacing_ )
	{
		// Debug.Log( $"cellSize {cellSize_.x} {cellSize_.y}" );
		// Debug.Log( $"spacing  {spacing_.x}  {spacing_.y}"  );
		// Debug.Log( $"padding  {padding_.x}  {padding_.y} {padding_.z} {padding_.w}" );
		grid_.cellSize = cellSize_;

		grid_.padding.left   = (int)padding_.x;
		grid_.padding.right  = (int)padding_.y;
		grid_.padding.top    = (int)padding_.z;
		grid_.padding.bottom = (int)padding_.w;

		grid_.spacing = spacing_;
	}

	void SetGrid( GridLayoutGroup grid_ )
	{
		Rect parentRect = grid_.GetComponent<RectTransform>().rect;
		Vector2 cellSize = new( parentRect.width * 1/2, parentRect.height * 1/6);
		Vector4 padding  = new( 0, 0, parentRect.height * 1/24, 0 );
		Vector2 spacing  = new( 0, parentRect.height * 1/48 );

		SetGridSize( grid_, cellSize, padding, spacing);

		for (int i = 0; i < grid_.transform.childCount; i++)
		{
			grid_.transform.GetChild(i).localScale = new Vector3(1, 1, 1);
		}
	}

	void SetLanguageGrid( GridLayoutGroup grid_ )
	{
		Debug.Log($"UITitleScene | SetLanguageGrid");
		Rect parentRect = grid_.GetComponent<RectTransform>().rect;
		Vector2 cellSize = new( parentRect.width, parentRect.height * 2/7);
		Vector4 padding  = new( 0, 0, 0 /*parentRect.height * 1/24*/, 0 );
		Vector2 spacing  = new( 0, parentRect.height * 1/48 );

		SetGridSize( grid_, cellSize, padding, spacing);

		for (int i = 0; i < grid_.transform.childCount; i++)
		{
			grid_.transform.GetChild(i).localScale = new Vector3(1, 1, 1);
		}
	}
	void SetMenuPanel()
	{
		_menuGridPanel = Get<GridLayoutGroup>((int)GridPanels.MenuGridPanel);

		SetGrid(_menuGridPanel);

		_buttonStart = ManagerRoot.UI.MakeSubItem<UIPopupButton>(_menuGridPanel.transform);
		_buttonStart.SetCanvasSortOrder( ManagerRoot.UI.GetOrder() );
		_buttonStart.SetButtonText("^text_title_btn_Start");
		_buttonStart.SetLocalizeText();
		_buttonStart.SetClickedAction(OnStartClick);
		_btns.Add(_buttonStart);

		_buttonTutorial = ManagerRoot.UI.MakeSubItem<UIPopupButton>(_menuGridPanel.transform);
		_buttonTutorial.SetCanvasSortOrder( ManagerRoot.UI.GetOrder() );
		_buttonTutorial.SetButtonText("^text_title_btn_Tutorial");
		_buttonTutorial.SetLocalizeText();
		_buttonTutorial.SetClickedAction(OnTutorialClick);
		_btns.Add(_buttonTutorial);

		// todo: 키맵을 설정안으로?
		_buttonKeyMap = ManagerRoot.UI.MakeSubItem<UIPopupButton>(_menuGridPanel.transform);
		_buttonKeyMap.SetCanvasSortOrder( ManagerRoot.UI.GetOrder() );
		_buttonKeyMap.SetButtonText("^text_title_btn_Keymap");
		_buttonKeyMap.SetLocalizeText();
		_buttonKeyMap.SetClickedAction(OnKeymapClick);
		_btns.Add(_buttonKeyMap);

		_buttonSetting = ManagerRoot.UI.MakeSubItem<UIPopupButton>(_menuGridPanel.transform);
		_buttonSetting.SetCanvasSortOrder( ManagerRoot.UI.GetOrder() );
		_buttonSetting.SetButtonText("^text_title_btn_Settings");
		_buttonSetting.SetLocalizeText();
		_buttonSetting.SetClickedAction(OnSettingClick);
		_btns.Add(_buttonSetting);

		_buttonExit = ManagerRoot.UI.MakeSubItem<UIPopupButton>(_menuGridPanel.transform);
		_buttonExit.SetCanvasSortOrder( ManagerRoot.UI.GetOrder() );
		_buttonExit.SetButtonText("^text_title_btn_Exit");
		_buttonExit.SetLocalizeText();
		_buttonExit.SetClickedAction(OnExitClick);
		_btns.Add(_buttonExit);

	}

	void SetLanguagePanel()
	{
		Debug.Log("UITileScene | SetLanguagePanel ");
		_languageGridPanel = Get<GridLayoutGroup>((int)GridPanels.LanguageGridPanel);

		_buttonKorean = ManagerRoot.UI.MakeSubItem<UIPopupButton>(_languageGridPanel.transform);
		_buttonKorean.SetCanvasSortOrder( ManagerRoot.UI.GetOrder() );
		_buttonKorean.SetButtonText("한국어");
		_buttonKorean.SetClickedAction(OnLanguageKoreanClick);
		_btns.Add(_buttonKorean);

		_buttonEnglish = ManagerRoot.UI.MakeSubItem<UIPopupButton>(_languageGridPanel.transform);
		_buttonEnglish.SetCanvasSortOrder( ManagerRoot.UI.GetOrder() );
		_buttonEnglish.SetButtonText("English");
		_buttonEnglish.SetClickedAction(OnLanguageEnglishClick);
		_btns.Add(_buttonEnglish);

		_buttonJapan = ManagerRoot.UI.MakeSubItem<UIPopupButton>(_languageGridPanel.transform);
		_buttonJapan.SetCanvasSortOrder( ManagerRoot.UI.GetOrder() );
		_buttonJapan.SetButtonText("日本語");
		_buttonJapan.SetClickedAction(OnLanguageJapanClick);
		_btns.Add(_buttonJapan);

		SetLanguageGrid(_languageGridPanel);

	}

	void OnLanguageKoreanClick(string btnText)
	{
		ManagerRoot.Settings.ChangeLanguage(Settings.Language.KO);
		return;
	}
	void OnLanguageEnglishClick(string btnText)
	{
		ManagerRoot.Settings.ChangeLanguage(Settings.Language.EN);
		return;
	}
	void OnLanguageJapanClick(string btnText)
	{
		ManagerRoot.Settings.ChangeLanguage(Settings.Language.JA);
		return;
	}
	void OnStartClick(string btnText)
	{
		if (!ManagerRoot.Settings.IsTutorialCleared) return;
		
		Debug.Log("UITitleScene | 게임 시작");
		UICharSelecPopup uICharSelecPopup = ManagerRoot.UI.ShowPopupUI<UICharSelecPopup>(usePool:false);
		uICharSelecPopup.SetParentPopup(this);

		this.enabled = false;
	}

	void OnTutorialClick(string btnText)
	{
		Debug.Log("UITitleScene | 튜토리얼");
		
		this.enabled = false;

		SceneManagerEx.LoadScene("Tutorial");
	}

	void OnKeymapClick(string btnText)
	{
		Debug.Log("UITitleScene | 키맵 버튼");
		UIKeymapPopup uIKeymapPopup = ManagerRoot.UI.ShowPopupUI<UIKeymapPopup>(usePool:false);
		uIKeymapPopup.SetParentPopup(this);

		this.enabled = false;
	}

	void OnSettingClick(string btnText)
	{
		Debug.Log("UITitleScene | 설정 버튼");
		UISettingPopup uISettingPopup = ManagerRoot.UI.ShowPopupUI<UISettingPopup>(usePool:false);
		uISettingPopup.SetParentPopup(this);

		this.enabled = false;
	}

	void OnExitClick(string btnText)
	{
		Debug.Log("UITitleScene | 게임 종료");

		#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif
	}


	void EnableBtns()
	{
		foreach(var btn in _btns)
		{
			btn.gameObject.SetActive(true);
		}
		_creditButton?.gameObject.SetActive(true);
	}

	void DisableBtns()
	{
		foreach(var btn in _btns)
		{
			btn.gameObject.SetActive(false);
		}
		_creditButton?.gameObject.SetActive(false);
	}
}
