using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LOONACIA.Unity.Managers;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Honeti;

public class UIKeymapPopup : UIPopup
{
	enum Images
	{
		Blocker,
	}

	enum GridPanels 
	{
		MenuGridPanel,
	}

	GridLayoutGroup _menuGridPanel;

	UIBase _uIBaseParent;
	

	protected override void Init()
	{
		base.Init();

		Bind<Image, Images>();
		Bind<GridLayoutGroup, GridPanels>();

		SetMenuPanel();
		ManagerRoot.I18N.setLanguage(Convert.ToString(ManagerRoot.Settings.CurrentLanguage));
	}

	public void SetParentPopup(UIBase uIBaseParent_)
	{
		Debug.Log($"UIKeymapPopup | SetParentPopup | {uIBaseParent_.name}");

		_uIBaseParent = uIBaseParent_;
	}

	private void OnEnable() {
		Debug.Log("UIKeymapPopup | OnEnable");   
	}

	private void OnDisable() {
		Debug.Log("UIKeymapPopup | OnDisable");
	}


	public void SetGridSize( GridLayoutGroup grid_, Vector2 cellSize_, Vector4 padding_, Vector2 spacing_ )
	{
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
		Vector2 cellSize = new( parentRect.width * 4/5, parentRect.height * 4/5 );
		Vector4 padding  = new( 0, 0, parentRect.height * 1/10, 0 );
		Vector2 spacing  = new( 0, parentRect.height * 1/10 );

		SetGridSize( grid_, cellSize, padding, spacing);

		for (int i = 0; i < grid_.transform.childCount; i++)
		{
			grid_.transform.GetChild(i).localScale = new Vector3(1, 1, 1);
		}
	}

	void SetMenuPanel()
	{
		_menuGridPanel = Get<GridLayoutGroup>((int)GridPanels.MenuGridPanel);

		UIPopupButton buttonClose = ManagerRoot.UI.MakeSubItem<UIPopupButton>(transform);
		buttonClose.transform.SetParent(_menuGridPanel.transform);
		buttonClose.name = "Close Button";
		buttonClose.SetCanvasSortOrder( ManagerRoot.UI.GetOrder() );
		buttonClose.SetButtonText("^text_panel_keymap_btn_Close");
		buttonClose.SetLocalizeText();
		buttonClose.SetClickedAction(OnCloseClick);
		buttonClose.SetCanceledAction(OnCloseClick);

		SetGrid(_menuGridPanel);

		EventSystem eventSystem = FindObjectOfType(typeof(@EventSystem)) as EventSystem;
		eventSystem.SetSelectedGameObject(buttonClose.gameObject);
	}

	void OnCloseClick(string btnText)
	{
		switch(SceneManagerEx.CurrentScene.SceneType)
		{
			case SceneType.Title:
				ManagerRoot.UI.ClosePopupUI();
				break;
			case SceneType.Game:
				GameUIManager.Instance.CloseKeymapPopup();
				break;
			case SceneType.Tutorial:
				GameUIManager.Instance.CloseKeymapPopup();
				break;
		}

		_uIBaseParent.enabled = true;
	}
}
