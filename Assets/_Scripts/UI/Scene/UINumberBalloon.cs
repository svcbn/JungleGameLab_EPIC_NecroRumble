using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using TMPro;
using UnityEngine;
using LOONACIA.Unity.Managers;
using LOONACIA.Unity;
using LOONACIA.Unity.Coroutines;

public class UINumberBalloon : UIScene
{
	private string _text;

	private Color _color = Color.black;

	private CoroutineEx _positionHandler;
	private CoroutineEx _colorHandler;
	
	private const float _yOffset       = 2f;
	private float _duration   = 1.5f;
	private Vector3 _position;
	public Vector3 Position
	{
		get => _position;
		set => _position = value;
	}
	
	public float Duration
	{
		get => _duration;
		set => _duration = value;
	}

	private enum Texts
	{
		BalloonInnerText
	}

	private void OnEnable()
	{
		SetPositionEffect();
	}

	private void OnDisable()
	{
		if (_positionHandler?.IsRunning is true)
		{
            _positionHandler.Abort();
			_positionHandler = null;
		}

		if (_colorHandler?.IsRunning is true)
		{
            _colorHandler.Abort();
			_colorHandler = null;
		}

		_position = Vector3.zero;
	}

	protected override void Init()
	{
		var canvas = gameObject.GetOrAddComponent<Canvas>();

        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingLayerName = "DamageText";
		canvas.sortingOrder = 0;
		
		Bind<TextMeshProUGUI, Texts>();
		SetInnerText();
	}
	
	public void SetInitialPosition(Vector3 position)
	{
		_position = position;
		transform.position = _position;

		if (_positionHandler?.IsRunning is true)
		{
            _positionHandler.Abort();
			SetPositionEffect();
		}
	}

	public void SetText(string text, Color color)
	{
		_text = text;
		_color = color;
		SetInnerText();
	}
	
	public void SetSize(float size)
	{
		TextMeshProUGUI textBox = Get<TextMeshProUGUI, Texts>(Texts.BalloonInnerText);
		textBox.fontSize = size;
	}
    
	private void SetInnerText()
	{
		TextMeshProUGUI textBox = Get<TextMeshProUGUI, Texts>(Texts.BalloonInnerText);
		textBox.text = _text;
		textBox.color = _color;
		if (_colorHandler?.IsRunning is true)
		{
            _colorHandler.Abort();
		}
		SetColorEffect();
	}

	private void SetPositionEffect()
	{
		float end = _yOffset;
		_positionHandler = Utility.Lerp(
            from    : 0,
            to      : end,
            duration: Duration, 
            action  : (yOffset) => {
                        transform.position = _position + Vector3.up * yOffset; 
                      },
            callback: () => {
                        _positionHandler = null;
                        ManagerRoot.Resource.Release(gameObject);
                      });
	}
    
	
	private void SetColorEffect()
	{
		TextMeshProUGUI textBox = Get<TextMeshProUGUI, Texts>(Texts.BalloonInnerText);
		_colorHandler = Utility.Lerp(
            from    : 1,
            to      : 0f,
            duration: Duration,
            action  : (alpha) => textBox.color = new(textBox.color.r, textBox.color.g, textBox.color.b, alpha),
            callback: ()      => _colorHandler = null);
	}
}
