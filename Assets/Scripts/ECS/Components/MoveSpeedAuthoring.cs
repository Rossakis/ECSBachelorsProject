using Unity.Entities;
using UnityEngine;

/// <summary>
/// The "Authoring" type of Monobehaviour-derived classes are used to add the equivalent ECS component scripts at runtime (e.g. MoveSpeed)
/// </summary>
public class MoveSpeedAuthoring : MonoBehaviour
{
    public float WalkSpeed;
    public float RotateSpeed;

    private class Baker : Baker<MoveSpeedAuthoring>
    {
        public override void Bake(MoveSpeedAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic); //dynamic = entity will move during runtime
            AddComponent(entity, new MoveSpeed
            {
                WalkSpeed = authoring.WalkSpeed,
                RotateSpeed = authoring.RotateSpeed,
            });
        }
    }
}

//We place both the authoring and the component script into one file for convenience
public struct MoveSpeed : IComponentData
{
    public float WalkSpeed;
    public float RotateSpeed;
}