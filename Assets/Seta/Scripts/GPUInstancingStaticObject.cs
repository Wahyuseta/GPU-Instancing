using Unity.Collections;
using UnityEngine;

public class GPUInstancingStaticObject : MonoBehaviour
{
    [SerializeField]
    private TransformDataScriptable transformData;

    [SerializeField]
    private Material mat;
    [SerializeField]
    private Mesh mesh;
    RenderParams rp;

    private NativeArray<Matrix4x4> matrices;

    private void Start()
    {
        rp = new RenderParams(mat);
        matrices = new NativeArray<Matrix4x4>(transformData.transformDatas.Length, Allocator.Persistent);

        for (int i = 0; i < transformData.transformDatas.Length; i++)
        {
            Quaternion quat = new Quaternion(transformData.transformDatas[i].rotation.x, transformData.transformDatas[i].rotation.y,
                transformData.transformDatas[i].rotation.z, transformData.transformDatas[i].rotation.w);

            matrices[i] = Matrix4x4.TRS(transformData.transformDatas[i].position, quat, transformData.transformDatas[i].scale);
        }
    }

    private void OnDestroy()
    {
        matrices.Dispose();
    }

    private void Update()
    {
        Graphics.RenderMeshInstanced(rp, mesh, 0, matrices);
    }
}
