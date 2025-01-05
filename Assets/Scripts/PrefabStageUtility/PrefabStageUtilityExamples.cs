using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

#if UNITY_EDITOR
public class PrefabStageUtilityExamples : EditorWindow
{
    private Vector2 scrollPosition;
    private List<string> logMessages = new List<string>();

    [MenuItem("Examples/PrefabStageUtility/Open Window")]
    private static void ShowWindow()
    {
        var window = GetWindow<PrefabStageUtilityExamples>();
        window.titleContent = new GUIContent("PrefabStage Utility");
        window.Show();
    }

    private void OnEnable()
    {
        // Subscribe to prefab stage events to update our window
        PrefabStage.prefabStageOpened += OnPrefabStageChanged;
        PrefabStage.prefabStageClosing += OnPrefabStageChanged;
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        PrefabStage.prefabStageOpened -= OnPrefabStageChanged;
        PrefabStage.prefabStageClosing -= OnPrefabStageChanged;
    }

    private void OnPrefabStageChanged(PrefabStage _)
    {
        // Force window to repaint when prefab stage changes
        Repaint();
    }

    private void AddLog(string message)
    {
        logMessages.Add($"[{System.DateTime.Now:HH:mm:ss}] {message}");
        if (logMessages.Count > 100) // Keep log size manageable
        {
            logMessages.RemoveAt(0);
        }
        Repaint();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);

        // Example 1: Get Current Prefab Stage
        if (GUILayout.Button("Check Current Prefab Stage"))
        {
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null)
            {
                AddLog($"Current Prefab Stage: {stage.assetPath}");
                AddLog($"Scene Name: {stage.scene.name}");
                AddLog($"Root Object: {stage.prefabContentsRoot.name}");
            }
            else
            {
                AddLog("No prefab stage is currently active");
            }
        }

        EditorGUILayout.Space(5);

        // Example 2: Get Prefab Stage for Selected Object
        if (GUILayout.Button("Get Stage for Selected Object"))
        {
            var selectedObject = Selection.activeGameObject;
            if (selectedObject != null)
            {
                var stage = PrefabStageUtility.GetPrefabStage(selectedObject);
                if (stage != null)
                {
                    AddLog($"Found prefab stage for '{selectedObject.name}'");
                    AddLog($"Stage Path: {stage.assetPath}");
                }
                else
                {
                    AddLog($"Selected object '{selectedObject.name}' is not in a prefab stage");
                }
            }
            else
            {
                AddLog("No object selected");
            }
        }

        EditorGUILayout.Space(5);

        // Example 3: Open Prefab in Stage
        if (GUILayout.Button("Open Selected Prefab"))
        {
            var selectedObject = Selection.activeObject as GameObject;
            if (selectedObject != null && PrefabUtility.IsPartOfPrefabAsset(selectedObject))
            {
                string assetPath = AssetDatabase.GetAssetPath(selectedObject);
                bool opened = PrefabStageUtility.OpenPrefab(assetPath);
                if (opened)
                {
                    AddLog($"Successfully opened prefab: {assetPath}");
                }
                else
                {
                    AddLog($"Failed to open prefab: {assetPath}");
                }
            }
            else
            {
                AddLog("Please select a prefab asset in the Project window");
            }
        }

        EditorGUILayout.Space(5);

        // Example 4: Stage Information
        if (GUILayout.Button("Show Stage Details"))
        {
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null)
            {
                AddLog("=== Prefab Stage Details ===");
                AddLog($"Asset Path: {stage.assetPath}");
                AddLog($"Scene Path: {stage.scene.path}");
                AddLog($"Is Scene Dirty: {stage.scene.isDirty}");

                if (stage.openedFromInstanceRoot != null)
                {
                    AddLog($"Opened from instance: {stage.openedFromInstanceRoot.name}");
                    if (stage.openedFromInstanceObject != null && stage.openedFromInstanceObject != stage.openedFromInstanceRoot)
                    {
                        AddLog($"Specifically from object: {stage.openedFromInstanceObject.name}");
                    }
                }
            }
            else
            {
                AddLog("No active prefab stage to show details for");
            }
        }

        EditorGUILayout.Space(10);

        // Log Display
        EditorGUILayout.LabelField("Log", EditorStyles.boldLabel);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        foreach (var message in logMessages)
        {
            EditorGUILayout.LabelField(message, EditorStyles.wordWrappedLabel);
        }
        EditorGUILayout.EndScrollView();

        // Clear Log Button
        if (GUILayout.Button("Clear Log"))
        {
            logMessages.Clear();
            Repaint();
        }
    }
}
#endif