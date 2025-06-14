using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class KnightSpawnerAuthoring : MonoBehaviour
{

    [SerializeField] private float timer;
    [SerializeField] private float timerMax;
    [SerializeField] private float randomWalkingDistanceMin;
    [SerializeField] private float randomWalkingDistanceMax;
    [SerializeField] private int nearbyKnightAmountMax;
    [SerializeField] private float nearbyKnightAmountDistance;


    public class Baker : Baker<KnightSpawnerAuthoring> {

        public override void Bake(KnightSpawnerAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new KnightSpawner {
                timer = authoring.timer,
                timerMax = authoring.timerMax,
                randomWalkingDistanceMin = authoring.randomWalkingDistanceMin,
                randomWalkingDistanceMax = authoring.randomWalkingDistanceMax,
                nearbyKnightAmountMax = authoring.nearbyKnightAmountMax,
                nearbyKnightAmountDistance = authoring.nearbyKnightAmountDistance,
            });
        }
    }


}


public struct KnightSpawner : IComponentData {

    public float timer;
    public float timerMax;
    public float randomWalkingDistanceMin;
    public float randomWalkingDistanceMax;
    public int nearbyKnightAmountMax;
    public float nearbyKnightAmountDistance;

}