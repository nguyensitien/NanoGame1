using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopupWinController : PopupController {

    public void OnClickContinue()
    {
        GameplayController.Instance.ContinueGame();
        Hide();
    }
}
