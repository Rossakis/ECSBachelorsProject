using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.Reference
{
    public class SceneDataReferenceAuthoring : MonoBehaviour
    {
        public EcsSceneDataSO ecsSceneDataSO;
        
        private class SceneDataReferenceAuthoringBaker : Baker<SceneDataReferenceAuthoring>
        {
            public override void Bake(SceneDataReferenceAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent(entity, new SceneDataReference {
                    IsJobSystemOn = authoring.ecsSceneDataSO.IsJobSystemOn,
                    IsObjectPoolingOn = authoring.ecsSceneDataSO.IsObjectPoolingOn,
                    WizardsAmountToSpawn = authoring.ecsSceneDataSO.WizardsAmountToSpawn,
                    KnightsAmountToSpawn = authoring.ecsSceneDataSO.KnightsAmountToSpawn,
                });
            }
        }
    }

    public struct SceneDataReference : IComponentData
    {
        public bool IsJobSystemOn;
        public bool IsObjectPoolingOn;
        public int WizardsAmountToSpawn;
        public int KnightsAmountToSpawn;
    }
}