using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LOONACIA.Unity.Managers;
using UnityEngine;

public class CharSelectPanelController : MonoBehaviour
{
    public static GameObject _charGridPanel;
    public static GameObject _charDescriptionPanel;
    private Vector3 _initialScale;
    void Start()
    {
        _initialScale = transform.localScale;
        _charDescriptionPanel = transform.Find("CharDescriptionPanel").gameObject;
        _charDescriptionPanel.SetActive(false);
        _charGridPanel = transform.Find("CharGridPanel").gameObject;

        UICharButton char1 = _charGridPanel.transform.Find("Char 1").gameObject.GetComponent<UICharButton>();
        UICharButton char2 = _charGridPanel.transform.Find("Char 2").gameObject.GetComponent<UICharButton>();
        
        if (ManagerRoot.Unit.CurCharacter == UnitManager.CharacterType.Necromancer)
        {
            SelectFirstChar();
            if (char1 != null && char2 != null)
            {
                char1.StartCoroutine(char1.OnSubmitCoroutine());
                // char1.transform.Find("Border").gameObject.SetActive(true);
                char2.transform.Find("Border").gameObject.SetActive(false);
            }
        }
        else
        {
            SelectSecondChar();
            if (char1 != null && char2 != null)
            {
                char2.StartCoroutine(char2.OnSubmitCoroutine());
                char1.transform.Find("Border").gameObject.SetActive(false);
                // char2.transform.Find("Border").gameObject.SetActive(true);
            }
        }
        
    }

    private IEnumerator FirstCharFrame()
    {
        yield return new WaitForSeconds(0.5f);
        
    }

    public static void AllReset()
    {
        for (int i = 0; i < _charGridPanel.transform.childCount; i++)
        {
            _charGridPanel.transform.GetChild(i).GetChild(1).GetChild(2).gameObject.SetActive(false);
        }
        
        _charDescriptionPanel.transform.GetChild(0).gameObject.SetActive(false);
        _charDescriptionPanel.transform.GetChild(1).gameObject.SetActive(false);
    }
    
    public static void SelectFirstChar()
    {
        AllReset();
        _charDescriptionPanel.SetActive(true);
        _charDescriptionPanel.transform.GetChild(0).gameObject.SetActive(true);
    }
    
    public static void SelectSecondChar()
    {
        AllReset();
        _charDescriptionPanel.SetActive(true);
        _charDescriptionPanel.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void OnEnable()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(_initialScale, 0.5f).SetEase(Ease.OutBack);
    }
    
    public void OnDisable()
    {
        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutBack);
    }
}
