using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LOONACIA.Unity.Managers;
using System;
using TMPro;
using UnityEditor;
using Honeti;

public class UIDeviceScene : UIScene
{
	enum Texts
	{
		InputText,
	}

	enum Images
	{
		InputImage,
	}
	
	protected override void Init()
	{
		//base.Init();
		
		ManagerRoot.UI.SetCanvas(gameObject, false);
		
		Bind<TMP_Text, Texts>();
		Bind<Image, Images>();

		//TODO: Init이 어쩔 때만 불린다.. 왜이러지?
		if (ManagerRoot.Input.LastUsedInputType == DeviceChangeListener.ControlDeviceType.Gamepad)
		{
			Get<Image>((int)Images.InputImage).sprite = ManagerRoot.Resource.Load<Sprite>("Sprites/UIs/UIGamepad");
			Get<TMP_Text>((int)Texts.InputText).text = "^text_panel_device_Gamepad";
		}
		else
		{
			Get<Image>((int)Images.InputImage).sprite = ManagerRoot.Resource.Load<Sprite>("Sprites/UIs/UIKeyboard");
			Get<TMP_Text>((int)Texts.InputText).text = "^text_panel_device_Keyboard";
			
		}

		ManagerRoot.I18N.setLanguage(Convert.ToString(ManagerRoot.Settings.CurrentLanguage));
	}
	
}
