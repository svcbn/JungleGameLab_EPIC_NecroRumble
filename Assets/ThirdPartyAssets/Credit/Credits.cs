using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;
using DG.Tweening;
using LOONACIA.Unity.Managers;

using Unity.VisualScripting;

public class Credits : MonoBehaviour {

    public bool play;
    public int delay;
    public RectTransform textTrans;
    public Text text;
    public TextAsset creditsFile;

    public float lineHeight;
    public float yDistance;
    public float scrollSpeed;
    public int maxLinesOnScreen;

    private WaitForSeconds delayWFS;
    private float y;
    private Vector2 startingPos;
    private int linesDisplayed;

    private string[][] creditLines;
    private StringBuilder sb;

    public const string COLUMN_SEP = " - ";
    public const string ROW_SEP = "\n";
    
    private GameObject _zombieImage;
    private GameObject _kingManImage;
    private GameObject _priestImage; 
    private GameObject _lichImage; 
    private GameObject _dreadKnightImage; 

    public void Start()
    {
        _zombieImage      = transform.Find("ZombieImage").gameObject;
        _kingManImage     = transform.Find("KingManImage").gameObject;
        _priestImage      = transform.Find("PriestImage").gameObject;
        _lichImage        = transform.Find("LichImage").gameObject;
        _dreadKnightImage = transform.Find("DreadKnightImage").gameObject;
        
        _zombieImage.SetActive(false);
        _kingManImage.SetActive(false);
        _priestImage.SetActive(false);
        _lichImage.SetActive(false);
        _dreadKnightImage.SetActive(false);
        
        delayWFS = new WaitForSeconds(delay);
        startingPos = textTrans.anchoredPosition;

        sb = new StringBuilder();
        text.text = "";

        textTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxLinesOnScreen * lineHeight);

        //Break up our credits file into a jagged array
        //Every return (\r\n) is a new row
        //Every comma (,) is a new column in that row
        string[] lines = creditsFile.text.Split(new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
        creditLines = new string[lines.Length][];
        for(int i = 0; i < lines.Length; i++)
            creditLines[i] = lines[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        StartCoroutine(ImageCO());
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
            StartCoroutine(PlayCreditsDelayed());
    }

    private IEnumerator PlayCreditsDelayed()
    {
        yield return delayWFS;

        play = true;
    }

    public void Update()
    {
        if (!play)
            return;
        
        if (Input.anyKeyDown)
        {
            play = false;
            (SceneManagerEx.CurrentScene as TitleScene).EnableUITitleScene();
            Destroy(transform.parent.gameObject);
        }

        y += Time.deltaTime*scrollSpeed;

        while (y >= yDistance)
        {
            //Switch the alignment after the first credit has left the screen
            if (linesDisplayed > maxLinesOnScreen+2 && text.alignment != TextAnchor.UpperCenter)
                text.alignment = TextAnchor.UpperCenter;

            LinesToText();

            y -= yDistance;

            linesDisplayed++;

            if (linesDisplayed > creditLines.Length)
                play = false;
        }

        textTrans.anchoredPosition = startingPos + new Vector2(0, y);
    }

    public void LinesToText()
    {
        sb.Length = 0;

        //The index will be at the first line, until it's off screen
        int rowIndex = Mathf.Max(0, linesDisplayed - maxLinesOnScreen);
        //Allows fill-in, full screen and fill-out
        int rowCount = Mathf.Min(linesDisplayed, maxLinesOnScreen, creditLines.Length - linesDisplayed);

        for(int i = 0; i < rowCount; i++)
        {
            //Build row
            for(int j = 0; j < creditLines[rowIndex].Length; j++)
            {
                //Only separator inbetween columns
                if (j > 0)
                    sb.Append(COLUMN_SEP);

                sb.Append(creditLines[rowIndex][j]);
                if (creditLines[rowIndex][j] == "Thanks for playing!")
                {
                    StartCoroutine(DestroyCanvas());
                }
            }
            rowIndex++;
            //Only separator inbetween rows
            if(i < rowCount-1)
                sb.Append(ROW_SEP);
        }

        text.text = sb.ToString();
        
    }

    private IEnumerator ImageCO()
    {
        const float waitTime      = 5.35f;
        const float appearTime    = 0.7f; // Time for appear animation
        const float shakeTime     = 0.3f; // Time for shake animation
        const float disappearTime = 0.8f; // Time for disappear animation

        // Adjust the shake rotation magnitude
        const float shakeRotationMagnitude = 10f;

        // Define easing type
        //Ease easeType = Ease.OutElastic;

        yield return new WaitForSeconds(6f);

        AnimateImage(_dreadKnightImage, appearTime, shakeTime, shakeRotationMagnitude);
        _dreadKnightImage.GetComponent<Animator>().Play("DreadKnightCredit");
        yield return new WaitForSeconds(waitTime);
        AnimateDisappear(_dreadKnightImage, disappearTime);

        AnimateImage(_lichImage, appearTime, shakeTime, shakeRotationMagnitude);
        _lichImage.GetComponent<Animator>().Play("LichCredit");
        yield return new WaitForSeconds(waitTime);
        AnimateDisappear(_lichImage, disappearTime);

        AnimateImage(_zombieImage, appearTime, shakeTime, shakeRotationMagnitude);
        _zombieImage.GetComponent<Animator>().Play("ZombieCredit"); 
        yield return new WaitForSeconds(waitTime);
        AnimateDisappear(_zombieImage, disappearTime);

        AnimateImage(_kingManImage, appearTime, shakeTime, shakeRotationMagnitude);
        _kingManImage.GetComponent<Animator>().Play("KingManJump");
        yield return new WaitForSeconds(waitTime);
        AnimateDisappear(_kingManImage, disappearTime);

        AnimateImage(_priestImage, appearTime, shakeTime, shakeRotationMagnitude);
        _priestImage.GetComponent<Animator>().Play("PriestCredit");
        yield return new WaitForSeconds(waitTime);
        AnimateDisappear(_priestImage, disappearTime);

        yield return new WaitForSeconds(1f);

        AnimateImage(_dreadKnightImage, appearTime, shakeTime, shakeRotationMagnitude);
        _dreadKnightImage.GetComponent<Animator>().Play("DreadKnightCredit");
        yield return new WaitForSeconds(.2f);
        AnimateImage(_lichImage, appearTime, shakeTime, shakeRotationMagnitude);
        _lichImage.GetComponent<Animator>().Play("LichCredit");
        yield return new WaitForSeconds(.2f);
        AnimateImage(_zombieImage, appearTime, shakeTime, shakeRotationMagnitude);
        _zombieImage.GetComponent<Animator>().Play("ZombieCredit");
        yield return new WaitForSeconds(.2f);
        AnimateImage(_kingManImage, appearTime, shakeTime, shakeRotationMagnitude);
        _kingManImage.GetComponent<Animator>().Play("KingManJump");
        yield return new WaitForSeconds(.2f);
        AnimateImage(_priestImage, appearTime, shakeTime, shakeRotationMagnitude);
        _priestImage.GetComponent<Animator>().Play("PriestCredit");

    }

    private void AnimateImage(GameObject image, float appearTime_, float shakeTime, float shakeRotationMagnitude)
    {
        image.SetActive(false);
        image.transform.localScale = Vector3.zero;
        image.SetActive(true);

        image.transform.DOScale(6f, appearTime_).SetEase(Ease.OutElastic);
        image.transform.DOShakeRotation(shakeTime, new Vector3(0f, 0f, shakeRotationMagnitude));
    }

    private void AnimateDisappear(GameObject image, float disappearTime)
    {
        image.transform.DOScale(8f, 0.5f).SetEase(Ease.OutCubic);
        image.SetActive(false);
    }
    
    private IEnumerator DestroyCanvas()
    {
        yield return new WaitForSeconds(7f);
        (SceneManagerEx.CurrentScene as TitleScene).EnableUITitleScene();
        Destroy(transform.parent.gameObject);
    }
}

