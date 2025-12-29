using UnityEngine;
using Unity.Entities;

public class SimpleAuthoring : MonoBehaviour
{
    public bool ShouldUseTransform;
    public bool ShouldHaveInteraction;
}
public class SimpleAuthoringBaker : Baker<SimpleAuthoring>
{
    public override void Bake(SimpleAuthoring data)
    {
        var entity = GetEntity(data.ShouldUseTransform || data.ShouldHaveInteraction ? TransformUsageFlags.Dynamic : TransformUsageFlags.None);
    }
}