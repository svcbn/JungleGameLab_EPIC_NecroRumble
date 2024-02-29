using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using DG.Tweening;
using LOONACIA.Unity.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class UIOverlayCanvasScene : UIScene
{
	enum Elements
	{
		hpBar,
		HpContainer,
		Recall,
		Revive,
		Spear,
		Grind,
		//KeymapInfos,
		UndeadNumContainer,
	}

	enum Cameras
	{
		UIOverlayCamera,
	}

	enum Texts
	{
		hpText,
		recallText,
		reviveText,
		spearText,
		grindText,
		SkillReadyText,
		//roundText,
		roundTimeText,
		//darkEssenceText,
		undeadNumText,
	}

	enum Images
	{
		ReviveKeyImage,
		SpearKeyImage,
		GrindKeyImage,
		RecallKeyImage,
	}

	Color mpFullColor = new Color(0.04601282f, 0.06219262f, 0.2075472f, 1);
	Color mpColor = new Color(0.118592f, 0.1189871f, 0.1226415f, 1);
	private Coroutine _skillNotReadyRoutine;
	private DeviceChangeListener _deviceChangeListener => FindObjectOfType<DeviceChangeListener>();
	
	void Start()
	{
		Init();
		SetDeviceChangeKeyDescription();
		_deviceChangeListener.evtDeviceChanged += SetDeviceChangeKeyDescription;
	}

	protected override void Init()
	{
		base.Init(true);

		Bind<Camera, Cameras>();
		Bind<GameObject, Elements>();
		Bind<Image, Images>();
		Bind<TMP_Text, Texts>();

		Camera overlayCam = Get<Camera, Cameras>(Cameras.UIOverlayCamera);
		Camera mainCam = Camera.main;

		UniversalAdditionalCameraData mainCameraData = mainCam.GetUniversalAdditionalCameraData();
		mainCameraData.cameraStack.Add(overlayCam);
		
		

		SetHpBarLocalScale();
	}

	public void SetHpBarLocalScale()
	{
		GameObject hpBarObj = Get<GameObject, Elements>(Elements.hpBar);

		Vector2Int curResol = new (ManagerRoot.Settings.CurrentResolutionWidth, ManagerRoot.Settings.CurrentResolutionHeight);
        switch(curResol.x)
        {
            case 800:
				hpBarObj.transform.localScale = new Vector3(20, 150, 1);
                break;
            case 1200:
				hpBarObj.transform.localScale = new Vector3(30, 200, 1);
                break;
            case 1600:
				hpBarObj.transform.localScale = new Vector3(40, 250, 1);
                break;
            case 1920:
				hpBarObj.transform.localScale = new Vector3(50, 300, 1);
                break;
            default:
                Debug.LogWarning("UIOverlayCanvasScene | SetHp | Invalid Resolution");
                break;
        }
        Debug.Log($"UIOverlayCanvasScene | SetHp | {curResol.x}x{curResol.y} localScale: {hpBarObj.transform.localScale.x} x {hpBarObj.transform.localScale.y}");

	}

	public void SetHp(float curHp_, float maxHp_)
	{
		Get<TMP_Text, Texts>(Texts.hpText).text = $"{curHp_} / {maxHp_}";

		GameObject hpBarObj = Get<GameObject, Elements>(Elements.hpBar);
		if (hpBarObj.TryGetComponent(out HealthBar hpbar))
		{
			hpbar.HealthNormalized = curHp_ / maxHp_;
		}
	}
	
#region mp bar

	// private int _MpBarCount = 2;    //TODO : 나중에 mp바 늘어나는 일이 생기면 작업해야지~ 라고 생각하고 흔적만 남겨놨음

	// public void SetMp(float curMp_, float maxMp_)
	// {
	// 	// Get<TMP_Text, Texts>(Texts.mpText).text = $"{curMp_} / {maxMp_}";

	// 	GameObject mpBarObj = Get<GameObject, Elements>(Elements.MpContainer);
	// 	float mp1, mp2;
	// 	float eachBarMax = maxMp_ / _MpBarCount;

	// 	var mpBars = mpBarObj.GetComponentsInChildren<ManaBar>();
	// 	var meshRenderers = mpBarObj.GetComponentsInChildren<MeshRenderer>();

	// 	// 늘릴때 이부분 반복문 처리 해야될듯
	// 	if (curMp_ <= eachBarMax)
	// 	{
	// 		mp1 = curMp_;
	// 		mp2 = 0;

	// 		mpBars[0].ManaNormalized = mp1 / eachBarMax;
	// 		meshRenderers[0].materials[0].color = mpColor;

	// 		mpBars[1].ManaNormalized = mp2;
	// 		meshRenderers[1].materials[0].color = mpColor;
	// 	}
	// 	else if (curMp_ > eachBarMax && curMp_ < maxMp_)
	// 	{
	// 		mp1 = eachBarMax;
	// 		mp2 = curMp_ - eachBarMax;

	// 		mpBars[0].ManaNormalized = mp1;
	// 		meshRenderers[0].materials[0].color = mpFullColor;

	// 		mpBars[1].ManaNormalized = mp2 / eachBarMax;
	// 		meshRenderers[1].materials[0].color = mpColor;
	// 	}
	// 	else
	// 	{
	// 		mp1 = eachBarMax;
	// 		mp2 = eachBarMax;

	// 		mpBars[0].ManaNormalized = mp1;
	// 		meshRenderers[0].materials[0].color = mpFullColor;

	// 		mpBars[1].ManaNormalized = mp2;
	// 		meshRenderers[1].materials[0].color = mpFullColor;
	// 	}
	// }
#endregion
	
	public void SetShakeHp(float power_, float duration_)
	{
		GameObject hpBarObj = Get<GameObject, Elements>(Elements.hpBar);
		if (hpBarObj.TryGetComponent(out HealthBar hpbar))
		{
			hpbar.Shake(power_, duration_);
		}
	}

	// public void SetShakeMp(float power_, float duration_)
	// {
	// 	GameObject mpBarObj = Get<GameObject, Elements>(Elements.MpContainer);
	// 	var mpBars = mpBarObj.GetComponentsInChildren<ManaBar>();
	// 	foreach (var mpbar in mpBars)
	// 	{
	// 		mpbar.Shake(power_, duration_);
	// 	}
	// }

	public void SetTimeText(string roundTime_)
	{
		Get<TMP_Text, Texts>(Texts.roundTimeText).text = roundTime_;
	}

	public TMP_Text GetTimeText() => Get<TMP_Text, Texts>(Texts.roundTimeText);

	// public void SetDarkEssenceText(uint darkEssence_)
	// {
	// 	Get<TMP_Text, Texts>(Texts.darkEssenceText).text = $"{darkEssence_}";
	// }

	public void SetUndeadNumText(int curNum_, int maxNum_, bool redColor)
	{
		TMP_Text uNumText = Get<TMP_Text, Texts>(Texts.undeadNumText);
		uNumText.text = $"{curNum_} / {maxNum_}";
		if (redColor) StartCoroutine(RedTextRoutine(uNumText));
	}
	
	public void SetUndeadNumDoScale()
	{
		GameObject spearObj = Get<GameObject, Elements>(Elements.UndeadNumContainer);
		spearObj.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.1f)
			.SetEase(Ease.OutQuad) 
			.OnComplete(() =>
			{
				spearObj.transform.DOScale(Vector3.one, 0.1f)
					.SetEase(Ease.InQuad);
			});
	}
	
	public void SetHpBarDoScale()
	{
		GameObject hpBarObj = Get<GameObject, Elements>(Elements.HpContainer);
		hpBarObj.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f)
			.SetEase(Ease.OutQuad) 
			.OnComplete(() =>
			{
				hpBarObj.transform.DOScale(Vector3.one, 0.1f)
					.SetEase(Ease.InQuad);
			});
	}
	
	public void SetRecallDoScale()
	{
		GameObject recallObj = Get<GameObject, Elements>(Elements.Recall);
		recallObj.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f)
			.SetEase(Ease.OutQuad) 
			.OnComplete(() =>
			{
				recallObj.transform.DOScale(Vector3.one, 0.1f)
					.SetEase(Ease.InQuad);
			});
	}
	
	public void SetReviveDoScale(int lateMultiplier = 1)
	{
		GameObject reviveObj = Get<GameObject, Elements>(Elements.Revive);
		reviveObj.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f * lateMultiplier)
			.SetEase(Ease.OutQuad) 
			.OnComplete(() =>
			{
				reviveObj.transform.DOScale(Vector3.one, 0.1f * lateMultiplier)
					.SetEase(Ease.InQuad);
			});
	}
	
	public void SetSpearDoScale()
	{
		GameObject spearObj = Get<GameObject, Elements>(Elements.Spear);
		spearObj.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f)
			.SetEase(Ease.OutQuad) 
			.OnComplete(() =>
			{
				spearObj.transform.DOScale(Vector3.one, 0.1f)
					.SetEase(Ease.InQuad);
			});
	}
	
	public void SetGrindDoScale()
	{
		GameObject grindObj = Get<GameObject, Elements>(Elements.Grind);
		grindObj.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f)
			.SetEase(Ease.OutQuad) 
			.OnComplete(() =>
			{
				grindObj.transform.DOScale(Vector3.one, 0.1f)
					.SetEase(Ease.InQuad);
			});
	}
	
	
	IEnumerator RedTextRoutine(TMP_Text text)
	{
		// Color prevColor = text.color;
		text.color = Color.red;
		yield return new WaitForSeconds(0.5f);
		text.color = Color.white;
	}
	
	public void SetRecallCooldown()
	{
		GameObject recallObj = Get<GameObject, Elements>(Elements.Recall);
		if (recallObj.TryGetComponent(out SpellCooldown cooldown))
		{
			cooldown.UseSpell();
		}
	}

	public void SetReviveCooldown()
	{
		GameObject reviveObj = Get<GameObject, Elements>(Elements.Revive);
		if (reviveObj.TryGetComponent(out SpellCooldown cooldown))
		{
			cooldown.UseSpell();
		}
	}

	public void SetSpearCooldown()
	{
		GameObject spearObj = Get<GameObject, Elements>(Elements.Spear);
		if (spearObj.TryGetComponent(out SpellCooldown cooldown))
		{
			cooldown.UseSpell();
		}
	}
	public void SetGrindCooldown()
	{
		GameObject grindObj = Get<GameObject, Elements>(Elements.Grind);
		if (grindObj.TryGetComponent(out SpellCooldown cooldown))
		{
			cooldown.UseSpell();
		}
	}
	
	public void SetSkillCoolDownText(int mode = 0)
	{
		var finalText = "";
        if (mode == 1) finalText = $"{ManagerRoot.I18N.getValue("^text_popup_skill_OutOfSkullNum")}";
        else finalText = $"{ManagerRoot.I18N.getValue("^text_popup_skill_NotReady")}";
		TMP_Text skillReadyText = Get<TMP_Text, Texts>(Texts.SkillReadyText);
		skillReadyText.text = finalText;
		if (_skillNotReadyRoutine != null) StopCoroutine(_skillNotReadyRoutine);
		_skillNotReadyRoutine = StartCoroutine(ClearSkillCoolDownText());
	}
	
	public IEnumerator ClearSkillCoolDownText()
	{
		yield return new WaitForSeconds(1f);
		TMP_Text skillReadyText = Get<TMP_Text, Texts>(Texts.SkillReadyText);
		skillReadyText.text = "";
	}
	
	// public void SetKeyMapInfos(bool isActive_)
	// {
	// 	if (SceneManagerEx.CurrentScene.SceneType is not SceneType.Game) return;
		
	// 	GameObject keymapObj = Get<GameObject, Elements>(Elements.KeymapInfos);
	// 	if(isActive_)
	// 	{
	// 		keymapObj.SetActive(true);
	// 	}
	// 	else
	// 	{
	// 		keymapObj.SetActive(false);
	// 	}
	// }
	
	public void SetDeviceChangeKeyDescription()
	{
		Image reviveKeyImage = Get<Image, Images>(Images.ReviveKeyImage);
		Image spearKeyImage  = Get<Image, Images>(Images.SpearKeyImage);
		Image grindKeyImage  = Get<Image, Images>(Images.GrindKeyImage);
		Image recallKeyImage = Get<Image, Images>(Images.RecallKeyImage);

		if (ManagerRoot.Input.LastUsedInputType == DeviceChangeListener.ControlDeviceType.KeyboardAndMouse)
		{
			reviveKeyImage.sprite = Resources.Load<Sprite>("Sprites/UIs/UI_SpaceSmall");
			spearKeyImage.sprite  = Resources.Load<Sprite>("Sprites/UIs/UIMouseLeftClick");
			grindKeyImage.sprite  = Resources.Load<Sprite>("Sprites/UIs/UIMouseRightClick");
			recallKeyImage.sprite = Resources.Load<Sprite>("Sprites/UIs/UI_Q");
		}
		else
		{
			reviveKeyImage.sprite = Resources.Load<Sprite>("Sprites/UIs/UIGamepadAButton");
			spearKeyImage.sprite  = Resources.Load<Sprite>("Sprites/UIs/UI_LBRB");
			grindKeyImage.sprite  = Resources.Load<Sprite>("Sprites/UIs/UI_LTRT");
			recallKeyImage.sprite = Resources.Load<Sprite>("Sprites/UIs/UIGamepadXButton");
		}
	}
}
