using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class InputHandleClick : SingletonDontDestroy<InputHandleClick> {
    [SerializeField]
    private GameObject par;
    [SerializeField]
    private Camera cam;
    // Use this for initialization
	void Start () {
        Observable.FromMicroCoroutine(UpdateClick).Subscribe().AddTo(this);
    }
	
	IEnumerator UpdateClick() {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 posMouse = Input.mousePosition;
                posMouse = cam.ScreenToWorldPoint(posMouse);
                GameObject obj = SmartPool.Instance.Spawn(par,Vector3.zero,Quaternion.identity);
                obj.transform.localPosition = posMouse;
            }
            yield return null;
        }
	}
}
