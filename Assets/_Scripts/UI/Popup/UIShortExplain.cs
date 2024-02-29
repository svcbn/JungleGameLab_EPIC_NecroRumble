using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using LOONACIA.Unity.Managers;
using UnityEngine.UI;
using Honeti;
using System;
using LOONACIA.Unity;

public class UIShortExplain : UIPopup
{
	enum Texts
	{
		NameText,
		DescriptionText,
	}

	
	protected override void Init()
	{
		base.Init();

		Bind<TMP_Text,Texts>();

	}

	public void SetSize(float fivotX_)
	{
		float width;
		float height;

        int resolHeight = ManagerRoot.Settings.CurrentResolutionHeight;
		switch(resolHeight)
        {
            case 450:  width = 120; height = 50;  break;
            case 675:  width = 170; height = 60;  break;
            case 900:  width = 220; height = 100; break;
            case 1080: width = 300; height = 150; break;
            default:   width = 300; height = 150; break;
        }
		transform.localPosition = new Vector3(fivotX_, 0, 0);
        GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
	}

	public void SetText(string name_, string desc_)
	{
		Get<TMP_Text>((int)Texts.NameText       ).text = name_;
		Get<TMP_Text>((int)Texts.DescriptionText).text = desc_;
	}

	public void SetContent(SkillDataAttribute skillDataAttribute_)
	{
		if (skillDataAttribute_ == null) { Debug.LogWarning($"skillDataAttribute is null"); return; }
		Debug.Log($"UIShortExplain | SetContent | {skillDataAttribute_.DisplayName} {skillDataAttribute_.Description} ");

		SetText(skillDataAttribute_.DisplayName,
				skillDataAttribute_.Description
				 );
		SetLocalizeText();
		ManagerRoot.I18N.setLanguage(Convert.ToString(ManagerRoot.Settings.CurrentLanguage));
	}

	public void SetLocalizeText()
	{
		Get<TMP_Text>((int)Texts.NameText       ).gameObject.GetOrAddComponent<I18NText>();
		Get<TMP_Text>((int)Texts.DescriptionText).gameObject.GetOrAddComponent<I18NText>();
	}
}
