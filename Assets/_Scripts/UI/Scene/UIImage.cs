using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LOONACIA.Unity.Managers;
using LOONACIA.Unity;
using LOONACIA.Unity.Coroutines;

public class UIImage : UIScene
{
	private string _text;

	private Color _color = Color.black;
	
	private CoroutineEx _positionHandler;
	
	private const float _yOffset       = 1f;
	private const float _posDuration   = 1f;
	private const float _colorDuration = 1f;
	private Vector3 _position;
	public Vector3 Position
	{
		get => _position;
		set => _position = value;
	}

	private enum Texts
	{
		InnerText
	}

	private enum Images
	{
		InnerImage
	}
	
	private void OnEnable()
	{
		if (SceneManagerEx.CurrentScene.SceneType is SceneType.Tutorial) return;
		SetPositionEffect();
	}

	private void OnDisable()
	{
		if (_positionHandler?.IsRunning is true)
		{
			_positionHandler.Abort();
			_positionHandler = null;
		}

		_position = Vector3.zero;
	}

	protected override void Init()
	{
		var canvas = gameObject.GetOrAddComponent<Canvas>();

		canvas.renderMode = RenderMode.WorldSpace;
		canvas.sortingLayerName = "DamageText";
		canvas.sortingOrder = 0;
		
		Bind<TMP_Text, Texts>();
		Bind<Image, Images>();
		SetInnerText();
	}
	
	public void SetInitialPosition(Vector3 position, bool isDestroy = false)
	{
		_position = position;
		transform.position = _position;
		if (isDestroy)
		{
			if (_positionHandler?.IsRunning is true)
			{
				_positionHandler.Abort();
				SetPositionEffect();
			}
		}
	}

	public void SetText(string text, Color color, Vector3 offset = default)
	{
		_text = text;
		_color = color;
		SetInnerText(offset);
	}
	
	public void SetSize(float size)
	{
		TMP_Text textBox = Get<TMP_Text, Texts>(Texts.InnerText);
		textBox.fontSize = size;
	}

	private void SetInnerText(Vector3 offset = default)
	{
		TMP_Text textBox = Get<TMP_Text, Texts>(Texts.InnerText);
		textBox.text  = _text;
		textBox.color = _color;
		if (offset != default)
		{
			textBox.rectTransform.anchoredPosition = offset;
		}
	}

	public void SetInnerImage(Sprite sprite, float size = 1f)
	{
		Image image = Get<Image, Images>(Images.InnerImage);
		image.sprite = sprite;
		image.rectTransform.localScale = new Vector3(size, size, 1f);
	}
	
	public void SetInnerImageWithImageData(Sprite sprite_)
	{
		Image image = Get<Image, Images>(Images.InnerImage);
		image.sprite = sprite_;

		float originalScale = .25f;
		float referenceSize = 16f;
		float targetReferenceSize = image.sprite.rect.width;
		
		float scaleFactor = originalScale * (targetReferenceSize / referenceSize);

		RectTransform rectTransform = image.rectTransform;
		rectTransform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
	}

	private void SetPositionEffect()
	{
		float end = _yOffset;
		_positionHandler = Utility.Lerp(
			from    : 0,
			to      : end,
			duration: _posDuration, 
			action  : (yOffset) => {
				transform.position = _position + Vector3.up * yOffset; 
			},
			callback: () => {
				_positionHandler = null;
				ManagerRoot.Resource.Release(gameObject);
			});
	}
}
