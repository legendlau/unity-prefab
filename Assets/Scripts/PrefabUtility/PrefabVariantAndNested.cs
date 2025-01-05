using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;

public class PrefabVariantAndNested : MonoBehaviour
{
    public GameObject basePrefab;
    public GameObject variantPrefab;

    void PrefabVariantExamples()
    {
        #if UNITY_EDITOR
        // 1. Create a new prefab variant
        if (basePrefab != null)
        {
            // Instantiate the base prefab
            GameObject instance = PrefabUtility.InstantiatePrefab(basePrefab) as GameObject;

            // Modify the instance as needed
            instance.AddComponent<Rigidbody>();

            // Save as a variant
            string variantPath = "Assets/NewVariant.prefab";
            PrefabUtility.SaveAsPrefabAsset(instance, variantPath);
            DestroyImmediate(instance);
        }

        // 2. Check if object is a variant
        if (variantPrefab != null)
        {
            bool isVariant = PrefabUtility.IsPartOfVariantPrefab(variantPrefab);
            Debug.Log($"Is Variant: {isVariant}");

            // Get the original prefab source
            GameObject originalPrefab = PrefabUtility.GetCorrespondingObjectFromSource(variantPrefab);
        }
        #endif
    }

    void NestedPrefabExamples()
    {
        // 3. Working with nested prefabs
        if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
        {
            // Get the nearest prefab instance root
            GameObject nearestRoot = PrefabUtility.GetNearestPrefabInstanceRoot(gameObject);

            // Get the outermost prefab instance root
            GameObject outermostRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);

            // Check if this is a nested prefab
            bool isNested = nearestRoot != outermostRoot;

            if (isNested)
            {
                Debug.Log("This is part of a nested prefab structure");

                // Get all prefab instances with the same parent
                var parentPrefab = PrefabUtility.GetCorrespondingObjectFromSource(nearestRoot);
                var allInstances = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
                    .Where(go => PrefabUtility.GetCorrespondingObjectFromSource(go) == parentPrefab)
                    .ToArray();

                foreach (var instance in allInstances)
                {
                    Debug.Log($"Found instance: {instance.name}");
                }
            }
        }
    }

    void PrefabModificationTracking()
    {
        #if UNITY_EDITOR
        if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
        {
            // 4. Track prefab instance modifications
            bool hasModifications = PrefabUtility.HasPrefabInstanceAnyOverrides(gameObject, false);

            if (hasModifications)
            {
                // Get all modifications
                var modifications = PrefabUtility.GetPropertyModifications(gameObject);

                // Get object overrides
                var objectOverrides = PrefabUtility.GetObjectOverrides(gameObject);

                foreach (var modification in modifications)
                {
                    Debug.Log($"Modified: {modification.propertyPath}");
                }

                foreach (var objOverride in objectOverrides)
                {
                    Debug.Log($"Override: {objOverride.instanceObject.name}");
                }
            }
        }
        #endif
    }

    void PrefabStageHandling()
    {
        #if UNITY_EDITOR
        // 5. Working with Prefab Stage
        var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null)
        {
            // We are currently editing a prefab in isolation
            GameObject prefabRoot = prefabStage.prefabContentsRoot;
            string prefabPath = prefabStage.assetPath;

            Debug.Log($"Editing prefab: {prefabPath}");

            // Check if we're in a variant
            bool isVariant = PrefabUtility.IsPartOfVariantPrefab(prefabRoot);
        }
        #endif
    }
}