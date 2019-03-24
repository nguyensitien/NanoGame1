using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBall : MonoBehaviour
{
    
    [SerializeField]
    private float speed;
    private bool isRolling;
    private Vector2 dirMove = Vector2.zero;
    private Vector2 posCur;
    private CircleCollider2D circleHit;
    private float size;
    private void Start()
    {
        circleHit = GetComponent<CircleCollider2D>();
        size = transform.localScale.x;
    }
    public void Init()
    {
        isRolling = true;
        float angle = Random.Range(0,360);
        //angle = 0;
        dirMove.x = Mathf.Cos(Mathf.Deg2Rad*angle);
        dirMove.y = Mathf.Sin(Mathf.Deg2Rad*angle);
        posCur = transform.position;
    }
    public LayerMask layerMask;

    private float dist;

    
    private Vector2 posOld;
    private float deltaTime;
    private void Update()
    {
        if (GameplayController.Instance.isEndGame) return;
        if (isRolling)
        {
            deltaTime = Time.deltaTime;
            posOld = posCur = transform.localPosition;
            posCur += dirMove * speed * deltaTime;

            dist = (posCur - posOld).magnitude;
            RaycastHit2D hit = Physics2D.CircleCast(posOld, circleHit.radius * size, dirMove, dist, layerMask);
            if (hit.collider != null)
            {
                Vector3 reflectVec = Vector3.Reflect(dirMove, hit.normal);
                posCur = hit.point;

                if (dirMove.x * reflectVec.x < 0)
                {
                    //doi huong x
                    if (dirMove.x > 0)
                    {
                        posCur.x -= circleHit.radius * size;
                    }
                    else
                    {
                        posCur.x += circleHit.radius * size;
                    }
                }
                if (dirMove.y * reflectVec.y < 0)
                {
                    //doi huong y
                    if (dirMove.y > 0)
                    {
                        posCur.y -= circleHit.radius * size;
                    }
                    else
                    {
                        posCur.y += circleHit.radius * size;
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
            if (itemLine != null && itemLine.isCompleteSketching == false)
            {
                GameplayController.Instance.EndGame(false);
            }
        }
    }
}
