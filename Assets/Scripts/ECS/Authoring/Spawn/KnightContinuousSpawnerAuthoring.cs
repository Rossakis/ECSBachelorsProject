using Unity.Entities;
using UnityEngine;

public class KnightContinuousSpawnerAuthoring : MonoBehaviour
{

    [SerializeField] private float timer;
    [SerializeField] private float timerMax;
    [SerializeField] private int nearbyKnightAmountMax;
    [SerializeField] private float nearbyKnightAmountDistance;
    
    public class Baker : Baker<KnightContinuousSpawnerAuthoring> {

        public override void Bake(KnightContinuousSpawnerAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new KnightContinuousSpawner {
                timer = authoring.timer,
                timerMax = authoring.timerMax,
                nearbyKnightAmountMax = authoring.nearbyKnightAmountMax,
                nearbyKnightAmountDistance = authoring.nearbyKnightAmountDistance,
            });
        }
    }
}


public struct KnightContinuousSpawner : IComponentData {

    public float timer;
    public float timerMax;
    public int nearbyKnightAmountMax;
    public float nearbyKnightAmountDistance;

}