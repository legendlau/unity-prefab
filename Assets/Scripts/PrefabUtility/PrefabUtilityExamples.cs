using UnityEngine;
using UnityEditor;

public class PrefabUtilityExamples : MonoBehaviour
{
    public GameObject prefabReference;

    void Examples()
    {
        // 1. Get the prefab asset from an instance
        GameObject prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);

        // 2. Check if an object is a prefab instance
        bool isPrefabInstance = PrefabUtility.IsPartOfPrefabInstance(gameObject);
        bool isPrefabAsset = PrefabUtility.IsPartOfPrefabAsset(gameObject);

        // 3. Get the root GameObject of a prefab instance
        GameObject prefabRoot = PrefabUtility.GetNearestPrefabInstanceRoot(gameObject);

        // 4. Create a new prefab instance
        if (prefabReference != null)
        {
            GameObject newInstance = PrefabUtility.InstantiatePrefab(prefabReference) as GameObject;
            newInstance.transform.position = Vector3.zero;
        }

        // 5. Save modifications back to prefab asset
        if (isPrefabInstance)
        {
            PrefabUtility.ApplyPrefabInstance(gameObject, InteractionMode.UserAction);
        }

        // 6. Revert prefab instance changes
        if (isPrefabInstance)
        {
            PrefabUtility.RevertPrefabInstance(gameObject, InteractionMode.UserAction);
        }

        // 7. Check if prefab has any overrides
        bool hasOverrides = PrefabUtility.HasPrefabInstanceAnyOverrides(gameObject, false);

        // 8. Get all property modifications
        var modifications = PrefabUtility.GetPropertyModifications(gameObject);

        // 9. Create a new prefab from an existing GameObject
        #if UNITY_EDITOR
        GameObject newGameObject = new GameObject("NewPrefab");
        string prefabPath = "Assets/NewPrefab.prefab";
        PrefabUtility.SaveAsPrefabAsset(newGameObject, prefabPath);
        DestroyImmediate(newGameObject);
        #endif

        // 10. Connect a GameObject to a prefab
        if (prefabReference != null)
        {
            var settings = new ConvertToPrefabInstanceSettings();
            PrefabUtility.ConvertToPrefabInstance(gameObject, prefabReference, settings, InteractionMode.UserAction);
        }
    }

    void UnpackPrefabExample()
    {
        // 11. Unpack prefab completely
        if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
        {
            PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);
        }
    }

    void PrefabVariantExample()
    {
        #if UNITY_EDITOR
        // 12. Create a prefab variant
        if (prefabReference != null)
        {
            string variantPath = "Assets/PrefabVariant.prefab";
            GameObject instance = PrefabUtility.InstantiatePrefab(prefabReference) as GameObject;
            PrefabUtility.SaveAsPrefabAsset(instance, variantPath);
            DestroyImmediate(instance);
        }
        #endif
    }
}