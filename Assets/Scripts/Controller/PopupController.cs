using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController :MonoBehaviour {

    [SerializeField]
    private GameObject content;
    public virtual void Show()
    {
        content.SetActive(true);
    }

    public virtual void Hide()
    {
        content.SetActive(false);
    }
}
