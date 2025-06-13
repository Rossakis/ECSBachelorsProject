using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(ShootAttackSystem))]
partial struct AnimationStateSystem : ISystem {


    private ComponentLookup<ActiveAnimation> activeAnimationComponentLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        activeAnimationComponentLookup = state.GetComponentLookup<ActiveAnimation>(false);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        activeAnimationComponentLookup.Update(ref state);
        IdleWalkingAnimationStateJob idleWalkingAnimationStateJob = new IdleWalkingAnimationStateJob {
            activeAnimationComponentLookup = activeAnimationComponentLookup,
        };
        idleWalkingAnimationStateJob.ScheduleParallel();

        activeAnimationComponentLookup.Update(ref state);
        AimShootAnimationStateJob aimShootAnimationStateJob = new AimShootAnimationStateJob {
            activeAnimationComponentLookup = activeAnimationComponentLookup,
        };
        aimShootAnimationStateJob.ScheduleParallel();
    }


}



[BurstCompile]
public partial struct IdleWalkingAnimationStateJob : IJobEntity {

    [NativeDisableParallelForRestriction] public ComponentLookup<ActiveAnimation> activeAnimationComponentLookup;

    public void Execute(in AnimatedMesh animatedMesh, in UnitMover unitMover, in UnitAnimations unitAnimations) {
        RefRW<ActiveAnimation> activeAnimation = activeAnimationComponentLookup.GetRefRW(animatedMesh.meshEntity);
        if (unitMover.isMoving) {
            activeAnimation.ValueRW.nextAnimationType = unitAnimations.walkAnimationType;
        } else {
            activeAnimation.ValueRW.nextAnimationType = unitAnimations.idleAnimationType;
        }
    }

}


[BurstCompile]
public partial struct AimShootAnimationStateJob : IJobEntity {

    [NativeDisableParallelForRestriction] public ComponentLookup<ActiveAnimation> activeAnimationComponentLookup;

    public void Execute(in AnimatedMesh animatedMesh, in CastFireball castFireball, in UnitMover unitMover, in Target target, in UnitAnimations unitAnimations) {
        if (!unitMover.isMoving && target.targetEntity != Entity.Null) {
            RefRW<ActiveAnimation> activeAnimation = activeAnimationComponentLookup.GetRefRW(animatedMesh.meshEntity);
            activeAnimation.ValueRW.nextAnimationType = unitAnimations.aimAnimationType;
        }

        if (castFireball.onShoot.isTriggered) {
            RefRW<ActiveAnimation> activeAnimation = activeAnimationComponentLookup.GetRefRW(animatedMesh.meshEntity);
            activeAnimation.ValueRW.nextAnimationType = unitAnimations.shootAnimationType;
        }
    }

}