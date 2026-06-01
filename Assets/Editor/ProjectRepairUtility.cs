using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ProjectRepairUtility
{
    private static readonly string[] ImportantObjectNames =
    {
        "Main Camera",
        "Directional Light",
        "Global Volume",
        "Point Light",
        "CatchedFishCamera"
    };

    public static void RepairMissingScriptsAndValidateReferences()
    {
        int totalRemoved = 0;
        var changedAssets = new List<string>();

        foreach (string sceneGuid in AssetDatabase.FindAssets("t:Scene", new[] { "Assets" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(sceneGuid);
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            int removed = RemoveMissingScriptsFromScene(scene);

            if (removed > 0)
            {
                totalRemoved += removed;
                changedAssets.Add($"{path} ({removed})");
                EditorSceneManager.SaveScene(scene);
            }

            if (path == "Assets/Project/Scenes/SampleScene.unity")
                ValidateImportantObjects(scene);
        }

        foreach (string prefabGuid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(prefabGuid);
            var prefabRoot = PrefabUtility.LoadPrefabContents(path);
            int removed = RemoveMissingScriptsFromGameObjectTree(prefabRoot);

            if (removed > 0)
            {
                totalRemoved += removed;
                changedAssets.Add($"{path} ({removed})");
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
            }

            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"ProjectRepairUtility: removed {totalRemoved} missing script references.");

        foreach (string asset in changedAssets)
            Debug.Log($"ProjectRepairUtility: missing scripts removed from {asset}");
    }

    private static int RemoveMissingScriptsFromScene(Scene scene)
    {
        int removed = 0;
        foreach (GameObject root in scene.GetRootGameObjects())
            removed += RemoveMissingScriptsFromGameObjectTree(root);
        return removed;
    }

    private static int RemoveMissingScriptsFromGameObjectTree(GameObject root)
    {
        int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(root);
        foreach (Transform child in root.transform)
            removed += RemoveMissingScriptsFromGameObjectTree(child.gameObject);
        return removed;
    }

    private static void ValidateImportantObjects(Scene scene)
    {
        foreach (string objectName in ImportantObjectNames)
        {
            GameObject target = FindObjectByName(scene, objectName);
            if (target == null)
            {
                Debug.LogError($"ProjectRepairUtility: important object '{objectName}' not found in {scene.path}.");
                continue;
            }

            foreach (Component component in target.GetComponents<Component>())
            {
                if (component == null)
                {
                    Debug.LogError($"ProjectRepairUtility: '{objectName}' has a missing component in {scene.path}.");
                    continue;
                }

                var serializedObject = new SerializedObject(component);
                var iterator = serializedObject.GetIterator();
                while (iterator.NextVisible(true))
                {
                    if (iterator.propertyType != SerializedPropertyType.ObjectReference)
                        continue;

                    bool isBrokenReference = iterator.objectReferenceValue == null && iterator.objectReferenceInstanceIDValue != 0;
                    if (isBrokenReference)
                    {
                        Debug.LogError(
                            $"ProjectRepairUtility: broken serialized reference '{iterator.propertyPath}' on {objectName}/{component.GetType().Name} in {scene.path}.");
                    }
                }
            }
        }
    }

    private static GameObject FindObjectByName(Scene scene, string objectName)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            GameObject found = FindObjectByNameRecursive(root.transform, objectName);
            if (found != null)
                return found;
        }

        return null;
    }

    private static GameObject FindObjectByNameRecursive(Transform current, string objectName)
    {
        if (current.name == objectName)
            return current.gameObject;

        foreach (Transform child in current)
        {
            GameObject found = FindObjectByNameRecursive(child, objectName);
            if (found != null)
                return found;
        }

        return null;
    }
}
