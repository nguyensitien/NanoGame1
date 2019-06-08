using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBall : MonoBehaviour
{
    [SerializeField]
    private int[] arrAngleRandom;
    
    public float velocity;
    [HideInInspector]
    public float mass;
    private bool isRolling;
    private Vector2 dirMove = Vector2.zero;
    [HideInInspector]
    public Vector2 posCur;
    private CircleCollider2D circleHit;
    private float radius;
    [HideInInspector]
    public SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleHit = GetComponent<CircleCollider2D>();
        radius = circleHit.radius * transform.localScale.x;
        myTran = transform;
    }
    public void Init()
    {
        isRolling = true;
        float angle = arrAngleRandom[Random.Range(0, arrAngleRandom.Length)];
        //angle = 0;
        //angle = 215;
        dirMove.x = Mathf.Cos(Mathf.Deg2Rad * angle);
        dirMove.y = Mathf.Sin(Mathf.Deg2Rad * angle);
        posOld = posCur = transform.position;
    }
    public LayerMask layerMask;

    private float dist;

    private Vector2 pointHit;
    //private void OnDrawGizmos()
    //{
    //    if(name == "ItemBall")
    //    Gizmos.color = Color.red;
    //    else
    //        Gizmos.color = Color.blue;
    //    Gizmos.DrawCube(pointHit, Vector2.one);
    //}

    private Vector2 posOld;
    private float deltaTime;
    private RaycastHit2D[] hits = new RaycastHit2D[6];
    private RaycastHit2D hit;
    [HideInInspector]
    public Transform myTran;
    
    public void UpdateMove()
    {
        //if (GameplayController.Instance.isEndGame) return;
        deltaTime = Time.deltaTime;
        posCur += dirMove * velocity * deltaTime;

        dist = (posCur - posOld).magnitude;
        int result = Physics2D.CircleCastNonAlloc(posOld, radius, dirMove, hits, dist, layerMask);
        //Debug.Log("name:" + name + " dirMove:" + dirMove + " posOld:" + posOld + " posCur:" + posCur + " normal:" + hit.normal + " hit:" + hit.point + " radius:" + radius);

        for (int i = 0; i < result; i++)
        {
            hit = hits[i];
            //Debug.Log("name:" + name + " dirMove:" + dirMove + " posOld:" + posOld + " posCur:" + posCur + " normal:" + hit.normal + " hit:" + hit.point);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject != gameObject)
                {

                    if (hit.collider.tag == "ItemLine")
                    {
                        ItemLine itemLine = hit.collider.GetComponent<ItemLine>();
                       
                        if (itemLine != null)
                        {
                            if (itemLine.isCompleteSketching == false)
                            {
                                GameplayController.Instance.EndGame(false);
                            }
                            else
                            {
                                Vector3 reflectVec = Vector3.Reflect(dirMove, hit.normal);
                                pointHit = hit.point;
                                posCur.x = hit.point.x + hit.normal.x * (radius + radius * deltaTime);
                                posCur.y = hit.point.y + hit.normal.y * (radius + radius * deltaTime);
                                dirMove = reflectVec;
                            }

                        }
                        else
                        {
                            Vector3 reflectVec = Vector3.Reflect(dirMove, hit.normal);
                            pointHit = hit.point;
                            posCur.x = hit.point.x + hit.normal.x * (radius + radius * deltaTime);
                            posCur.y = hit.point.y + hit.normal.y * (radius + radius * deltaTime);
                            dirMove = reflectVec;
                        }
                        break;

                    }

                }

            }

        }
        transform.position = posCur;
        posOld = posCur;
    }
}
