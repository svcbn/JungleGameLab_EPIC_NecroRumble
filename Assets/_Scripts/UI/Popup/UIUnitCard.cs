using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using LOONACIA.Unity.Managers;
using UnityEngine.UI;

public class UIUnitCard : UIPopup
{
    enum Texts
    {
        NameText,
        DescriptionText,
        
        //Stat Container
        TextHp,
        TextDamage,
        TextRange,
        TextSpeed,
    }

    enum Images
    {
        UnitImage,
    }
    
    UIRewardButton _uIRewardButton;
    int _order = 0;
    
    protected override void Init()
    {
        base.Init(); // +1 해줌

        Bind<TMP_Text,Texts>();
        Bind<Image, Images>();

        Debug.Log($"GetOrder({ManagerRoot.UI.GetOrder()}) _order({_order})  ");

    }

    public void SetCanvasSortOrder(int order_)
    {
        _order = order_;
    }
    public void SetParent(UIRewardButton uIRewardButton_)
    {
        _uIRewardButton = uIRewardButton_;
        //transform.SetParent(uIRewardButton_.transform);
    }

    public void SetText(string name_, string desc_, int hp_, int damage_, float range_, float speed_)
    {
        Get<TMP_Text>((int)Texts.NameText       ).text = name_;
        Get<TMP_Text>((int)Texts.DescriptionText).text = desc_;
        Get<TMP_Text>((int)Texts.TextHp         ).text = hp_.ToString();
        Get<TMP_Text>((int)Texts.TextDamage     ).text = damage_.ToString();
        Get<TMP_Text>((int)Texts.TextRange      ).text = range_.ToString();
        Get<TMP_Text>((int)Texts.TextSpeed      ).text = speed_.ToString();
    }

    public void SetContent(Sprite icon_, UnitType unitType_)
    {
        PairUnitData unitData =  ManagerRoot.Unit.AllUnitData.Find(unit => unit.UnitInfo.UnitType == unitType_);
        if(unitData == null) { Debug.Log($"Not Found unit data in AllUnitData"); return; }

        BaseUnitStats stat = unitData.GetStats(Faction.Undead);

        Get<Image>((int)Images.UnitImage).sprite = icon_;

        SetText( unitData.UnitInfo.UnitName, 
                 unitData.UnitInfo.UnitDescription,
                 stat.BaseMaxHp,
                 stat.BaseAttackDamage,
                 stat.BaseAttackRange,
                 stat.BaseMoveSpeed);
    }
}
