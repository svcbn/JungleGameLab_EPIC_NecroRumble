using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellCooldown : MonoBehaviour
{
	private Image _imageCooldown;
	private TMP_Text _textCooldown;
	private Image _imageEdge;
	private Image _imageForeground;

	public SkillBase Skill;
	
	// Start is called before the first frame update
	private void Start()
	{
		Init();
		
		_textCooldown.gameObject.SetActive(false);
		_imageEdge.gameObject.SetActive(false);
		_imageCooldown.fillAmount = 0.0f;
	}

	// Update is called once per frame
	private void Update()
	{
		ApplyCooldown();
	}

	private void Init()
	{
		_textCooldown = GetComponentInChildren<TMP_Text>();
		_imageEdge = GetComponentsInChildren<Image>()[4];
		_imageCooldown = GetComponentsInChildren<Image>()[2];
		_imageForeground = GetComponentsInChildren<Image>()[3];
	}
	
	private void ApplyCooldown()
	{
		if(!Skill.IsCooldown)
		{
			_textCooldown.gameObject.SetActive(false);
			_imageEdge.gameObject.SetActive(false);
			_imageForeground.gameObject.SetActive(false);
			_imageCooldown.fillAmount = 0.0f;
		}
		else
		{
			_textCooldown.text = Mathf.RoundToInt(Skill.CurrentCooldown).ToString();
			_imageCooldown.fillAmount = Skill.CurrentCooldown / Skill.MaxCooldown;

			_imageEdge.transform.localEulerAngles = new Vector3(0, 0, 360.0f * (Skill.CurrentCooldown / Skill.MaxCooldown));
		}

	}

	// 밖에선 이거만 호출
	public bool UseSpell()
	{
		if(Skill.IsCooldown)
		{
			return false;
		}
		else
		{
			Skill.IsCooldown = true;
			_textCooldown.gameObject.SetActive(true);
			
			_textCooldown.text = Mathf.RoundToInt(Skill.CurrentCooldown).ToString();
			_imageCooldown.fillAmount = 1.0f;
			_imageForeground.gameObject.SetActive(true);
			_imageEdge.gameObject.SetActive(true);
			return true; 
		}
	}
}
