using ECS.Authoring.Reference;
using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;

[UpdateBefore(typeof(ActiveAnimationSystem))]
partial struct ChangeAnimationSystem : ISystem {
    
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<AnimationDataHolder>();
        state.RequireForUpdate<SceneDataReference>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var animationDataHolder = SystemAPI.GetSingleton<AnimationDataHolder>();
        var animBlob = animationDataHolder.animationDataBlobArrayBlobAssetReference;

        SceneDataReference sceneData = SystemAPI.GetSingleton<SceneDataReference>();

        if (sceneData.IsJobSystemOn) {
            var job = new ChangeAnimationJob {
                animationDataBlobArrayBlobAssetReference = animBlob
            };
            job.ScheduleParallel();
        }
        else {
            foreach (var (activeAnim, meshInfo) in SystemAPI.Query<RefRW<ActiveAnimation>, RefRW<MaterialMeshInfo>>()) {
                if (AnimationDataSO.IsAnimationUninterruptible(activeAnim.ValueRO.activeAnimationType)) continue;

                if (activeAnim.ValueRO.activeAnimationType != activeAnim.ValueRO.nextAnimationType) {
                    activeAnim.ValueRW.frame = 0;
                    activeAnim.ValueRW.frameTimer = 0f;
                    activeAnim.ValueRW.activeAnimationType = activeAnim.ValueRW.nextAnimationType;

                    ref var animData = ref animBlob.Value[(int)activeAnim.ValueRO.activeAnimationType];
                    meshInfo.ValueRW.Mesh = animData.intMeshIdBlobArray[0];
                }
            }
        }
    }


}


[BurstCompile]
public partial struct ChangeAnimationJob : IJobEntity {


    public BlobAssetReference<BlobArray<AnimationData>> animationDataBlobArrayBlobAssetReference;


    public void Execute(ref ActiveAnimation activeAnimation, ref MaterialMeshInfo materialMeshInfo) {
        if (AnimationDataSO.IsAnimationUninterruptible(activeAnimation.activeAnimationType)) {
            return;
        }

        if (activeAnimation.activeAnimationType != activeAnimation.nextAnimationType) {
            activeAnimation.frame = 0;
            activeAnimation.frameTimer = 0f;
            activeAnimation.activeAnimationType = activeAnimation.nextAnimationType;

            ref AnimationData animationData =
                ref animationDataBlobArrayBlobAssetReference.Value[(int)activeAnimation.activeAnimationType];

            materialMeshInfo.Mesh = animationData.intMeshIdBlobArray[0];
        }
    }

}