using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using LOONACIA.Unity.Managers;
using LOONACIA.Unity;
using LOONACIA.Unity.Coroutines;

public class UIText : UIScene
{
	private string _text;

	private Color _color = Color.black;

	
	private const float _yOffset       = 2f;
	private const float _posDuration   = 1.5f;
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

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
		_position = Vector3.zero;
	}

	protected override void Init()
	{
		var canvas = gameObject.GetOrAddComponent<Canvas>();

        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingLayerName = "DamageText";
		canvas.sortingOrder = 0;
		
		Bind<TMP_Text, Texts>();
		SetInnerText();
	}
	
	public void SetInitialPosition(Vector3 position)
	{
		_position = position;
		transform.position = _position;

	}

	public void SetText(string text, Color color)
	{
		_text = text;
		_color = color;
		SetInnerText();
	}
    
	public void SetSize(float size)
	{
		TMP_Text textBox = Get<TMP_Text, Texts>(Texts.InnerText);
		textBox.fontSize = size;
	}

	private void SetInnerText()
	{
		TMP_Text textBox = Get<TMP_Text, Texts>(Texts.InnerText);
		textBox.text  = _text;
		textBox.color = _color;
	}

}
