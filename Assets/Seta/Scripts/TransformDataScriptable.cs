using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct TransformData
{
    public float3 position;
    public float3 scale;
    public float4 rotation;
}

[CreateAssetMenu(fileName = "TransformDataScriptable", menuName = "Seta/Data/TransformDataScriptable", order = 1)]
public class TransformDataScriptable : ScriptableObject
{
    public TransformData[] transformDatas;
}
