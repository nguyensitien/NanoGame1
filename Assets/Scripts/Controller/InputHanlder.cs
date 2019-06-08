using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHanlder : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    private Vector2 originPos;
    private Vector2 endPos;

    void Update()
    {
        if (GameplayController.Instance.canCreateLine == false) return;
        if (Input.GetMouseButtonDown(0)) {
            originPos = Input.mousePosition;
            originPos = cam.ScreenToWorldPoint(originPos);
        }
        if (Input.GetMouseButtonUp(0))
        {
            endPos = Input.mousePosition;
            endPos = cam.ScreenToWorldPoint(endPos);
            Vector2 posMouse = originPos;


            TypeLineFind typeSwipe = TypeLineFind.vertical;
            if (Mathf.Abs(endPos.x - originPos.x) >= Mathf.Abs(endPos.y - originPos.y))
            {
                typeSwipe = TypeLineFind.horizontal;
            }
            
            for (int i = 0; i < GameplayController.Instance.pointsList.Count; i++)
            {
                Vector2[] points = GameplayController.Instance.GetPointsBoard(i);
                
                if (points.Length > 0 && Utilities.IsPointInPolygon(posMouse, points))
                {
                    RaycastHit2D hit = Physics2D.BoxCast(posMouse, Vector2.one * GameplayController.Instance.sizeLine.x*2, 0, Vector2.zero, 0);
                    if (hit.collider == null)
                    {
                        posMouse.x = Mathf.Ceil(posMouse.x);
                        posMouse.y = Mathf.Ceil(posMouse.y);
                        posMouse = GameplayController.Instance.RoundPosCreateLine(i,(Vector2)posMouse);
                        GameplayController.Instance.indexMask = i;
                        GameplayController.Instance.CreateLine(typeSwipe,posMouse);
                    }
                }
            }
            
        }
    }
}
