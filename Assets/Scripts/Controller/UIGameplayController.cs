using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGameplayController : Singleton<UIGameplayController> {
    [SerializeField]
    private Image imgPoint = null;
    [SerializeField]
    private Text txtPoint = null,txtLevel = null;
    [SerializeField]
    private UIPopupWinController popupWin;
    [SerializeField]
    private UIPopupLoseController popupLose;
    [SerializeField]
    private RectTransform boxTop = null,rectTxtTapToPlay = null,rectTxtNameGame = null,bgClick  = null;
    [SerializeField]
    private CanvasGroup groupTxtTapToPlay = null;
    [SerializeField]
    private DOTweenAnimation tweenBtnSetting = null,tweenBtnShop = null,tweenBtnRemoveAd = null,tweenTxtTapToPlay,tweenBoxTop = null;
    private Vector2 posOriginTxtName;
    private void Awake()
    {
        posOriginTxtName = rectTxtNameGame.anchoredPosition;
    }
    public void Start()
    {
        Init();
    }

    public void UpdateTxtLevel() {
        txtLevel.text = "Level " + ObjectDataController.Instance.IdNodeFighting;
    }
    

    public void Init()
    {
        UpdateTxtLevel();
        tweenBoxTop.DOPlayBackwards();
        tweenBtnSetting.DORestart();
        tweenBtnShop.DORestart();
        tweenBtnRemoveAd.DORestart();
        rectTxtTapToPlay.localScale = Vector2.one;
        rectTxtTapToPlay.gameObject.SetActive(true);
        groupTxtTapToPlay.DOPlayBackwards();
        groupTxtTapToPlay.alpha = 1;
        rectTxtNameGame.anchoredPosition = posOriginTxtName;
        bgClick.gameObject.SetActive(true);
    }
    public void EndGame(bool isWin)
    {
        if (isWin)
        {

            popupWin.Show();
            ObjectDataController.Instance.IdNodeFighting++;
        }
        else
        {
            popupLose.Show();
        }

    }

    public void UpdatePoint()
    {
        float percent = (float)GameplayController.Instance.pointCur / (float)GameplayController.Instance.pointTarget;
        imgPoint.DOFillAmount(percent,0.6f).SetEase(Ease.OutBack);
        txtPoint.text = Mathf.Min(100,(int)(percent*100))+"%";
    }
    public void OnClickPlay()
    {
        bgClick.gameObject.SetActive(false);
        tweenBoxTop.DORestart();
        tweenBtnSetting.DOPlayBackwards();
        tweenBtnShop.DOPlayBackwards();
        tweenBtnRemoveAd.DOPlayBackwards();
        tweenTxtTapToPlay.DOPause();
        groupTxtTapToPlay.DOFade(0, 0.6f).SetEase(Ease.OutCirc);
        rectTxtTapToPlay.DOScale(Vector2.one*5,0.6f).SetEase(Ease.OutCirc).OnComplete(()=> {
            rectTxtTapToPlay.gameObject.SetActive(false);
            rectTxtTapToPlay.localScale = Vector2.one;
            groupTxtTapToPlay.alpha = 1;
        });
        rectTxtNameGame.DOAnchorPosY(posOriginTxtName.y + 800, 1).SetEase(Ease.OutCirc);
        StartCoroutine(DelayPlayGame());
        
    }

    public void RestartGame()
    {
        tweenBoxTop.DORestart();
    }

    private IEnumerator DelayPlayGame()
    {
        GameplayController.Instance.ShowAlphaAll();
        //yield return new WaitForSeconds(1);
        yield return null;
        GameplayController.Instance.PlayGame();
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void ShowTabToPlay()
    {
        
    }
}
