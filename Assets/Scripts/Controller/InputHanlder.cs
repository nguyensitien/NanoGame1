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
                RaycastHit2D hit = Physics2D.BoxCast(posMouse,Vector2.one*GameplayController.Instance.sizeLine.x,0,Vector2.zero,0);
                if (hit.collider == null)
                {
                    posMouse.x = Mathf.Ceil(posMouse.x);
                    posMouse.y = Mathf.Ceil(posMouse.y);
                    posMouse = GameplayController.Instance.RoundPosCreateLine((Vector2)posMouse);
                    GameplayController.Instance.CreateLine(posMouse);
                }
            }
            else
            {
                //Debug.Log("khong vao:"+ posMouse);
            }
        }
    }
}
