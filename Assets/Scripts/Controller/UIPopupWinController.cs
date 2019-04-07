using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopupWinController : PopupController {
    [SerializeField]
    private GameObject[] starList = null;
    public void OnClickContinue()
    {
        GameplayController.Instance.ContinueGame();
        Hide();
    }
    

    public void Init(int numStar)
    {
        StartCoroutine(RunEffectStar(numStar));
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
