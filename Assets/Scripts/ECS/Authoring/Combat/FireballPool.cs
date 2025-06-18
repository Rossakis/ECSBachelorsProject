using Unity.Entities;

namespace ECS.Authoring.Combat
{
    public struct FireballPool : IComponentData
    {
        public Entity poolRoot;


    }
    public struct FireballPoolRoot : IComponentData {}

}