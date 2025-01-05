using UnityEngine;
using UnityEditor;

public class AdvancedPrefabOperations : MonoBehaviour
{
    public GameObject targetPrefab;

    void AdvancedExamples()
    {
        // 1. Get Prefab Asset Type
        if (targetPrefab != null)
        {
            PrefabAssetType assetType = PrefabUtility.GetPrefabAssetType(targetPrefab);
            Debug.Log($"Prefab Asset Type: {assetType}");
        }

        // 2. Get Prefab Instance Status
        PrefabInstanceStatus instanceStatus = PrefabUtility.GetPrefabInstanceStatus(gameObject);
        Debug.Log($"Instance Status: {instanceStatus}");

        // 3. Check for Nested Prefab Instance
        bool isNestedPrefabInstance = PrefabUtility.IsPartOfNonAssetPrefabInstance(gameObject);

        // 4. Get Original Source or Variant Source
        GameObject originalSource = PrefabUtility.GetCorrespondingObjectFromOriginalSource(gameObject);
        GameObject variantSource = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);

        // 5. Working with Prefab Modifications
        if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
        {
            // Get Added Components
            var addedComponents = PrefabUtility.GetAddedComponents(gameObject);
            foreach (var component in addedComponents)
            {
                Debug.Log($"Added Component: {component.GetType().Name}");
            }

            // Get Removed Components
            var removedComponents = PrefabUtility.GetRemovedComponents(gameObject);
            foreach (var component in removedComponents)
            {
                Debug.Log($"Removed Component: {component.GetType().Name}");
            }
        }

        // 6. Handle Prefab Override Window
        #if UNITY_EDITOR
        if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
        {
            // Open Prefab Override Window in the Inspector
            EditorUtility.SetDirty(gameObject);
            Selection.activeGameObject = gameObject;
        }
        #endif

        // 7. Working with Property Modifications
        var propertyModifications = PrefabUtility.GetPropertyModifications(gameObject);
        if (propertyModifications != null)
        {
            foreach (var modification in propertyModifications)
            {
                Debug.Log($"Modified Property: {modification.propertyPath} = {modification.value}");
            }
        }
    }

    void PrefabAssetHandling()
    {
        #if UNITY_EDITOR
        if (targetPrefab != null)
        {
            // 8. Load Prefab Contents
            var prefabContents = PrefabUtility.LoadPrefabContents("Assets/SomePrefab.prefab");

            // Modify prefab contents here

            // Save and unload
            PrefabUtility.SaveAsPrefabAsset(prefabContents, "Assets/ModifiedPrefab.prefab");
            PrefabUtility.UnloadPrefabContents(prefabContents);
        }
        #endif
    }

    void MergePrefabExample()
    {
        #if UNITY_EDITOR
        if (targetPrefab != null && PrefabUtility.IsPartOfPrefabInstance(gameObject))
        {
            // 9. Apply changes from one prefab to another
            GameObject sourceInstance = PrefabUtility.InstantiatePrefab(targetPrefab) as GameObject;
            if (sourceInstance != null)
            {
                PrefabUtility.ApplyPrefabInstance(sourceInstance, InteractionMode.UserAction);
            }
            DestroyImmediate(sourceInstance);
        }
        #endif
    }
}