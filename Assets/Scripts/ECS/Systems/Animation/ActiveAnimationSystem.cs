using Assets.Scripts.ECS.Authoring.Animation;
using Assets.Scripts.ECS.Authoring.Reference;
using Assets.Scripts.ScriptableObjects.Animation;
using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;

namespace Assets.Scripts.ECS.Systems.Animation
{
    partial struct ActiveAnimationSystem : ISystem {

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<AnimationDataHolder>();
            state.RequireForUpdate<SceneDataReference>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            var animationDataHolder = SystemAPI.GetSingleton<AnimationDataHolder>();
            var animBlob = animationDataHolder.animationDataBlobArrayBlobAssetReference;

            SceneDataReference sceneData = SystemAPI.GetSingleton<SceneDataReference>();

            if (sceneData.IsJobSystemOn)
            {
                var job = new ActiveAnimationJob
                {
                    deltaTime = deltaTime,
                    animationDataBlobArrayBlobAssetReference = animBlob,
                };
                job.ScheduleParallel();
            }
            else
            {
                foreach (var (activeAnim, meshInfo) in SystemAPI.Query<RefRW<ActiveAnimation>, RefRW<MaterialMeshInfo>>())
                {
                    ref var animData = ref animBlob.Value[(int)activeAnim.ValueRO.activeAnimationType];

                    activeAnim.ValueRW.frameTimer += deltaTime;
                    if (activeAnim.ValueRO.frameTimer > animData.frameTimerMax)
                    {
                        activeAnim.ValueRW.frameTimer -= animData.frameTimerMax;
                        activeAnim.ValueRW.frame = (activeAnim.ValueRO.frame + 1) % animData.frameMax;
                        meshInfo.ValueRW.Mesh = animData.intMeshIdBlobArray[activeAnim.ValueRO.frame];

                        if (activeAnim.ValueRO.frame == 0 &&
                            AnimationDataSO.IsAnimationUninterruptible(activeAnim.ValueRO.activeAnimationType))
                        {
                            activeAnim.ValueRW.activeAnimationType = AnimationDataSO.AnimationType.None;
                        }
                    }
                }
            }
        }




        [BurstCompile]
        public partial struct ActiveAnimationJob : IJobEntity
        {

            public float deltaTime;
            public BlobAssetReference<BlobArray<AnimationData>> animationDataBlobArrayBlobAssetReference;


            public void Execute(ref ActiveAnimation activeAnimation, ref MaterialMeshInfo materialMeshInfo)
            {
                ref AnimationData animationData =
                    ref animationDataBlobArrayBlobAssetReference.Value[(int)activeAnimation.activeAnimationType];

                activeAnimation.frameTimer += deltaTime;
                if (activeAnimation.frameTimer > animationData.frameTimerMax)
                {
                    activeAnimation.frameTimer -= animationData.frameTimerMax;
                    activeAnimation.frame =
                        (activeAnimation.frame + 1) % animationData.frameMax;


                    materialMeshInfo.Mesh =
                        animationData.intMeshIdBlobArray[activeAnimation.frame];


                    if (activeAnimation.frame == 0 &&
                        AnimationDataSO.IsAnimationUninterruptible(activeAnimation.activeAnimationType))
                    {

                        activeAnimation.activeAnimationType = AnimationDataSO.AnimationType.None;
                    }
                }


            }
        }

    }
}