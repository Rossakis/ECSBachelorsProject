using Unity.Entities;

namespace Assets.Scripts.ECS.Authoring.Combat
{
    public struct FireballPool : IComponentData
    {
        public Entity poolRoot;


    }
    public struct FireballPoolRoot : IComponentData {}

}