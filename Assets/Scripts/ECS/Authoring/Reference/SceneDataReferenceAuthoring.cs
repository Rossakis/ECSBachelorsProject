using Assets.Scripts.DataTracking;
using Assets.Scripts.ScriptableObjects.Scene;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS.Authoring.Reference
{
    public class SceneDataReferenceAuthoring : MonoBehaviour
    {
        public EcsSceneDataSO ecsSceneDataSO;

            private class SceneDataReferenceAuthoringBaker : Baker<SceneDataReferenceAuthoring>
            {
                public override void Bake(SceneDataReferenceAuthoring authoring)
                {
                    Entity entity = GetEntity(TransformUsageFlags.None);
                    
                    AddComponent(entity, new SceneDataReference {
                        IsJobSystemOn = authoring.ecsSceneDataSO.IsJobSystemOn,
                        IsObjectPoolingOn = authoring.ecsSceneDataSO.IsObjectPoolingOn,
                        
                        WizardsAmountToSpawn = authoring.ecsSceneDataSO.WizardsAmountToSpawn,
                        WizardMaxHealth = authoring.ecsSceneDataSO.WizardMaxHealth,
                        WizardDamage = authoring.ecsSceneDataSO.WizardDamage,
                        
                        IsKnightInfiniteSpawnOn = authoring.ecsSceneDataSO.IsKnightSpawnInfinite,
                        KnightsAmountToSpawn = authoring.ecsSceneDataSO.KnightsAmountToSpawn,
                        KnightMaxHealth = authoring.ecsSceneDataSO.KnightMaxHealth,
                        KnightDamage = authoring.ecsSceneDataSO.KnightDamage
                    });
                }
            }
    }

    public struct SceneDataReference : IComponentData
        {
            public bool IsJobSystemOn;
            public bool IsObjectPoolingOn;

            public int WizardsAmountToSpawn;
            public int WizardMaxHealth;
            public int WizardDamage;

            public bool IsKnightInfiniteSpawnOn;
            public int KnightsAmountToSpawn;
            public int KnightMaxHealth;
            public int KnightDamage;
        }
}