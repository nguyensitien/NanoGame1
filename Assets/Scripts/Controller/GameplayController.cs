using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameplayController : Singleton<GameplayController>
{
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
    private SpriteMask itemMaskScript;
    
    public List<ItemLineInfo> itemLineInfoList;
    private Vector2[] points;
    public Vector2[] GetPointsBoard()
    {
        
        return points;
    }
    private GameObject nodeMap;
    private ItemBall[] arrItemBall;
    private ConditionNode conditionMain;
    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //nodeMap = GameObject.Find("Node1");
        //GameObject mapPrefab = Resources.Load<GameObject>("Maps/Node" + 1);
        //GameObject nodeMap = Instantiate(mapPrefab);
        nodeMap = Instantiate(ObjectDataController.Instance.nodeMapFighting);
        conditionMain = nodeMap.GetComponent<ConditionNode>();
        nodeMap.transform.Find("Tilemap").GetComponent<TilemapRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        arrItemBall = FindObjectsOfType<ItemBall>();
        
        nodeMap.transform.localPosition = Vector3.zero;
        tileMap = nodeMap.transform.GetChild(0).GetComponent<Tilemap>();
        itemMaskScript = itemMask.GetComponent<SpriteMask>();
        itemLineInfoList = new List<ItemLineInfo>();
        foreach (var pos in tileMap.cellBounds.allPositionsWithin)
        {
            Sprite sprite = tileMap.GetSprite(pos);
            
            if (sprite != null)
            {
                Vector2 p = new Vector2(pos.x * +sprite.bounds.size.x + sprite.bounds.size.x / 2, pos.y * sprite.bounds.size.y + sprite.bounds.size.y / 2);
                
                if (sprite.name == TypeLine.line_bot_left.ToString())
                {
                    ItemLineInfo itemLine = new ItemLineInfo(p,TypeLine.line_bot_left);
                    itemLineInfoList.Add(itemLine);
                }
                else if (sprite.name == TypeLine.line_bot_right.ToString())
                {
                    ItemLineInfo itemLine = new ItemLineInfo(p, TypeLine.line_bot_right);
                    itemLineInfoList.Add(itemLine);
                }
                else if (sprite.name == TypeLine.line_top_left.ToString())
                {
                    ItemLineInfo itemLine = new ItemLineInfo(p, TypeLine.line_top_left);
                    itemLineInfoList.Add(itemLine);
                }
                else if (sprite.name == TypeLine.line_top_right.ToString())
                {
                    ItemLineInfo itemLine = new ItemLineInfo(p, TypeLine.line_top_right);
                    itemLineInfoList.Add(itemLine);
                }
            }
        }
       
        itemLineInfoList = SortPoints(itemLineInfoList);
       
        Init();
        
    }

    private void InitPoints()
    {
        int length = itemLineInfoList.Count;
        points = new Vector2[length];
        for (int i = 0; i < length; i++)
        {
            points[i] = itemLineInfoList[i].point;
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

    
    public void Init()
    {
        isEndGame = false;
        canCreateLine = false;
        typeLineCur = TypeLineFind.vertical;
        ShowTypeLinePreview();
        InitPoints();
        points = CreatePointsBorder(points);
        triangle = new Triangulator(points);
        bgBoard.localPosition = new Vector2(triangle.centerX,triangle.centerY);
        bgBoard.localScale = new Vector2(triangle.width/100,triangle.height/100);
        colors = new Color[triangle.width*triangle.height];
        //InitTriangle();
        originSizeBoard = (int)triangle.Area();
        pointCur = 0;
        pointTarget = (int)(originSizeBoard*conditionMain.percentWin);
        InitItemMask();
        
        

    }
    private Triangulator triangle;
    public SpriteRenderer spriteRenderern;
    Color[] colors;
    private Sprite spriteMask;
    private int minXOri, minYOri;
    private void InitTriangle()
    {

        InitPoints();
        Vector2[] pointsBorder = CreatePointsBorder(points);
        triangle = new Triangulator(pointsBorder);

        //test

        int minX = (int)triangle.minX;

        int minY = (int)triangle.minY;
        if (spriteMask == null)
        {
            int maxX = (int)triangle.maxX;
            minXOri = (int)triangle.minX;
            minYOri = (int)triangle.minY;
            int maxY = (int)triangle.maxY;
            int width = (int)(maxX - minX);
            int height = (int)(maxY - minY);
            Vector2 center = new Vector2((maxX + minX + sizeLine.x / 2) / 2.0f, (maxY + minY + sizeLine.x / 2) / 2.0f);
            Texture2D tex2D = new Texture2D(width + (int)sizeLine.x / 2, height + (int)sizeLine.x / 2);
            spriteMask = Sprite.Create(tex2D, new Rect(0, 0, width + sizeLine.x / 2, height + sizeLine.x / 2), Vector2.one * 0.5f, 100, 0, SpriteMeshType.Tight);
            itemMaskScript.transform.position = center;
        }

        //Debug.Log("width:"+width+" height:"+height);
        //Texture2D tex2D = new Texture2D(width + (int)sizeLine.x / 2, height + (int)sizeLine.x / 2);
        //Sprite sprite = Sprite.Create(tex2D, new Rect(0, 0, width+sizeLine.x / 2, height + sizeLine.x/2), Vector2.one * 0.5f,100,0,SpriteMeshType.Tight);
        ushort[] tris = new ushort[triangle.Triangulate().Length];
        int[] trisss = triangle.Triangulate();
        
        //Vector2[] pointsBorder = points ;
        Vector2[] verticles = new Vector2[pointsBorder.Length];
        Vector2 ori = new Vector2(minXOri, minYOri);
        //Debug.Log("verticles---------------------:"+points.Length+" width:"+width+" height:"+height+"minX:"+minX+" maxX:"+maxX+" minY:"+minY+" maxY:"+maxY);
        for (int i = 0; i < pointsBorder.Length; i++)
        {
            //Debug.Log("pointBorder:"+i+":"+pointsBorder[i]+" - "+ori);
            verticles[i] = (pointsBorder[i] - ori);
        }
        //Debug.Log("triangles------------------------:"+tris.Length);
        for (int i = 0; i < tris.Length; i++)
        {
            tris[i] = (ushort)(trisss[i]);
            //Debug.Log(i + ":" + tris[i]);
        }
        //for (int i = 0; i < tris.Length; i += 3)
        //{
        //    //Debug.Log("draw:" + i);
        //    Debug.DrawLine(pointsBorder[tris[i]], pointsBorder[tris[i + 1]], Color.red, 10);
        //    Debug.DrawLine(pointsBorder[tris[i + 1]], pointsBorder[tris[i + 2]], Color.red, 10);
        //    Debug.DrawLine(pointsBorder[tris[i + 2]], pointsBorder[tris[i]], Color.red, 10);
        //}
        //Debug.Log("verticles:" + verticles.Length+ " tris" + tris.Length+" minX:"+minX+" minY:"+minY+" width:"+width+" height:"+height);
        spriteMask.OverrideGeometry(verticles, tris);
        //sprite.OverrideGeometry(sprite.vertices, sprite.triangles);
        itemMaskScript.sprite = spriteMask;
        //itemMaskScript.transform.position = center;
        //spriteRenderern.sprite = sprite;
        //spriteRenderern.transform.position = center;
    }

    private Vector2[] CreatePointsBorder(Vector2[] pointsTmp)
    {
        Vector2[] result = new Vector2[pointsTmp.Length];
        for (int i = 0; i < result.Length; i++)
        {
            Vector2 pCenter = pointsTmp[i];
            Vector2 pTopRight = new Vector2(pCenter.x+ (sizeLine.x / 2), pCenter.y +  (sizeLine.x / 2));
            Vector2 pTopLeft = new Vector2(pCenter.x- (sizeLine.x / 2), pCenter.y + (sizeLine.x / 2));
            Vector2 pBotLeft = new Vector2(pCenter.x-  (sizeLine.x / 2), pCenter.y - (sizeLine.x / 2));
            Vector2 pBotRight = new Vector2(pCenter.x+  (sizeLine.x / 2), pCenter.y - (sizeLine.x / 2));
            List<Vector2> pointsOut = new List<Vector2>();
            List<Vector2> pointsIn = new List<Vector2>();
            if (Utilities.IsPointInPolygon(pTopRight, pointsTmp)) pointsIn.Add(pTopRight);
            else pointsOut.Add(pTopRight);

            if (Utilities.IsPointInPolygon(pTopLeft, pointsTmp)) pointsIn.Add(pTopLeft);
            else pointsOut.Add(pTopLeft);

            if (Utilities.IsPointInPolygon(pBotLeft, pointsTmp)) pointsIn.Add(pBotLeft);
            else pointsOut.Add(pBotLeft);

            if (Utilities.IsPointInPolygon(pBotRight, pointsTmp)) pointsIn.Add(pBotRight);
            else pointsOut.Add(pBotRight);

            if (pointsIn.Count == 1)
            {
                result[i] = pCenter + (pCenter - pointsIn[0]) * 1;
            }
            else if (pointsOut.Count == 1)
            {
                result[i] = pointsOut[0];
            }
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

    public void ContinueGame()
    {
        Debug.Log("ContinueGame");
        SceneManager.LoadScene("Lobby");
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
        //if (CorouUpdateBall != null)
        //{
        //    StopCoroutine(CorouUpdateBall);
        //}
        //MainThreadDispatcher.(UpdateBallEnumator);
        Debug.Log("EndGame:" + isWin);
        HideBoardGame();
        if (isWin)
        {
            DataController.Instance.UserDataNodeList[ObjectDataController.Instance.idNodeFighting - 1].numStar = 3;
            if (DataController.Instance.UserData.idNodeHighest == ObjectDataController.Instance.idNodeFighting)
            {
                DataController.Instance.UserData.idNodeHighest += 1;
            }

        }
        isEndGame = true;
        UIGameplayController.Instance.EndGame(isWin);
    }

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
        UpdateBallEnumator = UpdateBall();
        Observable.FromMicroCoroutine(UpdateBall).Subscribe().AddTo(this);
        //MainThreadDispatcher.StartUpdateMicroCoroutine(UpdateBallEnumator);
        //Observable.FromMicroCoroutine(UpdateBall);

    }

    private Coroutine CorouUpdateBall;


    private void InitItemMask()
    {
        pointsComplete = new List<Vector2>();
        InitTriangle();
        pointCur = originSizeBoard - (int)triangle.Area();
        UIGameplayController.Instance.UpdatePoint();
        if (pointCur >= pointTarget)
        {
            EndGame(true);
        }

    }

    public Triangulator GetBoard()
    {
        return triangle;
    }

    public void CreateLine(Vector3 posCreate)
    {
        canCreateLine = false;
        for (int i = 0; i < 2; i++)
        {
            GameObject obj = Instantiate(linePrefab[(int)typeLineCur], boardTran);
            ItemLine itemLine = obj.GetComponent<ItemLine>();
            obj.transform.position = posCreate;
            obj.transform.localScale = Vector3.zero;
            itemLine.Init(typeLineCur,i, false);
        }
        
    }

    public void ChangeTypeSketching()
    {
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
                            //Debug.Log("add new:" + i + " dir:" + itemLineSketchingList[i].dir + " point:" + pointsComplete[i]+":"+ TypeLine.line_bot_right);
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
                            //Debug.Log("add new:" + i + " dir:" + itemLineSketchingList[i].dir + " point:" + pointsComplete[i] + ":" + TypeLine.line_top_right);
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
                       // Debug.Log("add new:" + (typeAdd - 1) + " dir:" + itemLineSketchingList[typeAdd - 1].dir + " point:" + pointsComplete[typeAdd - 1] + ":" + TypeLine.line_bot_right);
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
            
            for(int i = 0; i < itemLineSketchingList.Count; i++)
            {
                itemLineSketchingList[i].isCompleteSketching = true;

            }
            canCreateLine = true;
            for (int i = 0; i < 2; i++)
            {

                for (int typeAdd = 0; typeAdd < 4; typeAdd++)
                {
                    //Debug.Log("Process :"+i+"-"+typeAdd);
                    List<ItemLineInfo> itemLineInfoListTmp = new List<ItemLineInfo>(itemLineInfoList);
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
                        if (itemLineInfoListTmp.Count >= 4 && Utilities.IsPointInPolygon(arrItemBall[0].transform.position, pointsTmp))
                        {
                            CompleteReCreateBoard(itemLineInfoListTmp);
                            return;
                        }
                        else
                        {
                            //Debug.Log("ball khong nam trong points:" + itemLineInfoListTmp.Count);
                            for (int j = 0; j < itemLineInfoListTmp.Count; j++)
                            {
                                itemLineInfoList.Remove(itemLineInfoListTmp[j]);
                            }
                            //itemLineInfoList.Remove(itemLineInfoListTmp[itemLineInfoListTmp.Count - 1]);
                            //Debug.Log("after remove:" + itemLineInfoList.Count);
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
                        itemLineInfoListTmp = SortPoints(itemLineInfoListTmp);

                        Vector2[] pointsTmp = ParsePointFromItemInfo(itemLineInfoListTmp);
                        if (itemLineInfoListTmp.Count >= 4 && Utilities.IsPointInPolygon(arrItemBall[0].transform.position, pointsTmp))
                        {
                            CompleteReCreateBoard(itemLineInfoListTmp);
                            return;
                        }
                        else
                        {
                            //Debug.Log("ball khong nam trong points:" + itemLineInfoListTmp.Count);
                            for (int j = 0; j < itemLineInfoListTmp.Count; j++)
                            {
                                itemLineInfoList.Remove(itemLineInfoListTmp[j]);
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
            
            
        }
        
    }

    private void CompleteReCreateBoard(List<ItemLineInfo> itemLineInfoListTmp)
    {
        //Debug.Log("nam trong ne");
        itemLineInfoList = new List<ItemLineInfo>(itemLineInfoListTmp);
        //for (int j = 0; j < itemLineInfoList.Count; j++)
        //{
        //    Debug.Log(j + ":" + itemLineInfoList[j].point + ":" + itemLineInfoList[j].typeLine);
        //}
        InitItemMask();
        ChangeTypeSketching();
        itemLineSketchingList = new List<ItemLine>();
        pointsComplete = new List<Vector2>();
        CheckConditionEndGame();
        return;
    }

    private void CheckConditionEndGame()
    {
        for (int i = 0; i < arrItemBall.Length; i++)
        {
            if (Utilities.IsPointInPolygon(arrItemBall[i].transform.position, points) == false)
            {
                EndGame(false);
            }
        }
    }

    public List<ItemLineInfo> SortPoints(List<ItemLineInfo> itemLineInfoListTmp)
    {
        //Debug.Log("SortPoints:"+itemLineInfoListTmp.Count);
        //for (int i = 0; i < itemLineInfoListTmp.Count; i++)
        //{
        //    Debug.Log(i + ":" + itemLineInfoListTmp[i].point + ":" + itemLineInfoListTmp[i].typeLine);
        //}
        if (itemLineInfoListTmp.Count <= 0) return new List<ItemLineInfo>();
        List<ItemLineInfo> result = new List<ItemLineInfo>();
        ItemLineInfo itemLineCur = itemLineInfoListTmp[0];
        result.Add(itemLineCur);
        //itemLineList.RemoveAt(0);
        int index = 0;
        while (index <= 100)
        {
            itemLineCur = FindPointNextValid(itemLineCur, itemLineInfoListTmp, result);
            //Debug.Log("result:"+result.Count+":"+itemLineCur);
            if (itemLineCur != null)
            {
                result.Add(itemLineCur);
                index++;
                //itemLineList.Remove(itemLineCur);

                if (itemLineCur.point == result[0].point)
                {
                    //Debug.Log("end o day ha 1");
                    result.RemoveAt(result.Count - 1);
                    return result;
                }
                else if (result.Count > 2)
                {
                    if (itemLineCur.point == result[1].point)
                    {
                        //Debug.Log("end o day ha 3");
                        List<ItemLineInfo> resultNew = new List<ItemLineInfo>();
                        resultNew.Add(result[0]);
                        return resultNew;
                    }
                }
            }
            else
            {
                //Debug.Log("end o day ha");
                //if (result.Count > 4)
                //{
                //    result.RemoveAt(result.Count - 1);
                //}
                //result.RemoveAt(result.Count - 1);
                List<ItemLineInfo> resultNew = new List<ItemLineInfo>();
                resultNew.Add(result[result.Count-1]);
                return new List<ItemLineInfo>();
            }
            //return null;

        }
        //for (int i = 0; i < result.Count; i++)
        //{
        //    Debug.Log(result[i].point + ":" + result[i].typeLine);
        //}
        //Debug.Log("end o day ha 2");
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
        //Debug.Log("FindPointsNext:" + itemLineInfoCur.point + ":" + itemLineInfoCur.typeLine + ":" + itemLineInfoListTmp.Count + ":" + typeLineFind);
        List<ItemLineInfo> list = FindPointsNext(itemLineInfoCur, typeLineFind, itemLineInfoListTmp);
        //Debug.Log("list:" + list.Count);
        //for (int i = 0; i < list.Count; i++)
        //{
        //    Debug.Log("point:"+i+":"+list[i].point+":"+list[i].typeLine+":"+ itemLineInfoCur.point);
        //}
        return FindPointNextValidNearst(itemLineInfoCur, list);
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
                            //Debug.Log("vao day chu ha:" + (int)itemLineTmp.point.y + "==" + (int)itemLine.point.y + " && " + itemLineTmp.point.x + "<" + itemLine.point.x);
                            if ((Mathf.Abs(itemLineTmp.point.y - itemLine.point.y) <= 1) && itemLineTmp.point.x < itemLine.point.x )
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
                            if ((Mathf.Abs(itemLineTmp.point.x - itemLine.point.x) <= 1) && itemLineTmp.point.y < itemLine.point.y)
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
                            if ((Mathf.Abs(itemLineTmp.point.y - itemLine.point.y) <= 1) && itemLineTmp.point.x > itemLine.point.x)
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
                            if ((Mathf.Abs(itemLineTmp.point.x - itemLine.point.x) <= 1) && itemLineTmp.point.y < itemLine.point.y)
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
                            if ((Mathf.Abs(itemLineTmp.point.y - itemLine.point.y) <= 1) && itemLineTmp.point.x > itemLine.point.x)
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
                            if ((Mathf.Abs(itemLineTmp.point.x - itemLine.point.x) <= 1) && itemLineTmp.point.y > itemLine.point.y)
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
                            if ((Mathf.Abs(itemLineTmp.point.y - itemLine.point.y) <= 1) && itemLineTmp.point.x < itemLine.point.x)
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
                            //Debug.Log("vao day chu ha:" + itemLineTmp.point.x + "==" + itemLine.point.x + " && " + itemLineTmp.point.y + ">" + itemLine.point.y);
                            if ((Mathf.Abs(itemLineTmp.point.x - itemLine.point.x) <= 1) && itemLineTmp.point.y > itemLine.point.y)
                                result.Add(itemLineTmp);
                        }
                    }

                }
                break;
        }
        //Debug.Log("FindPointsNext:"+itemLine.point+":"+itemLine.typeLine+":"+itemLine.typeLineFind);
        //for (int i = 0; i < result.Count; i++)
        //{
        //    Debug.Log(i+":"+result[i].point+":"+result[i].typeLine);
        //}
        return result;
    }

    public Vector2 RoundPosCreateLine(Vector2 posMouse)
    {
        if (typeLineCur == TypeLineFind.vertical)
        {
            for (int i = 0; i < points.Length; i++)
            {
                //Debug.Log("vertical:"+i+":"+points[i]+":"+posMouse);
                if (Mathf.Abs(posMouse.x - points[i].x) <= sizeLine.x)
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