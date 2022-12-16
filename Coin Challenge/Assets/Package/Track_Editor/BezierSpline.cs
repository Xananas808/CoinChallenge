/*Property of Mining Studio (All Rights Reserved)*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[System.Serializable]
public class BezierSpline
{
    public BezierSpline() { }

    public BezierSpline(Vector3[] _wayPoints)
    {
        SetCtrlPoints(_wayPoints);
    }

    public enum BezierControlPointMode
    {
        Free,
        Aligned,
        Mirrored
    }

    public float _linearLengh;

    [SerializeField]
    public Vector3[] points;

    public int ControlPointCount
    {
        get
        {
            return points.Length;
        }
    }

    public int CurveCount
    {
        get
        {
            return (points.Length - 1) / 3;
        }
    }

    public static Vector3 GetPoint(Vector3[] pts, float t)
    {
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        return pts[0] * (omt2 * omt) +
                pts[1] * (3f * omt2 * t) +
                pts[2] * (3f * omt * t2) +
                pts[3] * (t2 * t);
    }

    public Vector3 GetPoint(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }

        return Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t);
    }

    public Vector3 GetVelocity(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t);
    }

    public Vector3 GetDirection(float t)
    {
        return GetVelocity(t).normalized;
    }

    public Vector3 GetNormal(float t)
    {
        return Vector3.Cross(GetDirection(t), Vector3.up);
    }

    public Vector3 GetUpDir(float t)
    {
        return Vector3.Cross(GetDirection(t), Vector3.left);
    }

    public void AddCurve(Vector3 _P0, Vector3 _P1, Vector3 _P2)
    {
        Array.Resize(ref points, points.Length + 3);
        points[points.Length - 3] = _P0;
        points[points.Length - 2] = _P1;
        points[points.Length - 1] = _P2;

        if (points.Length - 4 >= 0) EnforceMode(points.Length - 4);

    }

    public void EnforceMode(int index)
    {
        int modeIndex = (index + 1) / 3;


        int middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;

        if (index <= middleIndex)
        {
            fixedIndex = middleIndex - 1;
            if (fixedIndex < 0)
            {
                fixedIndex = points.Length - 2;
            }
            enforcedIndex = middleIndex + 1;
            if (enforcedIndex >= points.Length)
            {
                enforcedIndex = 1;
            }
        }
        else
        {
            fixedIndex = middleIndex + 1;
            if (fixedIndex >= points.Length)
            {
                fixedIndex = 1;
            }
            enforcedIndex = middleIndex - 1;
            if (enforcedIndex < 0)
            {
                enforcedIndex = points.Length - 2;
            }
        }

        Vector3 middle = points[middleIndex];
        Vector3 enforcedTangent = middle - points[fixedIndex];

        if (enforcedIndex == 1) return;
        points[enforcedIndex] = middle + enforcedTangent;
    }

    public void SetCtrlPoints(Vector3[] _wayPoints)
    {
        points = new Vector3[1];
        points[0] = _wayPoints[0];
        List<Vector3> _curveToAd = new List<Vector3>();
        List<Vector3> wayPointList = _wayPoints.ToList();

        _linearLengh = 0;


        for (int i = 1; i < wayPointList.Count; i++)
        {
            _curveToAd.Add(wayPointList[i]);

            if (_curveToAd.Count == 3)
            {
                AddCurve(_curveToAd[0], _curveToAd[1], _curveToAd[2]);
                _curveToAd.Clear();
            }

        }

        CalculateEvenlySpacePoints();

        _linearLengh = GetLinearLengh();
    }

    void CalculateEvenlySpacePoints()
    {
        List<Vector3> _evenlySpacedPoints = new List<Vector3>();
        _evenlySpacedPoints.Add(points[0]);
        Vector3 _previousPoint = points[0];
        Vector3 _pointOnCurve, _newEvenlySpacedPoint;
        float _distSinceLastEvenPoint = 0;
        float _overshootDst;
        float _spacing = 1;

        for (int i = 0; i < points.Length - 3; i += 3)
        {
            float _t = 0;
            while (_t <= 1)
            {
                _t += 0.1f;

                _pointOnCurve = EvaluateCubic(points[i], points[i + 1], points[i + 2], points[i + 3], _t);
                _distSinceLastEvenPoint += Vector3.Distance(_previousPoint, _pointOnCurve);

                while (_distSinceLastEvenPoint >= _spacing)
                {
                    _overshootDst = _distSinceLastEvenPoint - _spacing;
                    _newEvenlySpacedPoint = _pointOnCurve + ((_previousPoint - _pointOnCurve).normalized * _overshootDst);

                    _evenlySpacedPoints.Add(_newEvenlySpacedPoint);
                    _distSinceLastEvenPoint = _overshootDst;
                    _previousPoint = _newEvenlySpacedPoint;
                }

                _previousPoint = _pointOnCurve;

            }
        }

        points = _evenlySpacedPoints.ToArray();

        // for (int i = 0; i < points.Length; i++) MeshToolBox.CreateDebugSphereAtPos(points[i]);

    }

    public static Vector3 EvaluateQuadratic(Vector3 a, Vector3 b, Vector3 c, float _t)
    {
        Vector3 p0 = Vector3.Lerp(a, b, _t);
        Vector3 p1 = Vector3.Lerp(b, c, _t);
        return Vector3.Lerp(p0, p1, _t);
    }

    public static Vector3 EvaluateCubic(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float _t)
    {
        Vector3 p0 = EvaluateQuadratic(a, b, c, _t);
        Vector3 p1 = EvaluateQuadratic(b, c, d, _t);
        return Vector3.Lerp(p0, p1, _t);
    }

    public void SetCtrlPoints(List<Transform> _wayPointsTransforms)
    {
        Vector3[] _wayPoints = new Vector3[_wayPointsTransforms.Count];
        for (int i = 0; i < _wayPointsTransforms.Count; i++) _wayPoints[i] = _wayPointsTransforms[i].position;
        SetCtrlPoints(_wayPoints);
    }

    float GetLinearLengh()
    {
        float _linearLengh = 0;

        for (int i = 0; i < points.Length - 1; i++)
        {
            _linearLengh += Vector3.Distance(points[i], points[i + 1]);
        }

        return _linearLengh;
    }

    public List<OrientedPoint> GetOrientedPoints(float _distBetweenPts)
    {
        _linearLengh = GetLinearLengh();
        float _subDivisionNb = (int)(_linearLengh / _distBetweenPts);
        float _tStep = (1f / _subDivisionNb);

        List<OrientedPoint> _orientedPoints = new List<OrientedPoint>();

        float _t = 0;

        for (int i = 0; i <= _subDivisionNb + 1; i++)
        {
            _orientedPoints.Add(new OrientedPoint(GetPoint(_t), GetDirection(_t), GetNormal(_t)));
            _t += _tStep;
        }

        /*
        GameObject _debugGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _debugGo.transform.position = new Vector3(_orientedPoints[_orientedPoints.Count - 1]._pos.x, 130, _orientedPoints[_orientedPoints.Count - 1]._pos.z);
        _debugGo.transform.localScale = new Vector3(5, 10, 5);
        */

        return _orientedPoints;
    }
}


public static class Bezier
{

    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * p0 +
            2f * oneMinusT * t * p1 +
            t * t * p2;
    }

    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return
            2f * (1f - t) * (p1 - p0) +
            2f * t * (p2 - p1);
    }

    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {

        t = Mathf.Clamp01(t);
        float OneMinusT = 1f - t;

        /*
        float _resultY = (OneMinusT * OneMinusT * OneMinusT * p0 +
            3f * OneMinusT * OneMinusT * t * p1 +
            3f * OneMinusT * t * t * p2 +
            t * t * t * p3).y;
        Debug.Log(p0.y + "/" + p1.y + "/" + p3.y+"=>"+ _resultY);
        */
        return
            OneMinusT * OneMinusT * OneMinusT * p0 +
            3f * OneMinusT * OneMinusT * t * p1 +
            3f * OneMinusT * t * t * p2 +
            t * t * t * p3;
    }

    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            3f * oneMinusT * oneMinusT * (p1 - p0) +
            6f * oneMinusT * t * (p2 - p1) +
            3f * t * t * (p3 - p2);
    }
}

[System.Serializable]
public struct OrientedPoint
{
    public OrientedPoint(Vector3 _pos, Vector3 _dir, Vector3 _normal)
    {
        this._pos = _pos;
        this._dir = _dir;
        this._normal = _normal;
    }

    public OrientedPoint(OrientedPoint _ref)
    {
        this._pos = _ref._pos;
        this._dir = _ref._dir;
        this._normal = _ref._normal;
    }

    public Vector3 _pos;
    public Vector3 _dir;
    public Vector3 _normal;

}

public interface IBezierSplineUser
{
    BezierSpline bottomToTopSpline { get; }
    List<Transform> ctrlPoints { get; }
}