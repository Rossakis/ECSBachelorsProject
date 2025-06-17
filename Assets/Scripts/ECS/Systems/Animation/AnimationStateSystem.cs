using ECS.Authoring.Reference;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECS.Systems.Animation
{
    [UpdateAfter(typeof(Combat.CastFireballSystem))]
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
                var readyJob = new ReadySpellAnimationStateJob {
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
                foreach (var (animatedMesh, castFireball, unitMover, target, unitAnimations) in SystemAPI.Query<AnimatedMesh, CastFireball, UnitMover, Target, UnitAnimations>()) {
                    if (!unitMover.isMoving && target.targetEntity != Entity.Null) {
                        RefRW<ActiveAnimation> activeAnimation = activeAnimationComponentLookup.GetRefRW(animatedMesh.meshEntity);
                        activeAnimation.ValueRW.nextAnimationType = unitAnimations.readySpellAnimationType;
                    }
                    if (castFireball.onShoot.isTriggered) {
                        RefRW<ActiveAnimation> activeAnimation = activeAnimationComponentLookup.GetRefRW(animatedMesh.meshEntity);
                        activeAnimation.ValueRW.nextAnimationType = unitAnimations.castFireballAnimationType;
                    }
                }

                activeAnimationComponentLookup.Update(ref state);
                foreach (var (animatedMesh, meleeAttack, unitAnimations) in SystemAPI.Query<AnimatedMesh, MeleeAttack, UnitAnimations>()) {
                    if (meleeAttack.onAttacked) {
                        RefRW<ActiveAnimation> activeAnimation = activeAnimationComponentLookup.GetRefRW(animatedMesh.meshEntity);
                        activeAnimation.ValueRW.nextAnimationType = unitAnimations.knightAttackAnimationType;
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
    public partial struct ReadySpellAnimationStateJob : IJobEntity {

        [NativeDisableParallelForRestriction] public ComponentLookup<ActiveAnimation> activeAnimationComponentLookup;

        public void Execute(in AnimatedMesh animatedMesh, in CastFireball castFireball, in UnitMover unitMover, in Target target, in UnitAnimations unitAnimations) {
            if (!unitMover.isMoving && target.targetEntity != Entity.Null) {
                RefRW<ActiveAnimation> activeAnimation = activeAnimationComponentLookup.GetRefRW(animatedMesh.meshEntity);
                activeAnimation.ValueRW.nextAnimationType = unitAnimations.readySpellAnimationType;
            }

            if (castFireball.onShoot.isTriggered) {
                RefRW<ActiveAnimation> activeAnimation = activeAnimationComponentLookup.GetRefRW(animatedMesh.meshEntity);
                activeAnimation.ValueRW.nextAnimationType = unitAnimations.castFireballAnimationType;
            }
        }

    }


    [BurstCompile]
    public partial struct KnightAttackAnimationStateJob : IJobEntity {

        [NativeDisableParallelForRestriction] public ComponentLookup<ActiveAnimation> activeAnimationComponentLookup;

        public void Execute(in AnimatedMesh animatedMesh, in MeleeAttack meleeAttack, in UnitAnimations unitAnimations) {
            if (meleeAttack.onAttacked) {
                RefRW<ActiveAnimation> activeAnimation = activeAnimationComponentLookup.GetRefRW(animatedMesh.meshEntity);
                activeAnimation.ValueRW.nextAnimationType = unitAnimations.knightAttackAnimationType;
            }
        }

    }
}