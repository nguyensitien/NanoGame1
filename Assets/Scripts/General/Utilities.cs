using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utilities 
{
    public static bool IS_DEBUG = true;
    public static float SIZE_WIDTH = 1080;
    public static float SIZE_HEIGHT = 1920;
    public static float SIZE = 48;
    public static float SAISO = 2;
    public static float GetRatioDevice()
    {
        return (float)Screen.height / (float)Screen.width;
    }

    public static float ConvertToLocal(float value)
    {
        return value;
    }

    public static bool CheckOverrideRectPoint(Rect rect, Vector2 p)
    {
        return p.x >= rect.x - rect.width / 2 &&
            p.x <= rect.x + rect.width / 2 &&
            p.y >= rect.y - rect.height / 2 &&
            p.y <= rect.y + rect.height / 2;
    }
    public static double GetScreenDimension()
    {
        return Math.Truncate(((float)Screen.height / Screen.width) * 100.0) / 100.0;
    }
    public static List<Vector2Angle> ArrangePoints(List<Vector2> pts)
    {
        List<Vector2Angle> points = new List<Vector2Angle>();
        for (int i = 0; i < pts.Count; i++)
        {
            points.Add(new Vector2Angle(pts[i]));
        }
        var minX = points[0].vector2.x;
        var maxX = points[0].vector2.x;
        var minY = points[0].vector2.y;
        var maxY = points[0].vector2.y;

        for (var i = 1; i < points.Count; i++)
        {
            if (points[i].vector2.x < minX) minX = points[i].vector2.x;
            if (points[i].vector2.x > maxX) maxX = points[i].vector2.x;
            if (points[i].vector2.y < minY) minY = points[i].vector2.y;
            if (points[i].vector2.y > maxY) maxY = points[i].vector2.y;
        }


        // choose a "central" point
        var center = new Vector2(minX + (maxX - minX) / 2,minY + (maxY - minY) / 2);
        Debug.Log("center:"+center);
        // precalculate the angles of each point to avoid multiple calculations on sort
        for (var i = 0; i<points.Count; i++) {
            points[i].angle = (int)(Mathf.Acos((points[i].vector2.x - center.x) / lineDistance(center, points[i].vector2))*Mathf.Rad2Deg);
            Debug.Log(i+":"+points[i].vector2+":"+points[i].angle);
            if (points[i].vector2.y > center.y) {
                points[i].angle = (int)(Mathf.Rad2Deg*Mathf.PI + Mathf.PI *Mathf.Rad2Deg - points[i].angle);
                Debug.Log("vao day roi:"+i+":"+points[i].vector2 + ":"+points[i].angle);
            }
        }

        // sort by angle
       points.Sort((a, b) => {
            return (int)(a.angle - b.angle);
        });

        Debug.Log("sau khi sort------------");
        for (int i = 0; i < points.Count; i++)
        {
            Debug.Log(i+":"+points[i].vector2+":"+points[i].angle);
        }
        return points;
    
    }
    static float lineDistance(Vector2 point1, Vector2 point2)
    {
        var xs = 0.0;
        var ys = 0.0;

        xs = point2.x - point1.x;
        xs = xs * xs;

        ys = point2.y - point1.y;
        ys = ys * ys;

        return Mathf.Sqrt((float)xs + (float)ys);
    }
    static float GetDeterminant(float x1, float y1, float x2, float y2)
    {
        //Debug.Log("GetDeterminant:" + x1 + "," + y1 + "->" + x2 + "," + y2);
        return x1 * y2 - x2 * y1;
    }

    public static float GetArea(IList<Vector2> vertices)
    {
        if (vertices.Count < 3)
        {
            return 0;
        }
        float area = GetDeterminant(vertices[vertices.Count - 1].x, vertices[vertices.Count - 1].y, vertices[0].x, vertices[0].y);
        for (int i = 1; i < vertices.Count; i++)
        {
            area += GetDeterminant(vertices[i - 1].x, vertices[i - 1].y, vertices[i].x, vertices[i].y);
        }
        return Mathf.Abs(area) / 2;
    }

    public static float GetAreaPolygon(List<Vector2> points)
    {
        points.Add(points[0]);
        var area = Mathf.Abs(points.Take(points.Count - 1)
           .Select((p, i) => (points[i + 1].x - p.x) * (points[i + 1].y + p.y))
           .Sum() / 2);

        return area;
    }

    public static bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
    {
        int polygonLength = polygon.Length, i = 0;
        bool inside = false;
        // x, y for tested point.
        float pointX = point.x, pointY = point.y;
        // start / end point for the current polygon segment.
        float startX, startY, endX, endY;
        Vector2 endPoint = polygon[polygonLength - 1];
        endX = endPoint.x;
        endY = endPoint.y;
        while (i < polygonLength)
        {
            startX = endX; startY = endY;
            endPoint = polygon[i++];
            endX = endPoint.x; endY = endPoint.y;
            //
            inside ^= (endY > pointY ^ startY > pointY) /* ? pointY inside [startY;endY] segment ? */
                      && /* if so, test if it is under the segment */
                      ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
        }
        return inside;
    }

    public static float GetVelcoityAfterCollide(float vA,float vB,float mA,float mB)
    {
        return ((mA - mB) / (mA + mB)) * vA + ((2*mB/(mA+mB)))*vB;
    }

}
