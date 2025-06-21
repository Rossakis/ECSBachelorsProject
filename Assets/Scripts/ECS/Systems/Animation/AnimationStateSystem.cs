using Assets.Scripts.ECS.Authoring.Animation;
using Assets.Scripts.ECS.Authoring.Combat;
using Assets.Scripts.ECS.Authoring.Movement;
using Assets.Scripts.ECS.Authoring.Reference;
using Assets.Scripts.ScriptableObjects.Animation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Assets.Scripts.ECS.Systems.Animation
{
    [UpdateAfter(typeof(Combat.FireballInstantiateSystem))]
    partial struct AnimationStateSystem : ISystem {
        private ComponentLookup<ActiveAnimation> activeAnimationComponentLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            activeAnimationComponentLookup = state.GetComponentLookup<ActiveAnimation>(false);
            state.RequireForUpdate<SceneDataReference>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            activeAnimationComponentLookup.Update(ref state);

            SceneDataReference sceneData = SystemAPI.GetSingleton<SceneDataReference>();

            if (sceneData.IsJobSystemOn) {
                var idleJob = new IdleWalkingAnimationStateJob {
                    activeAnimationComponentLookup = activeAnimationComponentLookup,
                };
                idleJob.ScheduleParallel();

                activeAnimationComponentLookup.Update(ref state);
                var readyJob = new CastFireballAnimationStateJob {
                    activeAnimationComponentLookup = activeAnimationComponentLookup,
                };
                readyJob.ScheduleParallel();

                activeAnimationComponentLookup.Update(ref state);
                var attackJob = new KnightAttackAnimationStateJob {
                    activeAnimationComponentLookup = activeAnimationComponentLookup,
                };
                attackJob.ScheduleParallel();
            }
            else {
                foreach (var (animatedMesh, unitMover, unitAnimations) in SystemAPI.Query<AnimatedMesh, UnitMover, UnitAnimations>()) {
                    RefRW<ActiveAnimation> activeAnimation = activeAnimationComponentLookup.GetRefRW(animatedMesh.meshEntity);
                    activeAnimation.ValueRW.nextAnimationType = unitMover.isMoving ? unitAnimations.walkAnimationType : unitAnimations.idleAnimationType;
                }
                activeAnimationComponentLookup.Update(ref state);
                
                foreach (var (animatedMesh, castFireball, unitAnimations) in SystemAPI.Query<AnimatedMesh, CastFireball, UnitAnimations>()) {
                    if (castFireball.onShoot.isTriggered) {
                        RefRW<ActiveAnimation> activeAnimation = activeAnimationComponentLookup.GetRefRW(animatedMesh.meshEntity);
                        activeAnimation.ValueRW.nextAnimationType = unitAnimations.castFireballAnimationType;
                    }
                }
                activeAnimationComponentLookup.Update(ref state);
                
                foreach (var (animatedMesh, meleeAttack, unitAnimations) in SystemAPI.Query<AnimatedMesh, RefRW<MeleeAttack>, UnitAnimations>()) {
                    if (meleeAttack.ValueRO.OnAttack) {
                        RefRW<ActiveAnimation> activeAnimation = activeAnimationComponentLookup.GetRefRW(animatedMesh.meshEntity);
                        
                        // Animation finished, reset OnAttack
                        if (activeAnimation.ValueRO.activeAnimationType == AnimationDataSO.AnimationType.KnightAttack &&
                            activeAnimation.ValueRO.frame == meleeAttack.ValueRO.lastFrameAttack && // Animation looped back to start
                            AnimationDataSO.IsAnimationUninterruptible(activeAnimation.ValueRO.activeAnimationType))
                        {
                            meleeAttack.ValueRW.OnAttack = false;
                        }
                        
                        activeAnimation.ValueRW.nextAnimationType = unitAnimations.knightAttackAnimationType;
                        activeAnimationComponentLookup.Update(ref state);
                    }
                }
            }
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
    public partial struct CastFireballAnimationStateJob : IJobEntity {

        [NativeDisableParallelForRestriction] public ComponentLookup<ActiveAnimation> activeAnimationComponentLookup;

        public void Execute(in AnimatedMesh animatedMesh, in CastFireball castFireball, in UnitMover unitMover, in Target target, in UnitAnimations unitAnimations) {
            if (castFireball.onShoot.isTriggered) {
                RefRW<ActiveAnimation> activeAnimation = activeAnimationComponentLookup.GetRefRW(animatedMesh.meshEntity);
                activeAnimation.ValueRW.nextAnimationType = unitAnimations.castFireballAnimationType;
            }
        }

    }


    [BurstCompile]
    public partial struct KnightAttackAnimationStateJob : IJobEntity {

        [NativeDisableParallelForRestriction] public ComponentLookup<ActiveAnimation> activeAnimationComponentLookup;

        public void Execute(in AnimatedMesh animatedMesh, ref MeleeAttack meleeAttack, in UnitAnimations unitAnimations) {
            if (meleeAttack.OnAttack) {
                RefRW<ActiveAnimation> activeAnimation = activeAnimationComponentLookup.GetRefRW(animatedMesh.meshEntity);
                
                // Animation finished, reset OnAttack
                if (activeAnimation.ValueRO.activeAnimationType == AnimationDataSO.AnimationType.KnightAttack &&
                    activeAnimation.ValueRO.frame == meleeAttack.lastFrameAttack && // Animation looped back to start
                    AnimationDataSO.IsAnimationUninterruptible(activeAnimation.ValueRO.activeAnimationType))
                {
                    meleeAttack.OnAttack = false;
                }
                
                activeAnimation.ValueRW.nextAnimationType = unitAnimations.knightAttackAnimationType;
            }
        }

    }
}