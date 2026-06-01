using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FishSO))]
public class FishSOEditor : Editor
{
    private PreviewRenderUtility previewUtility;
    private GameObject previewInstance;
    private Material lastAppliedMaterial;
    private FishSO fishSO;

    private Vector2 previewDrag;
    private float previewRotationX = 20f;
    private float previewRotationY = 90f;

    private void OnEnable()
    {
        fishSO = (FishSO)target;
        CreatePreviewInstance();
    }

    private void OnDisable()
    {
        CleanupPreview();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("nameString"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("strength"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rarity"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("sellValue"));
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("averageSize"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("deltaSize"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("width"));
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("fishModel"));

        // Custom material choose
        SerializedProperty matProp = serializedObject.FindProperty("fishMaterial");

        string filterPrefix = GetFishMaterialPrefix();

        if (string.IsNullOrEmpty(filterPrefix))
        {
            EditorGUILayout.PropertyField(matProp, new GUIContent("Fish Material"));
            EditorGUILayout.HelpBox("FishModel name format not recognized. Showing all materials.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.LabelField("Fish Material");

            Material newMat = DrawFilteredMaterialPopup(matProp.objectReferenceValue as Material, filterPrefix);

            if (newMat != matProp.objectReferenceValue)
            {
                matProp.objectReferenceValue = newMat;
            }
        }

        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(10);

        // Fish preview
        if (fishSO.fishModel != null)
        {
            GUILayout.Label("Fish Preview", EditorStyles.boldLabel);
            Rect previewRect = GUILayoutUtility.GetRect(256, 256, GUIStyle.none);

            HandlePreviewEvents(previewRect);

            if (Event.current.type == EventType.Repaint)
            {
                if (previewInstance == null || previewInstance.name != fishSO.fishModel.name)
                    CreatePreviewInstance();

                if (fishSO.fishMaterial != null && lastAppliedMaterial != fishSO.fishMaterial)
                    ApplyMaterialToPreview(fishSO.fishMaterial);

                previewUtility.BeginPreview(previewRect, GUIStyle.none);

                Vector3 cameraPosition = Quaternion.Euler(previewRotationX, previewRotationY, 0) * new Vector3(0, 0, -3);
                previewUtility.camera.transform.position = cameraPosition;
                previewUtility.camera.transform.LookAt(Vector3.zero);
                previewUtility.camera.Render();

                Texture previewTexture = previewUtility.EndPreview();
                GUI.DrawTexture(previewRect, previewTexture, ScaleMode.ScaleToFit, false);
            }
        }
    }

    private string GetFishMaterialPrefix()
    {
        if (fishSO.fishModel == null)
            return null;

        string modelName = fishSO.fishModel.name;

        var parts = modelName.Split('_');
        if (parts.Length >= 2)
        {
            string numberPart = parts[1];
            return $"fish_{numberPart}";
        }

        return null;
    }

    private Material DrawFilteredMaterialPopup(Material currentMat, string prefix)
    {
        string[] guids = AssetDatabase.FindAssets("t:Material");

        var filteredMaterials = guids
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .Select(path => AssetDatabase.LoadAssetAtPath<Material>(path))
            .Where(mat => mat != null && mat.name.StartsWith(prefix))
            .ToList();

        string[] options = filteredMaterials.Select(m => m.name).ToArray();

        int currentIndex = -1;
        for (int i = 0; i < filteredMaterials.Count; i++)
        {
            if (filteredMaterials[i] == currentMat)
            {
                currentIndex = i;
                break;
            }
        }

        int selectedIndex = EditorGUILayout.Popup(currentIndex, options);

        if (selectedIndex >= 0 && selectedIndex < filteredMaterials.Count)
            return filteredMaterials[selectedIndex];

        return currentMat;
    }


    private void HandlePreviewEvents(Rect previewRect)
    {
        Event e = Event.current;

        if (e.type == EventType.MouseDrag && previewRect.Contains(e.mousePosition))
        {
            previewRotationY += e.delta.x;
            previewRotationX += e.delta.y;
            previewRotationX = Mathf.Clamp(previewRotationX, -89f, 89f);

            e.Use();
            GUI.changed = true;
        }
    }

    private void CreatePreviewInstance()
    {
        CleanupPreview();

        if (fishSO.fishModel == null)
            return;

        previewUtility = new PreviewRenderUtility();

        // Adding two light sources
        previewUtility.lights[0].intensity = 0.7f;
        previewUtility.lights[0].transform.rotation = Quaternion.Euler(50f, 50f, 0);

        previewUtility.lights[1].intensity = 0.5f;
        previewUtility.lights[1].transform.rotation = Quaternion.Euler(340f, 218f, 0);

        previewUtility.ambientColor = Color.gray * 0.5f;

        previewUtility.cameraFieldOfView = 30f;

        previewInstance = (GameObject)PrefabUtility.InstantiatePrefab(fishSO.fishModel);
        if (previewInstance == null)
        {
            Debug.LogWarning("Could not instantiate preview object.");
            return;
        }

        foreach (var t in previewInstance.GetComponentsInChildren<Transform>())
        {
            t.gameObject.layer = 0; // Default layer
        }

        previewUtility.AddSingleGO(previewInstance);
        previewInstance.transform.position = Vector3.zero;

        if (fishSO.fishMaterial != null)
        {
            ApplyMaterialToPreview(fishSO.fishMaterial);
        }
    }

    private void ApplyMaterialToPreview(Material mat)
    {
        if (previewInstance == null) return;

        foreach (var renderer in previewInstance.GetComponentsInChildren<Renderer>())
        {
            renderer.sharedMaterial = mat;
        }

        lastAppliedMaterial = mat;
    }

    private void CleanupPreview()
    {
        if (previewInstance != null)
            Object.DestroyImmediate(previewInstance);

        if (previewUtility != null)
        {
            previewUtility.Cleanup();
            previewUtility = null;
        }

        previewInstance = null;
        lastAppliedMaterial = null;
    }
}
