﻿using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameplayController : Singleton<GameplayController>
{
    [SerializeField]
    private CanvasGroup csGroupBgBlack;
    [SerializeField]
    private GameObject maskMapPrefab,ballTrailPrefab;
    public Vector2 sizeLine;
    [SerializeField]
    private Vector2 sizeBoard;
    [SerializeField]
    private Transform boardTran = null;
    [SerializeField]
    private Transform itemMask = null;
    public TypeLineFind typeLineCur;
    [SerializeField]
    private GameObject[] typeLinePreview;
    [SerializeField]
    private GameObject[] linePrefab;
    [HideInInspector]
    public bool canCreateLine;
    [HideInInspector]
    public bool isEndGame;
    [HideInInspector]
    public int pointCur, pointTarget;
    [SerializeField]
    private Transform bgBoard = null;
    private int originSizeBoard;
    [HideInInspector]
    public Tilemap tileMap;

    public List<List<ItemLineInfo>> itemLineInfoList;
    public Vector2[] GetPointsBoard(int index)
    {

        return pointsList[index];
    }
    [HideInInspector]
    public int indexMask = -1;
    private GameObject nodeMap;
    private ItemBall[] arrItemBall;
    private ConditionNode conditionMain;
    private SpriteMask[] maskMapArr;
    public List<Vector2[]> pointsList;
    private SpriteRenderer bgColorSprite;
    private void Awake()
    {
        bgColorSprite = bgBoard.GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        //Application.targetFrameRate = 10;
        //QualitySettings.vSyncCount = 0;

        InitGame();
    }

    public void InitGame()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //nodeMap = GameObject.Find("Node26");
        //GameObject mapPrefab = Resources.Load<GameObject>("Maps/Node" + 1);
        //GameObject nodeMap = Instantiate(mapPrefab);
        //ObjectDataController.Instance.IdNodeFighting = 46;
        GameObject mapPrefab = Resources.Load<GameObject>("Maps/Node" + ObjectDataController.Instance.IdNodeFighting);
        nodeMap = Instantiate(mapPrefab);
        conditionMain = nodeMap.GetComponent<ConditionNode>();
        GameObject tileMapObj = nodeMap.transform.Find("Tilemap").gameObject;
        tileMapObj.GetComponent<TilemapRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        arrItemBall = FindObjectsOfType<ItemBall>();
        CreateTrailBallList();
        CreateMaskMap(arrItemBall.Length);
        nodeMap.transform.localPosition = Vector3.zero;
        tileMap = tileMapObj.GetComponent<Tilemap>();
        HideAlphaAll();
        CreateIemInfoList(arrItemBall.Length);
        List<ItemLineInfo> itemLineInfoListTmp = new List<ItemLineInfo>();
        foreach (var pos in tileMap.cellBounds.allPositionsWithin)
        {
            Sprite sprite = tileMap.GetSprite(pos);

            if (sprite != null)
            {
                Vector2 p = new Vector2(pos.x * +sprite.bounds.size.x + sprite.bounds.size.x / 2, pos.y * sprite.bounds.size.y + sprite.bounds.size.y / 2);

                if (sprite.name == TypeLine.line_bot_left.ToString())
                {
                    ItemLineInfo itemLine = new ItemLineInfo(p, TypeLine.line_bot_left);
                    itemLineInfoListTmp.Add(itemLine);
                }
                else if (sprite.name == TypeLine.line_bot_right.ToString())
                {
                    ItemLineInfo itemLine = new ItemLineInfo(p, TypeLine.line_bot_right);
                    itemLineInfoListTmp.Add(itemLine);
                }
                else if (sprite.name == TypeLine.line_top_left.ToString())
                {
                    ItemLineInfo itemLine = new ItemLineInfo(p, TypeLine.line_top_left);
                    itemLineInfoListTmp.Add(itemLine);
                }
                else if (sprite.name == TypeLine.line_top_right.ToString())
                {
                    ItemLineInfo itemLine = new ItemLineInfo(p, TypeLine.line_top_right);
                    itemLineInfoListTmp.Add(itemLine);
                }
            }
        }



        Init(itemLineInfoListTmp);
    }

    public void ContinueGame()
    {
        if (itemLineList.Count >= 2)
        {
            DestroyImmediate(itemLineList[itemLineList.Count-1].gameObject);
            DestroyImmediate(itemLineList[itemLineList.Count-2].gameObject);
            itemLineList.RemoveAt(itemLineList.Count - 1);
            itemLineList.RemoveAt(itemLineList.Count - 1);
        }
        DataController.Instance.UserData.attemptNodeCur++;
        itemLineSketchingList = new List<ItemLine>();
        pointsComplete = new List<Vector2>();
        ShowAlphaAll(() => {
            isEndGame = false;
            PlayGame();
        });
    }

    public void NextLevel()
    {
        RemoveAll();
        UIGameplayController.Instance.UpdateTxtLevel();
        InitGame();
        ShowAlphaAll(() => {
            PlayGame();
        });
    }

    public void RemoveAll()
    {
        boardTran.gameObject.SetActive(true);
        if (nodeMap != null)
        {
            DestroyImmediate(nodeMap);
        }
        for (int i = 0; i < maskMapArr.Length; i++)
        {
            DestroyImmediate(maskMapArr[i].gameObject);
        }
        RemoveItemLineList();
        itemLineInfoList = null;
        pointsList = null;
        maskMapArr = null;
        isEndGame = false;
        pointsComplete = new List<Vector2>();
        itemLineSketchingList = new List<ItemLine>();
        typeLineCur = TypeLineFind.vertical;
        isInited = false;
    }

    public void RestartGame()
    {
        RemoveAll();
        InitGame();
        UIGameplayController.Instance.Init();
        GameplayController.Instance.HideAlphaAll();
        //ShowAlphaAll(()=> {
        //    PlayGame();
        //});
    }

    public void RemoveItemLineList()
    {
        for (int i = 0; i < itemLineList.Count; i++)
        {
            DestroyImmediate(itemLineList[i].gameObject);
        }
        itemLineList = new List<SpriteRenderer>();
    }

    private void CreateTrailBallList()
    {
        for (int i = 0; i < arrItemBall.Length; i++)
        {
            Transform ballTrail = Instantiate(ballTrailPrefab,arrItemBall[i].transform).transform;
            ballTrail.transform.localPosition = Vector3.zero;
        }
      
    }

    private void CreateIemInfoList(int length)
    {
        itemLineInfoList = new List<List<ItemLineInfo>>();
        pointsList = new List<Vector2[]>();
        triangleList = new Triangulator[length];
        for (int i = 0; i < length; i++)
        {
            itemLineInfoList.Add(new List<ItemLineInfo>());
            pointsList.Add(new Vector2[0]);
            triangleList[i] = null;
        }
    }

    private void CreateMaskMap(int length)
    {
        maskMapArr = new SpriteMask[length];
        for (int i = 0; i < length; i++)
        {
            GameObject maskObj = GameObject.Instantiate(maskMapPrefab);
            maskObj.SetActive(false);
            maskObj.transform.SetParent(boardTran);
            maskMapArr[i] = maskObj.GetComponent<SpriteMask>();
        }
    }

    private void InitPoints()
    {
        int length = itemLineInfoList.Count;
        for (int i = 0; i < length; i++)
        {
            List<ItemLineInfo> itemList = itemLineInfoList[i];
            int lengthPoint = itemList.Count;
            pointsList[i] = new Vector2[lengthPoint];
            for (int j = 0; j < lengthPoint; j++)
            {
                pointsList[i][j] = itemList[j].point;
            }
        }
    }

    public Vector2[] ParsePointFromItemInfo(List<ItemLineInfo> itemLineInfoListTmp)
    {
        Vector2[] result = new Vector2[itemLineInfoListTmp.Count];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = itemLineInfoListTmp[i].point;
        }
        return result;
    }

    public List<Vector2[]> ClonePointsList()
    {
        List<Vector2[]> result = new List<Vector2[]>();
        for (int i = 0; i < pointsList.Count; i++)
        {
            Vector2[] ps = pointsList[i];
            Vector2[] psTmp = new Vector2[ps.Length];
            
            for (int j = 0; j < ps.Length; j++)
            {
                psTmp[j] = ps[j];
            }
            result.Add(psTmp);
        }
        return result;
    }

    private int minX, maxX, minY, maxY, width, height;
    private Vector2 center;
    
    public void Init(List<ItemLineInfo> itemLineInfoListTmp)
    {
        isEndGame = false;
        canCreateLine = false;
        typeLineCur = TypeLineFind.vertical;
        ShowTypeLinePreview();
        Vector2[] minmax = GetMinMaxByItemInfoList(itemLineInfoListTmp);
        minX = (int)minmax[0].x;
        maxX = (int)minmax[0].y;
        minY = (int)minmax[1].x;
        maxY = (int)minmax[1].y;
        center = new Vector2((minX+maxX + sizeLine.x / 2) /2,(minY+maxY + sizeLine.x / 2) /2);
        width = maxX - minX;
        height = maxY - minY;
        minXOri = minX;
        minYOri = minY;
        bgBoard.localPosition = center;
        bgBoard.localScale = new Vector2(width/100.0f,height/100.0f);
        //Debug.Log("minX:"+minX+" maxX:"+maxX+" minY:"+minY+" maxY:"+maxY+" center:"+center+" width:"+width+" height:"+height);
        List<ItemLineInfo> itemLineInfoListTmpTmp = new List<ItemLineInfo>(itemLineInfoListTmp);
        for (int i = 0; i < arrItemBall.Length; i++)
        {
            itemLineInfoListTmp = SortPoints(itemLineInfoListTmpTmp);
            Vector2[] pointsTmp = ParsePointFromItemInfo(itemLineInfoListTmp);
            //Debug.Log("----------------:"+i+":"+ itemLineInfoListTmp.Count+"::"+ itemLineInfoListTmpTmp.Count);
            if (itemLineInfoListTmp.Count >= 4)
            {
                for (int j = 0; j < itemLineInfoListTmp.Count; j++)
                {
                    itemLineInfoListTmpTmp.Remove(itemLineInfoListTmp[j]);
                }
                CompleteReCreateBoard(i, itemLineInfoListTmp, true);
            }
        }



        //InitTriangle();

        originSizeBoard = (int)GetAreaAll();
        pointCur = 0;
        pointTarget = (int)(originSizeBoard * conditionMain.percentWin);
        UpdatePoint();
    }
    private Triangulator[] triangleList;
    public SpriteRenderer spriteRenderern;
    private Sprite[] spriteMaskList;
    private int minXOri, minYOri;

   

    private bool isInited;
    private void InitTriangle()
    {

        InitPoints();
        List<Vector2[]> pointsListClone = ClonePointsList();
        List<Vector2[]> pointsBorderList = CreatePointsBorder(pointsListClone);
        //Debug.Log("pointsBorderList:" + pointsBorderList.Count);
        for (int i = 0; i < pointsBorderList.Count; i++)
        {
            Vector2[] pointsBorder = pointsBorderList[i];
            //Debug.Log("InitTriangle:" + i + ":" + pointsBorder.Length);
            if (pointsBorder.Length == 0) continue;
            Triangulator triangle = new Triangulator(pointsBorder);
            //test
           
            triangleList[i] = triangle;
            
            ushort[] tris = new ushort[triangle.Triangulate().Length];
            int[] trisss = triangle.Triangulate();
           
            Vector2[] verticles = new Vector2[pointsBorder.Length];
            Vector2 ori = new Vector2(minXOri-sizeLine.x/2, minYOri- sizeLine.x / 2);
            for (int j = 0; j < pointsBorder.Length; j++)
            {
                verticles[j] = (pointsBorder[j] - ori);
            }
            for (int j = 0; j < tris.Length; j++)
            {
                tris[j] = (ushort)(trisss[j]);
            }
            if (isInited == false)
            {
                isInited = true;
               
                
                for (int j = 0; j < maskMapArr.Length; j++)
                {
                    Texture2D tex2D = new Texture2D(width + (int)sizeLine.x , height + (int)sizeLine.x );
                    Sprite spriteMask = Sprite.Create(tex2D, new Rect(0, 0, width + sizeLine.x , height + sizeLine.x ), Vector2.one * 0.5f, 100, 0, SpriteMeshType.Tight);
                    maskMapArr[j].gameObject.transform.position = new Vector2(center.x- sizeLine .x/ 4,center.y-sizeLine.x/4);
                    //for (int k = 0; k < verticles.Length; k++)
                    //{
                    //    Debug.Log("verticle Init:" + k + ":" + verticles[k]);
                    //}
                    maskMapArr[j].sprite = spriteMask;
                    spriteMask.OverrideGeometry(verticles, tris);

                }
                return;
            }
            
            Sprite spriteTmp = maskMapArr[i].sprite;
            for (int index = 0; index < verticles.Length; index++)
            {
                Debug.Log("verticles:" + index + ":" + verticles[index]);
            }
            Debug.Log("width:" + width + " height:" + height + " center:" + center);
            spriteTmp.OverrideGeometry(verticles, tris);
        }
    }

    private Vector2[] GetMinMaxByItemInfoList(List<ItemLineInfo> list)
    {
        Vector2[] minmax = new Vector2[2];
        minmax[0] = list[0].point;
        minmax[1] = list[0].point;
        for (int i = 1; i < list.Count; i++)
        {
            //tim mixX
            if (list[i].point.x < minmax[0].x)
            {
                minmax[0].x = list[i].point.x;
            }
            //tim maxX
            if (list[i].point.x > minmax[0].y)
            {
                minmax[0].y = list[i].point.x;
            }

            //tim minY
            if (list[i].point.y < minmax[1].x)
            {
                minmax[1].x = list[i].point.y;
            }
            //tim maxY
            if (list[i].point.y > minmax[1].y)
            {
                minmax[1].y = list[i].point.y;
            }
        }
        return minmax;
    }

    private List<Vector2[]> CreatePointsBorder(List<Vector2[]> pointsTmp)
    {
        List<Vector2[]> result = new List<Vector2[]>();
        for (int i = 0; i < pointsTmp.Count; i++)
        {
            Vector2[] points = pointsTmp[i];
            Vector2[] pointsResult = new Vector2[points.Length];
            int length = points.Length;
            float padding = 2;
            for (int j = 0; j < length; j++)
            {
                pointsResult[j] = points[j];
                Vector2 pCenter = points[j];
                Vector2 pTopRight = new Vector2(pCenter.x + (sizeLine.x / 2), pCenter.y + (sizeLine.x / 2));
                Vector2 pTopLeft = new Vector2(pCenter.x - (sizeLine.x / 2), pCenter.y + (sizeLine.x / 2));
                Vector2 pBotLeft = new Vector2(pCenter.x - (sizeLine.x / 2), pCenter.y - (sizeLine.x / 2));
                Vector2 pBotRight = new Vector2(pCenter.x + (sizeLine.x / 2), pCenter.y - (sizeLine.x / 2));
                //Debug.Log("Before pTopRight:" + pTopRight + " pCenter:" + pCenter);
                //Debug.Log("Before pTopLeft:" + pTopLeft + " pCenter:" + pCenter);
                //Debug.Log("Before pBotLeft:" + pBotLeft + " pCenter:" + pCenter);
                //Debug.Log("Before pBotRight:" + pBotRight + " pCenter:" + pCenter);
                //pTopRight = new Vector2(Mathf.FloorToInt(pTopRight.x), Mathf.FloorToInt(pTopRight.y));
                //pTopLeft = new Vector2(Mathf.FloorToInt(pTopLeft.x), Mathf.FloorToInt(pTopLeft.y));
                //pBotLeft = new Vector2(Mathf.FloorToInt(pBotLeft.x), Mathf.FloorToInt(pBotLeft.y));
                //pBotRight = new Vector2(Mathf.FloorToInt(pBotRight.x), Mathf.FloorToInt(pBotRight.y));
                //Debug.Log("After pTopRight:" + pTopRight + " pCenter:" + pCenter);
                //Debug.Log("After pTopLeft:" + pTopLeft + " pCenter:" + pCenter);
                //Debug.Log("After pBotLeft:" + pBotLeft + " pCenter:" + pCenter);
                //Debug.Log("After pBotRight:" + pBotRight + " pCenter:" + pCenter);
                //Debug.Log("****************");
                List<Vector2> pointsOut = new List<Vector2>();
                List<Vector2> pointsIn = new List<Vector2>();
                if (Utilities.IsPointInPolygon(new Vector2(pCenter.x + padding, pCenter.y + padding), points))
                {
                    //Debug.Log("TopRight In:"+points.Length+":"+pCenter+":"+ new Vector2(pCenter.x + padding, pCenter.y + padding));
                    //for (int k = 0; k < points.Length; k++)
                    //{
                    //    Debug.Log(k+":"+points[k]);
                    //}
                    pointsIn.Add(pTopRight);
                }
                else {
                    //Debug.Log("TopRight Out");
                    pointsOut.Add(pTopRight);
                }

                if (Utilities.IsPointInPolygon(new Vector2(pCenter.x - padding, pCenter.y + padding), points))
                {
                    //Debug.Log("pTopLeft In");
                    pointsIn.Add(pTopLeft);
                }
                else {
                    //Debug.Log("pTopLeft Out");
                    pointsOut.Add(pTopLeft);
                }

                if (Utilities.IsPointInPolygon(new Vector2(pCenter.x - padding, pCenter.y - padding), points))
                {
                    //Debug.Log("pBotLeft In");
                    pointsIn.Add(pBotLeft);
                }
                else {
                    //Debug.Log("pBotLeft Out");
                    pointsOut.Add(pBotLeft);
                }

                if (Utilities.IsPointInPolygon(new Vector2(pCenter.x + padding, pCenter.y - padding), points))
                {
                    //Debug.Log("pBotRight In");
                    pointsIn.Add(pBotRight);
                }
                else {
                    //Debug.Log("pBotRight Out");
                    pointsOut.Add(pBotRight);
                }

                //Debug.Log("pointsIn:"+pointsIn.Count +" pointsOut:"+pointsOut.Count+ " pCenter:"+ pCenter);
                if (pointsIn.Count == 1)
                {
                    pointsResult[j] = pCenter + (pCenter - pointsIn[0]) * 1;
                }
                else if (pointsOut.Count == 1)
                {
                    pointsResult[j] = pointsOut[0];
                }
            }
            result.Add(pointsResult);
        }
        
        return result;
    }

    private void OnDestroy()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }

    //private void OnDrawGizmosSelected()
    //{
    //    for (int i = 0; i < points.Length; i++)
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawCube(points[i], Vector2.one * 10);
    //    }
    //}

    
    public void TryAgain()
    {
        Debug.Log("Trygain");
        SceneManager.LoadScene("Gameplay");
    }

    public void PlayGame()
    {
        csGroupBgBlack.alpha = 0;
        DOTween.To(() => csGroupBgBlack.alpha, (x) => csGroupBgBlack.alpha = x, 1, 0.5f).SetEase(Ease.Linear);
        StartCoroutine(PlayGameDelay());
    }

    private void HideBoardGame()
    {
        boardTran.gameObject.SetActive(false);
        nodeMap.SetActive(false);
        for (int i = 0; i < arrItemBall.Length; i++)
        {
            arrItemBall[i].gameObject.SetActive(false);
        }
        
    }
    private IEnumerator UpdateBallEnumator;
    public void EndGame(bool isWin)
    {
        iUpdateBall.Dispose();
        HideAlphaAll();
        if (isWin)
        {
            DataController.Instance.UserData.idNodeHighest += 1;

        }
        else
        {
            
        }
        isEndGame = true;
        UIGameplayController.Instance.EndGame(isWin);
    }

    private IDisposable iUpdateBall;
    private IEnumerator PlayGameDelay()
    {

        canCreateLine = false;
        for (int i = 0; i < arrItemBall.Length; i++)
        {
            arrItemBall[i].Init();
        }
        yield return null;
        yield return null;
        yield return null;
        isEndGame = false;
        canCreateLine = true;
        //CorouUpdateBall = StartCoroutine(UpdateBall());
        iUpdateBall = Observable.FromMicroCoroutine(UpdateBall).Subscribe().AddTo(this);
        //MainThreadDispatcher.StartUpdateMicroCoroutine(UpdateBallEnumator);
        //Observable.FromMicroCoroutine(UpdateBall);

    }

    public void HideAlphaAll()
    {
        //Color32 color = new Color32(255,255,255,60);
        //for (int i = 0; i < itemLineList.Count; i++)
        //{
        //    int index = i;
        //    DOTween.To(() => itemLineList[index].color, (x) => itemLineList[index].color = x, color, 0.3f).SetEase(Ease.InSine);
        //}
        //for (int i = 0; i < arrItemBall.Length; i++)
        //{
        //    arrItemBall[i].spriteRenderer.DOFade(0.3f, 0.5f).SetEase(Ease.InSine);
        //}
        //DOTween.To(() => bgColorSprite.color, (x) => tileMap.color = x, color, 0.5f).SetEase(Ease.InSine);
        //DOTween.To(() => tileMap.color, (x) => tileMap.color = x, color, 0.5f).SetEase(Ease.InSine).OnComplete(() => {
           
        //});
    }

    public void ShowAlphaAll(Action cb = null)
    {
        for (int i = 0; i < itemLineList.Count; i++)
        {
            int index = i;
            DOTween.To(() => itemLineList[index].color, (x) => itemLineList[index].color = x, Color.white, 0.3f).SetEase(Ease.InSine);
        }
        for (int i = 0; i < arrItemBall.Length; i++)
        {
            arrItemBall[i].spriteRenderer.DOFade(1,0.5f).SetEase(Ease.InSine);
        }
        if (cb != null)
            cb();
        DOTween.To(() => bgColorSprite.color, (x) => tileMap.color = x, Color.white, 0.5f).SetEase(Ease.InSine);
        DOTween.To(()=>tileMap.color,(x)=>tileMap.color = x,Color.white,0.5f).SetEase(Ease.InSine).OnComplete(()=> {
            
        });
    }

    private Coroutine CorouUpdateBall;

    private float GetAreaAll()
    {
        float result = 0.0f;
        //Debug.Log("GetAreaAll:"+triangleList.Length);
        for (int i = 0; i < triangleList.Length; i++)
        {
            if (triangleList[i] != null)
            {
                //Debug.Log("index:"+i+":"+triangleList[i].Area());
                result += triangleList[i].Area();
            }
        }
        return result;
    }
    private void InitItemMask()
    {
        InitTriangle();
        

    }
    private List<SpriteRenderer> itemLineList = new List<SpriteRenderer>();
    public void CreateLine(TypeLineFind typeSwipe, Vector3 posCreate)
    {
        canCreateLine = false;
        typeLineCur = typeSwipe;
        for (int i = 0; i < 2; i++)
        {
            GameObject obj = Instantiate(linePrefab[(int)typeLineCur], boardTran);
            ItemLine itemLine = obj.GetComponent<ItemLine>();
            obj.transform.position = posCreate;
            obj.transform.localScale = Vector3.zero;
            itemLine.Init(typeSwipe, i, false);
            itemLineList.Add(obj.GetComponent<SpriteRenderer>());
        }
        
    }

    public void ChangeTypeSketching()
    {
        return;
        int curTmp = (int)typeLineCur + 1;
        curTmp = (int)Mathf.Repeat(curTmp, Enum.GetValues(typeLineCur.GetType()).Length);
        typeLineCur = (TypeLineFind)curTmp;
        ShowTypeLinePreview();
    }
    public void ShowTypeLinePreview()
    {
        for (int i = 0; i < typeLinePreview.Length; i++)
        {
            if (typeLineCur == (TypeLineFind)i)
            {
                typeLinePreview[i].SetActive(true);
            }
            else
            {
                typeLinePreview[i].SetActive(false);
            }
        }
    }

    private void RemovePointsSame(List<ItemLineInfo> itemLineInfoListTmp)
    {
        for (int i = 0; i < itemLineSketchingList.Count; i++)
        {
            for (int j = 0; j < itemLineInfoListTmp.Count; j++)
            {
                if ((pointsComplete[i] - itemLineInfoListTmp[j].point).magnitude <= sizeLine.x-1)
                {
                    //Debug.Log("remove Same:"+itemLineInfoListTmp[j].point+":"+itemLineInfoListTmp[j].typeLine);
                    itemLineInfoListTmp.RemoveAt(j);
                    break;
                }
            }
        }
    }

    private IEnumerator UpdateBall()
    {
        if (GameplayController.Instance.isEndGame) yield break;
        int length = arrItemBall.Length;
        while (true)
        {
            for (int i = 0; i < length; i++)
            {
                arrItemBall[i].UpdateMove();
                //yield return null;
            }
            yield return null;
        }
    }
    private void AddNewPointBoard(TypeRemoveLine typeRemove,List<ItemLineInfo> itemLineInfoListTmp,int typeAdd)
    {
        //Debug.Log("AddNewPointBoard:" + typeRemove + ":" + itemLineSketchingList.Count + ":" + itemLineInfoList.Count);
        //for (int i = 0; i < itemLineInfoListTmp.Count; i++)
        //{
        //    Debug.Log("before:" + itemLineInfoListTmp[i].point + ":" + itemLineInfoListTmp[i].typeLine + ":" + itemLineInfoListTmp.Count);
        //}
        RemovePointsSame(itemLineInfoListTmp);
        //Debug.Log("AddNewPointBoard:" + ":" + itemLineInfoListTmp.Count+" typeRemove:"+typeRemove+" typeAdd");
        //for (int i = 0; i < itemLineInfoListTmp.Count; i++)
        //{
        //    Debug.Log("after:" + itemLineInfoListTmp[i].point + ":" + itemLineInfoListTmp[i].typeLine + ":" + itemLineInfoListTmp.Count);
        //}
        switch (typeRemove)
        {
            case TypeRemoveLine.Bot:
                if (typeAdd == 0)
                {
                    for (int i = 0; i < itemLineSketchingList.Count; i++)
                    {

                        if (itemLineSketchingList[i].dir == -1)
                        {
                            //Debug.Log("add new:" + i + " dir:" + itemLineSketchingList[i].dir + " point:" + pointsComplete[i] + ":" + TypeLine.line_top_right);
                            itemLineInfoListTmp.Insert(0, new ItemLineInfo(pointsComplete[i], TypeLine.line_top_right));
                        }
                        else
                        {
                            //Debug.Log("add new:" + i + " dir:" + itemLineSketchingList[i].dir + " point:" + pointsComplete[i] + ":" + TypeLine.line_top_left);
                            itemLineInfoListTmp.Insert(0, new ItemLineInfo(pointsComplete[i], TypeLine.line_top_left));
                        }
                        //if(Check)
                    }
                }
                else if (typeAdd == 3) { }
                else
                {
                    if (itemLineSketchingList[typeAdd - 1].dir == -1)
                    {
                        //Debug.Log("add new:" + (typeAdd - 1) + " dir:" + itemLineSketchingList[typeAdd - 1].dir + " point:" + pointsComplete[typeAdd - 1] + ":" + TypeLine.line_top_right);
                        itemLineInfoListTmp.Insert(0, new ItemLineInfo(pointsComplete[typeAdd - 1], TypeLine.line_top_right));
                    }
                    else
                    {
                        //Debug.Log("add new:" + (typeAdd - 1) + " dir:" + itemLineSketchingList[typeAdd - 1].dir + " point:" + pointsComplete[typeAdd - 1] + ":" + TypeLine.line_top_left);
                        itemLineInfoListTmp.Insert(0, new ItemLineInfo(pointsComplete[typeAdd - 1], TypeLine.line_top_left));
                    }
                }
                
                break;
            case TypeRemoveLine.Top:
                if (typeAdd == 0)
                {
                    for (int i = 0; i < itemLineSketchingList.Count; i++)
                    {
                        
                        if (itemLineSketchingList[i].dir == -1)
                        {
                            //Debug.Log("add new:" + i + " dir:" + itemLineSketchingList[i].dir + " point:" + pointsComplete[i] + ":" + TypeLine.line_bot_right);
                            itemLineInfoListTmp.Insert(0, new ItemLineInfo(pointsComplete[i], TypeLine.line_bot_right));
                        }
                        else
                        {
                            //Debug.Log("add new:" + i + " dir:" + itemLineSketchingList[i].dir + " point:" + pointsComplete[i] + ":" + TypeLine.line_bot_left);
                            itemLineInfoListTmp.Insert(0, new ItemLineInfo(pointsComplete[i], TypeLine.line_bot_left));
                        }
                    }
                }
                else if (typeAdd == 3) { }
                else
                {
                    
                    if (itemLineSketchingList[typeAdd - 1].dir == -1)
                    {
                        //Debug.Log("add new:" + (typeAdd - 1) + " dir:" + itemLineSketchingList[typeAdd - 1].dir + " point:" + pointsComplete[typeAdd - 1] + ":" + TypeLine.line_bot_right);
                        itemLineInfoListTmp.Insert(0, new ItemLineInfo(pointsComplete[typeAdd - 1], TypeLine.line_bot_right));
                    }
                    else
                    {
                        //Debug.Log("add new:" + (typeAdd - 1) + " dir:" + itemLineSketchingList[typeAdd - 1].dir + " point:" + pointsComplete[typeAdd - 1] + ":" + TypeLine.line_bot_left);
                        itemLineInfoListTmp.Insert(0, new ItemLineInfo(pointsComplete[typeAdd - 1], TypeLine.line_bot_left));
                    }
                }
                
                break;
            case TypeRemoveLine.Left:
                if (typeAdd == 0)
                {
                    for (int i = 0; i < itemLineSketchingList.Count; i++)
                    {
                        if (itemLineSketchingList[i].dir == -1)
                        {
                           // Debug.Log("add new:" + i + " dir:" + itemLineSketchingList[i].dir + " point:" + pointsComplete[i] + ":" + TypeLine.line_top_right);
                            itemLineInfoListTmp.Insert(0, new ItemLineInfo(pointsComplete[i], TypeLine.line_top_right));
                        }
                        else
                        {
                            //Debug.Log("add new:" + i + " dir:" + itemLineSketchingList[i].dir + " point:" + pointsComplete[i] + ":" + TypeLine.line_bot_right);
                            itemLineInfoListTmp.Insert(0, new ItemLineInfo(pointsComplete[i], TypeLine.line_bot_right));
                        }
                    }
                }
                else if (typeAdd == 3) { }
                else
                {
                    
                    if (itemLineSketchingList[typeAdd - 1].dir == -1)
                    {
                        //Debug.Log("add new:" + (typeAdd - 1) + " dir:" + itemLineSketchingList[typeAdd - 1].dir + " point:" + pointsComplete[typeAdd - 1] + ":" + TypeLine.line_top_right);
                        itemLineInfoListTmp.Insert(0, new ItemLineInfo(pointsComplete[typeAdd - 1], TypeLine.line_top_right));
                    }
                    else
                    {
                        //Debug.Log("add new:" + (typeAdd - 1) + " dir:" + itemLineSketchingList[typeAdd - 1].dir + " point:" + pointsComplete[typeAdd - 1] + ":" + TypeLine.line_bot_right);
                        itemLineInfoListTmp.Insert(0, new ItemLineInfo(pointsComplete[typeAdd - 1], TypeLine.line_bot_right));
                    }
                }
                break;
            case TypeRemoveLine.Right:
                if (typeAdd == 0)
                {
                    for (int i = 0; i < itemLineSketchingList.Count; i++)
                    {
                        if (itemLineSketchingList[i].dir == -1)
                        {
                            //Debug.Log("add new:" + i + " dir:" + itemLineSketchingList[i].dir + " point:" + pointsComplete[i] + ":" + TypeLine.line_top_left);
                            itemLineInfoListTmp.Insert(0, new ItemLineInfo(pointsComplete[i], TypeLine.line_top_left));
                        }
                        else
                        {
                            //Debug.Log("add new:" + i + " dir:" + itemLineSketchingList[i].dir + " point:" + pointsComplete[i] + ":" + TypeLine.line_bot_left);
                            itemLineInfoListTmp.Insert(0, new ItemLineInfo(pointsComplete[i], TypeLine.line_bot_left));
                        }
                    }
                }
                else if (typeAdd == 3) { }
                else
                {
                    
                    if (itemLineSketchingList[typeAdd-1].dir == -1)
                    {
                        //Debug.Log("add new:" + (typeAdd - 1) + " dir:" + itemLineSketchingList[typeAdd - 1].dir + " point:" + pointsComplete[typeAdd - 1] + ":" + TypeLine.line_top_left);
                        itemLineInfoListTmp.Insert(0, new ItemLineInfo(pointsComplete[typeAdd - 1], TypeLine.line_top_left));
                    }
                        else
                    {
                        //Debug.Log("add new:" + (typeAdd - 1) + " dir:" + itemLineSketchingList[typeAdd - 1].dir + " point:" + pointsComplete[typeAdd - 1] + ":" + TypeLine.line_bot_left);
                        itemLineInfoListTmp.Insert(0, new ItemLineInfo(pointsComplete[typeAdd - 1], TypeLine.line_bot_left));
                    }
                }
                break;
        }
        
    }
    /// <summary>
    /// v2 + v3 = v0+v1
    /// --v2 = (v0+v1) - v3
    /// -- (v0+v1) + v3 = v0+v1
    /// </summary>
    private List<Vector2> pointsComplete = new List<Vector2>();
    private List<ItemLine> itemLineSketchingList = new List<ItemLine>();
    public void CompleteSketching(Vector2 pos,ItemLine itemlineSketching)
    {

        pointsComplete.Add(pos);
        itemLineSketchingList.Add(itemlineSketching);
        if (pointsComplete.Count == 2)
        {

            for (int i = 0; i < itemLineSketchingList.Count; i++)
            {
                itemLineSketchingList[i].isCompleteSketching = true;

            }
            canCreateLine = true;
            bool isCreateNewMask = false;
            List<ItemLineInfo> itemLineInfoListTmpTmp = new List<ItemLineInfo>(itemLineInfoList[indexMask]);
            int numCreated = 0;
            for (int i = 0; i < 2; i++)
            {

                for (int typeAdd = 0; typeAdd < 4; typeAdd++)
                {
                    //Debug.Log("Process :"+i+"-"+typeAdd);
                    List<ItemLineInfo> itemLineInfoListTmp = new List<ItemLineInfo>(itemLineInfoListTmpTmp);
                    if (typeLineCur.Equals(TypeLineFind.horizontal))
                    {
                        if (i == 0)
                        {
                            //cat ben tren
                            //Debug.Log("cat ben tren");
                            AddNewPointBoard(TypeRemoveLine.Top, itemLineInfoListTmp, typeAdd);
                        }
                        else if (i == 1)
                        {
                            //cat ben bot
                            //Debug.Log("cat ben bot");
                            AddNewPointBoard(TypeRemoveLine.Bot, itemLineInfoListTmp, typeAdd);
                        }
                        itemLineInfoListTmp = SortPoints(itemLineInfoListTmp);
                        Vector2[] pointsTmp = ParsePointFromItemInfo(itemLineInfoListTmp);
                        if (itemLineInfoListTmp.Count >= 4 && CheckAnyBallInArena(pointsTmp))
                        {
                            for (int j = 0; j < itemLineInfoListTmp.Count; j++)
                            {
                                itemLineInfoListTmpTmp.Remove(itemLineInfoListTmp[j]);
                            }
                            CompleteReCreateBoard(indexMask, itemLineInfoListTmp, isCreateNewMask);
                            numCreated++;
                            isCreateNewMask = true;
                            if (numCreated == 2)
                            {
                                indexMask = -1;
                                ChangeTypeSketching();
                                itemLineSketchingList = new List<ItemLine>();
                                pointsComplete = new List<Vector2>();
                                UpdatePoint();
                                return;
                            }
                        }
                        else
                        {
                            //Debug.Log("ball khong nam trong points:" + itemLineInfoListTmp.Count);
                            for (int j = 0; j < itemLineInfoListTmp.Count; j++)
                            {
                                itemLineInfoListTmpTmp.Remove(itemLineInfoListTmp[j]);
                            }
                            //itemLineInfoList.Remove(itemLineInfoListTmp[itemLineInfoListTmp.Count - 1]);
                            // Debug.Log("after remove:" + itemLineInfoList.Count);
                        }
                    }
                    else
                    {
                        
                        if (i == 0)
                        {
                            //cat ben phai
                            //Debug.Log("cat ben phai");
                            AddNewPointBoard(TypeRemoveLine.Right, itemLineInfoListTmp, typeAdd);
                        }
                        else if (i == 1)
                        {
                            //cat ben trai
                            //Debug.Log("cat ben trai");

                            AddNewPointBoard(TypeRemoveLine.Left, itemLineInfoListTmp, typeAdd);
                        }
                        //Debug.Log("tiep tuc cat:" + i + ":" + typeAdd + ":" + itemLineInfoListTmp.Count );
                        itemLineInfoListTmp = SortPoints(itemLineInfoListTmp);

                        Vector2[] pointsTmp = ParsePointFromItemInfo(itemLineInfoListTmp);
                        
                        if (itemLineInfoListTmp.Count >= 4 && CheckAnyBallInArena(pointsTmp))
                        {
                            for (int j = 0; j < itemLineInfoListTmp.Count; j++)
                            {
                                itemLineInfoListTmpTmp.Remove(itemLineInfoListTmp[j]);
                            }
                            CompleteReCreateBoard(indexMask, itemLineInfoListTmp, isCreateNewMask);
                            numCreated++;
                            isCreateNewMask = true;
                            if (numCreated == 2) {
                                indexMask = -1;
                                ChangeTypeSketching();
                                itemLineSketchingList = new List<ItemLine>();
                                pointsComplete = new List<Vector2>();
                                UpdatePoint();
                                return;
                            }
                        }
                        else
                        {
                            //Debug.Log("ball khong nam trong points:" + itemLineInfoListTmp.Count);
                            for (int j = 0; j < itemLineInfoListTmp.Count; j++)
                            {
                                itemLineInfoListTmpTmp.Remove(itemLineInfoListTmp[j]);
                            }
                            //itemLineInfoList.Remove(itemLineInfoListTmp[itemLineInfoListTmp.Count-1]);
                            //Debug.Log("after remove:" + itemLineInfoList.Count);
                        }
                    }
                }
            }
            //itemLineInfoList = SortPoints(itemLineInfoList);
            //
            //Debug.Log("----After Sort-----");
            UpdatePoint();
            indexMask = -1;
            ChangeTypeSketching();
            itemLineSketchingList = new List<ItemLine>();
            pointsComplete = new List<Vector2>();
        }

    }

    public void UpdatePoint()
    {
        int arenAll = (int)GetAreaAll();
        pointCur = originSizeBoard - arenAll;
        pointCur = Mathf.Max(0, pointCur);
        UIGameplayController.Instance.UpdatePoint();
        if (pointCur >= pointTarget)
        {
            EndGame(true);
        }
    }

    private bool CheckAllBallInArena(int index,Vector2[] points)
    {
        for (int i = 0; i < arrItemBall.Length; i++)
        {
            if (Utilities.IsPointInPolygon((Vector2)arrItemBall[i].transform.position, points) == false)
                return false;
        }
        return true;
    }

    private bool CheckAnyBallInArena(Vector2[] points)
    {
        for (int i = 0; i < arrItemBall.Length; i++)
        {
            if (Utilities.IsPointInPolygon((Vector2)arrItemBall[i].transform.position, points))
                return true;
        }
        return false;
    }
    private void CompleteReCreateBoard(int index,List<ItemLineInfo> itemLineInfoListTmp,bool isCreateNewMask)
    {
        //Debug.Log("CompleteReCreateBoard:" + isCreateNewMask+":"+index+":"+itemLineInfoListTmp.Count);
        if (isCreateNewMask == false)
        {
            itemLineInfoList[index] = new List<ItemLineInfo>(itemLineInfoListTmp);
            InitItemMask();

        }
        else
        {
            for (int i = 0; i < itemLineInfoList.Count; i++)
            {
                if (itemLineInfoList[i].Count == 0)
                {
                   
                    itemLineInfoList[i] = new List<ItemLineInfo>(itemLineInfoListTmp);
                    //Debug.Log("Create Board:" + i + ":" + itemLineInfoList[i].Count+":"+itemLineInfoListTmp.Count);
                    InitItemMask();
                    maskMapArr[i].gameObject.SetActive(true);
                    return;
                }
            }
        }
        
    }

    //private void CheckConditionEndGame()
    //{
    //    for (int i = 0; i < arrItemBall.Length; i++)
    //    {
    //        if (Utilities.IsPointInPolygon(arrItemBall[i].transform.position, points) == false)
    //        {
    //            EndGame(false);
    //        }
    //    }
    //}

    public List<ItemLineInfo> SortPoints(List<ItemLineInfo> itemLineInfoListTmp)
    {
        
        if (itemLineInfoListTmp.Count <= 0) return new List<ItemLineInfo>();
        List<ItemLineInfo> result = new List<ItemLineInfo>();
        ItemLineInfo itemLineCur = itemLineInfoListTmp[0];
        result.Add(itemLineCur);
        int index = 0;
        while (index <= 100)
        {
            itemLineCur = FindPointNextValid(itemLineCur, itemLineInfoListTmp, result);
            if (itemLineCur != null)
            {
                result.Add(itemLineCur);
                index++;

                if (itemLineCur.point == result[0].point)
                {
                    result.RemoveAt(result.Count - 1);
                    return result;
                }
                else if (result.Count > 2)
                {
                    if (itemLineCur.point == result[1].point)
                    {
                        List<ItemLineInfo> resultNew = new List<ItemLineInfo>();
                        resultNew.Add(result[0]);
                        return resultNew;
                    }
                }
            }
            else
            {
                List<ItemLineInfo> resultNew = new List<ItemLineInfo>();
                resultNew.Add(result[result.Count-1]);
                return new List<ItemLineInfo>();
            }

        }
       
        result.RemoveAt(result.Count - 1);
        return result;
    }

    private ItemLineInfo FindPointNextValid(ItemLineInfo itemLineInfoCur,List<ItemLineInfo> itemLineInfoListTmp, List<ItemLineInfo> result)
    {
        TypeLineFind typeLineFind = TypeLineFind.horizontal;
        if (itemLineInfoCur.typeLine == TypeLine.line_bot_right || itemLineInfoCur.typeLine == TypeLine.line_top_left)
        {
            typeLineFind = TypeLineFind.vertical;
        }
        if (result.Count > 1)
        {
            TypeLineFind typeLineFindPre = result[result.Count - 2].typeLineFind;
            if (typeLineFindPre == TypeLineFind.horizontal)
            {
                typeLineFind = TypeLineFind.vertical;
            }
            else if (typeLineFindPre == TypeLineFind.vertical)
            {
                typeLineFind = TypeLineFind.horizontal;
            }
        }
        List<ItemLineInfo> list = FindPointsNext(itemLineInfoCur, typeLineFind, itemLineInfoListTmp);
     
        ItemLineInfo itemLineNearst = FindPointNextValidNearst(itemLineInfoCur, list);
        if (itemLineNearst == null) return null;
        return CheckItemSortValid(itemLineNearst, itemLineInfoCur, itemLineInfoListTmp);
    }

    private ItemLineInfo CheckItemSortValid(ItemLineInfo itemLineNearst,ItemLineInfo itemLineInfoCur, List<ItemLineInfo> list)
    {
        //Debug.Log("CheckItemSortValid:"+ itemLineInfoCur.point+" : "+itemLineNearst.point+": "+list.Count);
        if (Mathf.Abs(itemLineInfoCur.point.x - itemLineNearst.point.x) <= Utilities.SAISO)
        {
            for (int i = 0; i < list.Count; i++)
            {
                //Debug.Log(" x :"+ list[i].point + " - "+ itemLineInfoCur.point);
                if (Mathf.Abs(list[i].point.x - itemLineInfoCur.point.x) <= Utilities.SAISO)
                {
                    if (itemLineInfoCur.point.y < itemLineNearst.point.y)
                    {
                        if (list[i].point.y < itemLineNearst.point.y && list[i].point.y > itemLineInfoCur.point.y)
                        {
                            return null;
                        }
                    }
                    else
                    {
                        if (list[i].point.y < itemLineInfoCur.point.y && list[i].point.y > itemLineNearst.point.y)
                        {
                            return null;
                        }
                    }
                }
            }
        }
        else if (Mathf.Abs(itemLineInfoCur.point.y - itemLineNearst.point.y) <= Utilities.SAISO)
        {
            for (int i = 0; i < list.Count; i++)
            {
                //Debug.Log(" y :" + list[i].point + " - " + itemLineInfoCur.point);
                if (Mathf.Abs(list[i].point.y - itemLineInfoCur.point.y) <= Utilities.SAISO)
                {
                    if (itemLineInfoCur.point.x < itemLineNearst.point.x)
                    {
                        if (list[i].point.x < itemLineNearst.point.x && list[i].point.x > itemLineInfoCur.point.x)
                        {
                            return null;
                        }
                    }
                    else
                    {
                        if (list[i].point.x < itemLineInfoCur.point.x && list[i].point.x > itemLineNearst.point.x)
                        {
                            return null;
                        }
                    }
                }
            }
        }
        return itemLineNearst;
    }

    private ItemLineInfo FindPointNextValidNearst(ItemLineInfo itemLineInfoCur, List<ItemLineInfo> itemLineList)
    {
        if (itemLineList.Count == 0) return null;
        if (itemLineList.Count == 1) return itemLineList[0];
        int indexMin = 0;
        float distanceMin = (itemLineList[indexMin].point - itemLineInfoCur.point).magnitude;
        for (int i = 1; i < itemLineList.Count; i++)
        {
            float distanceTmp = (itemLineList[i].point - itemLineInfoCur.point).magnitude;
            //Debug.Log("i:"+i+"=>"+ distanceTmp + "<"+distanceMin+" :: "+itemLineInfoList[i].point+"-"+itemLineInfoCur.point);
            if (distanceTmp < distanceMin)
            {
                indexMin = i;
                distanceMin = distanceTmp;
            }
        }
        return itemLineList[indexMin];
    }
    private List<ItemLineInfo> FindPointsNext(ItemLineInfo itemLine,TypeLineFind typeLineFind,List<ItemLineInfo> list)
    {
        itemLine.typeLineFind = typeLineFind;
        List<ItemLineInfo> result = new List<ItemLineInfo>();
        switch (itemLine.typeLine)
        {
            case TypeLine.line_bot_left:
                // __ 
                //   |
                if (typeLineFind == TypeLineFind.horizontal)
                {
                    for (int i = 0;i< list.Count; i++)
                    {
                        ItemLineInfo itemLineTmp = list[i];
                        //Debug.Log("dasdas:" + i + ":" + itemLineTmp.point + ":" + itemLineTmp.typeLine);
                        if (itemLineTmp.typeLine == TypeLine.line_bot_right || itemLineTmp.typeLine == TypeLine.line_top_right)
                        {
                            if ((Mathf.Abs(itemLineTmp.point.y - itemLine.point.y) <= Utilities.SAISO) && itemLineTmp.point.x < itemLine.point.x )
                                result.Add(itemLineTmp);
                        }
                    }
                }
              
                else
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        ItemLineInfo itemLineTmp = list[i];
                        //Debug.Log("dasdas:" + i + ":" + itemLineTmp.point + ":" + itemLineTmp.typeLine);
                        if (itemLineTmp.typeLine == TypeLine.line_top_right || itemLineTmp.typeLine == TypeLine.line_top_left)
                        {
                            if ((Mathf.Abs(itemLineTmp.point.x - itemLine.point.x) <= Utilities.SAISO) && itemLineTmp.point.y < itemLine.point.y)
                                result.Add(itemLineTmp);
                        }
                    }

                }
                break;
            case TypeLine.line_bot_right:
                //    __
                //   |
                if (typeLineFind == TypeLineFind.horizontal)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        ItemLineInfo itemLineTmp = list[i];
                        //Debug.Log("dasdas:" + i + ":" + itemLineTmp.point + ":" + itemLineTmp.typeLine);
                        if (itemLineTmp.typeLine == TypeLine.line_top_left || itemLineTmp.typeLine == TypeLine.line_bot_left)
                        {
                            if ((Mathf.Abs(itemLineTmp.point.y - itemLine.point.y) <= Utilities.SAISO) && itemLineTmp.point.x > itemLine.point.x)
                                result.Add(itemLineTmp);
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        ItemLineInfo itemLineTmp = list[i];
                        //Debug.Log("dasdas:" + i + ":" + itemLineTmp.point + ":" + itemLineTmp.typeLine);
                        if (itemLineTmp.typeLine == TypeLine.line_top_right || itemLineTmp.typeLine == TypeLine.line_top_left)
                        {
                            if ((Mathf.Abs(itemLineTmp.point.x - itemLine.point.x) <= Utilities.SAISO) && itemLineTmp.point.y < itemLine.point.y)
                                result.Add(itemLineTmp);
                        }
                    }

                }
                break;
            case TypeLine.line_top_right:
                //   | 
                //   ---
                if (typeLineFind == TypeLineFind.horizontal)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        ItemLineInfo itemLineTmp = list[i];
                        //Debug.Log("dasdas:" + i + ":" + itemLineTmp.point + ":" + itemLineTmp.typeLine);
                        if (itemLineTmp.typeLine == TypeLine.line_top_left || itemLineTmp.typeLine == TypeLine.line_bot_left)
                        {
                            if ((Mathf.Abs(itemLineTmp.point.y - itemLine.point.y) <= Utilities.SAISO) && itemLineTmp.point.x > itemLine.point.x)
                                result.Add(itemLineTmp);
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        ItemLineInfo itemLineTmp = list[i];
                        //Debug.Log("dasdas:" + i + ":" + itemLineTmp.point + ":" + itemLineTmp.typeLine);
                        if (itemLineTmp.typeLine == TypeLine.line_bot_left || itemLineTmp.typeLine == TypeLine.line_bot_right)
                        {
                            if ((Mathf.Abs(itemLineTmp.point.x - itemLine.point.x) <= Utilities.SAISO) && itemLineTmp.point.y > itemLine.point.y)
                                result.Add(itemLineTmp);
                        }
                    }

                }
                break;
            case TypeLine.line_top_left:
                //   | 
                // ---
                if (typeLineFind == TypeLineFind.horizontal)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        ItemLineInfo itemLineTmp = list[i];
                        //Debug.Log("dasdas:" + i + ":" + itemLineTmp.point + ":" + itemLineTmp.typeLine);
                        if (itemLineTmp.typeLine == TypeLine.line_top_right || itemLineTmp.typeLine == TypeLine.line_bot_right)
                        {
                            if ((Mathf.Abs(itemLineTmp.point.y - itemLine.point.y) <= Utilities.SAISO) && itemLineTmp.point.x < itemLine.point.x)
                                result.Add(itemLineTmp);
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        ItemLineInfo itemLineTmp = list[i];
                        //Debug.Log("dasdas:" + i + ":" + itemLineTmp.point + ":" + itemLineTmp.typeLine);
                        if (itemLineTmp.typeLine == TypeLine.line_bot_left || itemLineTmp.typeLine == TypeLine.line_bot_right)
                        {
                            if ((Mathf.Abs(itemLineTmp.point.x - itemLine.point.x) <= Utilities.SAISO) && itemLineTmp.point.y > itemLine.point.y)
                                result.Add(itemLineTmp);
                        }
                    }

                }
                break;
        }
        //Debug.Log("FindPointsNext:" + itemLine.point + ":" + itemLine.typeLine + ":" + itemLine.typeLineFind);
        //for (int i = 0; i < result.Count; i++)
        //{
        //    Debug.Log(i + ":" + result[i].point + ":" + result[i].typeLine);
        //}
        return result;
    }

    public Vector2 RoundPosCreateLine(int indexMask,Vector2 posMouse)
    {
        Vector2[] points = pointsList[indexMask];
       
        if (typeLineCur == TypeLineFind.vertical)
        {
            for (int i = 0; i < points.Length; i++)
            {
                Debug.Log("vertical:" + i + ":" + points[i] + ":" + posMouse);
                points[i].x = MathfExtension.FloorToInt(points[i].x);
                if (Mathf.Abs(posMouse.x -  points[i].x) <= sizeLine.x)
                {
                    //Debug.Log("RoundPosCreateLine:"+ new Vector2(points[i].x, posMouse.y));
                    return new Vector2(points[i].x, posMouse.y);
                    
                }
                else if (Mathf.Abs(posMouse.x - points[i].x) <= sizeLine.x * 2)
                {
                    return new Vector2(posMouse.x <= points[i].x ? points[i].x - sizeLine.x : points[i].x + sizeLine.x, posMouse.y);
                }
            }
        }
        else
        {
            for (int i = 0; i < points.Length; i++)
            {
                //Debug.Log("horizontal:" + i + ":" + points[i] + ":" + posMouse);
                if (Mathf.Abs(posMouse.y - points[i].y) <= sizeLine.x)
                {
                    //Debug.Log("RoundPosCreateLine:" + new Vector2(points[i].x, posMouse.y));
                    return new Vector2(posMouse.x, points[i].y);
                }
                else if (Mathf.Abs(posMouse.y - points[i].y) <= sizeLine.x*2)
                {
                    
                    return new Vector2(posMouse.x, posMouse.y <= points[i].y ? points[i].y - sizeLine.x : points[i].y + sizeLine.x);
                }
            }
        }
        //Debug.Log("RoundPosCreateLine khong cham:" + posMouse);
        return posMouse;
    }

}


public class Vector2Angle
{
    public Vector2 vector2;
    public int angle;
    public Vector2Angle(Vector2 vector2)
    {
        this.vector2 = vector2;
        angle = 0;
    }
}