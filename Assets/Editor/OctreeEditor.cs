using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OctreeComponent))]
public class ChunkEditor : Editor
{

    OctreeComponent octree;
    Editor editor;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        /*
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            if (check.changed)
            {
                chunkGenerator.GenerateChunks();
            }
        }
        */

        if (GUILayout.Button("Subdivide Octree"))
        {
            octree.Subdivide();
        }

    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (foldout)
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed)
                    {
                        if (onSettingsUpdated != null)
                        {
                            onSettingsUpdated();
                        }
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        octree = (OctreeComponent)target;
    }
}