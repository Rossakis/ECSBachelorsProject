using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS.Authoring.Reference
{
    /// <summary>
    /// A helper class for storing references to our gameObjects that will be turned into Entities 
    /// </summary>
    public class EntitiesReferencesAuthoring : MonoBehaviour {
    
        public GameObject knightPrefabGameObject;
        public GameObject wizardPrefabGameObject;
        public GameObject fireballPrefabGameObject;

        public class Baker : Baker<EntitiesReferencesAuthoring> {


            public override void Bake(EntitiesReferencesAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EntitiesReferences {
                    knightPrefabEntity = GetEntity(authoring.knightPrefabGameObject, TransformUsageFlags.Dynamic),
                    wizardPrefabGameObject = GetEntity(authoring.wizardPrefabGameObject, TransformUsageFlags.Dynamic),
                    fireballPrefabEntity = GetEntity(authoring.fireballPrefabGameObject, TransformUsageFlags.Dynamic),
                });
            }

        }

    }


    public struct EntitiesReferences : IComponentData {

        public Entity fireballPrefabEntity;
        public Entity knightPrefabEntity;
        public Entity wizardPrefabGameObject;

    }
}