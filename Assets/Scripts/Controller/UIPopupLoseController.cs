using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopupLoseController : PopupController {

    public void OnClickTry()
    {
        GameplayController.Instance.TryAgain();
        Hide();
    }
}
