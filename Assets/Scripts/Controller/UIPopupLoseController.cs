using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupLoseController : PopupController {
    [SerializeField]
    private Image imgFillTime = null;
    [SerializeField]
    private RectTransform rectTxtNodeThanks = null;
    [SerializeField]
    private Text txtNoThanks = null,txtMove = null,txtAttempt = null;
    [SerializeField]
    private CanvasGroup groupBtnContinue = null,groupMove = null,groupAttempt = null;
    [SerializeField]
    private DOTweenAnimation tweenGroupImgFill = null;
    [SerializeField]
    private ParticleSystem particleFill = null;
    public override void Show()
    {
        DataController.Instance.UserData.attemptNodeCur += 1;
        base.Show();
        isTimeOut = false;
        DataBaseNode dataNode = ObjectDataController.Instance.DataBaseMap.nodeList[ObjectDataController.Instance.IdNodeFighting-1];
        txtMove.text = dataNode.numSketch.ToString();
        txtAttempt.text = DataController.Instance.UserData.attemptNodeCur.ToString();
        groupMove.alpha = groupAttempt.alpha = 0;
        txtNoThanks.text = "NO THANKS";
        rectTxtNodeThanks.gameObject.SetActive(false);
        imgFillTime.fillAmount = 1;
        imgFillTime.DOFillAmount(0, ObjectDataController.Instance.ConfigGame.timeOutFail).SetEase(Ease.Linear).OnComplete(() =>
        {
            ChangeToRestart();
        }).SetId(this);
        corouDDelayShowNoThanks = StartCoroutine(DelayShowNoThanks());
        
    }
    private Coroutine corouDDelayShowNoThanks;
    private IEnumerator DelayShowNoThanks()
    {
        yield return new WaitForSeconds(ObjectDataController.Instance.ConfigGame.timeDelayShowNoThanks);
        rectTxtNodeThanks.gameObject.SetActive(true);
    }

    public void OnClickNoThaks()
    {
        if (isTimeOut == false)
        {
            ChangeToRestart();
        }
        else
        {
            GameplayController.Instance.RestartGame();
            Hide();
        }
    }

    private bool isTimeOut;
    public void ChangeToRestart()
    {
        particleFill.Stop();
        isTimeOut = true;
        groupBtnContinue.DOFade(0, 0.4f).SetEase(Ease.Linear);
        groupMove.DOFade(1, 0.4f).SetEase(Ease.Linear);
        groupAttempt.DOFade(1, 0.4f).SetEase(Ease.Linear);
        tweenGroupImgFill.DOPlayBackwards();
        txtNoThanks.text = "TAP TO RESTART";
    }
    
    public void OnClickTry()
    {
        GameplayController.Instance.TryAgain();
        Hide();
    }

    public void OnClickContinue()
    {
        GameplayController.Instance.ContinueGame();
        Hide();
    }

    public override void Hide()
    {
        base.Hide();
        DOTween.Kill(this);
        if (corouDDelayShowNoThanks != null)
        {
            StopCoroutine(corouDDelayShowNoThanks);
        }
    }
}
