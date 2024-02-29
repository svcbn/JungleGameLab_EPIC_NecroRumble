using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LOONACIA.Unity.Managers;
using LOONACIA.Unity;
using LOONACIA.Unity.Coroutines;
using Unity.VisualScripting;
using Honeti;

public class UIUnitExpPanel : UIScene
{
	private CoroutineEx _colorHandler;
	private const float _colorDuration = 2f;

	GridLayoutGroup _unitGridPanel;

	List<UIUnitContainer>        _unitContainers = new ();
	Dictionary<UnitType, Sprite> _unitSpriteDic  = new ();

	private enum GridPanels
	{
		UnitGridPanel
	}

	private void OnEnable()
	{
		//SetColorEffect(); // 사라지게 하는 효과. todo: 사라지게? 계속? 토글?
	}

	private void OnDisable()
	{
		if (_colorHandler?.IsRunning is true)
		{
			_colorHandler.Abort();
			_colorHandler = null;
		}
	}

	protected override void Init()
	{
		Bind<GridLayoutGroup, GridPanels>();

		LoadSprite();
		SetUnitGridPanel();
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
		Vector2 cellSize = new( parentRect.width, parentRect.height * 1/5);
		Vector4 padding  = new( 0, 0, 0, 0 );
		Vector2 spacing  = new( parentRect.width * 1/5, parentRect.width * 1/5 );

		SetGridSize( grid_, cellSize, padding, spacing);

		for (int i = 0; i < grid_.transform.childCount; i++)
		{
			grid_.transform.GetChild(i).localScale = new Vector3(1, 1, 1);
		}
	}

	public void SetUnitGridPanel() // todo: 코드개선 필요, 일단 4종류 유닛고정이라서 그냥 이렇게 함
	{
		_unitGridPanel = Get<GridLayoutGroup>((int)GridPanels.UnitGridPanel);

		_unitContainers.Clear();
		
		//MakeUnitContainer(UnitType.SpearMan);
		MakeUnitContainer(UnitType.SwordMan);
		MakeUnitContainer(UnitType.ArcherMan);
		MakeUnitContainer(UnitType.Assassin);
		//MakeUnitContainer(UnitType.HorseMan);

		SetGrid(_unitGridPanel);
	}

	void MakeUnitContainer(UnitType unitType_)
	{
		UIUnitContainer unitConta = ManagerRoot.UI.MakeSubItem<UIUnitContainer>(_unitGridPanel.transform);

		int level  = UnitManager.UnitExpDic[unitType_].level;
		int curExp = UnitManager.UnitExpDic[unitType_].curExp;
		int maxExp = UnitManager.UnitExpDic[unitType_].maxExp;
		
		bool isMaxLevel = level >= UnitManager.UNIT_MAX_LEVEL;

		switch (unitType_)
		{
			case UnitType.SwordMan:
				unitConta.SetNameText("^text_panel_exp_Swordman");
				break;
			case UnitType.ArcherMan:
				unitConta.SetNameText("^text_panel_exp_Archer");
				break;
			// case UnitType.HorseMan:
			// 	unitConta.SetNameText($"언데드 기마병");
			// 	break;
			case UnitType.Assassin:
				unitConta.SetNameText("^text_panel_exp_Assassin");
				break;
		}
		
		GameUIManager.Instance.AddUnitExpPanelPosList(unitConta.GetComponent<RectTransform>().parent.position); //파티클 들어가는 위치
		var levelText = isMaxLevel ? "Lv Max" : $"Lv {level}";
		unitConta.SetLvText(levelText);
		//unitConta.SetExpText($"{curExp}/{maxExp}");
		if (isMaxLevel is false)
		{
			unitConta.SetExpSlider((float)curExp / (float)maxExp);
		}
		else
		{
			unitConta.SetExpSlider(1);
		}

		unitConta.SetIcon(_unitSpriteDic[unitType_]);
		unitConta.SetLocalizeText();

		_unitContainers.Add(unitConta);
	}

	public void SetPosition(Vector3 position_)
	{
		transform.position = position_;
	}


	// private void SetText(string text_, Color color_)
	// {
	// 	TMP_Text unitText = Get<TMP_Text, TmpTexts>(TmpTexts.UnitText);
	// 	unitText.text = text_;
	// 	unitText.color = color_;
	// 	if (_colorHandler?.IsRunning is true) // 텍스트가 바뀌면 처음부터 새로 계산
	// 	{
	//         _colorHandler.Abort();
	// 	}
	// 	SetColorEffect();
	// }


	public void UpdateInfo(UnitType unitType_, int deltaLevel_,int deltaExp_)
	{
		Debug.Log("UIUnitExpPanel | UpdateInfo");

		ClearGridLayoutGroup(_unitGridPanel.transform);
		SetUnitGridPanel();
	}

	// private void SetColorEffect()
	// {
	// 	TMP_Text unitText = Get<TMP_Text, TmpTexts>(TmpTexts.UnitText);

	// 	if (_colorHandler?.IsRunning is true)
	// 	{
	//         _colorHandler.Abort();
	// 	}

	// 	_colorHandler = Utility.Lerp(
	//         from    : 1,
	//         to      : 0f,
	//         duration: _colorDuration,
	//         action  : (alpha) => {
	// 					unitText.color   = new(unitText.color.r, unitText.color.g, unitText.color.b, alpha);
	// 					_unitContainers.ForEach(container => container.SetAlpha(alpha));
	// 				},
	//         callback: ()      => {
	// 					_colorHandler = null;
	// 					ManagerRoot.Resource.Release(gameObject);
	// 				});

	// }


	void LoadSprite()
	{
		// Texture2D spearTex = ManagerRoot.Resource.Load<Texture2D>("Sprites/Units/Humans/Spear");
		// Sprite spearSprite = Sprite.Create(spearTex, new Rect(6,0,22,16), new Vector2(0.5f,0.5f));
		// _unitSpriteDic.Add(UnitType.SpearMan, spearSprite);

		Texture2D skeletonTex = ManagerRoot.Resource.Load<Texture2D>("Sprites/Units/Undeads/Skeleton");
		Sprite skeletonSprite = Sprite.Create(skeletonTex, new Rect(6,0,22,18), new Vector2(0.5f,0.5f));
		_unitSpriteDic.Add(UnitType.SwordMan, skeletonSprite);

		Texture2D archerTex = ManagerRoot.Resource.Load<Texture2D>("Sprites/Units/Undeads/SkeletonArcher");
		Sprite skeArcherSprite = Sprite.Create(archerTex, new Rect(6,0,22,18), new Vector2(0.5f,0.5f));
		_unitSpriteDic.Add(UnitType.ArcherMan, skeArcherSprite);

		Texture2D ghostTex = ManagerRoot.Resource.Load<Texture2D>("Sprites/Units/Undeads/Ghost");
		Sprite ghostSprite = Sprite.Create(ghostTex, new Rect(5,0,21,24), new Vector2(0.5f,0.5f));
		_unitSpriteDic.Add(UnitType.Assassin, ghostSprite);

		// Texture2D horseManTex = ManagerRoot.Resource.Load<Texture2D>("Sprites/Units/Humans/Horse");
		// Sprite horseManSprite = Sprite.Create(horseManTex, new Rect(4,0,24,26), new Vector2(0.5f,0.5f));
		// _unitSpriteDic.Add(UnitType.HorseMan, horseManSprite);

	}

	public void ClearGridLayoutGroup(Transform gridLayoutGroupTransform)
	{
		foreach (Transform child in gridLayoutGroupTransform)
		{
			Destroy(child.gameObject);
		}
	}
	
	public void CreateParticleVector(string path, Vector2 createPos)
	{
		GameObject particlePrefab = ManagerRoot.Resource.Instantiate(path, transform);
		
		//convert Vector2 into rectposition
		particlePrefab.GetComponent<RectTransform>().anchoredPosition = createPos;
	}
	
	public void SetUIBlink(int unitType, int blinkCount)
	{
		var idx = -1;
		if (unitType == (int)UnitType.SwordMan) idx = 0;
		else if (unitType == (int)UnitType.ArcherMan) idx = 1;
		else if (unitType == (int)UnitType.Assassin) idx = 2;

		if (idx == -1) return;
		
		if (blinkCount == 1)
		{
			_unitContainers[idx].Blink();
		}
		else if (blinkCount == 2)
		{
			_unitContainers[idx].BlinkTwice();
		}

	}
}
