using System.Collections;
using System.Collections.Generic;
using LOONACIA.Unity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton_test : UIPopup
{

    enum Buttons
    {
        PointButton,
    }

    enum Texts
    {
        PointText,
        ScoreText,
    }

    enum GameObjects
    {
        TestObject,
    }

    enum Images
    {
        PointImage,
    }

    private void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        Bind<Button,Buttons>();
        Bind<Text,Texts>();
        Bind<GameObject,GameObjects>();
        Bind<Image,Images>();

       
        Get<Text>((int)Texts.PointText).text = "Point";
        
        GameObject go = Get<Image>((int)Images.PointImage).gameObject;
        RegisterPointerEvent((PointerEventData data)=>{ go.transform.position = data.position;}, UIEventType.Click);

        var button = Get<Button>((int)Buttons.PointButton);
        
    }

    public void OnButtonClicked(PointerEventData data)
    {
        int point = Random.Range(1, 100);
        Get<Text>((int)Texts.PointText).text = $"Point : {point}";
    }
}