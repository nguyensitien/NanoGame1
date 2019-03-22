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
    private void Start()
    {
        circleHit = GetComponent<CircleCollider2D>();
    }
    public void Init()
    {
        isRolling = true;
        float angle = Random.Range(0,360);
        dirMove.x = Mathf.Cos(Mathf.Deg2Rad*angle);
        dirMove.y = Mathf.Sin(Mathf.Deg2Rad*angle);
        posCur = transform.position;
    }
    public LayerMask layerMask;

    private float dist;
    private void Update()
    {
        if (GameplayController.Instance.isEndGame) return;
        if (isRolling)
        {
            posCur.x += dirMove.x * speed * Time.deltaTime;
            posCur.y += dirMove.y * speed * Time.deltaTime;


            dist = (posCur - (Vector2)transform.position).magnitude;
            RaycastHit2D[] hits = Physics2D.CircleCastAll(posCur, circleHit.radius, dirMove, dist, layerMask);
            if (hits.Length > 0)
            {
                Vector3 reflectVec = Vector3.Reflect(dirMove, hits[0].normal);
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
