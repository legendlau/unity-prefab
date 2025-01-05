using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public static class PrefabOperation
{
    /// <summary>
    /// Creates a new prefab at the specified path with optional components
    /// </summary>
    /// <param name="prefabName">Name of the prefab to create</param>
    /// <param name="targetPath">Target directory path (relative to Assets folder)</param>
    /// <param name="components">Array of component types to add to the prefab</param>
    /// <returns>The created prefab GameObject or null if creation failed</returns>
    public static GameObject CreatePrefab(string prefabName, string targetPath, Type[] components = null)
    {
        if (string.IsNullOrEmpty(prefabName) || string.IsNullOrEmpty(targetPath))
        {
            Debug.LogError("Prefab name and target path cannot be empty");
            return null;
        }

        // Ensure target directory exists
        string fullPath = Path.Combine("Assets", targetPath);
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        // Create base object with components
        GameObject baseObject = new GameObject(prefabName);
        if (components != null)
        {
            foreach (Type componentType in components)
            {
                if (componentType != null && componentType.IsSubclassOf(typeof(Component)))
                {
                    baseObject.AddComponent(componentType);
                }
            }
        }

        // Save prefab
        string prefabPath = Path.Combine(fullPath, $"{prefabName}.prefab");
        GameObject prefabAsset = null;
        try
        {
            prefabAsset = PrefabUtility.SaveAsPrefabAsset(baseObject, prefabPath);
            Debug.Log($"Prefab created successfully at {prefabPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create prefab: {e.Message}");
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(baseObject);
            AssetDatabase.Refresh();
        }

        return prefabAsset;
    }

    /// <summary>
    /// Duplicates an existing prefab with optional modifications
    /// </summary>
    /// <param name="sourcePrefab">Source prefab to duplicate</param>
    /// <param name="newName">Name for the duplicated prefab</param>
    /// <param name="targetPath">Optional new directory path (relative to Assets folder)</param>
    /// <param name="additionalComponents">Optional array of component types to add to the duplicate</param>
    /// <returns>The duplicated prefab GameObject or null if duplication failed</returns>
    public static GameObject DuplicatePrefab(GameObject sourcePrefab, string newName, string targetPath = null, Type[] additionalComponents = null)
    {
        if (sourcePrefab == null || string.IsNullOrEmpty(newName))
        {
            Debug.LogError("Source prefab and new name cannot be null");
            return null;
        }

        string sourceAssetPath = AssetDatabase.GetAssetPath(sourcePrefab);
        string destinationPath = string.IsNullOrEmpty(targetPath)
            ? Path.GetDirectoryName(sourceAssetPath)
            : Path.Combine("Assets", targetPath);

        // Ensure target directory exists
        if (!Directory.Exists(destinationPath))
        {
            Directory.CreateDirectory(destinationPath);
        }

        string newPath = Path.Combine(destinationPath, $"{newName}.prefab");

        // Create duplicate
        GameObject duplicatedPrefab = null;
        if (AssetDatabase.CopyAsset(sourceAssetPath, newPath))
        {
            duplicatedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(newPath);

            // Add additional components if specified
            if (additionalComponents != null && duplicatedPrefab != null)
            {
                GameObject tempInstance = PrefabUtility.InstantiatePrefab(duplicatedPrefab) as GameObject;
                foreach (Type componentType in additionalComponents)
                {
                    if (componentType != null && componentType.IsSubclassOf(typeof(Component)))
                    {
                        tempInstance.AddComponent(componentType);
                    }
                }
                PrefabUtility.SaveAsPrefabAsset(tempInstance, newPath);
                UnityEngine.Object.DestroyImmediate(tempInstance);
            }

            AssetDatabase.Refresh();
            Debug.Log($"Prefab duplicated successfully to {newPath}");
        }
        else
        {
            Debug.LogError($"Failed to duplicate prefab to {newPath}");
        }

        return duplicatedPrefab;
    }

    /// <summary>
    /// Creates a prefab variant from an existing prefab
    /// </summary>
    /// <param name="sourcePrefab">Source prefab to create variant from</param>
    /// <param name="variantName">Name for the variant prefab</param>
    /// <param name="targetPath">Target directory path (relative to Assets folder)</param>
    /// <param name="modifications">Action to modify the variant before saving</param>
    /// <returns>The created variant prefab GameObject or null if creation failed</returns>
    public static GameObject CreatePrefabVariant(GameObject sourcePrefab, string variantName, string targetPath, Action<GameObject> modifications = null)
    {
        if (sourcePrefab == null || string.IsNullOrEmpty(variantName) || string.IsNullOrEmpty(targetPath))
        {
            Debug.LogError("Source prefab, variant name, and target path cannot be null");
            return null;
        }

        string fullPath = Path.Combine("Assets", targetPath);
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        string variantPath = Path.Combine(fullPath, $"{variantName}.prefab");
        GameObject variant = null;

        try
        {
            // Create temporary instance for modification
            GameObject tempInstance = PrefabUtility.InstantiatePrefab(sourcePrefab) as GameObject;

            // Apply custom modifications if provided
            modifications?.Invoke(tempInstance);

            // Create variant
            variant = PrefabUtility.SaveAsPrefabAsset(tempInstance, variantPath);

            if (variant != null)
            {
                Debug.Log($"Prefab variant created successfully at {variantPath}");
            }
            else
            {
                Debug.LogError("Failed to create prefab variant");
            }

            UnityEngine.Object.DestroyImmediate(tempInstance);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error creating prefab variant: {e.Message}");
        }
        finally
        {
            AssetDatabase.Refresh();
        }

        return variant;
    }

    /// <summary>
    /// Deletes a prefab asset and returns success status
    /// </summary>
    /// <param name="prefab">Prefab to delete</param>
    /// <returns>True if deletion was successful, false otherwise</returns>
    public static bool DeletePrefab(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("Cannot delete null prefab");
            return false;
        }

        string assetPath = AssetDatabase.GetAssetPath(prefab);
        if (string.IsNullOrEmpty(assetPath))
        {
            Debug.LogError("Invalid prefab asset path");
            return false;
        }

        bool success = AssetDatabase.DeleteAsset(assetPath);
        if (success)
        {
            Debug.Log($"Prefab deleted successfully: {assetPath}");
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError($"Failed to delete prefab at {assetPath}");
        }

        return success;
    }

    /// <summary>
    /// Reverts all changes made to a prefab instance
    /// </summary>
    /// <param name="prefabInstance">The prefab instance to revert</param>
    /// <returns>True if revert was successful, false otherwise</returns>
    public static bool RevertPrefabChanges(GameObject prefabInstance)
    {
        if (prefabInstance == null)
        {
            Debug.LogError("Cannot revert null prefab instance");
            return false;
        }

        if (!PrefabUtility.IsPartOfPrefabInstance(prefabInstance))
        {
            Debug.LogError("GameObject is not a prefab instance");
            return false;
        }

        try
        {
            PrefabUtility.RevertPrefabInstance(prefabInstance, InteractionMode.UserAction);
            Debug.Log($"Prefab instance reverted successfully: {prefabInstance.name}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to revert prefab instance: {e.Message}");
            return false;
        }
    }
}
