using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public void EndGame(bool isWin)
    {
        if (isWin) popupWin.Show();
        else popupLose.Show();
    }

    public void UpdatePoint()
    {
        txtPoint.text = GameplayController.Instance.pointCur + "/" + GameplayController.Instance.pointTarget;
    }
    public void OnClickPlay()
    {
        btnPlay.gameObject.SetActive(false);
        GameplayController.Instance.PlayGame();
    }

    public void ShowTabToPlay()
    {
        btnPlay.gameObject.SetActive(true);
    }
}
