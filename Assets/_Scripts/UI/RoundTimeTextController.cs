using System.Collections;
using DG.Tweening;
using LOONACIA.Unity.Managers;
using TMPro;
using UnityEngine;

public class RoundTimeTextController : MonoBehaviour
{
    private TMP_Text roundTimeText;
    private bool isPulsating = false;

    private void Start()
    {
        roundTimeText = GetComponent<TMP_Text>();

        if (roundTimeText == null)
        {
            Debug.LogWarning("RoundTimeTextController: TMP_Text component not found!");
        }
    }

    public void Update()
    {
        int parsedValue;
        if (int.TryParse(roundTimeText.text, out parsedValue))
        {
            if (GameManager.Instance.State != GameManager.GameState.Round) return;
            if (parsedValue <= 10f && !isPulsating)
            {
                StartCoroutine(PulsateEffect());
            }
        }

    }

    IEnumerator PulsateEffect()
    {
        isPulsating = true;
        ManagerRoot.Sound.PlaySfx("Heart beating 2", 1f);
        //TODO: BGM Sound down

        float totalTime = 1f; // Total time for pulsating effect in seconds
        float sizeChangeTime = 0.3f; // Time for size change
        float colorChangeTime = 0.5f; // Time for color change

        // Scale animation
        roundTimeText.transform.DOScale(new Vector3(3f, 3f, 3f), sizeChangeTime)
            .SetEase(Ease.InExpo)
            .OnComplete(() =>
            {
                roundTimeText.transform.DOScale(new Vector3(1f, 1f, 1f), totalTime - sizeChangeTime)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            });
        
        // Color animation
        roundTimeText.DOColor(Color.red, colorChangeTime)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                roundTimeText.DOColor(Color.white, totalTime - colorChangeTime)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            });

        yield return new WaitForSeconds(totalTime);

        // Reset
        isPulsating = false;
        
        // Kill tweens
        roundTimeText.transform.DOKill();
        roundTimeText.DOKill();
    }


}