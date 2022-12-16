using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(TrackEditor))]
public class TrackCustomEditor : Editor
{
    TrackEditor _target;

    private void OnEnable()
    {
        _target = (TrackEditor)target;
        _target.DisplayWayPoints();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        EditorGUI.BeginChangeCheck();

        _target._roadWidth = EditorGUILayout.FloatField("Road Width", _target._roadWidth);

        _target._wallBaseWidth = EditorGUILayout.FloatField("Wall Base Width", _target._wallBaseWidth);

        _target._wallHeight = EditorGUILayout.FloatField("Wall Height", _target._wallHeight);

        _target._crenelScale = EditorGUILayout.Vector3Field("Crenel Scale", _target._crenelScale);

        if (GUILayout.Button("Add section")) _target.AddSection();

        if (EditorGUI.EndChangeCheck())
        {
            _target.UpdateMesh();
        }

        if (GUILayout.Button("Erase all"))
        {
            if (EditorUtility.DisplayDialog("Erase track", "Be carefull this action is undoable.", "Erase", "Cancel")) _target.ClearAllTrack();
        }
    }

    private void OnDisable()
    {
        _target.RemoveWayPoints();
        EditorUtility.SetDirty(_target);
    }
}
