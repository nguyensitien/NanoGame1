using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBall : MonoBehaviour
{
    [SerializeField]
    private int[] arrAngleRandom;
    [SerializeField]
    private float speed;
    private bool isRolling;
    private Vector2 dirMove = Vector2.zero;
    private Vector2 posCur;
    private CircleCollider2D circleHit;
    private float radius;
    private float size;
    private void Start()
    {
        circleHit = GetComponent<CircleCollider2D>();
        size = transform.localScale.x;
        radius = circleHit.radius;
    }
    public void Init()
    {
        isRolling = true;
        float angle = arrAngleRandom[Random.Range(0,arrAngleRandom.Length)];
        //angle = 0;
        //angle = 215;
        dirMove.x = Mathf.Cos(Mathf.Deg2Rad*angle);
        dirMove.y = Mathf.Sin(Mathf.Deg2Rad*angle);
        posCur = transform.position;
    }
    public LayerMask layerMask;

    private float dist;

    
    private Vector2 posOld;
    private float deltaTime;
    private void FixedUpdate()
    {
        if (GameplayController.Instance.isEndGame) return;
        if (isRolling)
        {
            deltaTime = Time.fixedDeltaTime;
            posOld = posCur = transform.localPosition;
            posCur += dirMove * speed * deltaTime;
            
            dist = (posCur - posOld).magnitude;
            RaycastHit2D hit = Physics2D.CircleCast(posOld, radius * size, dirMove, dist, layerMask);
            if (hit.collider != null)
            {
                Vector3 reflectVec = Vector3.Reflect(dirMove, hit.normal);
                posCur = hit.point;
                //Debug.Log("dirMove:"+dirMove+" reflecVec:"+reflectVec+" posOld:"+posOld+" posCur:"+posCur+" normal:"+hit.normal);
                //Debug.DrawLine(posOld,posCur,Color.red,Time.fixedDeltaTime);
                //Debug.DrawLine(posCur, posCur + (Vector2)hit.normal * 100, Color.yellow, Time.fixedDeltaTime);
                //Debug.DrawLine(posCur, posCur + (Vector2)reflectVec*100,Color.green,Time.fixedDeltaTime);
                if (dirMove.x * reflectVec.x * dirMove.y * reflectVec.y > 0)
                {
                    posCur.x -= (dirMove.x/Mathf.Abs(dirMove.x)) * radius * size;
                    posCur.y -= (dirMove.y/Mathf.Abs(dirMove.y)) * radius * size;
                }
                else
                {
                    if (dirMove.x * reflectVec.x < 0)
                    {
                        //doi huong x
                        if (dirMove.x > 0)
                        {
                            posCur.x -= radius * size;
                        }
                        else
                        {
                            posCur.x += radius * size;
                        }
                    }
                    if (dirMove.y * reflectVec.y < 0)
                    {
                        //doi huong y
                        if (dirMove.y > 0)
                        {
                            posCur.y -= radius * size;
                        }
                        else
                        {
                            posCur.y += radius * size;
                        }
                    }
                }
                dirMove = reflectVec;
            }
            transform.position = posCur;
        }

    }
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isRolling && collision.tag.Equals("ItemLine"))
        {
            ItemLine itemLine = collision.GetComponent<ItemLine>();
            
            if (itemLine != null )
            {
                //Debug.Log("va cham ne ItemLine:"+itemLine.isCompleteSketching);
                if (itemLine.isCompleteSketching == false)
                {
                    //Debug.Log("va cham ne:" +itemLine.isCompleteSketching);
                    GameplayController.Instance.EndGame(false);
                }
                
            }
        }
    }
}
