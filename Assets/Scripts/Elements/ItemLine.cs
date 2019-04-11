using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLine : MonoBehaviour
{
    public TypeLineFind typeLine;
    [HideInInspector]
    public bool isCompleteSketching = true;
    private Vector2 scaleCur;
    private BoxCollider2D boxHit;
    [HideInInspector]
    public float dir;
    private void Awake()
    {
        boxHit = GetComponent<BoxCollider2D>();
    }
    [HideInInspector]
    public Vector2 posTarget;
    [SerializeField]
    private LayerMask layerMask;
    public void Init(TypeLineFind typeLine,int index,bool isSketching)
    {
        this.typeLine = typeLine;
        this.dir = index ==0 ?-1:1;
        isCompleteSketching = isSketching;
        RaycastHit2D hit;
        if (typeLine == TypeLineFind.horizontal)
        {
            scaleCur = new Vector2(0, 1);
            hit = Physics2D.Linecast(transform.position, (Vector2)transform.position + Vector2.right * dir * 20000, layerMask);
            posTarget = hit.point;
            posTarget.x += dir * GameplayController.Instance.sizeLine.x/2;
        }
        else
        {
            scaleCur = new Vector2(1,0);
            hit = Physics2D.Linecast(transform.position, (Vector2)transform.position + Vector2.up * dir * 20000, layerMask);
            posTarget = hit.point;
            posTarget.y += dir * GameplayController.Instance.sizeLine.x/2;
        }
        //Debug.Log("hit:"+transform.position+"=>"+posTarget);
    }

    public void UnTrigger()
    {
        boxHit.enabled = false;
    }


    private void Update()
    {

        if (isCompleteSketching == false && GameplayController.Instance.isEndGame == false)
        {
            if (typeLine.Equals(TypeLineFind.vertical))
            {
                scaleCur.y += GameConfig.SPEED_SCALE * Time.deltaTime * dir;
                if (Mathf.Abs(scaleCur.y) >= Mathf.Abs((posTarget.y - transform.position.y) / 100.0f))
                {
                    scaleCur.y = (posTarget.y - transform.position.y) / 100.0f;
                    CompleteSketching(posTarget);
                }

            }
            else if (typeLine.Equals(TypeLineFind.horizontal))
            {
                scaleCur.x += GameConfig.SPEED_SCALE * Time.deltaTime * dir;
                if (Mathf.Abs(scaleCur.x) >= Mathf.Abs((posTarget.x - transform.position.x) / 100.0f))
                {
                    scaleCur.x = (posTarget.x - transform.position.x) / 100.0f;
                    CompleteSketching(posTarget);
                }
            }
            transform.localScale = scaleCur;
        }
    }

    private void CompleteSketching(Vector2 pos)
    {
        isCompleteSketching = true;
        GameplayController.Instance.CompleteSketching(pos,this);
    }

}

public class ItemLineInfo
{
    public Vector2 point;
    public TypeLine typeLine;
    public TypeLineFind typeLineFind;
    public ItemLineInfo(Vector2 point,TypeLine typeLine)
    {
        this.point = point;
        this.point.x = Mathf.Ceil(point.x);
        this.point.y = Mathf.Ceil(point.y);
        this.typeLine = typeLine;
    }
}