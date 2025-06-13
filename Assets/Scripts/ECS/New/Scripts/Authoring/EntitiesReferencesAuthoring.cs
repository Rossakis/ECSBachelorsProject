using Unity.Entities;
using UnityEngine;

/// <summary>
/// A helper class for storing references to our gameObjects that will be turned into Entities 
/// </summary>
public class EntitiesReferencesAuthoring : MonoBehaviour {


    public GameObject bulletPrefabGameObject;
    public GameObject zombiePrefabGameObject;
    public GameObject shootLightPrefabGameObject;
    public GameObject scoutPrefabGameObject;
    public GameObject soldierPrefabGameObject;

    public GameObject buildingTowerPrefabGameObject;
    public GameObject buildingBarracksPrefabGameObject;
    public GameObject buildingIronHarvesterPrefabGameObject;
    public GameObject buildingGoldHarvesterPrefabGameObject;
    public GameObject buildingOilHarvesterPrefabGameObject;

    public GameObject buildingTowerVisualPrefabGameObject;
    public GameObject buildingBarracksVisualPrefabGameObject;
    public GameObject buildingIronHarvesterVisualPrefabGameObject;
    public GameObject buildingGoldHarvesterVisualPrefabGameObject;
    public GameObject buildingOilHarvesterVisualPrefabGameObject;

    public GameObject buildingConstructionPrefabGameObject;
    public GameObject droneHarvesterPrefabGameObject;


    public class Baker : Baker<EntitiesReferencesAuthoring> {


        public override void Bake(EntitiesReferencesAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntitiesReferences {
                fireballPrefabEntity = GetEntity(authoring.bulletPrefabGameObject, TransformUsageFlags.Dynamic),
                knightPrefabEntity = GetEntity(authoring.zombiePrefabGameObject, TransformUsageFlags.Dynamic),
                fireballLightPrefabEntity = GetEntity(authoring.shootLightPrefabGameObject, TransformUsageFlags.Dynamic),
            });
        }

    }

}


public struct EntitiesReferences : IComponentData {

    public Entity fireballPrefabEntity;
    public Entity knightPrefabEntity;
    public Entity fireballLightPrefabEntity;

}