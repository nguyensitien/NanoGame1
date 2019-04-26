using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupWinController : PopupController {
    [SerializeField]
    private GameObject[] starList = null;
    [SerializeField]
    private Text txtMove = null, txtAttempt = null;

    public void OnClickContinue()
    {
        GameplayController.Instance.ContinueGame();
        Hide();
    }
    public override void Show()
    {
        base.Show();
        DataBaseNode dataNode = ObjectDataController.Instance.DataBaseMap.nodeList[ObjectDataController.Instance.IdNodeFighting - 1];
        txtMove.text = dataNode.numSketch.ToString();
        txtAttempt.text = DataController.Instance.UserData.attemptNodeCur.ToString();
        DataController.Instance.UserData.attemptNodeCur = 0;

    }

    public void OnClickNext()
    {
        GameplayController.Instance.NextLevel();
        Hide();
    }

    private IEnumerator RunEffectStar(int numStar)
    {
        for (int i = 0; i < numStar; i++)
        {
            starList[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
