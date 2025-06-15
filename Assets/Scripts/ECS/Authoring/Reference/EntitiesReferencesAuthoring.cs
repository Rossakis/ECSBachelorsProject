using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// A helper class for storing references to our gameObjects that will be turned into Entities 
/// </summary>
public class EntitiesReferencesAuthoring : MonoBehaviour {
    
    public GameObject fireballPrefabGameObject;
    public GameObject knightPrefabGameObject;
    public GameObject wizardPrefabGameObject;
    public GameObject fireballLightPrefabGameObject;
    
    public class Baker : Baker<EntitiesReferencesAuthoring> {


        public override void Bake(EntitiesReferencesAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntitiesReferences {
                fireballPrefabEntity = GetEntity(authoring.fireballPrefabGameObject, TransformUsageFlags.Dynamic),
                knightPrefabEntity = GetEntity(authoring.knightPrefabGameObject, TransformUsageFlags.Dynamic),
                wizardPrefabGameObject = GetEntity(authoring.wizardPrefabGameObject, TransformUsageFlags.Dynamic),
                fireballLightPrefabEntity = GetEntity(authoring.fireballLightPrefabGameObject, TransformUsageFlags.Dynamic),
            });
        }

    }

}


public struct EntitiesReferences : IComponentData {

    public Entity fireballPrefabEntity;
    public Entity knightPrefabEntity;
    public Entity wizardPrefabGameObject;
    public Entity fireballLightPrefabEntity;

}