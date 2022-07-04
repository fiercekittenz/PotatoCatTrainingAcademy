using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpriteShapeWater))]
public class SpriteShapeWaterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SpriteShapeWater water = (SpriteShapeWater)target;

        if (GUILayout.Button("Rebuild Water"))
        {
            water.RebuildWater();
        }
    }
}
