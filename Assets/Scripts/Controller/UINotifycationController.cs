using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINotifycationController : BasePopup {

    [SerializeField]
    private Text txtNotify = null;

    public override void Show(params object[] data)
    {
        base.Show(data);
        txtNotify.text = data[0].ToString();
    }
}
