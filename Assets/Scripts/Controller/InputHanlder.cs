using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHanlder : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    void Update()
    {
        if (GameplayController.Instance.canCreateLine == false) return;
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 posMouse = Input.mousePosition;
            posMouse = cam.ScreenToWorldPoint(posMouse);
            //posMouse.x = 300;
            //posMouse.y = 300;
            posMouse.z = 0;
            Rect rect = GameplayController.Instance.GetRectBoard();
            rect.width = Utilities.ConvertToLocal(rect.width);
            rect.height = Utilities.ConvertToLocal(rect.height);
            if (Utilities.CheckOverrideRectPoint(rect, posMouse))
            {
                GameplayController.Instance.CreateLine(posMouse);
            }
        }
    }
}
