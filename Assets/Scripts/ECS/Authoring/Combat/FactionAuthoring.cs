using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS.Authoring.Combat
{
    public class FactionAuthoring : MonoBehaviour {
    
        public FactionType factionType;

        public class Baker : Baker<FactionAuthoring> {
        
            public override void Bake(FactionAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new Faction {
                    factionType = authoring.factionType,
                });
            }
        }

    }



    public struct Faction : IComponentData {

        public FactionType factionType;

    }
}