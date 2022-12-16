using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Drawing;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class TrackEditor : MonoBehaviour
{
    enum Dir { Left, Right }

    // [HideInInspector]
    [SerializeField]
    List<ObjToRender> _ObjToRender;

    // [HideInInspector]
    [SerializeField] public List<WayPoint> _wayPointList;
    BezierSpline _spline;
    Mesh _mesh;
    List<Vector3> _verts;
    List<int> _tris;
    List<Vector2> _uvs;
    List<UnityEngine.Color> _colors;
    MeshFilter _meshFilter;

    [HideInInspector]
    [SerializeField] Transform _wayPointsParent;

    [SerializeField] Mesh _crenelModel;
    [SerializeField] public Vector3 _crenelScale = Vector3.one;

    [HideInInspector]
    public float _roadWidth = 1;
    [HideInInspector]
    public float _wallHeight = 3;
    [HideInInspector]
    public float _wallBaseWidth = 3;

    ObjToRender _road
    {
        get { return _ObjToRender[0]; }
    }

    ObjToRender _walls
    {
        get { return _ObjToRender[1]; }
    }

    ObjToRender _crenel
    {
        get { return _ObjToRender[2]; }
    }

    ObjToRender _caps
    {
        get { return _ObjToRender[3]; }
    }

    bool _needUpdate
    {
        get
        {
            foreach (var _point in _wayPointList)
            {
                if (_point._hasMoved) return true;
            }

            return false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateMesh();
    }

#if UNITY_EDITOR
    private void Update()
    {
        // UpdateMesh();
        if (!Application.isPlaying && _needUpdate) UpdateMesh();
    }
#endif

    public void DisplayWayPoints()
    {
        foreach (var wayPoint in _wayPointList)
        {
            if (wayPoint._transform == null) wayPoint._transform = new GameObject("Way_Point").transform; ;

            wayPoint._transform.position = wayPoint._worldPos;
            if (wayPoint._transform.gameObject.GetComponent<DisplayGizmo>() == null) wayPoint._transform.gameObject.AddComponent<DisplayGizmo>();
            wayPoint._transform.SetParent(_wayPointsParent);
        }
    }


#if UNITY_EDITOR
    public void RemoveWayPoints()
    {
        GameObject _activeGo = Selection.activeGameObject;

        if (_activeGo != null)
        {
            if (_activeGo != this.gameObject && _activeGo.transform.root == this.transform) return;

            foreach (var wayPoint in _wayPointList)
            {
                if (wayPoint._transform.gameObject == _activeGo) return;
            }
        }

        List<Transform> _childList = new List<Transform>();

        foreach (Transform child in _wayPointsParent) _childList.Add(child);
        foreach (Transform child in _childList) DestroyImmediate(child.gameObject);

    }
#endif
    public void SaveWayPoints()
    {
        foreach (var _point in _wayPointList) _point.SavePosition();
    }

    public void ClearAllTrack()
    {
        _wayPointList.Clear();
        foreach (var _obj in _ObjToRender)
        {
            _obj._meshFilter.sharedMesh = _mesh = new Mesh();
            if (_obj._collider != null) _obj._collider.sharedMesh = new Mesh();
        }
    }

    public void AddSection()
    {
        Vector3 _lastPos = Vector3.zero;
        Vector3 _dir = transform.forward;

        int _wayPointToCreate = (_wayPointList.Count > 0) ? 3 : 4;

        if (_wayPointList.Count > 0)
        {
            _lastPos = _wayPointList.Last()._worldPos;

            Vector3 _beforLast = _wayPointList[_wayPointList.Count - 2]._worldPos;
            _dir = (_lastPos - _beforLast).normalized;
        }

        for (int i = 0; i < _wayPointToCreate; i++)
        {
            _wayPointList.Add(new WayPoint(_lastPos + (_dir * (5 * i))));
        }


        DisplayWayPoints();
        UpdateMesh();
    }

    public void UpdateMesh()
    {
        if (!Application.isPlaying)SaveWayPoints();

        _verts = new List<Vector3>();
        _tris = new List<int>();
        _uvs = new List<Vector2>();
        _colors = new List<UnityEngine.Color>();

        Vector3[] _wayPoints = new Vector3[_wayPointList.Count];
        for (int i = 0; i < _wayPoints.Length; i++) _wayPoints[i] = _wayPointList[i]._transform.position;
        _spline = new BezierSpline(_wayPoints);

        List<OrientedPoint> _orientedPoints = _spline.GetOrientedPoints(2);

        ClearMeshDatas();
        for (int i = 0; i < _orientedPoints.Count - 1; i++) CreateRoadSegment(_orientedPoints[i], _orientedPoints[i + 1]);
        SetRndVertColor(_road);
        SetMeshDatas(_road._meshFilter);
        _road._collider.sharedMesh = _mesh;

        ClearMeshDatas();
        for (int i = 0; i < _orientedPoints.Count - 1; i++) CreateWallSegment(_orientedPoints[i], _orientedPoints[i + 1], Dir.Left);
        for (int i = 0; i < _orientedPoints.Count - 1; i++) CreateWallSegment(_orientedPoints[i], _orientedPoints[i + 1], Dir.Right);
        SetRndVertColor(_walls);
        SetMeshDatas(_walls._meshFilter);

        ClearMeshDatas();
        CreateWallCaps(_orientedPoints[0]);
        SetMeshDatas(_caps._meshFilter);

        ClearMeshDatas();
        for (int i = 0; i < _orientedPoints.Count - 1; i++)
        {
            CreateSingleCrenel(_orientedPoints[i], Dir.Left);
            CreateSingleCrenel(_orientedPoints[i], Dir.Right);
        }
        SetRndVertColor(_walls);

        SetMeshDatas(_crenel._meshFilter);
        _crenel._collider.sharedMesh = _mesh;

        foreach (var _point in _wayPointList) _point._hasMoved = false;

    }

    void CreateRoadSegment(OrientedPoint _startPoint, OrientedPoint _endPoint)
    {
        int _vertCount = _verts.Count;

        _verts.Add(_startPoint._pos + (_startPoint._normal * _roadWidth));
        _verts.Add(_startPoint._pos - (_startPoint._normal * _roadWidth));
        _verts.Add(_endPoint._pos + (_endPoint._normal * _roadWidth));
        _verts.Add(_endPoint._pos - (_endPoint._normal * _roadWidth));

        //  Lower left triangle.
        _tris.Add(0 + _vertCount);
        _tris.Add(2 + _vertCount);
        _tris.Add(1 + _vertCount);

        //  Upper right triangle.   
        _tris.Add(2 + _vertCount);
        _tris.Add(3 + _vertCount);
        _tris.Add(1 + _vertCount);

        _uvs.Add(new Vector2(0, 0));
        _uvs.Add(new Vector2(1, 0));
        _uvs.Add(new Vector2(0, 1));
        _uvs.Add(new Vector2(1, 1));
    }

    void CreateWallSegment(OrientedPoint _startPoint, OrientedPoint _endPoint, Dir _direction)
    {
        int _vertCount = _verts.Count;
        if (_direction == Dir.Right)
        {
            _verts.Add(_startPoint._pos - (_startPoint._normal * _roadWidth));
            _verts.Add(_startPoint._pos - (_startPoint._normal * _wallBaseWidth) + new Vector3(0, -_wallHeight, 0));
            _verts.Add(_endPoint._pos - (_endPoint._normal * _roadWidth));
            _verts.Add(_endPoint._pos - (_endPoint._normal * _wallBaseWidth) + new Vector3(0, -_wallHeight, 0));
        }
        else
        {
            _verts.Add(_startPoint._pos + (_startPoint._normal * _roadWidth));
            _verts.Add(_startPoint._pos + (_startPoint._normal * _wallBaseWidth) + new Vector3(0, -_wallHeight, 0));
            _verts.Add(_endPoint._pos + (_endPoint._normal * _roadWidth));
            _verts.Add(_endPoint._pos + (_endPoint._normal * _wallBaseWidth) + new Vector3(0, -_wallHeight, 0));
        }


        if (_direction == Dir.Right)
        {
            //  Lower left triangle.
            _tris.Add(0 + _vertCount);
            _tris.Add(2 + _vertCount);
            _tris.Add(1 + _vertCount);

            //  Upper right triangle.   
            _tris.Add(2 + _vertCount);
            _tris.Add(3 + _vertCount);
            _tris.Add(1 + _vertCount);
        }
        else
        {
            //  Lower left triangle.
            _tris.Add(1 + _vertCount);
            _tris.Add(2 + _vertCount);
            _tris.Add(0 + _vertCount);

            //  Upper right triangle.   
            _tris.Add(1 + _vertCount);
            _tris.Add(3 + _vertCount);
            _tris.Add(2 + _vertCount);
        }




        _uvs.Add(new Vector2(0, 0));
        _uvs.Add(new Vector2(_wallHeight / 4, 0));
        _uvs.Add(new Vector2(0, 1));
        _uvs.Add(new Vector2(_wallHeight / 4, 1));



    }

    public void SetRndVertColor(ObjToRender _obj)
    {
        if (_obj._meshFilter.sharedMesh == null)
        return;
        _obj._meshFilter.sharedMesh.GetColors(_colors);
        if (_colors.Count == _verts.Count) return;

        for (int i = _colors.Count; i < _verts.Count; i++)
        {
            _colors.Add(GetRndColor());

        }
    }

    public Vector3 oa, ob, oc, od;

    public Vector2 a, b, c, d;

    void CreateWallCaps(OrientedPoint _point)
    {
        int _vertCount = _verts.Count;

        _verts.Add(_point._pos + (_point._normal * _roadWidth) + oa);
        _verts.Add(_point._pos - (_point._normal * _roadWidth) + ob);
        _verts.Add(_point._pos + (_point._normal * _wallBaseWidth) + new Vector3(0, -_wallHeight, 0) + oc);
        _verts.Add(_point._pos - (_point._normal * _wallBaseWidth) + new Vector3(0, -_wallHeight, 0) + od);

        //  Lower left triangle.
        _tris.Add(1 + _vertCount);
        _tris.Add(2 + _vertCount);
        _tris.Add(0 + _vertCount);

        //  Upper right triangle.   
        _tris.Add(1 + _vertCount);
        _tris.Add(3 + _vertCount);
        _tris.Add(2 + _vertCount);

        float _ratio = (_roadWidth / _wallBaseWidth) / 2;
        _uvs.Add(new Vector2((1 - _ratio), _wallHeight / 3));
        _uvs.Add(new Vector2(_ratio, _wallHeight / 3));
        _uvs.Add(new Vector2(1, 0));
        _uvs.Add(new Vector2(0, 0));







        /* _uvs.Add(a);
         _uvs.Add(b);
         _uvs.Add(c);
         _uvs.Add(d);*/





    }


    void CreateSingleCrenel(OrientedPoint _point, Dir _dir)
    {
        int _vertCount = _verts.Count;
        float _dirFloat = (_dir == Dir.Right) ? -1 : 1;
        Quaternion rotation = Quaternion.LookRotation(Vector3.up, _point._dir);
        Matrix4x4 m = Matrix4x4.Rotate(rotation);
        List<Vector3> _refVerts = new List<Vector3>();
        _crenelModel.GetVertices(_refVerts);
        for (int i = 0; i < _refVerts.Count; i++)
        {
            Vector3 _posTmp = _refVerts[i];
            _posTmp.x *= _crenelScale.x;
            _posTmp.y *= _crenelScale.y;
            _posTmp.z *= _crenelScale.z;
            _verts.Add(m.MultiplyPoint3x4(_posTmp) + _point._pos + (_dirFloat * _point._normal * _roadWidth));
        }

        List<int> _refTris = new List<int>();
        _crenelModel.GetTriangles(_refTris, 0);
        for (int i = 0; i < _refTris.Count; i++) _tris.Add(_refTris[i] + _vertCount);

        List<Vector2> _refUvs = new List<Vector2>();
        _crenelModel.GetUVs(0, _refUvs);
        for (int i = 0; i < _refUvs.Count; i++) _uvs.Add(_refUvs[i]);




    }

    void ClearMeshDatas()
    {
        _verts.Clear();
        _tris.Clear();
        _uvs.Clear();
        _colors.Clear();
    }

    void SetMeshDatas(MeshFilter _meshFilter)
    {
        _mesh = new Mesh();
        _mesh.SetVertices(_verts);
        _mesh.SetTriangles(_tris, 0);
        _mesh.SetUVs(0, _uvs);
        _meshFilter.sharedMesh = _mesh;
        if (_colors.Count > 0) _mesh.SetColors(_colors);
    }

    UnityEngine.Color GetRndColor()
    {
        int _rnd = (Random.Range(0f, 1f) > 0.5f) ? 1 : 0;
        UnityEngine.Color _col = new UnityEngine.Color(0, 0, 0, _rnd);
        return _col;
    }

    public static float AngleFromDir(Vector3 _dir)
    {
        float _angle = (float)(System.Math.Atan2(_dir.x, _dir.y) * 180f / System.Math.PI);
        if (_angle > 0) return _angle;

        _angle = 360 + _angle;

        return _angle;
    }

    [System.Serializable]
    public class WayPoint
    {
        public WayPoint(Vector3 _worldPos)
        {
            this._worldPos = _worldPos;
        }

        public Vector3 _worldPos;
        public Transform _transform;

        public void SavePosition()
        {
            _worldPos = _transform.position;
        }

        public bool _hasMoved
        {
            get
            {
                if (_transform == null) return false;
                return _transform.hasChanged;
            }

            set { _transform.hasChanged = false; }
        }

        public int _sectionID;
    }

    [System.Serializable]
    public class ObjToRender
    {
        public MeshFilter _meshFilter;
        public MeshCollider _collider;
    }


}
