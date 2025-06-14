using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.Combat
{
    public class WizardSpawnerAuthoring : MonoBehaviour
    {
        [SerializeField] private float timer;
        [SerializeField] private float timerMax;
        [SerializeField] private float randomWalkingDistanceMin;
        [SerializeField] private float randomWalkingDistanceMax;
        [SerializeField] private int nearbyWizardAmountMax;
        [SerializeField] private float nearbyWizardAmountDistance;
        
        private class WizardSpawnerAuthoringBaker : Baker<WizardSpawnerAuthoring>
        {
            public override void Bake(WizardSpawnerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new WizardSpawner() {
                    timerMax = authoring.timerMax,
                    randomWalkingDistanceMin = authoring.randomWalkingDistanceMin,
                    randomWalkingDistanceMax = authoring.randomWalkingDistanceMax,
                    nearbyWizardAmountMax = authoring.nearbyWizardAmountMax,
                    nearbyWizardAmountDistance = authoring.nearbyWizardAmountDistance,
                });
            }
        }
    }
    
    
    public struct WizardSpawner : IComponentData {

        public float timer;
        public float timerMax;
        public float randomWalkingDistanceMin;
        public float randomWalkingDistanceMax;
        public int nearbyWizardAmountMax;
        public float nearbyWizardAmountDistance;

    }
}