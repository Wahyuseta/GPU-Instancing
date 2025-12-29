using System;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class TakeTransformData : EditorWindow
{
    [MenuItem("Seta/Utils/GetObjectTransformData")]
    public static void ShowMyEditor()
    {
        EditorWindow wnd = GetWindow<TakeTransformData>();
        wnd.titleContent = new GUIContent("Get Object Transform Data");

        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1920, 720);
    }

    private string fileName = "FileName";
    public UnityEngine.Object objectToScan;
    public TransformDataScriptable dataContainer;
    List<Vector3> positions;
    List<Quaternion> rotations;
    List<Vector3> scales;
    public UnityEngine.Object txtToRead;
    public TransformDataScriptable dataContainerToWriteFromTXT;

    void OnGUI()
    {
        GUILayout.Label("Object to scan", EditorStyles.boldLabel);
        GUILayout.Label("From : ", EditorStyles.label);
        objectToScan = EditorGUILayout.ObjectField(objectToScan, typeof(GameObject), true);
        GUILayout.Label("To : ", EditorStyles.label);
        dataContainer = (TransformDataScriptable)EditorGUILayout.ObjectField(dataContainer, typeof(TransformDataScriptable), true);
        fileName = EditorGUILayout.TextField("Saved text file name", fileName);

        if (GUILayout.Button("Scan Mesh And Write Data"))
        {
            ScanForMesh();
        }

        GUILayout.Space(50);

        GUILayout.Label("Read data from text file", EditorStyles.boldLabel);
        GUILayout.Label("From : ", EditorStyles.label);
        txtToRead = EditorGUILayout.ObjectField(txtToRead, typeof(UnityEngine.Object), true);
        GUILayout.Label("To : ", EditorStyles.label);
        dataContainerToWriteFromTXT = (TransformDataScriptable)EditorGUILayout.ObjectField(dataContainerToWriteFromTXT, typeof(TransformDataScriptable), true);

        if (GUILayout.Button("Write data from text file"))
        {
            ReadFromTextToScriptable();
        }
    }

    void ReadFromTextToScriptable()
    {
        if (txtToRead == null || dataContainerToWriteFromTXT == null)
        {
            Debug.LogError("object to scan is null");
            return;
        }

        string path = AssetDatabase.GetAssetPath(txtToRead);
        string[] readText = File.ReadAllLines(path);
        //float.Parse("41.00027357629127", CultureInfo.InvariantCulture.NumberFormat);

        List<float> tempData = new List<float>();

        int lineNum = 0;
        foreach (string line in readText)
        {
            if (!String.IsNullOrEmpty(line))
            {
                lineNum++;
            }

            string[] trans = line.Split(':');
            for (int i = 0; i < trans.Length; i++)
            {
                string[] pos = trans[i].Split(",");

                string nameVar = i == 0 ? "pos" : i == 1 ? "quat" : "scale";
                foreach (string s in pos)
                {
                    if (float.TryParse(s, out float val))
                    {
                        tempData.Add(val);
                    }
                }
            }
        }

        TransformData[] datas = new TransformData[lineNum];
        dataContainerToWriteFromTXT.transformDatas = new TransformData[lineNum];

        for (int i = 0; i < dataContainerToWriteFromTXT.transformDatas.Length; i++)
        {
            datas[i] = (new TransformData()
            {
                position = new float3(tempData[i], tempData[i+1], tempData[i+2]),
                scale = new float3(tempData[i + 3], tempData[i + 4], tempData[i + 5]),
                rotation = new Vector4(tempData[i + 6], tempData[i + 7], tempData[i + 8], tempData[i + 9])
            });

            i += 8;
        }

        dataContainerToWriteFromTXT.transformDatas = datas;

        EditorUtility.SetDirty(dataContainerToWriteFromTXT);
        Undo.RecordObject(dataContainerToWriteFromTXT, $"writing data to {dataContainerToWriteFromTXT}");
        AssetDatabase.Refresh();
        Debug.Log($"Write data for {fileName} is completed! {lineNum}");
    }
    
    void ScanForMesh()
    {
        if (objectToScan == null || objectToScan is not GameObject)
        {
            Debug.LogError("object to scan is null");
            return;
        }

        GameObject obj = (GameObject)objectToScan;

        positions = new List<Vector3>();
        rotations = new List<Quaternion>();
        scales = new List<Vector3>();

        IsItAMesh(obj.transform);
        CheckChildrens(obj.transform);

        WriteToFile(positions, scales, rotations);
    }

    bool IsItAMesh(Transform t)
    {
        if (t.GetComponent<MeshFilter>() != null)
        {
            //if (t.name.Contains("Cliff"))
            //{
                //scales.Add(t.parent.localScale);
            //}
            //else
            //{
            scales.Add(t.localScale);
            //}
            positions.Add(t.position);
            rotations.Add(t.rotation);
            return true;
        }

        return false;
    }

    void CheckChildrens(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);

            IsItAMesh(child);
            
            if (child.childCount > 0)
                CheckChildrens(child);
        }
    }

    /// <summary>
    /// Write the data inputed from scanning game object to a txt files
    /// </summary>
    public void WriteToFile(List<Vector3> pos, List<Vector3> scale, List<Quaternion> rot)
    {
        TransformData[] datas = new TransformData[pos.Count];

        for (int i = 0; i < datas.Length; i++)
        {
            datas[i] = (new TransformData()
            {
                position = pos[i],
                scale = scale[i],
                rotation = new Vector4(rot[i].x, rot[i].y, rot[i].z, rot[i].w)
            });
        }

        string path = Application.dataPath + $"/Transform Data/{objectToScan}_{dataContainer}_{fileName}.txt";
        File.WriteAllText(path, "");

        dataContainer.transformDatas = new TransformData[datas.Length];

        for (int i = 0; i < datas.Length; i++)
        {
            string content =
                $"{datas[i].position.x},{datas[i].position.y},{datas[i].position.z}:" +
                $"{datas[i].rotation.x},{datas[i].rotation.y},{datas[i].rotation.z},{datas[i].rotation.w}:" +
                $"{datas[i].scale.x},{datas[i].scale.y},{datas[i].scale.z}\n";

            dataContainer.transformDatas[i] = datas[i];

            File.AppendAllText(path, content);
        }

        EditorUtility.SetDirty(dataContainer);
        Undo.RecordObject(dataContainer, $"writing data to {dataContainer}");
        AssetDatabase.Refresh();
        Debug.Log($"Write data for {fileName} is completed!");
    }

}

#endif