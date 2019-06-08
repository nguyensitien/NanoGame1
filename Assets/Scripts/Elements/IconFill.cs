using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class IconFill : MonoBehaviour {
    [SerializeField]
    private float padding;
    [SerializeField]
    private float angleOrigin;
    private Image imgFill;
    private RectTransform thisRect;
    private RectTransform parentRect;
    // Use this for initialization
    private float radius;
    private float ratioCur;
	void Awake () {
        thisRect = GetComponent<RectTransform>();
        imgFill = transform.parent.GetComponent<Image>();
        parentRect = transform.parent.GetComponent<RectTransform>();
        radius = parentRect.sizeDelta.x / 2;

    }
	
	// Update is called once per frame
	void Update () {
        if (ratioCur != imgFill.fillAmount)
        {
            float angleCur = (1-imgFill.fillAmount) * 360;
            angleCur += angleOrigin;
            thisRect.anchoredPosition = new Vector2((radius+padding) * Mathf.Cos(angleCur*Mathf.Deg2Rad), (radius+padding) * Mathf.Sin(angleCur * Mathf.Deg2Rad));
            ratioCur = imgFill.fillAmount;
        }
	}
}
