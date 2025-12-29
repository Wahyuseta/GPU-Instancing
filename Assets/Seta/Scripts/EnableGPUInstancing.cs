using System;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class EnableGPUInstancing : EditorWindow
{
    [MenuItem("Seta/Utils/EnableGPUInstancing")]
    public static void ShowMyEditor()
    {
        EditorWindow wnd = GetWindow<EnableGPUInstancing>();
        wnd.titleContent = new GUIContent("EnableGPUInstancing");

        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1920, 720);
    }

    public UnityEngine.Object objectToScan;
    public UnityEngine.Object folderToScan;
    void OnGUI()
    {
        GUILayout.Label("Material to scan", EditorStyles.boldLabel);
        objectToScan = EditorGUILayout.ObjectField(objectToScan, typeof(UnityEngine.Material), true);

        if (GUILayout.Button("Enable Material GPU Instancing"))
        {
            EnableMaterialGPUInstancing();
        }

        GUILayout.Space(20);

        GUILayout.Label("Folder to scan", EditorStyles.boldLabel);
        folderToScan = EditorGUILayout.ObjectField(folderToScan, typeof(UnityEngine.Object), true);

        if (GUILayout.Button("Search and Enable Material GPU Instancing"))
        {
            SearchMaterial();
        }
    }

    void EnableMaterialGPUInstancing()
    {
        if (objectToScan == null || objectToScan is not Material)
        {
            Debug.LogError("object to scan is null");
            return;
        }

        var mat = objectToScan as Material;

        Debug.Log($"Before enable instancing for {mat.name}, now the status is {mat.enableInstancing}");

        mat.enableInstancing = true;

        Debug.Log($"After enable instancing for {mat.name}, now the status is {mat.enableInstancing}");
    }

    void SearchMaterial()
    {
        string folderPath = AssetDatabase.GetAssetPath(folderToScan);

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogError($"Invalid folder path: {folderPath}");
            return;
        }

        foreach (var guid in AssetDatabase.FindAssets("t:Material", new[] { folderPath }))
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            if (material != null)
            {
                material.enableInstancing = true;
            }
        }
    }
}

#endif