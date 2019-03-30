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
            posMouse.z = 0;
            if (Utilities.IsPointInPolygon(posMouse, GameplayController.Instance.GetPointsBoard()))
            {
                posMouse.x = Mathf.Ceil(posMouse.x);
                posMouse.y = Mathf.Ceil(posMouse.y);

                GameplayController.Instance.CreateLine(posMouse);
            }
            else
            {
                //Debug.Log("khong vao:"+ posMouse);
            }
        }
    }
}
