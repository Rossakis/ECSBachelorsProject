using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Serialization;

public class KnightArmyAuthoring : MonoBehaviour {


    public float startTimer;
    public float spawnTimerMax;
    public int knightAmountToSpawn;
    public float spawnAreaWidth;
    public float spawnAreaHeight;
    public GameObject minimapIconGameObject;


    public class Baker : Baker<KnightArmyAuthoring> {

        public override void Bake(KnightArmyAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            Entity minimapIconEntity = GetEntity(authoring.minimapIconGameObject, TransformUsageFlags.Dynamic);
            AddComponent(entity, new KnightArmy {
                startTimer = authoring.startTimer,
                spawnTimerMax = authoring.spawnTimerMax,
                knightAmountToSpawn = authoring.knightAmountToSpawn,
                spawnAreaWidth = authoring.spawnAreaWidth,
                spawnAreaHeight = authoring.spawnAreaHeight,
                random = new Unity.Mathematics.Random((uint)entity.Index),
                isSetup = false,
                minimapIconEntity = minimapIconEntity,
            });
        }
    }
}



public struct KnightArmy : IComponentData {


    public float startTimer;
    public float spawnTimer;
    public float spawnTimerMax;
    public int knightAmountToSpawn;
    public float spawnAreaWidth;
    public float spawnAreaHeight;
    public Unity.Mathematics.Random random;
    public bool onStartSpawning;
    public bool onStartSpawningSoon;
    public bool isSetup;
    public Entity minimapIconEntity;

}