using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameplayController : Singleton<GameplayController>
{
    public Vector2 sizeLine;
    [SerializeField]
    private Vector2 sizeBoard;
    [SerializeField]
    private ItemBall itemBall = null;
    [SerializeField]
    private Transform boardTran = null;
    [SerializeField]
    private Transform itemMask = null;
    [SerializeField]
    private TypeLineFind typeLineCur;
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

    private int originSizeBoard;
    public TilemapCollider2D tileHit;
    public Tilemap tileMap;
    private SpriteMask itemMaskScript;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(tileHit.bounds.center,Vector2.one*10);
    }
    public List<ItemLineInfo> itemLineInfoList;
    private Vector2[] points;
    public Vector2[] GetPointsBoard()
    {
        
        return points;
    }
    private void Start()
    {
       
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
        ItemLineInfo item = itemLineInfoList[4];
        itemLineInfoList.RemoveAt(4);
        itemLineInfoList.Add(item);
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
    public void Init()
    {
        isEndGame = false;
        canCreateLine = false;
        typeLineCur = TypeLineFind.vertical;
        ShowTypeLinePreview();
        InitPoints();
        triangle = new Triangulator(points);
        colors = new Color[triangle.width*triangle.height];
        //InitTriangle();
        originSizeBoard = (int)triangle.Area();
        pointCur = 0;
        pointTarget = originSizeBoard / 2;
        InitItemMask();
        
        

    }
    private Triangulator triangle;
    public SpriteRenderer spriteRenderern;
    Color[] colors;
    private void InitTriangle()
    {

        InitPoints();
        triangle = new Triangulator(points);

        //test
        
        int minX = (int)triangle.minX;
        int maxX = (int)triangle.maxX;
        int minY = (int)triangle.minY;
        int maxY = (int)triangle.maxY;
        int width = (int)(maxX - minX) ;
        int height = (int)(maxY - minY);
        Vector2 center = new Vector2((maxX + minX) / 2, (maxY + minY) / 2);
        Texture2D tex2D = new Texture2D(width,height);
        //Vector2 vec = Vector2.zero;
        //Color color = Color.white;
        //for (int y = 0; y < height; y++)
        //{
        //    for (int x = 0; x < width; x++)
        //    {
        //        vec.x = x + minX;
        //        vec.y = y + minY;
        //        if (Utilities.IsPointInPolygon(vec, points))
        //        {
        //            color.a = 1;
        //        }
        //        else
        //        {
        //            color.a = 0;
        //        }
        //        colors[y * width + x] = color;
        //    }

        //}
        //tex2D.SetPixels(0, 0, width, height, colors);
        Sprite sprite = Sprite.Create(tex2D, new Rect(0, 0, width, height), Vector2.one * 0.5f);
        ushort[] tris = new ushort[triangle.Triangulate().Length];
        int[] trisss = triangle.Triangulate();
        Vector2[] verticles = new Vector2[points.Length];
        Vector2 ori = new Vector2(minX, minY);
        //Debug.Log("verticles---------------------:"+points.Length+" width:"+width+" height:"+height+"minX:"+minX+" maxX:"+maxX+" minY:"+minY+" maxY:"+maxY);
        for (int i = 0; i < points.Length; i++)
        {
            verticles[i] = (points[i] - ori);
            //Debug.Log(i + ":" + verticles[i]+":"+points[i]);
        }
        //Debug.Log("triangles------------------------:"+tris.Length);
        for (int i = 0; i < tris.Length; i++)
        {
            tris[i] = (ushort)(trisss[i]);
            //Debug.Log(i + ":" + tris[i]);
        }
        for (int i = 0; i < tris.Length; i += 3)
        {
            //Debug.Log("draw:" + i);
            Debug.DrawLine(points[tris[i]],points[tris[i +1]], Color.red,10);
            Debug.DrawLine(points[tris[i +1]],points[tris[i +2]],Color.red,10);
            Debug.DrawLine(points[tris[i +2]], points[tris[i]], Color.red,10);
        }
        //Debug.Log("verticles:" + verticles.Length+ " tris" + tris.Length+" minX:"+minX+" minY:"+minY+" width:"+width+" height:"+height);
        sprite.OverrideGeometry(verticles, tris);
        //sprite.OverrideGeometry(sprite.vertices, sprite.triangles);
        itemMaskScript.sprite = sprite;
        itemMaskScript.transform.position = center;
        //spriteRenderern.sprite = sprite;
        //spriteRenderern.transform.position = center;
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(points[i], Vector2.one * 10);
        }
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
        //itemBall.Init();

    }
    

    private void InitItemMask()
    {
        pointsComplete = new List<Vector2>();
        InitTriangle();
        pointCur = originSizeBoard - (int)triangle.Area();
        //Debug.Log("pointTarget:" + pointTarget + " pointCur:" + pointCur + " Arena:" + triangle.Area());
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
        ShowTypeLinePreview();
    }

    public void ChangeTypeSketching()
    {
        int curTmp = (int)typeLineCur + 1;
        curTmp = (int)Mathf.Repeat(curTmp, Enum.GetValues(typeLineCur.GetType()).Length);
        typeLineCur = (TypeLineFind)curTmp;
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
    private void AddNewPointBoard(TypeRemoveLine typeRemove)
    {
        //Debug.Log("AddNewPointBoard:"+typeRemove+":"+itemLineSketchingList.Count+":"+ itemLineInfoList.Count);
        switch (typeRemove)
        {
            case TypeRemoveLine.Bot:
                for(int i=0;i<itemLineSketchingList.Count;i++)
                {
                    if (itemLineSketchingList[i].dir == -1)
                    {
                        itemLineInfoList.Insert(0,new ItemLineInfo(pointsComplete[i],TypeLine.line_top_right));
                    }
                    else
                    {
                        itemLineInfoList.Insert(0, new ItemLineInfo(pointsComplete[i], TypeLine.line_top_left));
                    }
                }
                break;
            case TypeRemoveLine.Top:
                for (int i = 0; i < itemLineSketchingList.Count; i++)
                {
                    if (itemLineSketchingList[i].dir == -1)
                    {
                        itemLineInfoList.Insert(0, new ItemLineInfo(pointsComplete[i], TypeLine.line_bot_right));
                    }
                    else
                    {
                        itemLineInfoList.Insert(0, new ItemLineInfo(pointsComplete[i], TypeLine.line_bot_left));
                    }
                }
                break;
            case TypeRemoveLine.Left:
                for (int i = 0; i < itemLineSketchingList.Count; i++)
                {
                    if (itemLineSketchingList[i].dir == -1)
                    {
                        itemLineInfoList.Insert(0, new ItemLineInfo(pointsComplete[i], TypeLine.line_top_right));
                    }
                    else
                    {
                        itemLineInfoList.Insert(0, new ItemLineInfo(pointsComplete[i], TypeLine.line_bot_right));
                    }
                }
                break;
            case TypeRemoveLine.Right:
                for (int i = 0; i < itemLineSketchingList.Count; i++)
                {
                    if (itemLineSketchingList[i].dir == -1)
                    {
                        itemLineInfoList.Insert(0, new ItemLineInfo(pointsComplete[i], TypeLine.line_top_left));
                    }
                    else
                    {
                        itemLineInfoList.Insert(0, new ItemLineInfo(pointsComplete[i], TypeLine.line_bot_left));
                    }
                }
                break;
        }
        itemLineSketchingList = new List<ItemLine>();
        pointsComplete = new List<Vector2>();
    }
    private List<Vector2> pointsComplete = new List<Vector2>();
    private List<ItemLine> itemLineSketchingList = new List<ItemLine>();
    public void CompleteSketching(Vector2 pos,ItemLine itemlineSketching)
    {
        //Debug.Log("CompleteSketching:"+pos);
        pointsComplete.Add(pos);
        itemLineSketchingList.Add(itemlineSketching);
        if (pointsComplete.Count == 2)
        {
            canCreateLine = true;
            if (typeLineCur.Equals(TypeLineFind.horizontal))
            {
                if (pointsComplete[0].y >= itemBall.transform.position.y)
                {
                    //cat ben tren
                    //Debug.Log("cat ben tren");
                    AddNewPointBoard(TypeRemoveLine.Top);
                    //RemovePointsWhenComplete(TypeRemoveLine.Top);
                }
                else
                {
                    //cat ben bot
                    //Debug.Log("cat ben bot");
                    AddNewPointBoard(TypeRemoveLine.Bot);
                    //RemovePointsWhenComplete(TypeRemoveLine.Bot);
                }
            }
            else
            {
                if (pointsComplete[0].x >= itemBall.transform.position.x)
                {
                    //cat ben phai
                    //Debug.Log("cat ben phai");
                    AddNewPointBoard(TypeRemoveLine.Right);
                    //RemovePointsWhenComplete(TypeRemoveLine.Right);
                }
                else
                {
                    //cat ben trai
                    //Debug.Log("cat ben trai");
                    AddNewPointBoard(TypeRemoveLine.Left);
                    //RemovePointsWhenComplete(TypeRemoveLine.Left);
                }
            }
            itemLineInfoList = SortPoints(itemLineInfoList);
            //itemLineInfoList.RemoveAt(itemLineInfoList.Count-1);
            //Debug.Log("----After Sort-----");
            //for (int i = 0; i < itemLineInfoList.Count; i++)
            //{
            //    Debug.Log(i + ":" + itemLineInfoList[i].point + ":" + itemLineInfoList[i].typeLine);
            //}
            InitItemMask();
            ChangeTypeSketching();
        }
        
    }

    private void RemovePointsWhenComplete(TypeRemoveLine typeRemove)
    {
        switch (typeRemove)
        {
            case TypeRemoveLine.Bot:
                
                break;
            case TypeRemoveLine.Top:

                break;
            case TypeRemoveLine.Left:
                itemLineInfoList = SortPoints(itemLineInfoList);
                break;
            case TypeRemoveLine.Right:
                
                itemLineInfoList = SortPoints(itemLineInfoList);

                break;
        }
    }
    public List<ItemLineInfo> SortPoints(List<ItemLineInfo> itemLineList)
    {
        //for (int i = 0; i < itemLineList.Count; i++)
        //{
        //    Debug.Log(i+":"+itemLineList[i].point + ":" + itemLineList[i].typeLine);
        //}
        if (itemLineList.Count <= 0) return new List<ItemLineInfo>();
        List<ItemLineInfo> result = new List<ItemLineInfo>();
        ItemLineInfo itemLineCur = itemLineList[0];
        result.Add(itemLineCur);
        //itemLineList.RemoveAt(0);
        int index = 0;
        while (index <= 30)
        {
            itemLineCur = FindPointNextValid(itemLineCur,itemLineList, result);
            //Debug.Log("result:"+result.Count+":"+itemLineCur);
            if (itemLineCur != null)
            {
                result.Add(itemLineCur);
                index++;
                //itemLineList.Remove(itemLineCur);
                if (itemLineCur.point == result[0].point) {
                    //Debug.Log("end o day ha 1");
                    result.RemoveAt(result.Count - 1);
                    return result;
                }
            }
            else
            {
                //Debug.Log("end o day ha");
                result.RemoveAt(result.Count - 1);
                return result;
            }
            //return null;

        }
        //for (int i = 0; i < result.Count; i++)
        //{
        //    Debug.Log(result[i].point+":"+result[i].typeLine);
        //}
        //Debug.Log("end o day ha 2");
        result.RemoveAt(result.Count - 1);
        return result;
    }

    private ItemLineInfo FindPointNextValid(ItemLineInfo itemLineInfoCur,List<ItemLineInfo> itemLineList,List<ItemLineInfo> result)
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
        //Debug.Log("FindPointsNext:" + itemLineInfoCur.point + ":" + itemLineInfoCur.typeLine + ":" + itemLineList.Count + ":" + typeLineFind);
        List<ItemLineInfo> list = FindPointsNext(itemLineInfoCur, typeLineFind,itemLineList);
        //Debug.Log("list:" + list.Count);
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
            if (distanceTmp < distanceMin)
            {
                indexMin = i;
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