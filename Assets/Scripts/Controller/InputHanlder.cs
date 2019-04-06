using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHanlder : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private LayerMask layerMask;

    

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
                posMouse.x = (int)(posMouse.x);
                posMouse.y = (int)(posMouse.y);
                float sizeLine = 96;
                if (GameplayController.Instance.typeLineCur == TypeLineFind.horizontal)
                {
                    RaycastHit2D[] hits = Physics2D.BoxCastAll(posMouse, new Vector2(3000, sizeLine),0,Vector2.zero, layerMask);
                    for (int i = 0; i < hits.Length; i++)
                    {
                        RaycastHit2D hit = hits[i];
                        if (hit.collider.tag == "ItemLine")
                        {
                            ItemLine itemLine = hit.collider.GetComponent<ItemLine>();
                            if (itemLine.typeLine == TypeLine.line_bot_left || itemLine.typeLine == TypeLine.line_bot_right ||
                                itemLine.typeLine == TypeLine.line_top_left || itemLine.typeLine == TypeLine.line_top_right)
                            {
                                if (Mathf.Abs(posMouse.y - hit.collider.transform.position.y) <= sizeLine / 2)
                                {
                                    RaycastHit2D hitTmp = Physics2D.BoxCast(posMouse, new Vector2(sizeLine / 2, sizeLine / 2), 0, Vector2.zero, layerMask);
                                    Debug.Log("hit.colll:" + hitTmp.collider);
                                    if (hitTmp.collider == null)
                                    {
                                        posMouse.y = hit.collider.transform.position.y;
                                    }
                                    else
                                    {
                                        if (posMouse.y > hit.collider.transform.position.y)
                                        {
                                            posMouse.y = hit.collider.transform.position.y + sizeLine;
                                        }
                                        else
                                        {
                                            posMouse.y = hit.collider.transform.position.y - sizeLine;
                                        }
                                    }
                                }

                                else
                                {
                                    if (posMouse.y > hit.collider.transform.position.y)
                                    {
                                        posMouse.y = hit.collider.transform.position.y + sizeLine;
                                    }
                                    else
                                    {
                                        posMouse.y = hit.collider.transform.position.y - sizeLine;
                                    }
                                }
                                break;
                            }
                            else
                            {
                                Debug.Log("click vao cai gi vay ne");
                            }
                        }
                    }
                }
                else
                {
                    RaycastHit2D[] hits = Physics2D.BoxCastAll(posMouse, new Vector2(sizeLine, 3000), 0, Vector2.zero, layerMask);
                    for (int i = 0; i < hits.Length; i++)
                    {
                        RaycastHit2D hit = hits[i];
                        if (hit.collider.tag == "ItemLine")
                        {
                            ItemLine itemLine = hit.collider.GetComponent<ItemLine>();
                            if (itemLine.typeLine == TypeLine.line_bot_left || itemLine.typeLine == TypeLine.line_bot_right ||
                                itemLine.typeLine == TypeLine.line_top_left || itemLine.typeLine == TypeLine.line_top_right)
                            {
                                if (Mathf.Abs(posMouse.x - hit.collider.transform.position.x) <= sizeLine / 2)
                                {
                                    RaycastHit2D hitTmp = Physics2D.BoxCast(posMouse, new Vector2(sizeLine / 2, sizeLine / 2), 0, Vector2.zero, layerMask);
                                    Debug.Log("hit.colll:" + hitTmp.collider);
                                    if (hitTmp.collider == null)
                                    {
                                        posMouse.x = hit.collider.transform.position.x;
                                    }
                                    else
                                    {
                                        if (posMouse.x > hit.collider.transform.position.x)
                                        {
                                            posMouse.x = hit.collider.transform.position.x + sizeLine;
                                        }
                                        else
                                        {
                                            posMouse.x = hit.collider.transform.position.x - sizeLine;
                                        }
                                    }
                                }
                                else
                                {
                                    if (posMouse.x > hit.collider.transform.position.x)
                                    {
                                        posMouse.x = hit.collider.transform.position.x + sizeLine;
                                    }
                                    else
                                    {
                                        posMouse.x = hit.collider.transform.position.x - sizeLine;
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
                GameplayController.Instance.CreateLine(posMouse);
            }
            else
            {
                //Debug.Log("khong vao:"+ posMouse);
            }
        }
    }
}
