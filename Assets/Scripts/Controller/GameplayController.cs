using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayController : Singleton<GameplayController>
{
    public Vector2 sizeLine;
    [SerializeField]
    private Vector2 sizeBoard;
    [SerializeField]
    private ItemBall itemBall = null;
    [SerializeField]
    private GameObject itemLinePrefab = null;
    [SerializeField]
    private Transform boardTran = null;
    [SerializeField]
    private Transform itemMask = null;
    [SerializeField]
    private TypeLine typeLineCur;
    [SerializeField]
    private GameObject[] typeLinePreview;
    public ItemLine lineLeft,lineRight,lineTop,lineBot;

    [HideInInspector]
    public bool canCreateLine;
    [HideInInspector]
    public bool isEndGame;
    [HideInInspector]
    public int pointCur, pointTarget;

    private int originSizeBoard;
    private void Start()
    {
        
        Init();
    }
    public void Init()
    {
        isEndGame = false;
        canCreateLine = false;
        typeLineCur = TypeLine.Vertical;
        ShowTypeLinePreview();
        lineLeft.transform.localScale = lineRight.transform.localScale = new Vector2(sizeLine.x, sizeLine.y);
        lineTop.transform.localScale = lineBot.transform.localScale = new Vector2(sizeLine.y, sizeLine.x) ;

        lineLeft.transform.localPosition = new Vector2(-Utilities.ConvertToLocal(sizeBoard.x)/2, 0);
        lineRight.transform.localPosition = new Vector2(Utilities.ConvertToLocal(sizeBoard.x) / 2, 0);
        lineTop.transform.localPosition = new Vector2(0, Utilities.ConvertToLocal(sizeBoard.y) / 2);
        lineBot.transform.localPosition = new Vector2(0,-Utilities.ConvertToLocal(sizeBoard.y) / 2);

        lineLeft.Init(TypeLine.Vertical,true);
        lineRight.Init(TypeLine.Vertical, true);
        lineTop.Init(TypeLine.Horizontal, true);
        lineBot.Init(TypeLine.Horizontal, true);
        Rect rect = GetRectBoard();
        originSizeBoard = (int)(rect.width + rect.height) * 2;
        pointCur = 0;
        pointTarget = originSizeBoard / 2;
        InitItemMask();
        
        

    }

    public void ContinueGame()
    {
        Debug.Log("ContinueGame");
        SceneManager.LoadScene("Gameplay");
    }

    public void TryAgain()
    {
        Debug.Log("Trygain");
        SceneManager.LoadScene("Gameplay");
    }

    public void PlayGame()
    {
        StartCoroutine(PlayGameDelay());
    }

    public void EndGame(bool isWin)
    {
        Debug.Log("EndGame:"+isWin);
        isEndGame = true;
        UIGameplayController.Instance.EndGame(isWin);
    }

    private IEnumerator PlayGameDelay()
    {
        canCreateLine = false;
        yield return new WaitForSeconds(0.5f);
        isEndGame = false;
        canCreateLine = true;
        itemBall.Init();
    }
    

    private void InitItemMask()
    {
        Rect rect = GetRectBoard();
        itemMask.position = new Vector2(rect.x,rect.y);
        itemMask.localScale = new Vector2(rect.width,rect.height);
        pointCur = originSizeBoard - (int)(rect.width + rect.height) * 2;
        UIGameplayController.Instance.UpdatePoint();
        if (pointCur >= pointTarget)
        {
            EndGame(true);
        }
        
    }

    public Rect GetRectBoard()
    {
        Rect rect = new Rect(0,0,0,0);
        rect.x = (lineRight.transform.position.x + lineLeft.transform.position.x) / 2;
        rect.y = (lineTop.transform.position.y + lineBot.transform.position.y) / 2;
        rect.width = lineRight.transform.position.x * GameConfig.RATIO_GAME + sizeLine.x / 2 - (lineLeft.transform.position.x * GameConfig.RATIO_GAME) + sizeLine.x / 2;
        rect.height = lineTop.transform.position.y * GameConfig.RATIO_GAME + sizeLine.x / 2 - (lineBot.transform.position.y *GameConfig.RATIO_GAME) + sizeLine.x / 2 ;
        return rect;
    }

    public void CreateLine(Vector3 posCreate)
    {
        canCreateLine = false;
        GameObject obj = Instantiate(itemLinePrefab, boardTran);
        ItemLine itemLine = obj.GetComponent<ItemLine>();
        obj.transform.position = posCreate;
        obj.transform.localScale = Vector3.one * sizeLine.x;
        itemLine.Init(typeLineCur,false);
        int curTmp = (int)typeLineCur + 1;
        curTmp = (int)Mathf.Repeat(curTmp,Enum.GetValues(typeLineCur.GetType()).Length);
        typeLineCur = (TypeLine)curTmp;
        ShowTypeLinePreview();
    }

    public void ShowTypeLinePreview()
    {
        for (int i = 0; i < typeLinePreview.Length; i++)
        {
            if (typeLineCur == (TypeLine)i)
            {
                typeLinePreview[i].SetActive(true);
            }
            else
            {
                typeLinePreview[i].SetActive(false);
            }
        }
    }

    public void CompleteSketching(ItemLine itemLineSketching)
    {
        canCreateLine = true;
        Rect rectBoard = GetRectBoard();
        if (itemLineSketching.typeLine.Equals(TypeLine.Horizontal))
        {
            if (itemLineSketching.transform.position.y >= itemBall.transform.position.y)
            {
                //cat ben tren
                lineTop.UnTrigger();
                lineTop = itemLineSketching;
            }
            else
            {
                //cat ben bot
                lineBot.UnTrigger();
                lineBot = itemLineSketching;
            }
        }
        else if (itemLineSketching.typeLine.Equals(TypeLine.Vertical))
        {
            if (itemLineSketching.transform.position.x >= itemBall.transform.position.x)
            {
                //cat ben phai
                lineRight.UnTrigger();
                lineRight = itemLineSketching;
            }
            else
            {
                //cat ben trai
                lineLeft.UnTrigger();
                lineLeft = itemLineSketching;
            }
        }
        InitItemMask();
    }


}
