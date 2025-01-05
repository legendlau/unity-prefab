using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Linq;

#if UNITY_EDITOR
[InitializeOnLoad]
public class PrefabStageExamples
{
    // Constructor that subscribes to PrefabStage events
    static PrefabStageExamples()
    {
        PrefabStage.prefabStageOpened += OnPrefabStageOpened;
        PrefabStage.prefabStageClosing += OnPrefabStageClosing;
        EditorSceneManager.sceneSaving += OnSceneSaving;
    }

    // Example 1: Handle Prefab Stage Opening
    private static void OnPrefabStageOpened(PrefabStage prefabStage)
    {
        // Get the root GameObject of the prefab being edited
        GameObject prefabRoot = prefabStage.prefabContentsRoot;
        Debug.Log($"Opened Prefab Stage for: {prefabRoot.name}");

        // Get the stage's properties
        string stagePath = prefabStage.assetPath;
        bool isPartOfPrefabVariant = PrefabUtility.IsPartOfVariantPrefab(prefabRoot);

        // Get instance information if opened from instance
        GameObject instanceRoot = prefabStage.openedFromInstanceRoot;
        GameObject instanceObject = prefabStage.openedFromInstanceObject;

        if (instanceRoot != null)
        {
            Debug.Log($"Opened from instance: {instanceRoot.name}");
            if (instanceObject != null && instanceObject != instanceRoot)
            {
                Debug.Log($"Specifically opened from child object: {instanceObject.name}");
            }
        }

        Debug.Log($"Prefab Path: {stagePath}");
        Debug.Log($"Is Variant: {isPartOfPrefabVariant}");
    }

    // Example 2: Handle Prefab Stage Closing
    private static void OnPrefabStageClosing(PrefabStage prefabStage)
    {
        GameObject prefabRoot = prefabStage.prefabContentsRoot;
        Debug.Log($"Closing Prefab Stage for: {prefabRoot.name}");

        // Check if the stage is dirty
        Scene prefabScene = prefabStage.scene;
        if (prefabScene.isDirty)
        {
            Debug.Log("Warning: There are unsaved changes in the prefab!");
        }
    }

    // Example 3: Handle Scene Saving
    private static void OnSceneSaving(Scene scene, string path)
    {
        var currentStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (currentStage != null && currentStage.scene == scene)
        {
            Debug.Log($"Saving prefab stage at path: {path}");
            ValidatePrefabContents(currentStage);
        }
    }

    // Example 4: Working with Current Prefab Stage
    [MenuItem("Examples/PrefabStage/Check Current Stage")]
    private static void CheckCurrentPrefabStage()
    {
        var currentStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (currentStage != null)
        {
            // Get scene information
            var scene = currentStage.scene;
            Debug.Log($"Editing prefab in scene: {scene.name}");

            // Check if any object is selected
            if (Selection.activeGameObject != null)
            {
                bool isPartOfCurrentStage = currentStage.IsPartOfPrefabContents(Selection.activeGameObject);
                Debug.Log($"Selected object is part of current stage: {isPartOfCurrentStage}");
            }

            // Clear dirtiness if needed
            if (scene.isDirty)
            {
                currentStage.ClearDirtiness();
                Debug.Log("Cleared prefab stage dirtiness");
            }
        }
        else
        {
            Debug.Log("Not currently editing a prefab in isolation");
        }
    }

    // Example 5: Prefab Validation
    private static void ValidatePrefabContents(PrefabStage stage)
    {
        GameObject root = stage.prefabContentsRoot;

        // Example validation: Check for required components
        if (!root.GetComponent<Transform>())
        {
            Debug.LogError("Prefab root must have a Transform component!");
        }

        // Check all child objects in the prefab
        var children = root.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            // Example: Ensure all children have unique names
            var duplicateNames = children
                .Where(t => t.parent == child.parent && t.name == child.name)
                .ToList();

            if (duplicateNames.Count > 1)
            {
                Debug.LogWarning($"Multiple objects named '{child.name}' found under parent '{child.parent?.name ?? "root"}'!");
            }
        }
    }

    // Example 6: Opening a Prefab in Isolation
    [MenuItem("Examples/PrefabStage/Open Prefab")]
    private static void OpenPrefabInIsolation()
    {
        // Get the selected prefab asset
        GameObject selectedPrefab = Selection.activeGameObject;
        if (selectedPrefab != null && PrefabUtility.IsPartOfPrefabAsset(selectedPrefab))
        {
            string prefabPath = AssetDatabase.GetAssetPath(selectedPrefab);
            AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath));
        }
        else
        {
            Debug.LogWarning("Please select a prefab asset in the Project window");
        }
    }

    // Example 7: Handling Prefab Modifications
    [MenuItem("Examples/PrefabStage/Check Modifications")]
    private static void CheckPrefabModifications()
    {
        var stage = PrefabStageUtility.GetCurrentPrefabStage();
        if (stage != null)
        {
            GameObject root = stage.prefabContentsRoot;
            bool hasModifications = PrefabUtility.HasPrefabInstanceAnyOverrides(root, false);

            if (hasModifications)
            {
                var modifications = PrefabUtility.GetPropertyModifications(root);
                foreach (var mod in modifications)
                {
                    Debug.Log($"Modified property: {mod.propertyPath} = {mod.value}");
                }
            }
        }
    }
}
#endif