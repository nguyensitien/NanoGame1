using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLine : MonoBehaviour
{
    public TypeLine typeLine;
    [HideInInspector]
    public bool isCompleteSketching = true;
    private int numHit;
    private Vector2 scaleCur;
    private BoxCollider2D boxHit;
    private void Awake()
    {
        boxHit = GetComponent<BoxCollider2D>();
    }
    public void Init(TypeLine typeLine,bool isSketching)
    {
        this.typeLine = typeLine;
        isCompleteSketching = isSketching;
        scaleCur = Vector2.one * GameplayController.Instance.sizeLine.x;
        numHit = 0;
    }

    public void UnTrigger()
    {
        boxHit.enabled = false;
    }
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCompleteSketching ) return;
        if (collision.tag.Equals("ItemLine"))
        {
            numHit++;
            
            if (numHit == 2)
            {
                CompleteSketching();
            }
        }
    }

    private void Update()
    {
        if (isCompleteSketching == false && GameplayController.Instance.isEndGame == false)
        {
            if (typeLine.Equals(TypeLine.Vertical))
            {
                scaleCur.y += GameConfig.SPEED_SCALE * Time.deltaTime;
            }
            else if (typeLine.Equals(TypeLine.Horizontal))
            {
                scaleCur.x += GameConfig.SPEED_SCALE * Time.deltaTime;
            }
            transform.localScale = scaleCur;
        }
    }

    private void CompleteSketching()
    {
        isCompleteSketching = true;
        GameplayController.Instance.CompleteSketching(this);
    }

}
