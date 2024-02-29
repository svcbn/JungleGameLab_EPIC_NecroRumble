using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UIOverlayEffectScene : UIScene
{
    enum Cameras
    {
        UIOverlayEffectCamera,
    }

    enum Texts
    {
        TmpInfoText,
    }

    protected override void Init()
    {
        base.Init(false, true);

        Bind<TMP_Text, Texts>();
        Bind<GameObject, Cameras>();

        GameObject overlayCamGO = Get<GameObject, Cameras>(Cameras.UIOverlayEffectCamera);
        if (overlayCamGO.TryGetComponent(out Camera overlayEffCam))
        {
            Camera mainCam = Camera.main;
            UniversalAdditionalCameraData mainCameraData = mainCam.GetUniversalAdditionalCameraData();
            mainCameraData.cameraStack.Add(overlayEffCam);
        }
    }


    public void SetRoundText(string text_)
    {
        Get<TMP_Text, Texts>(Texts.TmpInfoText).text = $"Round {text_}";
    }
}
