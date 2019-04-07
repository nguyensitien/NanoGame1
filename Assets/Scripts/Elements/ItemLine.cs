using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLine : MonoBehaviour
{
    public TypeLineFind typeLineFind;
    [SerializeField]
    private Sprite[] spritesLine = null;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
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
    public Vector2 posTarget,pointHit;
    [SerializeField]
    private LayerMask layerMask;
    private void Start()
    {
        GameplayController.Instance.actionRemoveItemLine += OnRemoveItemLine;
    }
   
    private void OnRemoveItemLine(TypeLine typeLine, Vector2 point)
    {
        //Debug.Log("OnRemoveItemLine:"+this.typeLine+"=="+typeLine+" && "+transform.localPosition +"=="+ point);
        if (this.typeLine == typeLine && (Vector2)transform.localPosition == point && gameObject.activeInHierarchy)
        {
            //Debug.Log("Remove:" + typeLine + ":" + point);
            SmartPool.Instance.Despawn(gameObject);
        }
    }

    public void Init(TypeLineFind typeLine,int index,bool isSketching)
    {
        this.typeLineFind = typeLine;
        this.dir = index ==0 ?-1:1;
        isCompleteSketching = isSketching;
        RaycastHit2D hit;
        if (typeLine == TypeLineFind.horizontal)
        {
            scaleCur = new Vector2(0, 1);
            hit = Physics2D.Linecast(transform.position, (Vector2)transform.position + Vector2.right * dir * 20000, layerMask);
            posTarget = hit.point;
            posTarget.x += dir * (GameplayController.Instance.sizeLine.x/2.0f);
            pointHit = posTarget;
            posTarget.x -= dir * Utilities.SIZE/2;
        }
        else
        {
            scaleCur = new Vector2(1,0);
            hit = Physics2D.Linecast(transform.position, (Vector2)transform.position + Vector2.up * dir * 20000, layerMask);
            posTarget = hit.point;
            posTarget.y += dir * (GameplayController.Instance.sizeLine.x / 2.0f);
            pointHit = posTarget;
            posTarget.y -= dir * Utilities.SIZE/2;
        }
        //Debug.Log("hit:"+transform.position+"=>"+posTarget);
    }
    public TypeLine typeLine;

    public void InitTypeLine(TypeLine typeLine)
    {
        this.typeLine = typeLine;
    }

    public void UnTrigger()
    {
        boxHit.enabled = false;
    }


    private void Update()
    {

        if (isCompleteSketching == false && GameplayController.Instance.isEndGame == false)
        {
            if (typeLineFind.Equals(TypeLineFind.vertical))
            {
                scaleCur.y += GameConfig.SPEED_SCALE * Time.deltaTime * dir;
                if (Mathf.Abs(scaleCur.y) >= Mathf.Abs((posTarget.y - transform.position.y) / 100.0f))
                {
                    scaleCur.y = (posTarget.y - transform.position.y) / 100.0f;
                    CompleteSketching(pointHit);
                }

            }
            else if (typeLineFind.Equals(TypeLineFind.horizontal))
            {
                scaleCur.x += GameConfig.SPEED_SCALE * Time.deltaTime * dir;
                if (Mathf.Abs(scaleCur.x) >= Mathf.Abs((posTarget.x - transform.position.x) / 100.0f))
                {
                    scaleCur.x = (posTarget.x - transform.position.x) / 100.0f;
                    CompleteSketching(pointHit);
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
    public Vector2 size;
    public ItemLineInfo(Vector2 point,TypeLine typeLine,Vector2 size)
    {
        this.point = point;
        this.typeLine = typeLine;
        this.size = size;
    }

}