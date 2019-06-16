using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLine : MonoBehaviour
{
    public TypeLineFind typeLine;
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
        isRun = true;
        this.typeLine = typeLine;
        this.dir = index ==0 ?-1:1;
        isCompleteSketching = isSketching;
        RaycastHit2D hit;
        if (typeLine == TypeLineFind.horizontal)
        {
            scaleCur = new Vector2(0, 1);
            hit = Physics2D.Linecast(transform.position, (Vector2)transform.position + Vector2.right * dir * 20000, layerMask);
            posTarget = hit.point;
            int posX = MathfExtension.FloorToInt(posTarget.x);
            Debug.Log("posTarGetX:" + posTarget.x + " point:" + hit.point + " posX:" + posX + " POSTARGET:" + posTarget);

            posTarget.x = posX + ((int)dir * GameConfig.SIZE_HALF_LINE);
        }
        else
        {
            scaleCur = new Vector2(1,0);
            hit = Physics2D.Linecast(transform.position, (Vector2)transform.position + Vector2.up * dir * 20000, layerMask);
            posTarget = hit.point;
            int posY = MathfExtension.FloorToInt(posTarget.y);
            Debug.Log("posTarGetY:" + posTarget.x + " point:" + hit.point + " posY:" + posY + " POSTARGET:" + posTarget);
            posTarget.y = posY +((int)dir * GameConfig.SIZE_HALF_LINE);
            
        }
        //Debug.Log("hit:"+transform.position+"=>"+posTarget);
    }

    public void UnTrigger()
    {
        boxHit.enabled = false;
    }

    private bool isRun;
    private void Update()
    {
        if (isRun == false) return;
        if (isCompleteSketching == false && GameplayController.Instance.isEndGame == false)
        {
            if (typeLine.Equals(TypeLineFind.vertical))
            {
                scaleCur.y += GameConfig.SPEED_SCALE * Time.deltaTime * dir;
                if (Mathf.Abs(scaleCur.y) >= Mathf.Abs((posTarget.y - transform.position.y) / 100.0f))
                {
                    scaleCur.y = (posTarget.y - transform.position.y) / 100.0f;
                    posTarget.y = MathfExtension.FloorToIntAbs(posTarget.y);
                    CompleteSketching(posTarget);
                }

            }
            else if (typeLine.Equals(TypeLineFind.horizontal))
            {
                scaleCur.x += GameConfig.SPEED_SCALE * Time.deltaTime * dir;
                if (Mathf.Abs(scaleCur.x) >= Mathf.Abs((posTarget.x - transform.position.x) / 100.0f))
                {
                    scaleCur.x = (posTarget.x - transform.position.x) / 100.0f;
                    posTarget.x = MathfExtension.FloorToIntAbs(posTarget.x);
                    CompleteSketching(posTarget);
                    
                }
            }
            transform.localScale = scaleCur;
        }
    }

    private void CompleteSketching(Vector2 pos)
    {
        isRun = false;
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
        this.point.x = MathfExtension.FloorToIntAbs(point.x);
        this.point.y = MathfExtension.FloorToIntAbs(point.y);
        this.typeLine = typeLine;
    }
}