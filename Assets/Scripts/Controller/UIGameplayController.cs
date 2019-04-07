using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGameplayController : Singleton<UIGameplayController> {
    [SerializeField]
    private GameObject btnPlay = null;
    [SerializeField]
    private Text txtPoint = null;
    [SerializeField]
    private UIPopupWinController popupWin;
    [SerializeField]
    private UIPopupLoseController popupLose;
    [SerializeField]
    private RectTransform boxTop = null;

    public void EndGame(bool isWin)
    {
        boxTop.DOAnchorPosY(300, 1).SetEase(Ease.InOutBack).OnComplete(()=> {
            if (isWin)
            {

                popupWin.Show();
                popupWin.Init(DataController.Instance.UserDataNodeList[ObjectDataController.Instance.idNodeFighting - 1].numStar);
            }
            else
            {
                popupLose.Show();
            }
        });
        
    }

    public void UpdatePoint()
    {
        float percent = (float)GameplayController.Instance.pointCur / (float)GameplayController.Instance.pointTarget;
        //Debug.Log("percent:"+percent);
        txtPoint.text = Mathf.Min(100,(int)(percent*100))+"%";
    }
    public void OnClickPlay()
    {
        btnPlay.gameObject.SetActive(false);
        GameplayController.Instance.PlayGame();
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void ShowTabToPlay()
    {
        btnPlay.gameObject.SetActive(true);
    }
}
