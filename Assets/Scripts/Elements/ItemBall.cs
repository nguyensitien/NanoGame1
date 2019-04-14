using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBall : MonoBehaviour
{
    [SerializeField]
    private int[] arrAngleRandom;
    
    public float velocity;
    public float mass;
    private bool isRolling;
    private Vector2 dirMove = Vector2.zero;
    [HideInInspector]
    public Vector2 posCur;
    private CircleCollider2D circleHit;
    private float radius;
    private void Start()
    {
        circleHit = GetComponent<CircleCollider2D>();
        radius = circleHit.radius * transform.localScale.x; ;
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
    private void OnDrawGizmos()
    {
        if(name == "ItemBall")
        Gizmos.color = Color.red;
        else
            Gizmos.color = Color.blue;
        Gizmos.DrawCube(pointHit, Vector2.one);
    }

    private Vector2 posOld;
    private float deltaTime;
    private RaycastHit2D[] hits = new RaycastHit2D[4];
    private RaycastHit2D hit;
    public void UpdateMove()
    {
        //if (GameplayController.Instance.isEndGame) return;
        if (isRolling)
        {
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

                        if (hit.collider.tag == "ItemBall" )
                        {

                            Vector3 reflectVec = Vector3.Reflect(dirMove, hit.normal);
                            pointHit = new Vector2(hit.transform.position.x + hit.normal.x * (radius), hit.transform.position.y + hit.normal.y * (radius));
                            //Debug.DrawLine(posOld - dirMove * 100, posCur, Color.red, Time.fixedDeltaTime);
                            posCur.x = hit.transform.position.x + hit.normal.x * (radius + radius + radius*deltaTime);
                            posCur.y = hit.transform.position.y + hit.normal.y * (radius + radius + +radius * deltaTime);
                            //Debug.Log("------Collider name:" + name + " dirMove:" + dirMove + " reflectVec:" + reflectVec + " posOld:" + posOld + " posCur:" + posCur + " normal:" + hit.normal + " hit:" + hit.point + " radius:" + radius);
                            ItemBall itemBall = hit.collider.GetComponent<ItemBall>();
                            if (itemBall != null)
                            {
                                Vector3 reflectVecOther = Vector3.Reflect(itemBall.dirMove, hit.normal);
                                float velocityNew = Utilities.GetVelcoityAfterCollide(velocity, itemBall.velocity, mass, itemBall.mass);
                                float velocityNewOther = Utilities.GetVelcoityAfterCollide(itemBall.velocity, velocity, itemBall.mass, mass);
                                velocity = velocityNew;
                                itemBall.velocity = velocityNewOther;
                                itemBall.dirMove = reflectVecOther;
                            }
                            dirMove = reflectVec;


                            Debug.DrawLine(posCur, posCur - (Vector2)dirMove * 100, Color.red, Time.fixedDeltaTime);
                            Debug.DrawLine(posCur, posCur + (Vector2)hit.normal * 100, Color.yellow, Time.fixedDeltaTime);
                            Debug.DrawLine(posCur, posCur + (Vector2)reflectVec * 100, Color.green, Time.fixedDeltaTime);
                        }
                        else if (hit.collider.tag == "ItemLine")
                        {
                            ItemLine itemLine = hit.collider.GetComponent<ItemLine>();
                            //Debug.Log("va cham ne:" + itemLine);
                            //if (itemLine != null)
                            //{
                            //    Debug.Log("cai gi nua day:"+itemLine.isCompleteSketching);
                            //}
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
                                    //Debug.DrawLine(posOld - dirMove * 100, posCur, Color.red, Time.fixedDeltaTime);
                                    posCur.x = hit.point.x + hit.normal.x * (radius + radius * deltaTime);
                                    posCur.y = hit.point.y + hit.normal.y * (radius + radius * deltaTime);
                                    //Debug.Log("------Collider name:" + name + " dirMove:" + dirMove + " reflectVec:" + reflectVec + " posOld:" + posOld + " posCur:" + posCur + " normal:" + hit.normal + " hit:" + hit.point + " radius:" + radius);

                                    dirMove = reflectVec;
                                }

                            }
                            else
                            {
                                Vector3 reflectVec = Vector3.Reflect(dirMove, hit.normal);
                                pointHit = hit.point;
                                //Debug.DrawLine(posOld - dirMove * 100, posCur, Color.red, Time.fixedDeltaTime);
                                posCur.x = hit.point.x + hit.normal.x * (radius + radius * deltaTime);
                                posCur.y = hit.point.y + hit.normal.y * (radius + radius * deltaTime);
                                //Debug.Log("------Collider name:" + name + " dirMove:" + dirMove + " reflectVec:" + reflectVec + " posOld:" + posOld + " posCur:" + posCur + " normal:" + hit.normal + " hit:" + hit.point + " radius:" + radius);

                                dirMove = reflectVec;
                            }
                            
                        }
                       
                        break;
                    }
                   
                }

            }
            transform.position = posCur;
            posOld = posCur;
        }

    }


    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Debug.Log("OnTriggerEnter2D");
    //    if (isRolling && collision.tag.Equals("ItemLine"))
    //    {
    //        Debug.Log("va cham cai coi");
    //        ItemLine itemLine = collision.GetComponent<ItemLine>();

    //        if (itemLine != null)
    //        {
    //            //Debug.Log("va cham ne ItemLine:"+itemLine.isCompleteSketching);
    //            if (itemLine.isCompleteSketching == false)
    //            {
    //                //Debug.Log("va cham ne:" +itemLine.isCompleteSketching);
    //                GameplayController.Instance.EndGame(false);
    //            }

    //        }
    //    }
    //}
}
