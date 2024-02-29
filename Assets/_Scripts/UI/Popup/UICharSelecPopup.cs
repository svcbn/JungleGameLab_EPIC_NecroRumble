using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LOONACIA.Unity.Managers;
using System;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Honeti;

public class UICharSelecPopup : UIPopup
{
	enum Images
	{
		Blocker,
	}

    enum GridPanels
    {
        CharGridPanel,
        StartGridPanel,
        MenuGridPanel,
        DifficultyGridPanel,
    }

    GridLayoutGroup _charGridPanel, _menuGridPanel, _startGridPanel,_difficultyGridPanel;
    List<GameObject> difficultyButtons = new List<GameObject>();

	UIBase _uIBaseParent;
	
	Sprite _char1Sprite, _char2Sprite;

	UICharButton _btnChar1, _btnChar2;
	UIPopupButton _btnStart, _btnClose;

	protected override void Init()
	{
		base.Init();

		Bind<Image, Images>();
		Bind<GridLayoutGroup, GridPanels>();

		LoadCharSprite();

		SetCharPanel();
        SetStartPanel();
		SetMenuPanel();
		SetDifficultyPanel();
		ManagerRoot.I18N.setLanguage(Convert.ToString(ManagerRoot.Settings.CurrentLanguage));
	}

	public void SetParentPopup(UIBase uIBaseParent_)
	{
		_uIBaseParent = uIBaseParent_;
	}

	private void OnEnable() {

		EventSystem eventSystem = FindObjectOfType(typeof(@EventSystem)) as EventSystem;
		eventSystem.SetSelectedGameObject(_btnChar1.gameObject);
		SetDifficultyPanel();
	}

	private void OnDisable() {
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

	void Update()
	{
		SetCharGrid();
		SetMenuGrid();
	}

	void SetCharGrid()
	{
		Rect parentRect = _charGridPanel.GetComponent<RectTransform>().rect;
		Vector2 cellSize = new( parentRect.width * 2/5, parentRect.height * 4/5 );
		Vector4 padding  = new( 0, 0, 0, 0 );
		Vector2 spacing  = new( parentRect.width * 1/10, 0 );

		SetGridSize( _charGridPanel, cellSize, padding, spacing);

		// 해상도 변경시 GridLayoutGroup의 cellSize가 변경되는데, 이때 자식의 scale이 변경되는것을 방지
		for (int i = 0; i < _charGridPanel.transform.childCount; i++)
		{
			_charGridPanel.transform.GetChild(i).localScale = new Vector3(1, 1, 1);
		}
	}

	void SetMenuGrid()
	{
		Rect parentRect = _menuGridPanel.GetComponent<RectTransform>().rect;
		Vector2 cellSize = new( parentRect.width * 4/5, parentRect.height * 4/5 );
		Vector4 padding  = new( 0, 0, parentRect.height * 1/10, 0 );
		Vector2 spacing  = new( 0, parentRect.height * 1/10 );

		SetGridSize( _menuGridPanel, cellSize, padding, spacing);

        for (int i = 0; i < _menuGridPanel.transform.childCount; i++)
        {
            _menuGridPanel.transform.GetChild(i).localScale = new Vector3(1, 1, 1);
        }
    }

    void SetStartGrid()
    {
        Rect parentRect = _startGridPanel.GetComponent<RectTransform>().rect;
        Vector2 cellSize = new( parentRect.width * 4/5, parentRect.height * 4/5 );
        Vector4 padding  = new( 0, 0, parentRect.height * 1/10, 0 );
        Vector2 spacing  = new( 0, parentRect.height * 1/10 );

        SetGridSize( _startGridPanel, cellSize, padding, spacing);

        for (int i = 0; i < _startGridPanel.transform.childCount; i++)
        {
            _startGridPanel.transform.GetChild(i).localScale = new Vector3(1, 1, 1);
        }
    }
    
    void SetCharPanel()
    {
        _charGridPanel = Get<GridLayoutGroup>((int)GridPanels.CharGridPanel);

		_btnChar1 = ManagerRoot.UI.MakeSubItem<UICharButton>();
		_btnChar1.transform.SetParent(_charGridPanel.transform);
		_btnChar1.name = "Char 1";
		_btnChar1.SetCanvasSortOrder( ManagerRoot.UI.GetOrder() );
		_btnChar1.SetCharIcon(_char1Sprite);
		_btnChar1.SetClickedAction(OnChar1Select);
		_btnChar1.SetCanceledAction(OnCloseClick);


		_btnChar2 = ManagerRoot.UI.MakeSubItem<UICharButton>();
		_btnChar2.transform.SetParent(_charGridPanel.transform);
		_btnChar2.name = "Char 2";
		_btnChar2.SetCanvasSortOrder( ManagerRoot.UI.GetOrder() );
		_btnChar2.SetCharIcon(_char2Sprite);
		_btnChar2.SetClickedAction(OnChar2Select);
		_btnChar2.SetCanceledAction(OnCloseClick);


		SetCharGrid();

    }

    void SetStartPanel()
    {
        _startGridPanel = Get<GridLayoutGroup>((int)GridPanels.StartGridPanel);

        _btnStart = ManagerRoot.UI.MakeSubItem<UIPopupButton>();
        _btnStart.transform.SetParent(_startGridPanel.transform);
        _btnStart.SetCanvasSortOrder( ManagerRoot.UI.GetOrder() );
        _btnStart.SetButtonText("^text_panel_character_btn_Start");
		_btnStart.SetLocalizeText();
        _btnStart.SetClickedAction(OnGameStart);
        _btnStart.SetCanceledAction(OnCloseClick);
        
        SetStartGrid();

    }

    void SetDifficultyPanel()
    {
	    _difficultyGridPanel = Get<GridLayoutGroup>((int)GridPanels.DifficultyGridPanel);
	    for (int i = 0; i < _difficultyGridPanel.transform.childCount; i++)
	    {
		    difficultyButtons.Add(_difficultyGridPanel.transform.GetChild(i).gameObject);
		    Button button = _difficultyGridPanel.transform.GetChild(i).GetComponent<Button>();
		    GameObject LockImage = _difficultyGridPanel.transform.GetChild(i).Find("Lock").gameObject;
		    if (i > GameManager.Instance.ClearedDifficulty)
		    {
			    button.interactable = false;
			    LockImage.SetActive(true);
		    }
		    else
		    {
			    button.interactable = true;
			    LockImage.SetActive(false);
		    }
		    
		    
		    int difficultyLevel = i + 1;
		    button.onClick.AddListener(() => SetDifficulty(difficultyLevel));
	    }
	    var finalDifficultyNum = Mathf.Min(_difficultyGridPanel.transform.childCount, GameManager.Instance.ClearedDifficulty + 1);
	    SetDifficulty(finalDifficultyNum);
	    
    }

    public void SetDifficulty(int difficulty_)
    {
	    if (difficulty_ > GameManager.Instance.ClearedDifficulty + 1)
	    {
		    //RectTransform targetTransform = difficultyButtons[difficulty_ - 1].transform as RectTransform;
		    //targetTransform.DOShakePosition(0.1f, 20, 10, 90, false, true);
		    return;
	    }
	    
	    foreach (var btn in difficultyButtons)
	    {
		    GameObject border = btn.transform.GetChild(0).gameObject;
		    border.SetActive(false);
	    }
	    ManagerRoot.Sound.PlaySfx("Simple Click Sound 1");
	    if (difficulty_ > 0 && difficulty_ <= difficultyButtons.Count)
	    {
		    difficultyButtons[difficulty_ - 1].transform.GetChild(0).gameObject.SetActive(true);
		    GameManager.Instance.CurDifficulty = difficulty_;
	    }
	    
	    TextMeshProUGUI difficultyDescriptionText = transform.Find("DifficultyDescriptionText").GetComponent<TextMeshProUGUI>();
	    difficultyDescriptionText.text = ManagerRoot.I18N.getValue($"^text_panel_difficultyDescription_{difficulty_}");
	    //ManagerRoot.I18N.setLanguage(Convert.ToString(ManagerRoot.Settings.CurrentLanguage));
    }
    
    void SetMenuPanel()
    {
        _menuGridPanel = Get<GridLayoutGroup>((int)GridPanels.MenuGridPanel);

		_btnClose = ManagerRoot.UI.MakeSubItem<UIPopupButton>();
		_btnClose.transform.SetParent(_menuGridPanel.transform);
		_btnClose.SetCanvasSortOrder( ManagerRoot.UI.GetOrder() );
		_btnClose.SetButtonText("^text_panel_character_btn_Totitle");
		_btnClose.SetLocalizeText();
		_btnClose.SetClickedAction(OnCloseClick);
		_btnClose.SetCanceledAction(OnCloseClick);


        SetMenuGrid();
    }

    public static void OnChar1Select(string text_)
    {
        ManagerRoot.Unit.CurCharacter = UnitManager.CharacterType.Necromancer;
        CharSelectPanelController.SelectFirstChar();
    }

    public static void OnChar2Select(string text_)
    {
        ManagerRoot.Unit.CurCharacter = UnitManager.CharacterType.CutePlayer;
        CharSelectPanelController.SelectSecondChar();
    }

    public static void OnGameStart(string btnText)
    {
        SceneManagerEx.LoadScene("Main");
    }
    
    void OnCloseClick(string btnText)
    {
        ManagerRoot.UI.ClosePopupUI();

		_uIBaseParent.enabled = true;
	}


	void LoadCharSprite()
	{
		Texture2D char1Tex = ManagerRoot.Resource.Load<Texture2D>("Sprites/Units/Player/Necromancer");
		_char1Sprite = Sprite.Create(char1Tex, new Rect(6,0,20,22), new Vector2(0.5f,0.5f));

		Texture2D char2Tex = ManagerRoot.Resource.Load<Texture2D>("Sprites/Units/Player/CutePlayer_InGame");
		_char2Sprite = Sprite.Create(char2Tex, new Rect(6,0,20,22), new Vector2(0.5f,0.5f));
	}
}
