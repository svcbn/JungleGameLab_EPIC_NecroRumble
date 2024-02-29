using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LOONACIA.Unity.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

public class UIRewardAnimationsInit : MonoBehaviour
{
    [Button("Start Appear Animation")]
    public void StartAppearAnimation()
    {
        transform.localScale = Vector3.one * 0.05f;

        float overshootAmount = 0.5f;

        var seq = DOTween.Sequence()
            .SetUpdate(true); //시간 멈춰도 애니메이션은 계속 돌아가게 하기

        // Stretch and squash
        seq.Append(transform.DOScale(new Vector3(1.2f, 0.8f, 1f), 0.2f))
            .Append(transform.DOScale(new Vector3(0.8f, 1.2f, 1f), 0.2f));
            //.Append(transform.DOScale(Vector3.one, 0.3f));

            seq.Join(transform.DOScale(Vector3.one * (1 + overshootAmount), 0.2f).SetEase(Ease.OutBack))
                .Append(transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InCubic))
                .InsertCallback(seq.Duration() - 0.05f, () =>
                {
                    ManagerRoot.Sound.PlaySfx("Body Flesh 10", 1f);
                });

        seq.Play();
    }
    
    public void PlayRareRewardAnimation()
    {
        float zoomInScale = 1.5f;
        //float zoomInScale2 = 1.6f;
        float zoomOutScale = 1.0f;
        float zoomInDuration = 1f;
        //float zoomIn2Duration = .1f;
        float zoomOutDuration = .05f;
        float flipDuration = 1f;
        //float shakeStrength = 0.2f;
        
        int curPosIdx = TryGetComponent(out UIRewardButton uIRewardButton) ? uIRewardButton.CurPosOrder : -1;
        if (curPosIdx == -1)
        {
            Debug.LogWarning("curPosIdx is -1");
            return;
        }

        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 originalPos = rectTransform.anchoredPosition;
        Vector2 offset = new Vector2(0, 0);
        
        if (curPosIdx == 0) offset = new Vector2(0f, 50f);
        else if (curPosIdx == 2) offset = new Vector2(0f, -50f);

        var seq = DOTween.Sequence()
            .SetUpdate(true); //시간 멈춰도 애니메이션은 계속 돌아가게 하기

        seq.Append(transform.DOScale(Vector3.one * zoomInScale, zoomInDuration)).SetEase(Ease.OutCubic)
            //.Join(rectTransform.DOAnchorPos(originalPos + offset, zoomInDuration)).SetEase(Ease.OutCubic) -> 조금 띄엄띄엄 움직이게 하려다가 실패
            .Join(transform.DORotate(new Vector3(360, 0, 0), flipDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic))
            // .AppendInterval(.1f)
            // .Append(transform.DOScale(Vector3.one * zoomInScale2, zoomIn2Duration).SetEase(Ease.InBack))
            // //.Join(rectTransform.DOAnchorPos(originalPos, zoomInDuration)).SetEase(Ease.OutCubic)
            // .AppendInterval(.2f)
            // .Append(transform.DOScale(Vector3.one * zoomInScale2, .2f).SetEase(Ease.InBack))
            // .AppendCallback(() =>
            // {
            //     ManagerRoot.Sound.PlaySfx("Staff Hitting (Flesh) 2", 1f);
            // })
            .Append(transform.DOScale(Vector3.one * zoomOutScale, zoomOutDuration).SetEase(Ease.OutCubic))
            .InsertCallback(seq.Duration() - 0.05f, () =>
            {
                ManagerRoot.Sound.PlaySfx("Staff Hitting (Flesh) 2", 1f);
            })
            .InsertCallback(seq.Duration() - 1.05f, () =>
            {
                ManagerRoot.Sound.PlaySfx("Spear throw 5", 1f);
            });
        
        seq.Play();
    }
}
