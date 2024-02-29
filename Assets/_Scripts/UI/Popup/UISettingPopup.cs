using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LOONACIA.Unity;
using LOONACIA.Unity.Managers;
using System;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;
using Honeti;

public class UISettingPopup : UIPopup
{
	enum Images
	{
		Blocker,
	}

	enum Sliders
	{
        masterVolumeSlider,
		bgmVolumeSlider,
		sfxVolumeSlider,
	}

	enum TmpDropdowns
	{
		ResolutionDropdown,
		ScreenModeDropdown,
	}

	enum GridPanels 
	{
		MenuGridPanel,
	}

    GridLayoutGroup _menuGridPanel;
    UIBase _uIBaseParent;
    TMP_Dropdown _resolDropdown, _modeDropdown;
    
    Slider _masterVolumeSlider, _bgmVolumeSlider, _sfxVolumeSlider;

	protected override void Init()
	{
		base.Init();

		Bind<Image, Images>();
		Bind<GridLayoutGroup, GridPanels>();
		Bind<Slider, Sliders>();
		Bind<TMP_Dropdown, TmpDropdowns>();

        SetMenuPanel();
        SetVolumePanel();
        SetResolutionPanel();
    

		ManagerRoot.I18N.setLanguage(Convert.ToString(ManagerRoot.Settings.CurrentLanguage));
	}

	public void SetGridSize( GridLayoutGroup grid_, Vector2 cellSize_, Vector4 padding_, Vector2 spacing_ )
	{
		grid_.cellSize = cellSize_;

		grid_.padding.left   = (int)padding_.x;
		grid_.padding.right  = (int)padding_.y;
		grid_.padding.top    = (int)padding_.z;
		grid_.padding.bottom = (int)padding_.w;
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
		buttonClose.SetCanvasSortOrder( ManagerRoot.UI.GetOrder() );
		buttonClose.SetButtonText("^text_panel_settings_btn_Close");
        buttonClose.SetLocalizeText();
		buttonClose.SetClickedAction(OnCloseClick);
		buttonClose.SetCanceledAction(OnCloseClick);

		SetGrid(_menuGridPanel);

		EventSystem eventSystem = FindObjectOfType(typeof(@EventSystem)) as EventSystem;
		eventSystem.SetSelectedGameObject(buttonClose.gameObject);
    }

	void SetVolumePanel()
	{
		_masterVolumeSlider = Get<Slider>((int)Sliders.masterVolumeSlider);
		_masterVolumeSlider.onValueChanged.AddListener((float value_) => {
			ManagerRoot.Settings.MasterVolume = value_;
            ManagerRoot.Sound.MasterVolume    = value_;
			Debug.Log($"UISettingPopup | masterSlider | {value_}");

            SetTitleCampFireVolume();
		});
        _masterVolumeSlider.value = ManagerRoot.Settings.MasterVolume;
        UISlider _masterVolUISlider = _masterVolumeSlider.gameObject.GetOrAddComponent<UISlider>();
        _masterVolUISlider.SetCanceledAction(OnCloseClick);

		_bgmVolumeSlider = Get<Slider>((int)Sliders.bgmVolumeSlider);
		_bgmVolumeSlider.onValueChanged.AddListener((float value_) => {
            ManagerRoot.Settings.BgmVolume = value_;
			ManagerRoot.Sound.BgmVolume    = value_;
			Debug.Log($"UISettingPopup | bgmSlider | {value_}");
		});
		_bgmVolumeSlider.value = ManagerRoot.Settings.BgmVolume;
        UISlider _bgmVolUISlider = _bgmVolumeSlider.gameObject.GetOrAddComponent<UISlider>();
        _bgmVolUISlider.SetCanceledAction(OnCloseClick);

		_sfxVolumeSlider = Get<Slider>((int)Sliders.sfxVolumeSlider);
		_sfxVolumeSlider.onValueChanged.AddListener((float value_) => {
			ManagerRoot.Settings.SfxVolume = value_;
			ManagerRoot.Sound.SfxVolume    = value_;
            Debug.Log($"UISettingPopup | sfxSlider | {value_}");
            
            SetTitleCampFireVolume();
		});
        _sfxVolumeSlider.value = ManagerRoot.Settings.SfxVolume;
        UISlider _sfxVolUISlider = _sfxVolumeSlider.gameObject.GetOrAddComponent<UISlider>();
        _sfxVolUISlider.SetCanceledAction(OnCloseClick);
	}

    void SetTitleCampFireVolume() // 예외처리용 함수 차후 SoundManager에 통합필요함
    {
        if( SceneManagerEx.CurrentScene.SceneType is SceneType.Title )
        {
            (SceneManagerEx.CurrentScene as TitleScene).SetCampFireVolume();
        }
    }


	public void SetParentPopup(UIBase uIBaseParent_)
	{
		_uIBaseParent = uIBaseParent_;
	
}
    void SetResolutionPanel()
    {
        List<FullScreenMode> modeOptions = new()
        {
            //FullScreenMode.ExclusiveFullScreen,
            FullScreenMode.FullScreenWindow,
            //FullScreenMode.MaximizedWindow,
            FullScreenMode.Windowed
        };
        MakeModeDropdown(modeOptions);

        List<Resolution> resolOptions = new()
        {
            new() { width = 800, height = 450 },
            new() { width = 1200, height = 675 },
            new() { width = 1600, height = 900 },
            new() { width = 1920, height = 1080 },
        };
        MakeResolDropdown(resolOptions);


        SetStateDropdown();

        //SetScreen(); // 여기서 콜 할 필요가 없음
    }

    void SetStateDropdown()
    {
        // 셋팅에서 값 가져오기
        FullScreenMode screenMode = ManagerRoot.Settings.CurrentScreenMode; // 처음에 풀
        int height = ManagerRoot.Settings.CurrentResolutionHeight;
        int width  = ManagerRoot.Settings.CurrentResolutionWidth;

        // switch(width)
        // {
        //     case 800:  _resolDropdown.value = 0; break; // 800x450
        //     case 1200: _resolDropdown.value = 1; break; // 1200x675
        //     case 1600: _resolDropdown.value = 2; break; // 1600x900
        //     case 1920: _resolDropdown.value = 3; break; // 1920x1080
        //     default: break;
        // }

        switch(height)
        {
            case 450: _resolDropdown.value = 0; break;
            case 675: _resolDropdown.value = 1; break;
            case 900: _resolDropdown.value = 2; break;
            case 1080:_resolDropdown.value = 3; break;
            default: break;
        }
    

        if(screenMode == FullScreenMode.FullScreenWindow)
        {
            _modeDropdown.value  = 0; // FullScreenWindow
            _resolDropdown.value = 3;
            _resolDropdown.interactable = false;

        }else{
            _modeDropdown.value  = 1; // Windowed
            _resolDropdown.interactable = true;
        }
    }

    void MakeModeDropdown(List<FullScreenMode> options_)
    {
        _modeDropdown = Get<TMP_Dropdown>((int)TmpDropdowns.ScreenModeDropdown);
        _modeDropdown.ClearOptions();

        List<string> optionsStr = new();
        foreach(FullScreenMode mode in options_)
        {
            optionsStr.Add(mode.ToString());
        }
        _modeDropdown.AddOptions(optionsStr);

        _modeDropdown.onValueChanged.AddListener((int idx_) => {
            _modeDropdown.value = idx_;

            ManagerRoot.Settings.SetScreenMode( options_[idx_] );

            SetScreen();
        });

        _modeDropdown.gameObject.GetOrAddComponent<UIDropdown>().SetCanceledAction(OnCloseClick);
    }

    void MakeResolDropdown(List<Resolution> options_)
    {
        _resolDropdown = Get<TMP_Dropdown>((int)TmpDropdowns.ResolutionDropdown);
        _resolDropdown.ClearOptions();

        List<string> optionsStr = new();
        foreach(Resolution resol in options_ )
        {
            optionsStr.Add( $"{resol.width} x {resol.height}" );
        }
        _resolDropdown.AddOptions(optionsStr);

        _resolDropdown.onValueChanged.AddListener((int idx_) => {
            _resolDropdown.value = idx_;

            ManagerRoot.Settings.SetResolution( options_[idx_].width, options_[idx_].height );
            ManagerRoot.Settings.SetScreenMode( FullScreenMode.Windowed );

            SetScreen();
        });

        _resolDropdown.gameObject.GetOrAddComponent<UIDropdown>().SetCanceledAction(OnCloseClick);
    }


    void SetScreen()
    {
        FullScreenMode screenMode = ManagerRoot.Settings.CurrentScreenMode;
        int width  = ManagerRoot.Settings.CurrentResolutionWidth;
        int height = ManagerRoot.Settings.CurrentResolutionHeight;

        SetStateDropdown();

        if(screenMode == FullScreenMode.FullScreenWindow)
        {
            width  = 1920;
            height = 1080;
        }

        ManagerRoot.Settings.SetResolution(width, height);
        ManagerRoot.Settings.SetScreenMode(screenMode);

        Screen.SetResolution(width, height, screenMode);

    }
	void OnCloseClick(string btnText)
	{
        ManagerRoot.Settings.Save();
		
        if( SceneManagerEx.CurrentScene is GameScene ){

            GameUIManager.Instance.CloseSettingsPopup();
        }
        else if( SceneManagerEx.CurrentScene is TutorialScene )
        {
            GameUIManager.Instance.CloseSettingsPopup();
        }
        else if( SceneManagerEx.CurrentScene is TitleScene )
        {
            ManagerRoot.UI.ClosePopupUI();
        }

        _uIBaseParent.enabled = true;
        SceneManagerEx.CurrentScene.Redraw();
    }
}
