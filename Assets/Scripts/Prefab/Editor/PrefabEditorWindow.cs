using UnityEngine;
using UnityEditor;
using System;

public class PrefabEditorWindow : EditorWindow
{
    private GameObject prefabInstance;
    private Vector2 scrollPosition;
    private bool showTransformSettings = true;
    private bool showComponents = true;

    [MenuItem("Window/Prefab Editor")]
    public static void ShowWindow()
    {
        GetWindow<PrefabEditorWindow>("Prefab Editor");
    }

    private void OnEnable()
    {
        LoadBasePrefab();
    }

    private void LoadBasePrefab()
    {
        string prefabPath = "Base";
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab != null)
        {
            prefabInstance = prefab;
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Prefab Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        if (GUILayout.Button("Create New Base Prefab"))
        {
            PrefabOperation.CreatePrefab("Base", "Assets/Resources", new Type[] { typeof(Rigidbody), typeof(Collider) });
        }

        EditorGUILayout.Space(10);

        if (prefabInstance == null)
        {
            EditorGUILayout.HelpBox("No Base prefab found in Resources folder", MessageType.Warning);
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Transform Settings
        showTransformSettings = EditorGUILayout.Foldout(showTransformSettings, "Transform Settings");
        if (showTransformSettings)
        {
            EditorGUI.indentLevel++;
            Vector3 position = EditorGUILayout.Vector3Field("Position", prefabInstance.transform.position);
            Vector3 rotation = EditorGUILayout.Vector3Field("Rotation", prefabInstance.transform.eulerAngles);
            Vector3 scale = EditorGUILayout.Vector3Field("Scale", prefabInstance.transform.localScale);

            if (GUI.changed)
            {
                Undo.RecordObject(prefabInstance.transform, "Modified Prefab Transform");
                prefabInstance.transform.position = position;
                prefabInstance.transform.eulerAngles = rotation;
                prefabInstance.transform.localScale = scale;
                EditorUtility.SetDirty(prefabInstance);
            }
            EditorGUI.indentLevel--;
        }

        // Component Management
        EditorGUILayout.Space(10);
        showComponents = EditorGUILayout.Foldout(showComponents, "Components");
        if (showComponents)
        {
            EditorGUI.indentLevel++;
            Component[] components = prefabInstance.GetComponents<Component>();
            foreach (Component component in components)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(component, component.GetType(), true);
                if (component.GetType() != typeof(Transform))
                {
                    if (GUILayout.Button("Remove", GUILayout.Width(60)))
                    {
                        Undo.DestroyObjectImmediate(component);
                        EditorUtility.SetDirty(prefabInstance);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Component"))
            {
                EditorGUIUtility.ShowObjectPicker<MonoScript>(null, false, "", 0);
            }
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndScrollView();

        // Save changes
        EditorGUILayout.Space(10);
        if (GUILayout.Button("Apply Changes"))
        {
            PrefabUtility.SavePrefabAsset(prefabInstance);
            Debug.Log("Prefab changes saved successfully");
        }
    }

    private void OnObjectPickerClosed()
    {
        MonoScript script = EditorGUIUtility.GetObjectPickerObject() as MonoScript;
        if (script != null)
        {
            System.Type componentType = script.GetClass();
            if (componentType != null && componentType.IsSubclassOf(typeof(Component)))
            {
                Undo.AddComponent(prefabInstance, componentType);
                EditorUtility.SetDirty(prefabInstance);
            }
        }
    }
}